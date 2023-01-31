using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Actually;
using GPLX.Core.Contracts.Statuses;
using GPLX.Core.DTO.Request.Actually;
using GPLX.Core.DTO.Response.Actually;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Core.Data.Actually
{
    public class ActuallySpentRepository : IActuallySpentRepository
    {
        private readonly Context _ctx;
        private readonly IMapper _mapper;
        private readonly ICostStatusesRepository _costStatusesRepository;
        private readonly IActionLogsRepository _actionLogsRepository;


        public ActuallySpentRepository(Context ctx, IMapper mapper, ICostStatusesRepository costStatusesRepository, IActionLogsRepository actionLogsRepository)
        {
            _ctx = ctx;
            _mapper = mapper;
            _costStatusesRepository = costStatusesRepository;
            _actionLogsRepository = actionLogsRepository;
        }

        /// <summary>
        /// Danh sách báo cáo thực chi
        /// </summary>
        /// <param name="request"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public async Task<SearchActuallySpentResponse> SearchAsync(SearchActuallySpentRequest request, int offset, int limit, string unitType)
        {
            try
            {
                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen,
                    GlobalEnums.ActuallySpent, GlobalEnums.Week, "BC Thực chi");

                var response = new SearchActuallySpentResponse();
                var searchQuery = from aSpent in _ctx.ActuallySpent
                                  join map in _ctx.ActuallySpentMapItem on aSpent.Id equals map.ActuallySpentId
                                  join actuallySpentItem in _ctx.ActuallySpentItem on map.ActuallySpentItemId equals actuallySpentItem.Id
                                  join u in _ctx.Units on aSpent.UnitId equals u.Id
                                  select new
                                  {
                                      ActuallySpent = aSpent,
                                      ActuallySpentItem = actuallySpentItem,
                                      u.OfficesSub,
                                      u.OfficesCode
                                  };


                if (request.UserUnit != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(p => p.ActuallySpent.UnitId == request.UserUnit);

                // trạng thái mà user được phép thấy

                var subStatusFilter = request.StatusAllowsSeen.Filter(next.All, true, request.Status);
                var unitStatusFilter = request.StatusAllowsSeen.Filter(next.All, false, request.Status);

                if (request.UserUnitsManages != null && request.UserUnitsManages.Count > 0)
                {
                    var unitIds = request.UserUnitsManages.Select(x => x.OfficeId);
                    searchQuery = searchQuery.Where(p => unitIds.Contains(p.ActuallySpent.UnitId));
                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    searchQuery = searchQuery.Where(p => p.ActuallySpent.ReportForWeekName.Contains(request.Keywords)
                                                         || p.ActuallySpent.ReportForWeekName.Contains(request.KeywordsNonUnicode)
                                                         || p.ActuallySpentItem.RequestCode == request.Keywords);
                if (request.FilterWeek != (int)GlobalEnums.StatusDefaultEnum.All)
                    searchQuery = searchQuery.Where(x => x.ActuallySpent.ReportForWeek == request.FilterWeek);
              

                var dataSearch = (await searchQuery.Select(x => new { x.ActuallySpent, x.OfficesSub, x.OfficesCode })
                    .Distinct().OrderByDescending(x => x.ActuallySpent.CreatedDate).ToListAsync()).Select(c => new
                    {
                        c.ActuallySpent,
                        c.OfficesCode,
                        OfficesSub = c.OfficesSub?.Equals("YT", StringComparison.CurrentCultureIgnoreCase) == true ? GlobalEnums.UnitIn : GlobalEnums.UnitOut
                    }).ToList();


                var latest = Extensions.Extensions.CreateList(dataSearch.FirstOrDefault());
                latest.Clear();
                // filter theo trạng thái được cấu hình
                foreach (var g in dataSearch)
                {
                    if (g.ActuallySpent.IsSub && subStatusFilter.Count > 0)
                    {
                        var subFil = subStatusFilter.FirstOrDefault(c => c.UnitType.Equals(GlobalEnums.ObjectSub, StringComparison.CurrentCultureIgnoreCase));
                        if (subFil?.Values.Any(c => c == g.ActuallySpent.Status) == true)
                            latest.Add(g);
                    }
                    else
                    {
                        if (unitStatusFilter.Count > 0)
                        {
                            foreach (var activatorResult in unitStatusFilter)
                            {
                                //g.OfficesSub?.Equals(activatorResult.UnitType) == true &&
                                if (activatorResult.Values.Contains(g.ActuallySpent.Status))
                                {
                                    latest.Add(g);
                                    break;
                                }
                            }
                        }
                    }
                }


                var data = new List<SearchActuallySpentData>();
                latest.ForEach(x =>
                {
                    var item = _mapper.Map<ActuallySpent, SearchActuallySpentData>(x.ActuallySpent);
                    item.Record = x.ActuallySpent.Id.ToString().StringAesEncryption(request.PageRequest);
                    item.Status = x.ActuallySpent.StatusName;
                    item.Approvalable = next._visible(x.ActuallySpent.Status, NextAction.APPROVED, request.PermissionApprove, x.ActuallySpent.IsSub, x.OfficesSub, string.Empty);
                    item.Declineable = next._visible(x.ActuallySpent.Status, NextAction.APPROVED, request.PermissionApprove, x.ActuallySpent.IsSub, x.OfficesSub, string.Empty);
                    item.Editable = next._visible(x.ActuallySpent.Status, NextAction.EDIT, request.PermissionEdit, x.ActuallySpent.IsSub, x.OfficesSub, string.Empty);
                    item.Viewable = true;


                    int minLevel = next._minLevel(x.ActuallySpent.IsSub, x.OfficesSub, string.Empty);
                    if (request.Status != (int)GlobalEnums.StatusDefaultEnum.All)
                    {
                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.Activator || request.Status == (int)GlobalEnums.StatusDefaultEnum.Active
                                                                                          || request.Status == (int)GlobalEnums.StatusDefaultEnum.Decline)
                            if (!item.Approvalable && !item.Declineable)
                                data.Add(item);

                        if (request.Status == (int)GlobalEnums.StatusDefaultEnum.InActive)
                            if (item.Approvalable || item.Declineable || minLevel == x.ActuallySpent.Status)
                                data.Add(item);
                    }
                    else
                        data.Add(item);
                });

                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Draw = request.Draw;
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                response.Data = data.Skip(offset).Take(limit).ToList();
                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error", request);
                return new SearchActuallySpentResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                    Data = null,
                    Draw = request.Draw,
                    Message = "Không tìm thấy dữ liệu yêu cầu"
                };
            }
        }

        /// <summary>
        /// Tạo mới báo cáo chi cho đơn vị
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateActuallyResponse> CreateAsync(CreateActuallySpentRequest request)
        {
            await using var transaction = _ctx.Database.BeginTransaction();
            try
            {
                var response = new CreateActuallyResponse();

                #region Tạo mới actually spent

                var newActually = new ActuallySpent
                {
                    Id = Guid.NewGuid(),
                    UnitName = request.UnitName,
                    UnitId = request.UnitId,
                    ReportForWeekName = $"Tuần {request.ReportForWeek}",
                    ReportForWeek = request.ReportForWeek,
                    Creator = request.Creator,
                    CreatorName = request.CreatorName,
                    Status = (int)GlobalEnums.StatusDefaultEnum.InActive,
                    StatusName = "Chờ duyệt",
                    TotalActualSpentAtTime = request.Data.Sum(p => p.ActualSpentAtTime),
                    TotalActuallySpent = request.Data.Sum(p => p.ActualSpent),
                    TotalAmountLeft = request.Data.Sum(p => p.AmountLeft),
                    TotalEstimateCost = request.Data.Sum(p => p.Cost),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsSub = request.IsSub
                };
                await _ctx.ActuallySpent.AddAsync(newActually);
                await _ctx.SaveChangesAsync();

                #endregion

                await transaction.CreateSavepointAsync("Before");

                #region Tạo bản ghi chi tiết

                var listActuallySpentItems = new List<ActuallySpentItem>();

                foreach (var searchActuallySpentRequest in request.Data)
                {
                    var newSpent = _mapper.Map<ActuallySpentItemResponse, ActuallySpentItem>(searchActuallySpentRequest);
                    newSpent.Id = Guid.NewGuid();
                    if (string.IsNullOrEmpty(newSpent.RequestCode))
                    {
                        newSpent.RequestPayWeekName = "Phát sinh ngoài dự trù";
                        newSpent.ActualSpentType = "Out";
                    }
                    else
                        newSpent.ActualSpentType = "In";

                    listActuallySpentItems.Add(newSpent);
                }

                await _ctx.ActuallySpentItem.AddRangeAsync(listActuallySpentItems);
                await _ctx.SaveChangesAsync();

                #endregion

                #region Đẩy vào bảng dữ liệu RAW từ file Excel upload
                var rangeSctSave = new List<SctData>();
                foreach (var sctView in request.SctData)
                {
                    var sctDataItem = _mapper.Map<SctData>(sctView);
                    sctDataItem.Id = Guid.NewGuid();
                    sctDataItem.Uploader = request.CreatorName;
                    sctDataItem.Path = request.PathBackup;
                    sctDataItem.ActuallySpentId = newActually.Id;
                    rangeSctSave.Add(sctDataItem);
                }

                await _ctx.SctData.AddRangeAsync(rangeSctSave);
                #endregion

                #region Đẩy vào bảng map
                var listMap = new List<ActuallySpentMapItem>();
                foreach (var actuallySpentItem in listActuallySpentItems)
                {
                    listMap.Add(new ActuallySpentMapItem
                    {
                        CreatorName = request.CreatorName,
                        Creator = request.Creator,
                        ActuallySpentId = newActually.Id,
                        ActuallySpentItemId = actuallySpentItem.Id,
                        CreatedDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                    });
                }

                await _ctx.ActuallySpentMapItem.AddRangeAsync(listMap);
                await _ctx.SaveChangesAsync();
                #endregion

                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    UserName = request.CreatorName,
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Create",
                    FunctionUnique = Functions.ActuallySpentCreate,
                    UserId = request.Creator
                });

                await transaction.CommitAsync();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Record = newActually.Id.ToString().StringAesEncryption(request.RequestPage);
                response.Message = "Tạo mới báo cáo thực chi thành công!";
                return response;
            }
            catch (Exception e)
            {

                Log.Error(e, "Error", request);

                await transaction.RollbackToSavepointAsync("Before");
                return new CreateActuallyResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Message = "Tạo mới báo cáo thực chi không thành công!"
                };
            }
        }

        /// <summary>
        /// Chỉnh sửa báo cáo thực chi
        /// </summary>
        /// <param name="request"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public async Task<CreateActuallyResponse> EditAsync(CreateActuallySpentRequest request, string unitType)
        {
            await using var transaction = _ctx.Database.BeginTransaction();
            try
            {
                var actuallyRecord = await _ctx.ActuallySpent.FirstOrDefaultAsync(x => x.Id == request.RawId);
                if (actuallyRecord == null)
                    return new CreateActuallyResponse
                    {
                        Message = "Không tìm thấy dữ liệu yêu cầu!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent
                    };

                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatsAllowSeen, GlobalEnums.ActuallySpent, GlobalEnums.Week, "BC Thực chi");
                bool canEdit = next._visible(actuallyRecord.Status, NextAction.EDIT, request.PermissionEdit, actuallyRecord.IsSub, unitType, string.Empty);
                if (!canEdit)
                {
                    return new CreateActuallyResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.NoContent,
                        Message = "Không có quyền chỉnh BC thực chi này!"
                    };
                }

                if (actuallyRecord.Status == (int)GlobalEnums.StatusDefaultEnum.Active)
                    return new CreateActuallyResponse
                    {
                        Message = "Báo cáo đã được duyệt, không được phép chỉnh sửa!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.NotOpenCode
                    };

                actuallyRecord.TotalActualSpentAtTime = request.Data.Sum(p => p.ActualSpentAtTime);
                actuallyRecord.TotalActuallySpent = request.Data.Sum(p => p.ActualSpent);
                actuallyRecord.TotalAmountLeft = request.Data.Sum(p => p.AmountLeft);
                actuallyRecord.TotalEstimateCost = request.Data.Sum(p => p.Cost);
                actuallyRecord.UpdatedDate = DateTime.Now;
                //sau khi chỉnh sửa ==> chuyển về trạng thái chờ duyệt
                actuallyRecord.Status = (int)GlobalEnums.StatusDefaultEnum.InActive;
                actuallyRecord.StatusName =
                    GlobalEnums.DefaultStatusNames[(int)GlobalEnums.StatusDefaultEnum.InActive];


                _ctx.ActuallySpent.Update(actuallyRecord);
                await _ctx.SaveChangesAsync();


                var listActuallySpentItemOnDb = await _ctx.ActuallySpentItem.AsNoTracking().
                    Join(_ctx.ActuallySpentMapItem, u => u.Id, v => v.ActuallySpentItemId, (x, y) => new
                    {
                        ActuallySpentItem = x,
                        Map = y
                    }).Where(x => x.Map.ActuallySpentId == request.RawId).ToListAsync();

                //request.Data.Any(m => m.RawId == x.ActuallySpentItem.Id && m.RawId != Guid.Empty)

                await transaction.CreateSavepointAsync("Before");
                bool isValidRecord = true;
                listActuallySpentItemOnDb.ForEach(x =>
                {
                    if (x.Map.ActuallySpentId != request.RawId)
                        isValidRecord = false;
                });
                if (!isValidRecord)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    return new CreateActuallyResponse
                    {
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                        Message = "Cập nhật không thành công, dữ liệu không hợp lệ"
                    };
                }

                //set giá trị để decrypt Record
                request.Data.ForEach(x => { x.RequestPage = request.RequestPage; });

                #region Cập nhật các bản ghi hiện có

                var listUpdateItem = new List<ActuallySpentItem>();
                foreach (var searchActuallySpentRequest in
                    listActuallySpentItemOnDb.Where(m => request.Data.Any(p => p.RawId == m.ActuallySpentItem.Id && p.RawId != Guid.Empty)))
                {
                    Guid actuallyRecordId = searchActuallySpentRequest.ActuallySpentItem.Id;
                    var updateRecordOnRequest = request.Data.FirstOrDefault(m => m.RawId == actuallyRecordId);
                    var updateItem = _mapper.Map<ActuallySpentItem>(updateRecordOnRequest);
                    updateItem.Id = actuallyRecordId;

                    listUpdateItem.Add(updateItem);
                }

                _ctx.ActuallySpentItem.UpdateRange(listUpdateItem);
                await _ctx.SaveChangesAsync();

                #endregion

                #region Xóa các bản ghi không có trong DB

                var listRemoved = listActuallySpentItemOnDb.Where(x =>
                    request.Data.All(p => x.ActuallySpentItem.Id != p.RawId && p.RawId != Guid.Empty)).ToList();
                if (listRemoved.Any())
                {
                    var listItemDbRemove = listRemoved.Select(p => p.ActuallySpentItem);
                    _ctx.ActuallySpentItem.RemoveRange(listItemDbRemove);
                    await _ctx.SaveChangesAsync();

                    var listMapDbRemove = listRemoved.Select(p => p.Map);
                    _ctx.ActuallySpentMapItem.RemoveRange(listMapDbRemove);
                    await _ctx.SaveChangesAsync();
                }

                #endregion

                #region Thêm các bản ghi được tạo mới - trường hợp có thêm bản ghi mới

                var listActuallyItemNew = request.Data.Where(x => x.RawId == Guid.Empty).ToList();
                var listActuallySpentItems = new List<ActuallySpentItem>();
                if (listActuallyItemNew.Any())
                {
                    foreach (var searchActuallySpentResponse in listActuallyItemNew)
                    {
                        var newSpent = _mapper.Map<ActuallySpentItem>(searchActuallySpentResponse);
                        newSpent.Id = Guid.NewGuid();
                        if (string.IsNullOrEmpty(newSpent.RequestCode))
                        {
                            newSpent.RequestPayWeekName = "Phát sinh ngoài dự trù";
                            newSpent.ActualSpentType = "Out";
                        }
                        else
                            newSpent.ActualSpentType = "In";

                        listActuallySpentItems.Add(newSpent);
                    }

                    await _ctx.ActuallySpentItem.AddRangeAsync(listActuallySpentItems);
                    await _ctx.SaveChangesAsync();

                    #region Đẩy vào bảng map
                    var listMap = new List<ActuallySpentMapItem>();
                    foreach (var actuallySpentItem in listActuallySpentItems)
                    {
                        listMap.Add(new ActuallySpentMapItem
                        {
                            CreatorName = request.CreatorName,
                            Creator = request.Creator,
                            ActuallySpentId = request.RawId,
                            ActuallySpentItemId = actuallySpentItem.Id,
                            CreatedDate = DateTime.Now,
                            Id = Guid.NewGuid(),
                        });
                    }

                    await _ctx.ActuallySpentMapItem.AddRangeAsync(listMap);
                    await _ctx.SaveChangesAsync();
                    #endregion
                }
                #endregion

                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    UserName = request.CreatorName,
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Edit",
                    FunctionUnique = Functions.ActuallySpentCreate,
                    UserId = request.Creator
                });

                await transaction.CommitAsync();
                return new CreateActuallyResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    Record = request.Record,
                    Message = "Chỉnh sửa báo cáo thực chi thành công!"
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "Error", request);

                await transaction.RollbackToSavepointAsync("Before");
                return new CreateActuallyResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error,
                    Record = request.Record,
                    Message = "Chỉnh sửa báo cáo thực chi không thành công!"
                };
            }
        }
        /// <summary>
        /// Chi tiết thực chi theo ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateActuallySpentRequest> GetByIdAsync(GetActuallySpentByIdRequest request)
        {
            try
            {
                var response = new CreateActuallySpentRequest();
                var query = await _ctx.ActuallySpent.FirstOrDefaultAsync(x =>
                    x.Id == request.RawId && (x.UnitId == request.UnitId || request.UnitId == (int)GlobalEnums.StatusDefaultEnum.All));
                if (query != null)
                {
                    response.ReportForWeek = query.ReportForWeek;
                    response.ReportForWeekName = query.ReportForWeekName;
                    response.Status = query.Status;

                    var joinQuery = await _ctx.ActuallySpentItem.
                        Join(_ctx.ActuallySpentMapItem, u => u.Id,
                        w => w.ActuallySpentItemId, (x, y) => new
                        {
                            ActuallySpentMapItem = x,
                            y.ActuallySpentId,

                        }).Where(p => p.ActuallySpentId == request.RawId).ToListAsync();

                    var rangeData = new List<ActuallySpentItemResponse>();

                    foreach (var j in joinQuery)
                    {
                        var item = _mapper.Map<ActuallySpentItem, ActuallySpentItemResponse>(j.ActuallySpentMapItem);
                        item.Record = j.ActuallySpentMapItem.Id.ToString().StringAesEncryption(request.PageRequest);
                        rangeData.Add(item);
                    }
                    response.Data = rangeData.OrderBy(x => x.ActualSpentType).ToList();
                    return response;
                }

                return null;
            }
            catch (Exception e)
            {

                Log.Error(e, "Error", request);

                return null;
            }
        }
        /// <summary>
        /// Phê duyệt từ chối
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActuallySpentApproveResponse> Approval(ActuallySpentApproveRequest request)
        {
            try
            {
                var response = new ActuallySpentApproveResponse();

                #region Query
                var searchQuery = from aSpent in _ctx.ActuallySpent
                                  join map in _ctx.ActuallySpentMapItem on aSpent.Id equals map.ActuallySpentId
                                  join actuallySpentItem in _ctx.ActuallySpentItem on map.ActuallySpentItemId equals actuallySpentItem.Id
                                  join u in _ctx.Units on aSpent.UnitId equals u.Id

                                  select new
                                  {
                                      ActuallySpent = aSpent,
                                      ActuallySpentItem = actuallySpentItem,
                                      u.OfficesSub,
                                      u.OfficesCode
                                  };
                var dataRecords = await searchQuery.
                    Where(x => x.ActuallySpent.Id == request.RawId && (x.ActuallySpent.UnitId == request.UnitId || request.UnitId == (int)GlobalEnums.StatusDefaultEnum.All)).ToListAsync();
                if (!dataRecords.Any())
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = "Không tìm thấy dữ liệu yêu cầu!";
                    return response;
                }

                // lấy ra mã trạng thái hiện tại
                var statusData = request.StatusAllowsSeen.FirstOrDefault(x => x.Value == dataRecords[0].ActuallySpent.Status);
                int intCurrentStats = statusData?.Value ?? -5000;

                if (statusData == null || intCurrentStats == -5000)
                {
                    // không tìm thấy trạng thái hiện tại -> lỗi
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.UnAuthor;
                    response.Message = "Bạn không có quyền thực hiện thao tác, vui lòng liên hệ với quản trị viên!";

                    return response;
                }

                var next = new NextStatExtension(await _costStatusesRepository.GetAll(), request.StatusAllowsSeen.ToList(), GlobalEnums.ActuallySpent, GlobalEnums.Week, "BC Thực chi");
                string rcUnit = dataRecords[0].OfficesSub.Equals("YT", StringComparison.CurrentCultureIgnoreCase)
                    ? GlobalEnums.UnitIn
                    : GlobalEnums.UnitOut;
                var allSpecialUnitFollowConfigs = await _costStatusesRepository.GetSpecialUnitFollowConfigs();
                var fSpecial = allSpecialUnitFollowConfigs.FirstOrDefault(g =>
                    g.UnitCode.Equals(dataRecords[0].OfficesCode, StringComparison.CurrentCultureIgnoreCase));

                var oNext = next._getNext(dataRecords[0].ActuallySpent.Status, request.IsApproval, dataRecords[0].ActuallySpent.IsSub, rcUnit, fSpecial?.UnitCode);
                if (!oNext.NextValid)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = oNext.Message;
                    return response;
                }

                var dataRecord = dataRecords[0];
                #endregion

                var oGroup = await _costStatusesRepository.GetUsedByGroup(oNext.Next.Id);

                await using var transaction = _ctx.Database.BeginTransaction();
                try
                {

                    await transaction.CreateSavepointAsync("Before");
                    var recordUpdate = dataRecord.ActuallySpent;
                    recordUpdate.Status = oNext.Next.Value;
                    recordUpdate.StatusName = oNext.Next.Name;

                    _ctx.ActuallySpent.Update(recordUpdate);
                    await _ctx.SaveChangesAsync();


                    #region Chèn vào bảng lịch sử

                    await _ctx.ActuallySpentLog.AddAsync(new ActuallySpentLog
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        ActuallySpentId = request.RawId,
                        ToStatus = oNext.Next.Value,
                        ToStatusName = oNext.Next.Name,
                        Reason = !request.IsApproval ? request.Reason : string.Empty,
                        CreatorName = request.UserName,
                        Creator = request.UserId,
                        FromStatus = dataRecord.ActuallySpent.Status,
                        PositionName = oGroup?.Name,
                        PositionId = oGroup?.Id ?? 0
                    });
                    await _ctx.SaveChangesAsync();
                    #endregion


                    #region Trường hợp phê duyệt ở bước cuối --> đẩy vào bảng map giữa CostEstimate & ActuallyItem để xác định y.c nào đã được chi

                    if (oNext.IsMaxOfFollow)
                    {
                        // lấy danh sách các yêu cầu
                        // để xác định yêu cầu đã được chi hết tiền hay chưa
                        // có trường hợp đã chi nhưng chưa đủ

                        var listCodeOfDataRecords = dataRecords.Where(x => !string.IsNullOrEmpty(x.ActuallySpentItem.RequestCode)).Select(x => x.ActuallySpentItem.RequestCode).ToList();

                        var queriesCostItem = await _ctx.CostEstimateItems.Where(x =>
                            listCodeOfDataRecords.Any(p => p == x.RequestCode)).ToListAsync();
                        var listRecordSave = new List<CostEstimateItemMapActuallySpentItem>();
                        // danh sách các mã dự trù được thanh toán 
                        var listRequestCodes = dataRecords
                            .Where(x => !string.IsNullOrEmpty(x.ActuallySpentItem.RequestCode))
                            .Select(x => x.ActuallySpentItem.RequestCode).ToList();


                        var listOldSpent = new List<ActuallySpentItem>();

                        // tìm xem có thực chi nào đã được chi ở các tuần trước
                        // nếu có sẽ lũy kế vào bản ghi hiện có
                        if (listRequestCodes.Any())
                        {
                            var listOldActually = await
                                _ctx.CostEstimateItemMapActuallySpentItem.Join(_ctx.ActuallySpent, w => w.ActuallySpentId, y => y.Id,
                                        (x, y) => new
                                        {
                                            CostEstimateItemMapActuallySpentItem = x,
                                            y.Status
                                        })

                                    .Where(x =>
                                    listRequestCodes.Contains(x.CostEstimateItemMapActuallySpentItem.RequestCode)
                                    && x.Status == (int)GlobalEnums.StatusDefaultEnum.Active).ToListAsync();
                            if (listOldActually != null && listOldActually.Any())
                            {
                                var listSpentIds = listOldActually.Select(x => x.CostEstimateItemMapActuallySpentItem.ActuallySpentItemId);
                                listOldSpent = await _ctx.ActuallySpentItem.Where(x => listSpentIds.Contains(x.Id)).ToListAsync();
                            }
                        }

                        foreach (var record in dataRecords)
                        {
                            var recordEstimate = queriesCostItem.FirstOrDefault(x =>
                                x.RequestCode == record.ActuallySpentItem.RequestCode);

                            // cộng tổng các thực chi trước
                            var oldSpentByRequestCode = listOldSpent.Where(c => c.RequestCode == record.ActuallySpentItem.RequestCode).ToList();
                            long oldActuallySpent = 0;
                            if (oldSpentByRequestCode.Any())
                                oldActuallySpent = oldSpentByRequestCode.Sum(m => m.ActualSpent);
                            // cộng thực chi trước
                            // với thực chi lần này
                            oldActuallySpent = oldActuallySpent + record.ActuallySpentItem.ActualSpent;

                            int statusActual = oldActuallySpent == 0 ? (int)GlobalEnums.StatusActuallyEnum.Wip :
                                recordEstimate?.Cost <= oldActuallySpent ? (int)GlobalEnums.StatusActuallyEnum.Done : (int)GlobalEnums.StatusActuallyEnum.NotDone;

                            listRecordSave.Add(new CostEstimateItemMapActuallySpentItem
                            {
                                Status = statusActual,
                                Id = Guid.NewGuid(),
                                ActuallySpentId = dataRecord.ActuallySpent.Id,
                                ActuallySpentItemId = record.ActuallySpentItem.Id,
                                StatusName = GlobalEnums.StatusActuallyNames.TryGetValue(statusActual, out var name) ? name : string.Empty,
                                RequestCode = record.ActuallySpentItem.RequestCode,
                                CostEstimateItemId = recordEstimate?.Id ?? Guid.Empty
                            });
                        }
                        await _ctx.CostEstimateItemMapActuallySpentItem.AddRangeAsync(listRecordSave);
                        await _ctx.SaveChangesAsync();
                    }


                    #endregion

                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        UserName = request.UserName,
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Approval",
                        FunctionUnique = Functions.ActuallySpentView,
                        UserId = request.UserId
                    });

                    var data = _mapper.Map<ActuallySpent, SearchActuallySpentData>(recordUpdate);
                    data.Status = recordUpdate.StatusName;
                    data.Record = recordUpdate.Id.ToString().StringAesEncryption(request.PageRequest);
                    data.Viewable = true;
                    data.Editable = false;
                    data.Approvalable = false;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = request.IsApproval
                        ? "Phê duyệt báo cáo thực chi thành công!"
                        : "Từ chối báo cáo thực chi thành công!";


                    response.Data = data;
                    await transaction.CommitAsync();
                    return response;
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error", request);
                    await transaction.ReleaseSavepointAsync("Before");
                    return new ActuallySpentApproveResponse
                    {
                        Message = request.IsApproval ? "Phê duyệt báo cáo không thành công!" : "Từ chối báo cáo không thành công!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error", request);
                return new ActuallySpentApproveResponse
                {
                    Message = request.IsApproval ? "Phê duyệt báo cáo không thành công!" : "Từ chối báo cáo không thành công!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
        }

        public async Task<ActuallySpent> GetActuallySpentApprovedByWeek(int week, int maxFollowStats)
        {
            try
            {
                return await _ctx.ActuallySpent.FirstOrDefaultAsync(x =>
                    x.ReportForWeek == week && x.Status == maxFollowStats);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<ActuallySpent>> GetAllByRangeDate(int startWeek, int endWeek, int year, int unit, int maxFollowStats)
        {
            try
            {
                var getAllSpentApproved = await _ctx.ActuallySpent.Where(pri =>
                    pri.ReportForWeek - 1 >= startWeek && pri.ReportForWeek - 1 <= endWeek &&
                    pri.CreatedDate.Year == year && pri.Status == maxFollowStats && pri.UnitId == unit).ToListAsync();
                return getAllSpentApproved;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error"); 
                return null;
            }
        }

        public async Task<IList<SctData>> GetAllSctDataByActuallyIds(IEnumerable<Guid> ids)
        {
            try
            {
                var rawData = await _ctx.SctData.Where(x => ids.Contains(x.ActuallySpentId) && (x.AccountReciprocalNumber == 1311 || x.AccountReciprocalNumber == 511)).ToListAsync();
                return rawData;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<ActuallySpentItem>> GetAllSpentItemsByActuallyIds(IEnumerable<Guid> ids)
        {
            try
            {
                var query = from item in _ctx.ActuallySpentItem
                            join map in _ctx.ActuallySpentMapItem on item.Id equals map.ActuallySpentItemId
                            where ids.Contains(map.ActuallySpentId)
                            select new { ActuallySpentItem = item };
                var data = await query.Select(x => x.ActuallySpentItem).ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<ActuallyLogResponse>> ViewHistories(ActuallyLogRequest record)
        {
            try
            {
                // loại bỏ trạng thái tạo mới / chỉnh sửa
                var query = await _ctx.ActuallySpentLog.Where(x => x.ActuallySpentId == record.RawId).OrderByDescending(x => x.CreatedDate).ToListAsync();
                var data = query.Select(x => new ActuallyLogResponse
                {
                    UserName = x.CreatorName,
                    PositionName = x.PositionName,
                    Reason = x.Reason,
                    TimeChange = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    Status = x.ToStatusName
                }).ToList();
                return data;

            }
            catch (Exception e)
            {
                Log.Error(e, "Error", record);
                return new List<ActuallyLogResponse>();
            }
        }

    }
}
