using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.ProfileCK;
using GPLX.Core.DTO.Request.ProfileCK;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.DTO.Response.ProfileCK;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.ProfileCK
{
    public class ProfileCKRepository : IProfileCKRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileCKRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public ProfileCKRepository(Context context, IMapper mapper, ILogger<ProfileCKRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<ProfileCKSearchResponse> Search(int skip, int length, ProfileCKSearchRequest request)
        {
            var response = new ProfileCKSearchResponse { Draw = request.Draw };

            var query = _context.ProfileCK.AsQueryable()
                .LeftJoin(_context.DMBS_ChuyenKhoa, profileck => profileck.ChuyenKhoaMa, chuyenkhoa => chuyenkhoa.Ma, (profileck, chuyenkhoa) => new
                {
                    Id = profileck.Id,
                    ProfileCKMa = profileck.ProfileCKMa,
                    ProfileCKTen = profileck.ProfileCKTen,
                    ChuyenKhoaMa = profileck.ChuyenKhoaMa,
                    ChuyenKhoaTen = chuyenkhoa.Ten,
                    IsActive = profileck.IsActive,
                    Createby = profileck.Createby,
                    Createdate = profileck.Createdate,
                    Updateby = profileck.Updateby,
                    Updatedate = profileck.Updatedate
                });
            if (!string.IsNullOrEmpty(request.ProfileCKMa))
                query = query.Where(x => x.ProfileCKMa.Contains(request.ProfileCKMa.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.ProfileCKTen))
                query = query.Where(x => x.ProfileCKTen.Contains(request.ProfileCKTen.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.ChuyenKhoaMa != "-1")
                query = query.Where(x => x.ChuyenKhoaMa == request.ChuyenKhoaMa);


            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<ProfileCKSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = new ProfileCKSearchResponseData
                {
                    Id = d.Id,
                    ProfileCKMa = d.ProfileCKMa,
                    ProfileCKTen = d.ProfileCKTen,
                    ChuyenKhoaMa = d.ChuyenKhoaMa,
                    ChuyenKhoaTen = d.ChuyenKhoaTen,
                    IsActive = d.IsActive,
                    IsActiveName = GlobalEnums.GetStatusName(d.IsActive),
                    Createby = d.Createby,
                    Createdate = d.Createdate,
                    Updateby = d.Updateby,
                    Updatedate = d.Updatedate
                };

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<ProfileCKSearchResponse> SearchAll(ProfileCKSearchRequest request)
        {
            var response = new ProfileCKSearchResponse { Draw = request.Draw };

            var query = _context.ProfileCK.AsQueryable()
                .LeftJoin(_context.DMBS_ChuyenKhoa, profileck => profileck.ChuyenKhoaMa, chuyenkhoa => chuyenkhoa.Ma, (profileck, chuyenkhoa) => new
                {
                    Id = profileck.Id,
                    ProfileCKMa = profileck.ProfileCKMa,
                    ProfileCKTen = profileck.ProfileCKTen,
                    ChuyenKhoaMa = profileck.ChuyenKhoaMa,
                    ChuyenKhoaTen = chuyenkhoa.Ten,
                    IsActive = profileck.IsActive,
                    Note = profileck.Note,
                    Createby = profileck.Createby,
                    Createdate = profileck.Createdate,
                    Updateby = profileck.Updateby,
                    Updatedate = profileck.Updatedate
                });

            if (!string.IsNullOrEmpty(request.ProfileCKMa))
                query = query.Where(x => x.ProfileCKMa.Contains(request.ProfileCKMa.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.ProfileCKTen))
                query = query.Where(x => x.ProfileCKTen.Contains(request.ProfileCKTen.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);
            if (request.ChuyenKhoaMa != "-1")
                query = query.Where(x => x.ChuyenKhoaMa == request.ChuyenKhoaMa);

            var data = await query.OrderByDescending(x => x.Createdate).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<ProfileCKSearchResponseData>();

            for (var i = 0; i < data.Count; i++)
            {
                var d = data[i];
                var dMap = new ProfileCKSearchResponseData
                {
                    Id = d.Id,
                    Index = i + 1,
                    ProfileCKMa = d.ProfileCKMa,
                    ProfileCKTen = d.ProfileCKTen,
                    ChuyenKhoaMa = d.ChuyenKhoaMa,
                    ChuyenKhoaTen = d.ChuyenKhoaTen,
                    IsActive = d.IsActive,
                    Note = d.Note,
                    IsActiveName = GlobalEnums.GetStatusName(d.IsActive),
                    Createby = d.Createby,
                    Createdate = d.Createdate,
                    Updateby = d.Updateby,
                    Updatedate = d.Updatedate,
                    CreatedateString = d.Createdate.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedateString = d.Updatedate?.ToString("HH:mm dd/MM/yyyy")
                };

                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<ProfileCKSearchResponseData> GetById(string profileCKMa)
        {
            var record = await _context.ProfileCK.FirstOrDefaultAsync(x => x.ProfileCKMa == profileCKMa);
            if (record == null)
                return null;
            var response = _mapper.Map<ProfileCKSearchResponseData>(record);
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ProfileCKCreateResponse> Create(ProfileCKCreateRequest request)
        {
            var response = new ProfileCKCreateResponse();

            request.ProfileCKMa = string.IsNullOrEmpty(request.ProfileCKMa)? "" : request.ProfileCKMa.Trim();
            request.ProfileCKTen = string.IsNullOrEmpty(request.ProfileCKTen) ? "" : request.ProfileCKTen.Trim();

            response = Validation(request);

            if (response.ListError.Count == 0)
            {
                var query = _context.ProfileCK.AsQueryable();
                query = query.Where(x => x.ProfileCKMa == request.ProfileCKMa);
                var data = await query.OrderBy(x => x.ProfileCKTen).ToListAsync();

                if (string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.ListError.Add(new ItemError { FieldError = "txtmaProfileCK", Message = "Mã profile ck đã tồn tại trong hệ thống." });
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.ProfileCK();
                        item.ProfileCKMa = request.ProfileCKMa;
                        item.ProfileCKTen = request.ProfileCKTen;
                        item.ChuyenKhoaMa = request.ChuyenKhoaMa;
                        item.IsActive = request.IsActive;
                        item.Note = request.Note;
                        item.Createby = request.CreatorName;
                        item.Createdate = DateTime.Now;
                        _context.ProfileCK.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "ProfileCK",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Thêm mới thông tin profile dịch vụ ck thành công!";
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.ProfileCKMa = request.ProfileCKMa;
                    item.ProfileCKTen = request.ProfileCKTen;
                    item.IsActive = request.IsActive;
                    item.Note = request.Note;
                    item.ChuyenKhoaMa = string.IsNullOrEmpty(request.ChuyenKhoaMa)? "" : request.ChuyenKhoaMa;
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
                        FunctionUnique = "ProfileCK",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Cập nhật thông tin profile dịch vụ ck thành công!";
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
        public async Task<ProfileCKCreateResponse> Remove(ProfileCKCreateRequest request)
        {
            var response = new ProfileCKCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.ProfileCK.AsQueryable();
                query = query.Where(x => x.ProfileCKMa == request.ProfileCKMa);
                var data = await query.OrderBy(x => x.ProfileCKTen).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.ProfileCK.Remove(item);
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
                    response.Message = "Xóa thông tin profile dịch vụ ck thành công!";
                }
            }

            return response;
        }

        public ProfileCKCreateResponse Validation(ProfileCKCreateRequest request)
        {
            var response = new ProfileCKCreateResponse();
            response.ListError = new List<ItemError>();

            if (string.IsNullOrEmpty(request.ProfileCKMa))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaProfileCK", Message = "Mã profile ck không được trống." });
            }
            else if (request.ProfileCKMa.Length > 20)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txtmaProfileCK", Message = "Mã profile ck có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.ProfileCKMa))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtmaProfileCK", Message = "Mã profile ck có ký tự đặc biệt." });
            }

            if (string.IsNullOrEmpty(request.ProfileCKTen))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenProfileCK", Message = "Tên profile ck không được trống." });
            }
            else if (request.ProfileCKTen.Length > 100)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                response.ListError.Add(new ItemError { FieldError = "txttenProfileCK", Message = "Tên profile ck có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.ProfileCKTen))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txttenProfileCK", Message = "Tên profile ck có ký tự đặc biệt." });
            }

            if (!string.IsNullOrEmpty(request.Note))
            {
                if (request.Note.Length > 3000)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "txtNote", Message = "Ghi chú có độ dài vượt quá số ký tự cho phép." });
                }
                //else if (!GlobalEnums.ValidateTextSpecially(request.Note))
                //{
                //    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                //    response.ListError.Add(new ItemError { FieldError = "txtNote", Message = "Ghi chú có ký tự đặc biệt." });
                //}
            }

            if (!string.IsNullOrEmpty(request.ChuyenKhoaMa))
            {
                if (request.ChuyenKhoaMa.Equals("-1"))
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                    response.ListError.Add(new ItemError { FieldError = "ddlStatusChuyenKhoa", Message = "Hãy chọn chuyên khoa." });
                }
            }

            return response;
        }

    }
}
