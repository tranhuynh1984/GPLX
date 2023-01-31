using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Request.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.DTO.Response.HDCTV;
using GPLX.Core.DTO.Response.TBL_CTVGROUPSUB1_DETAIL;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.TBL_CTVGROUPSUB1_DETAIL
{
    public class TBL_CTVGROUPSUB1_DETAILRepository : ITBL_CTVGROUPSUB1_DETAILRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TBL_CTVGROUPSUB1_DETAILRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public TBL_CTVGROUPSUB1_DETAILRepository(Context context, IMapper mapper, ILogger<TBL_CTVGROUPSUB1_DETAILRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<TBL_CTVGROUPSUB1_DETAILSearchResponse> Search(int skip, int length, TBL_CTVGROUPSUB1_DETAILSearchRequest request)
        {
            var response = new TBL_CTVGROUPSUB1_DETAILSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsNoTracking();

            query = query.Where(x => x.SubId == request.SubId);

            var data = await query.OrderBy(x => x.MaCP).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSUB1_DETAILSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSUB1_DETAILSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<TBL_CTVGROUPSUB1_DETAILSearchResponse> SearchAll(TBL_CTVGROUPSUB1_DETAILSearchRequest request)
        {
            var response = new TBL_CTVGROUPSUB1_DETAILSearchResponse { Draw = request.Draw };

            var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable();
            if (request.SubId != 0)
                query = query.Where(x => x.SubId.ToString().Contains(request.SubId.ToString().ToLower()));
            if (!string.IsNullOrEmpty(request.MaCP))
                query = query.Where(x => x.MaCP.Contains(request.MaCP.Trim().ToLower()));

            var data = await query.OrderBy(x => x.MaCP).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<TBL_CTVGROUPSUB1_DETAILSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<TBL_CTVGROUPSUB1_DETAILSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }
        public async Task<TBL_CTVGROUPSUB1_DETAILSearchResponseData> GetById(int subId, string maCP)
        {
            var record = await _context.TBL_CTVGROUPSUB1_DETAIL.FirstOrDefaultAsync(x => x.MaCP == maCP && x.SubId == subId);
            if (record == null)
                return null;
            var response = _mapper.Map<TBL_CTVGROUPSUB1_DETAILSearchResponseData>(record);
            return response;
        }

        public async Task<ImportToDBResponse> CreateRange(HdctvImportExcel<HdctvType1UploadResponse> request)
        {
            var response = new ImportToDBResponse();
            if (request?.ImportData == null) return response;
            
            var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable();
            query = query.Where(x => x.SubId == request.SubId);
            var existedDataByMaCp = await query.ToDictionaryAsync(x => x.MaCP, x => x);
            foreach (var importData in request.ImportData)
            {
                if (string.IsNullOrEmpty(importData.MaCP))
                {
                    response.AddErrorItem(new ImportDBErrorObj
                    {
                        Key = importData.MaCP,
                        Reason = "Mã không được để trống!"
                    });
                    continue;
                }
                if (importData.MaCP.Length > 50)
                {
                    response.AddErrorItem(new ImportDBErrorObj
                    {
                        Key = importData.MaCP,
                        Reason = "Mã có độ dài vượt quá số ký tự cho phép!"
                    });
                    continue;
                }
                if (existedDataByMaCp.ContainsKey(importData.MaCP))
                {
                    response.AddErrorItem(new ImportDBErrorObj
                    {
                        Key = importData.MaCP,
                        Reason = "Mã đã tồn tại!"
                    });
                    continue;
                }
                var item = new Database.Models.TBL_CTVGROUPSUB1_DETAIL
                {
                    SubId = request.SubId,
                    MaCP = importData.MaCP,
                    TenCP = importData.TenCP,
                    BP1 = importData.BP1,
                    BP2 = importData.BP2,
                    BP3 = importData.BP3,
                    BP4 = importData.BP4,
                    BP5 = importData.BP5,
                    BP6 = importData.BP6,
                    BP7 = importData.BP7,
                    BP8 = importData.BP8,
                    BP9 = importData.BP9,
                    BP10 = importData.BP10,
                    BP11 = importData.BP11,
                    IsActive = importData.IsActive,
                    Createby = request.CreatedUserName,
                    Createdate = DateTime.Now
                };
                
                _context.TBL_CTVGROUPSUB1_DETAIL.Add(item);
                response.AddSuccessItem(importData);
            }
            await _context.SaveChangesAsync();
            await _actionLogsRepository.AddLogAsync(new ActionLogs
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UserName = request.CreatedUserName,
                Content = JsonConvert.SerializeObject(request),
                Action = "Add",
                FunctionUnique = "TBL_CTVGROUPSUB1_DETAIL",
                UserId = request.CreatedUserId
            }).ConfigureAwait(false);
            
            return response;
        }

        public async Task<List<TBL_CTVGROUPSUB1_DETAILSearchResponseData>> GetAllBySubId(int subId)
        {
            var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable()
                .Where(x => x.SubId == subId);

            var results = await query.ToListAsync();
            return results.Select((x, i)  =>
            {
                var dMap = _mapper.Map<TBL_CTVGROUPSUB1_DETAILSearchResponseData>(x);
                dMap.Index = i + 1;
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                return dMap;
            }).ToList();
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TBL_CTVGROUPSUB1_DETAILCreateResponse> Create(TBL_CTVGROUPSUB1_DETAILCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUB1_DETAILCreateResponse();

            response = Validation(request);

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable();

                if (string.IsNullOrEmpty(request.Record))
                {
                    query = query.Where(x => x.SubId == request.SubId && x.MaCP == request.MaCP);
                    var data = await query.OrderBy(x => x.MaCP).ToListAsync();
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = "Mã dịch vụ đã tồn tại trong hệ thống.";
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.TBL_CTVGROUPSUB1_DETAIL();

                        item.SubId = request.SubId;
                        item.MaCP = request.MaCP;
                        item.TenCP = request.TenCP;
                        item.BP1 = request.BP1;
                        item.BP2 = request.BP2;
                        item.BP3 = request.BP3;
                        item.BP4 = request.BP4;
                        item.BP5 = request.BP5;
                        item.BP6 = request.BP6;
                        item.BP7 = request.BP7;
                        item.BP8 = request.BP8;
                        item.BP9 = request.BP9;
                        item.BP10 = request.BP10;
                        item.BP11 = request.BP11;
                        item.IsActive = request.IsActive;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        _context.TBL_CTVGROUPSUB1_DETAIL.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "TBL_CTVGROUPSUB1_DETAIL",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin dịch vụ thành công!";
                    }
                }
                else
                {
                    query = query.Where(x => x.SubId == request.SubId && x.MaCP == request.Record);
                    var data = await query.OrderBy(x => x.MaCP).ToListAsync();
                    if (request.MaCP.Equals(request.Record))
                    {
                        //Edit
                        var item = data.FirstOrDefault();
                        item.SubId = request.SubId;
                        item.MaCP = request.MaCP;
                        item.TenCP = request.TenCP;
                        item.BP1 = request.BP1;
                        item.BP2 = request.BP2;
                        item.BP3 = request.BP3;
                        item.BP4 = request.BP4;
                        item.BP5 = request.BP5;
                        item.BP6 = request.BP6;
                        item.BP7 = request.BP7;
                        item.BP8 = request.BP8;
                        item.BP9 = request.BP9;
                        item.BP10 = request.BP10;
                        item.BP11 = request.BP11;
                        item.IsActive = request.IsActive;
                        item.Updateby = request.CreatorName;
                        item.Updatedate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var query1 = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable();
                        query1 = query1.Where(x => x.SubId == request.SubId && x.MaCP == request.MaCP);
                        var data1 = await query1.OrderBy(x => x.MaCP).ToListAsync();
                        if (data1.Count > 0)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = "Mã dịch vụ đã tồn tại trong hệ thống.";
                            return response;
                        }
                        else
                        {
                            //Remove CP old
                            var item = data.FirstOrDefault();
                            _context.TBL_CTVGROUPSUB1_DETAIL.Remove(item);
                            await _context.SaveChangesAsync();

                            //Add New
                            item = new Database.Models.TBL_CTVGROUPSUB1_DETAIL();

                            item.SubId = request.SubId;
                            item.MaCP = request.MaCP;
                            item.TenCP = request.TenCP;
                            item.BP1 = request.BP1;
                            item.BP2 = request.BP2;
                            item.BP3 = request.BP3;
                            item.BP4 = request.BP4;
                            item.BP5 = request.BP5;
                            item.BP6 = request.BP6;
                            item.BP7 = request.BP7;
                            item.BP8 = request.BP8;
                            item.BP9 = request.BP9;
                            item.BP10 = request.BP10;
                            item.BP11 = request.BP11;
                            item.IsActive = request.IsActive;
                            item.Createby = request.CreatorName;
                            item.Createdate = DateTime.Now;
                            _context.TBL_CTVGROUPSUB1_DETAIL.Add(item);
                            await _context.SaveChangesAsync();
                        }
                    }

                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Edit",
                        FunctionUnique = "TBL_CTVGROUPSUB1_DETAIL",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin dịch vụ thành công!";
                }
            }

            return response;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TBL_CTVGROUPSUB1_DETAILCreateResponse> Remove(TBL_CTVGROUPSUB1_DETAILCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUB1_DETAILCreateResponse();
            
            var query = _context.TBL_CTVGROUPSUB1_DETAIL.AsQueryable();
            query = query.Where(x => x.SubId == request.SubId && x.MaCP == request.MaCP);
            var data = await query.OrderBy(x => x.MaCP).ToListAsync();

            if (data.Count > 0)
            {
                //Remove
                var item = data.FirstOrDefault();
                _context.TBL_CTVGROUPSUB1_DETAIL.Remove(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Delete",
                    FunctionUnique = "TBL_CTVGROUPSUB1_DETAIL",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Xóa thông tin dịch vụ thành công!";
            }

            return response;
        }

        public TBL_CTVGROUPSUB1_DETAILCreateResponse Validation(TBL_CTVGROUPSUB1_DETAILCreateRequest request)
        {
            var response = new TBL_CTVGROUPSUB1_DETAILCreateResponse();

            if (string.IsNullOrEmpty(request.MaCP))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "TBL_CTVGROUPSUB1_DETAIL không được trống.";
            }
            else if (request.MaCP.Length > 50)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.Message = "TBL_CTVGROUPSUB1_DETAIL có độ dài vượt quá số ký tự cho phép.";
            }

            return response;
        }
    }
}
