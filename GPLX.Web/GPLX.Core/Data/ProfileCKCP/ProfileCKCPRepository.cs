using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.ProfileCKCP;
using GPLX.Core.DTO.Request.ProfileCKCP;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.ProfileCKCP;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.ProfileCKCP
{
    public class ProfileCKCPRepository : IProfileCKCPRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileCKCPRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public ProfileCKCPRepository(Context context, IMapper mapper, ILogger<ProfileCKCPRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<ProfileCKCPSearchResponse> Search(string profileCKMa = "", string Keywords = "")
        {
            var response = new ProfileCKCPSearchResponse();

            var query = _context.ProfileCKCP.AsQueryable()
                .LeftJoin(_context.DMCP, ck => ck.CPMa, cp => cp.MaCP, (ck, cp) => new
                {
                    ProfileCKMa = ck.ProfileCKMa,
                    CPMa = ck.CPMa,
                    CPTen = cp.TenCP,
                    IsActive = ck.IsActive,
                    IsActiveName = GlobalEnums.GetStatusName(ck.IsActive),
                    Createby = ck.Createby,
                    Createdate = ck.Createdate,
                    Updateby = ck.Updateby,
                    Updatedate = ck.Updatedate
                });

            query = query.Where(x => x.ProfileCKMa == profileCKMa);
            if (!string.IsNullOrEmpty(Keywords))
                query = query.Where(x => x.CPTen.Contains(Keywords) || x.CPMa.Contains(Keywords));

            var data = await query.OrderBy(x => x.ProfileCKMa).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<ProfileCKCPSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = new ProfileCKCPSearchResponseData
                {
                    ProfileCKMa = d.ProfileCKMa,
                    CPMa = d.CPMa,
                    CPTen = d.CPTen,
                    IsActive = d.IsActive,
                    IsActiveName = GlobalEnums.GetStatusName(d.IsActive),
                    Createby = d.Createby,
                    Createdate = d.Createdate,
                    Updateby = d.Updateby,
                    Updatedate = d.Updatedate,
                    CreatedateString = d.Createdate.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedateString = d.Updatedate?.ToString("HH:mm dd/MM/yyyy"),
                };
                dataResponse.Add(dMap);
            }
            
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<ProfileCKCPSearchResponseData> GetById(string profileCKMa, string CPMa)
        {
            var record = await _context.ProfileCKCP.FirstOrDefaultAsync(x => x.ProfileCKMa == profileCKMa && x.CPMa == CPMa);
            if (record == null)
                return null;
            var response = _mapper.Map<ProfileCKCPSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ProfileCKCPCreateResponse> Create(ProfileCKCPCreateRequest request)
        {
            var response = new ProfileCKCPCreateResponse();

            var query = _context.ProfileCKCP.AsQueryable();
            query = query.Where(x => x.ProfileCKMa == request.ProfileCKMa && x.CPMa == request.CPMa);
            var data = await query.OrderBy(x => x.ProfileCKMa).ToListAsync();

            if(data.Count == 0)
            {
                //Add New
                var item = new Database.Models.ProfileCKCP();
                item.ProfileCKMa = request.ProfileCKMa;
                item.CPMa = request.CPMa;
                item.IsActive = request.IsActive;
                item.Createby = request.CreatorName;
                item.Createdate = DateTime.Now;
                _context.ProfileCKCP.Add(item);
                await _context.SaveChangesAsync();
                await _actionLogsRepository.AddLogAsync(new ActionLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    UserName = request.CreatorName,
                    Content = JsonConvert.SerializeObject(request),
                    Action = "Add",
                    FunctionUnique = "ProfileCKCP",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Thêm mới dịch vụ của chuyên khoa thành công!";
            }    
            else
            {
                //Edit
                var item = data.FirstOrDefault();
                item.IsActive = request.IsActive;
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
                    FunctionUnique = "ProfileCKCP",
                    UserId = request.Creator
                }).ConfigureAwait(false);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Cập nhật dịch vụ của chuyên khoa thành công!";

            }
            

            return response;
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ProfileCKCPCreateResponse> Remove(ProfileCKCPCreateRequest request)
        {
            var response = new ProfileCKCPCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.ProfileCKCP.AsQueryable();
                query = query.Where(x => x.ProfileCKMa == request.ProfileCKMa && x.CPMa == request.CPMa);
                var data = await query.OrderBy(x => x.ProfileCKMa).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.ProfileCKCP.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "ProfileCK",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Xóa dịch vụ của chuyên khoa thành công!";
                }
            }

            return response;
        }
    }
}
