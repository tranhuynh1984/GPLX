using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.TBL_CTVGROUPSUB
{
    public class TBL_CTVGROUPSUBRepository : ITBL_CTVGROUPSUBRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TBL_CTVGROUPSUBRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public TBL_CTVGROUPSUBRepository(Context context, IMapper mapper, ILogger<TBL_CTVGROUPSUBRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<TBL_CTVGROUPSUBSearchResponse> Search(int skip, int length, TBL_CTVGROUPSUBSearchRequest request)
        {
            var response = new TBL_CTVGROUPSUBSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUPSUB.AsNoTracking();

            if (request.SubId != 0)
                query = query.Where(x => x.SubId.ToString().Contains(request.SubId.ToString().ToLower()));
            if (!string.IsNullOrEmpty(request.SubName))
                query = query.Where(x => x.SubName.Contains(request.SubName.Trim().ToLower()));
            if (request.IsUse != -1)
                query = query.Where(x => x.IsUse == request.IsUse);
            if (request.CTVGroupID != -1)
                query = query.Where(x => x.CTVGroupID == request.CTVGroupID);

            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSUBSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSUBSearchResponseData>(d);
                dMap.IsUseName = GlobalEnums.GetStatusName(dMap.IsUse);
                if (dMap.CTVGroupID == 1)
                    dMap.CTVGroupName = "Nhóm I: Bác sĩ";
                else if (dMap.CTVGroupID == 2)
                    dMap.CTVGroupName = "Nhóm II: CTV ngành Y";
                else if (dMap.CTVGroupID == 3)
                    dMap.CTVGroupName = "Nhóm III: CTV ngoài ngành Y";
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSUBSearchResponse> GetAllDM()
        {
            var response = new TBL_CTVGROUPSUBSearchResponse { };

            var query = _context.TBL_CTVGROUPSUB.AsQueryable();
            var data = await query.OrderBy(x => x.SubName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSUBSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSUBSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSUBSearchResponse> SearchAll(TBL_CTVGROUPSUBSearchRequest request)
        {
            var response = new TBL_CTVGROUPSUBSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUPSUB.AsQueryable();
            if (request.SubId != 0)
                query = query.Where(x => x.SubId.ToString().Contains(request.SubId.ToString().ToLower()));
            if (!string.IsNullOrEmpty(request.SubName))
                query = query.Where(x => x.SubName.Contains(request.SubName.Trim().ToLower()));

            var data = await query.OrderBy(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSUBSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<TBL_CTVGROUPSUBSearchResponseData>(d);

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }
        public async Task<TBL_CTVGROUPSUBSearchResponseData> GetById(int subId)
        {
            var record = await _context.TBL_CTVGROUPSUB.FirstOrDefaultAsync(x => x.SubId == subId);
            if (record == null)
                return null;
            var response = _mapper.Map<TBL_CTVGROUPSUBSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TBL_CTVGROUPSUBCreateResponse> Create(TBL_CTVGROUPSUBCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUBCreateResponse();

            response = Validation(request);

            request.SubName = String.IsNullOrEmpty(request.SubName) ? "" : request.SubName.Trim();
            request.Note = String.IsNullOrEmpty(request.Note) ? "" : request.Note.Trim();
            request.WhyNotUse = String.IsNullOrEmpty(request.WhyNotUse) ? "" : request.WhyNotUse.Trim();

            if (response.ListError.Count == 0)
            {   
                var query = _context.TBL_CTVGROUPSUB.AsQueryable();
                query = query.Where(x => x.SubId == request.SubId);
                var data = await query.OrderBy(x => x.SubName).ToListAsync();

                if (request.SubId == 0)
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Mã hợp đồng đã tồn tại trong hệ thống.";
                    }
                    else
                    {
                        int maxSubId = 0;
                        if(_context.TBL_CTVGROUPSUB.Count()>0)
                            maxSubId = await _context.TBL_CTVGROUPSUB.MaxAsync(x => x.SubId);
                        //Add New
                        var item = new Database.Models.TBL_CTVGROUPSUB();

                        item.SubId = maxSubId + 1;
                        item.CTVGroupID = request.CTVGroupID;
                        item.SubName = request.SubName;
                        item.FromDate = request.FromDate;
                        item.ToDate = request.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                        item.Note = request.Note;
                        item.IsUse = request.IsUse;
                        item.WhyNotUse = request.WhyNotUse;
                        item.DateI = request.DateI;
                        item.UserI = request.UserI;
                        item.DisCount = request.DisCount;
                        item.CustomerPrice = request.CustomerPrice;

                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        _context.TBL_CTVGROUPSUB.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "TBL_CTVGROUPSUB",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.SubId = item.SubId;
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin hợp đồng thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.SubId = request.SubId;
                    item.CTVGroupID = request.CTVGroupID;
                    item.SubName = request.SubName;
                    item.FromDate = request.FromDate;
                    item.ToDate = request.ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    item.Note = request.Note;
                    item.IsUse = request.IsUse;
                    item.WhyNotUse = request.WhyNotUse;
                    item.DateI = request.DateI;
                    item.UserI = request.UserI;
                    item.DisCount = request.DisCount;
                    item.CustomerPrice = request.CustomerPrice;
                    item.Updateby = request.CreatorName;
                    item.Updatedate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Edit",
                        FunctionUnique = "TBL_CTVGROUPSUB",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.SubId = item.SubId;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin hợp đồng thành công!";
                }
            }

            if (response.ListError.Count > 0)
            {
                foreach (var item in response.ListError)
                {
                    response.Message = response.Message + "</br>" + item.Message;
                }

                response.Message = response.Message.Substring(5);
            }

            return response;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TBL_CTVGROUPSUBCreateResponse> Remove(TBL_CTVGROUPSUBCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUBCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.TBL_CTVGROUPSUB.AsQueryable();
                query = query.Where(x => x.SubId == request.SubId);
                var data = await query.OrderBy(x => x.SubName).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.TBL_CTVGROUPSUB.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "TBL_CTVGROUPSUB",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin hợp đồng thành công!";
                }
            }

            return response;
        }

        public TBL_CTVGROUPSUBCreateResponse Validation(TBL_CTVGROUPSUBCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUBCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.SubName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenHD", Message = "Tên hợp đồng không được trống." });
            }
            else
            {
                if (request.SubName.Length > 300)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txttenHD", Message = "Tên hợp đồng có độ dài vượt quá số ký tự cho phép." });
                }
                //if (!GlobalEnums.ValidateTextSpeciallyHDCTV(request.SubName))
                //{
                //    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                //    response.ListError.Add(new ItemError { FieldError = "txttenHD", Message = "Tên hợp đồng có ký tự đặc biệt." });
                //}
            }

            if (!string.IsNullOrEmpty(request.Note))
            {
                if (request.Note.Length > 3000)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txtNote", Message = "Ghi chú có độ dài vượt quá số ký tự cho phép." });

                }
                //if (!GlobalEnums.ValidateTextSpecially(request.Note))
                //{
                //    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                //    response.ListError.Add(new ItemError { FieldError = "txtNote", Message = "Ghi chú có ký tự đặc biệt." });
                //}
            }

            if (!string.IsNullOrEmpty(request.WhyNotUse))
            {
                if (request.WhyNotUse.Length > 2000)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txtlydo", Message = "Lý do có độ dài vượt quá số ký tự cho phép." });
                }

                //if (!GlobalEnums.ValidateTextSpecially(request.WhyNotUse))
                //{
                //    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                //    response.ListError.Add(new ItemError { FieldError = "txtlydo", Message = "Lý do có ký tự đặc biệt." });
                //}
            }

            if (request.FromDate == DateTime.MinValue)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtfromdate", Message = "Giá trị của thời gian bắt đầu không đúng." });
            }

            if (request.ToDate == DateTime.MinValue)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttodate", Message = "Giá trị của thời gian kết thúc không đúng." });
            }

            if (request.ToDate < request.FromDate)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtfromdate", Message = "Chọn lại giá trị của thời gian bắt đầu và thời gian kết thúc." });
                response.ListError.Add(new ItemError { FieldError = "txttodate", Message = "" });
            }

           if (request.DisCount != 0)
            {
                // Create a pattern for a word that starts with letter "M"  
                string pattern = @"^[0-9]*(?:\.[0-9]*)?$";
                // Create a Regex  
                Regex rg = new Regex(pattern);
                if(!rg.IsMatch(request.DisCount.ToString()))
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txttyle", Message = "Chọn lại giá trị của tỷ lệ." });
                }
            }

            if (request.CustomerPrice != 0)
            {
                // Create a pattern for a word that starts with letter "M"  
                string pattern = @"^[0-9]*(?:\.[0-9]*)?$";
                // Create a Regex  
                Regex rg = new Regex(pattern);
                if (!rg.IsMatch(request.CustomerPrice.ToString()))
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txtGiaKH", Message = "Chọn lại giá khách hàng." });
                }
            }

            return response;
        }
    }
}
