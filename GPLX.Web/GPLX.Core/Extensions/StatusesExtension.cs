using System;
using System.Collections.Generic;
using System.Linq;
using GPLX.Core.DTO.Response.CostStatus;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using Serilog;

namespace GPLX.Core.Extensions
{
    public enum NextAction
    {
        APPROVED = 1,
        DECLINE = 2,
        EDIT = 3,
        DELETE = 4
    }
    public class NextStatExtension
    {
        public IList<StatusesGranted> ListGranted { get; set; }
        public IList<CostStatuses> All { get; set; }
        public string Type { get; set; }
        public string CostType { get; set; }
        public string BaseMessage { get; set; }

        public NextStatExtension(IEnumerable<CostStatuses> all, IEnumerable<StatusesGranted> granted, string type, string costType, string baseMessage)
        {
            ListGranted = granted.ToList();
            All = all.ToList();
            Type = type;
            CostType = costType;
            BaseMessage = baseMessage;
        }
        /// <summary>
        /// Lấy trạng thái tiếp theo mà user đang thực hiện
        /// </summary>
        /// <returns></returns>
        public NextResponse _getNext(int statusAtTime, bool isApproved, bool isSubRecord, string unitType, string specifyOfficeCode)
        {
            var nextResponse = new NextResponse();
            try
            {
                if (!string.IsNullOrEmpty(specifyOfficeCode))
                    unitType = specifyOfficeCode;

                var sj = isSubRecord ? GlobalEnums.ObjectSub : unitType;

                if (ListGranted == null || ListGranted.Count == 0 || All == null)
                {
                    nextResponse.NextValid = false;
                    nextResponse.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";
                    return nextResponse;
                }
                var oStatAtTime = All.FirstOrDefault(x => x.Value == statusAtTime && x.Type == Type
                                                                                  && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                                                                  && x.StatusForCostEstimateType == CostType);
                if (oStatAtTime == null)
                {
                    nextResponse.NextValid = false;
                    nextResponse.Message = $"Không tìm thấy trạng thái của {BaseMessage}!";
                    return nextResponse;
                }

                var oFind = All.Where(x =>
                    x.Type == Type
                    && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                    && x.StatusForCostEstimateType == CostType).ToList();

                var minOfFollow = oFind.Min(x => x.Order);

                // thứ tự của trạng thái trong quy trình
                var qFind = oFind.Where(x => x.IsApprove == 1).ToList();

                int maxLevel = qFind
                    .Max(x => x.Order);
                int minLevel = qFind.Min(x => x.Order);

                var levelAt = oStatAtTime.Order;
                // phê duyệt ở cấp cao nhất
                if (levelAt == maxLevel)
                {
                    nextResponse.NextValid = false;
                    nextResponse.Message = $"{BaseMessage} đã được phê duyệt không thể thay đổi trạng thái!";
                    return nextResponse;
                }
                // phê duyệt ở cấp cao nhất
                int maxLevelGranted = ListGranted.Where(x => x.Type == Type
                                                             && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                                             && x.StatusForCostEstimateType == CostType).Max(x => x.Order);
                if (levelAt == maxLevelGranted)
                {
                    nextResponse.NextValid = false;
                    nextResponse.Message = $"Không thể thay đổi trạng thái của {BaseMessage}!";
                    return nextResponse;
                }

                // lấy trạng ở level tiếp theo
                var oNext = ListGranted.FirstOrDefault(x => x.Order == levelAt + 1 && (isApproved ? x.IsApprove == 1 : x.IsApprove == 0) && x.Type == Type && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase) && x.StatusForCostEstimateType == CostType && x.Used);
                var oUserLevel = ListGranted.FirstOrDefault(x => x.Used);

                // chỉ được phép phê duyệt các phiếu ở trạng thái chờ duyệt
                // hoặc duyệt bởi cấp dưới
                // không phê duyệt được các phiếu đang bị từ chối ...
                if (oNext == null || levelAt >= minLevel && levelAt != minOfFollow && oStatAtTime.IsApprove == 0)
                {
                    nextResponse.NextValid = false;
                    nextResponse.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";
                    return nextResponse;
                }


                nextResponse.NextValid = true;
                nextResponse.Next = oNext;
                // nếu là phê duyệt
                // và bước tiếp theo là cuối cùng của quy trình
                nextResponse.IsMaxOfFollow = maxLevel == oNext.Value && oNext.IsApprove == 1 && oUserLevel?.Order != oStatAtTime.Order;
                return nextResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                nextResponse.NextValid = false;
                nextResponse.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";
                return nextResponse;
            }
        }

