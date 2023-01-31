using GPLX.Core.Contracts.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.DTO.Request.Department;
using GPLX.Core.DTO.Response.Department;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GPLX.Core.Data.Department
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ILogger<DepartmentRepository> _logger;
        private readonly Context _ctx;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DepartmentRepository(ILogger<DepartmentRepository> logger, Context ctx, IMapper mapper, IActionLogsRepository actionLogsRepository)
        {
            _logger = logger;
            _ctx = ctx;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
        }
        /// <summary>
        /// Tìm kiếm
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="size"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DepartmentSearchResponse> SearchAsync(int skip, int size, DepartmentSearchRequest request)
        {
            var response = new DepartmentSearchResponse { Draw = request.Draw };
            try
            {
                var query = _ctx.Departments.Where(x =>
                    x.Status == request.Status || request.Status == (int)GlobalEnums.StatusDefaultEnum.All);
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.Name.ToLower().Contains(request.Keywords.Trim().ToLower()));
                var data = await query.OrderByDescending(x => x.CreatedDate).ToListAsync();

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<DepartmentSearchResponseData>();

                foreach (var d in data.Skip(skip).Take(size))
                {
                    var dMap = _mapper.Map<DepartmentSearchResponseData>(d);
                    dMap.Record = d.Id.ToString().StringAesEncryption(request.RequestPage);
                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }

        public async Task<DepartmentSearchResponseData> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return null;
                var record = await _ctx.Departments.FirstOrDefaultAsync(x => x.Id == id);
                if (record == null)
                    return null;
                var response = _mapper.Map<DepartmentSearchResponseData>(record);
                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return null;
            }
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DepartmentCreateResponse> Create(DepartmentCreateRequest request)
        {
            var response = new DepartmentCreateResponse();
            try
            {

                if (!string.IsNullOrEmpty(request.Record))
                {
                    if (request.RawId <= 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                    {
                        var record = await _ctx.Departments.FirstOrDefaultAsync(x => x.Id == request.RawId);
                        if (record == null)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = GlobalEnums.NoContentMessage;
                            return response;
                        }

                        record.Name = request.Name;
                        _ctx.Update(record);

                        var dMap = _mapper.Map<DepartmentSearchResponseData>(record);
                        dMap.Record = request.Record;
                        response.Data = dMap;
                        await _ctx.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Edit",
                            FunctionUnique = "Department",
                            UserId = request.Creator
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật thông tin phòng ban thành công!";
                    }
                }
                else
                {
                    var duplicateCheck =
                       await _ctx.Departments.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(request.Name) && x.Status != (int)GlobalEnums.StatusDefaultEnum.Deleted);
                    if (duplicateCheck != null)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Tên phòng ban đã tồn tại!";
                    }

                    var create = new Departments
                    {
                        CreatedDate = DateTime.Now,
                        Creator = request.Creator,
                        CreatorName = request.CreatorName,
                        Name = request.Name,
                        Status = (int)GlobalEnums.StatusDefaultEnum.Active,
                        StatusName = GlobalEnums.OtherStatusNames[(int)GlobalEnums.StatusDefaultEnum.Active]
                    };
                    _ctx.Departments.Add(create);
                    await _ctx.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Add",
                        FunctionUnique = "Department",
                        UserId = request.Creator
                    });


                    var dMap = _mapper.Map<DepartmentSearchResponseData>(create);
                    dMap.Record = create.Id.ToString().StringAesEncryption(request.RequestPage);
                    response.Data = dMap;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Thêm mới phòng ban thành công!";
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return response;
        }
        /// <summary>
        /// Đổi trạng thái
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DepartmentCreateResponse> ChangeStatus(DepartmentCreateRequest request)
        {
            var response = new DepartmentCreateResponse();
            try
            {

                if (!string.IsNullOrEmpty(request.Record))
                {
                    if (request.RawId <= 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                    {
                        var record = await _ctx.Departments.FirstOrDefaultAsync(x => x.Id == request.RawId);
                        if (record == null)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = GlobalEnums.NoContentMessage;
                            return response;
                        }

                        record.Status = request.Status;
                        record.StatusName = GlobalEnums.OtherStatusNames[request.Status];
                        _ctx.Update(record);

                        var dMap = _mapper.Map<DepartmentSearchResponseData>(record);
                        dMap.Record = request.Record;
                        response.Data = dMap;
                        await _ctx.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "UpdateStatus",
                            FunctionUnique = "Department",
                            UserId = request.Creator
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật trạng thái phòng ban thành công!";
                    }
                }
                else
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.Message = GlobalEnums.NoContentMessage;
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return response;
        }

        public async Task<IList<Departments>> GetAll(int status)
        {
            try
            {
                var q = await _ctx.Departments.Where(x => x.Status == status).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return null;
            }
        }
    }
}
