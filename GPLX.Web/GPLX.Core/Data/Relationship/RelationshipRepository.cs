using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Relationship;
using GPLX.Core.DTO.Request.Relationship;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.Relationship;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.Relationship
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RelationshipRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public RelationshipRepository(Context context, IMapper mapper, ILogger<RelationshipRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<RelationshipSearchResponse> Search(int skip, int length, RelationshipSearchRequest request)
        {
            var response = new RelationshipSearchResponse { Draw = request.Draw };

            var query = _context.Relationship.AsNoTracking();
            if (!string.IsNullOrEmpty(request.RelationshipCode))
                query = query.Where(x => x.RelationshipCode.Contains(request.RelationshipCode.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.RelationshipName))
                query = query.Where(x => x.RelationshipName.Contains(request.RelationshipName.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);


            var data = await query.OrderBy(x => x.RelationshipName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<RelationshipSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<RelationshipSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<RelationshipSearchResponse> GetAllRelationShip()
        {
            var response = new RelationshipSearchResponse { };

            var query = _context.Relationship.AsQueryable();
            query = query.Where(x => x.IsActive == 1);
            var data = await query.OrderBy(x => x.RelationshipName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<RelationshipSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<RelationshipSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<RelationshipSearchResponse> SearchAll(RelationshipSearchRequest request)
        {
            var response = new RelationshipSearchResponse { Draw = request.Draw };

            var query = _context.Relationship.AsQueryable();
            if (!string.IsNullOrEmpty(request.RelationshipCode))
                query = query.Where(x => x.RelationshipCode.Contains(request.RelationshipCode.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.RelationshipName))
                query = query.Where(x => x.RelationshipName.Contains(request.RelationshipName.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderBy(x => x.RelationshipName).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<RelationshipSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = _mapper.Map<RelationshipSearchResponseData>(d);
                dMap.Index = i + 1;
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dMap.CreatedateString = dMap.Createdate.ToString("HH:mm dd/MM/yyyy");
                dMap.UpdatedateString = dMap.Updatedate?.ToString("HH:mm dd/MM/yyyy");

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<RelationshipSearchResponseData> GetById(string RelationshipCode)
        {
            var record = await _context.Relationship.FirstOrDefaultAsync(x => x.RelationshipCode == RelationshipCode);
            if (record == null)
                return null;
            var response = _mapper.Map<RelationshipSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RelationshipCreateResponse> Create(RelationshipCreateRequest request)
        {
            var response = new RelationshipCreateResponse();

            response = Validation(request);

            request.RelationshipCode = String.IsNullOrEmpty(request.RelationshipCode) ? "" : request.RelationshipCode.Trim();
            request.RelationshipName = String.IsNullOrEmpty(request.RelationshipName) ? "" : request.RelationshipName.Trim();

            if (response.ListError.Count == 0)
            {
                var query = _context.Relationship.AsQueryable();
                query = query.Where(x => x.RelationshipCode == request.RelationshipCode);
                var data = await query.OrderBy(x => x.RelationshipName).ToListAsync();

                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.ListError.Add(new ItemError { FieldError = "txtmaRelationship", Message = "Mã quan hệ đã tồn tại trong hệ thống." });
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.Relationship();
                        item.RelationshipCode = request.RelationshipCode;
                        item.RelationshipName = request.RelationshipName;
                        item.IsActive = request.IsActive;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        item.Stt = request.Stt;
                        _context.Relationship.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "Relationship",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin quan hệ thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.RelationshipCode = request.RelationshipCode;
                    item.RelationshipName = request.RelationshipName;
                    item.IsActive = request.IsActive;
                    item.Updateby = request.CreatorName;
                    item.Updatedate = DateTime.Now;
                    item.Stt = request.Stt;
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Edit",
                        FunctionUnique = "Relationship",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin quan hệ thành công!";
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
        public async Task<RelationshipCreateResponse> Remove(RelationshipCreateRequest request)
        {
            var response = new RelationshipCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.Relationship.AsQueryable();
                query = query.Where(x => x.RelationshipCode == request.RelationshipCode);
                var data = await query.OrderBy(x => x.RelationshipName).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.Relationship.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "Relationship",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa thông tin quan hệ thành công!";
                }
            }

            return response;
        }

        public RelationshipCreateResponse Validation(RelationshipCreateRequest request)
        {
            var response = new RelationshipCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.RelationshipCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaRelationship", Message = "Mã quan hệ không được trống." });
            }
            else if (request.RelationshipCode.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmaRelationship", Message = "Mã quan hệ không được có khoảng trắng." });
            }
            else if (request.RelationshipCode.Length > 20)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaRelationship", Message = "Mã quan hệ có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.RelationshipCode))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmaRelationship", Message = "Mã quan hệ có ký tự đặc biệt." });
            }
            if (string.IsNullOrEmpty(request.RelationshipName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenRelationship", Message = "Tên quan hệ không được trống." });
            }
            else if (request.RelationshipName.Length > 200)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenRelationship", Message = "Tên quan hệ có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.RelationshipName))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenRelationship", Message = "Tên quan hệ có ký tự đặc biệt." });
            }
            if (request.Stt == 0)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtstt", Message = "Thứ tự quan hệ không được bỏ trống." });
            }

            return response;
        }
        public async Task<int> GetMaxStt()
        {
            int maxstt = 0;
            maxstt = await _context.Relationship.MaxAsync(e => e.Stt) + 1;
            return maxstt;
        }
    }
}