        public bool _visible(int statusAtTime, NextAction action, bool permissionAction, bool isSubRecord, string unitType, string specifyOfficeCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(specifyOfficeCode))
                    unitType = specifyOfficeCode;

                var sj = isSubRecord ? GlobalEnums.ObjectSub : unitType;
                // trạng thái ở cấp thấp nhất
                // tương đương khi đối tượng mới được khởi tạo
                int minLevel = All.Where(x => x.Type == Type
                                              && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                              && x.StatusForCostEstimateType == CostType).Min(x => x.Order);
                int minLevelValue = All.FirstOrDefault(x =>
                    x.Order == minLevel && x.Type == Type && x.StatusForSubject == sj &&
                    x.StatusForCostEstimateType == CostType)?.Value ?? -1;

                switch (action)
                {
                    case NextAction.APPROVED:
                        var oApprovedNext = _getNext(statusAtTime, true, isSubRecord, unitType, specifyOfficeCode);
                        return oApprovedNext.NextValid && permissionAction;
                    case NextAction.DECLINE:
                        var oDeclineNext = _getNext(statusAtTime, true, isSubRecord, unitType, specifyOfficeCode);
                        return oDeclineNext.NextValid && permissionAction;
                    case NextAction.EDIT:
                        var oLevelAt = All.FirstOrDefault(x => x.Value == statusAtTime && x.Type == Type
                                                                                       && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                                                                       && x.StatusForCostEstimateType == CostType);
                        if (oLevelAt == null)
                            return false;
                        int maxLevel = All.Where(x => x.Type == Type
                                                      && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                                      && x.StatusForCostEstimateType == CostType && x.IsApprove == 1).Max(x => x.Order);
                        var levelAt = oLevelAt.Order;
                        // đã phê duyệt ở cấp cao nhất
                        if (levelAt == maxLevel && oLevelAt.IsApprove == 1)
                            return false;

                        // phiếu mới tạo
                        // người dùng có quyền chỉnh sửa
                        if (minLevelValue == statusAtTime && permissionAction)
                            return true;

                        // nếu người dùng hiện tại có quyền phê duyệt hoặc từ chối ở level kế tiếp so với trạng thái của dữ liệu
                        // có quyền chỉnh sửa ở permission
                        // hoặc trạng thái phiếu đang là từ chối

                        var oUserLevel = ListGranted.FirstOrDefault(x => x.Used);


                        var oNextLevel = ListGranted.FirstOrDefault(x => x.Order == oLevelAt.Order + 1
                                                                         && x.Type == Type
                                                                         && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                                                         && x.StatusForCostEstimateType == CostType && x.Used);
                        // điều kiện 2
                        // trạng thái ko do nhóm người dùng này tạo
                        return oNextLevel != null && permissionAction || oLevelAt.IsApprove != 1 && permissionAction && oUserLevel?.Order != oLevelAt.Order;
                    case NextAction.DELETE:
                        var oDeleteLevelAt = All.FirstOrDefault(x => x.Value == statusAtTime
                                                                     && x.Type == Type
                                                                     && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase) && x.StatusForCostEstimateType == CostType);
                        if (oDeleteLevelAt == null)
                            return false;
                        var minFollowLevel = All.Where(x =>
                                x.Type == Type && x.StatusForSubject == sj &&
                                x.StatusForCostEstimateType == CostType)
                            .Min(x => x.Order);
                        // chỉ đc xóa phiếu mới tạo
                        if (minFollowLevel < oDeleteLevelAt.Order)
                            return false;
                        return permissionAction;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public int _minLevel(bool isSubRecord,
            string unitType, string specifyOfficeCode)
        {
            if (!string.IsNullOrEmpty(specifyOfficeCode))
                unitType = specifyOfficeCode;

            var sj = isSubRecord ? GlobalEnums.ObjectSub : unitType;
            // trạng thái ở cấp thấp nhất
            // tương đương khi đối tượng mới được khởi tạo
            var minLevelD = All.Where(x => x.Type == Type
                                           && x.StatusForSubject.Equals(sj, StringComparison.CurrentCultureIgnoreCase)
                                           && x.StatusForCostEstimateType == CostType);
            var costMinLevelD = minLevelD.ToList();
            int minLevel = costMinLevelD.Any() ? costMinLevelD.Min(x => x.Order) : 0;
            int minLevelValue = All.FirstOrDefault(x =>
                x.Order == minLevel && x.Type == Type && x.StatusForSubject == sj &&
                x.StatusForCostEstimateType == CostType)?.Value ?? -1;
            return minLevelValue;
        }
    }

    public class NextResponse
    {
        /// <summary>
        /// Kiểm tra thao tác có hợp lệ hay không
        /// </summary>
        public bool NextValid { get; set; }
        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Nếu hợp lệ sẽ gán trạng thái kế tiếp
        /// </summary>
        public StatusesGranted Next { get; set; }
        /// <summary>
        /// Có phải thao tác này là max của quy trình
        /// </summary>
        public bool IsMaxOfFollow { get; set; }
    }

    public static class FilterStats
    {
        public static List<ActivatorResult> Filter(this IEnumerable<StatusesGranted> value, IList<CostStatuses> allStatus, bool sub, int? statusFilter)
        {
            var valueList = value?.ToList();
            var rt = new List<ActivatorResult>();
            // nhóm theo từng loại

            if (valueList == null || !valueList.Any())
                return null;
            var dataGroups = valueList?.GroupBy(c => c.StatusForSubject, (x, y) => new
            {
                For = x,
                Data = y.ToList()
            }).ToList();

            foreach (var dataGroup in dataGroups)
            {
                var dataFor = dataGroup.Data;
                if (statusFilter != null && statusFilter.Value != (int)GlobalEnums.StatusDefaultEnum.All)
                {
                    // trường hợp request vào từ trang phê duyệt
                    // bao gồm trạng thái (phê duyệt - từ chối)

                    dataFor = dataFor.Where(p =>
                            statusFilter.Value == (int)GlobalEnums.StatusDefaultEnum.Activator || p.IsApprove == (statusFilter == (int)GlobalEnums.StatusDefaultEnum.Active ? 1 : 0))
                        .ToList();

                    var minOrder = allStatus.Where(c => c.StatusForSubject == dataGroup.For).Min(c => c.Order);

                    // loại bỏ tất cả các trạng thái
                    // khác ở vị trí min
                    // chờ duyệt
                    if (statusFilter.Value == (int)GlobalEnums.StatusDefaultEnum.Activator)
                        dataFor = dataFor.Where(c => c.Order > minOrder).ToList();
                    //if (statusFilter == (int) GlobalEnums.StatusDefaultEnum.InActive)
                    //    dataFor = dataFor.Where(x => x.Order == minOrder).ToList();
                    else if (statusFilter == (int)GlobalEnums.StatusDefaultEnum.Decline)
                        dataFor = dataFor.Where(x => x.Order != minOrder).ToList();

                }
                var rtQuery = dataFor.AsQueryable();
                // nếu là SUB --> chỉ lấy của sub
                // nếu khác SUB --> lấy của cả đơn vị yte - ngoài y tế
                rtQuery = sub
                    ? rtQuery.Where(x =>
                        x.StatusForSubject.Equals(GlobalEnums.ObjectSub,
                            StringComparison.CurrentCultureIgnoreCase))
                    : rtQuery.Where(x =>
                        !x.StatusForSubject.Equals(GlobalEnums.ObjectSub,
                            StringComparison.CurrentCultureIgnoreCase));


                var rtList = rtQuery.Select(x => x.Value).ToList();
                if (!rtList.Any())
                    rtList = new List<int> { -1000 };

                rt.Add(new ActivatorResult { UnitType = dataGroup.For, Values = rtList.Distinct().ToList() });
            }

            return rt;
        }
    }

    public class ActivatorResult
    {
        public string UnitType { get; set; }
        public List<int> Values { get; set; }
    }
}
