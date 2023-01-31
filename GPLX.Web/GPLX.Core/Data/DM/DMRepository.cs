using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DM;
using GPLX.Core.DTO.Request.DM;
using GPLX.Core.DTO.Response.DM;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.DM
{
    public class DMRepository : IDMRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DMRepository> _logger;
        private readonly IActionLogsRepository _actionLogsRepository;

        public DMRepository(Context context, IMapper mapper, ILogger<DMRepository> logger, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<DMSearchResponse> Search(int skip, int length, DMSearchRequest request)
        {
            var response = new DMSearchResponse { Draw = request.Draw };
           
            var query = _context.DM.AsNoTracking();
            if(request.IdTree.HasValue) 
                query = query.Where(x => x.IDTree == request.IdTree);
            if (!string.IsNullOrEmpty(request.MaDM))
                query = query.Where(x => x.MaDM.ToLower().Contains(request.MaDM.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenDM))
                query = query.Where(x => x.TenDM.ToLower().Contains(request.TenDM.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderBy(x => x.Stt).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMSearchResponseData>();

            foreach (var d in data.Skip(skip).Take(length))
            {
                var dMap = _mapper.Map<DMSearchResponseData>(d);
                dMap.IsActiveName = GlobalEnums.GetStatusName(dMap.IsActive);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMSearchResponse> GetAllDM(int idTree)
        {
            var response = new DMSearchResponse { };
           
            var query = _context.DM.AsQueryable();
            query = query.Where(x => x.IsActive == 1 && x.IDTree == idTree);
            var data = await query.OrderBy(x => x.Stt).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMSearchResponseData>();

            foreach (var d in data)
            {
                var dMap = _mapper.Map<DMSearchResponseData>(d);
                dataResponse.Add(dMap);
            }
            response.Data = dataResponse;
            response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;

            return response;
        }

        public async Task<DMSearchResponse> SearchAll(DMSearchRequest request)
        {
            var response = new DMSearchResponse { Draw = request.Draw };
            
            var query = _context.DM.AsQueryable();
            if(request.IdTree.HasValue) 
                query = query.Where(x => x.IDTree == request.IdTree);
            if (!string.IsNullOrEmpty(request.MaDM))
                query = query.Where(x => x.MaDM.ToLower().Contains(request.MaDM.Trim().ToLower()));
            if (!string.IsNullOrEmpty(request.TenDM))
                query = query.Where(x => x.TenDM.ToLower().Contains(request.TenDM.Trim().ToLower()));
            if (request.Status != -1)
                query = query.Where(x => x.IsActive == request.Status);

            var data = await query.OrderBy(x => x.Stt).ToListAsync();

            response.RecordsFiltered = data.Count;
            response.RecordsTotal = data.Count;
            var dataResponse = new List<DMSearchResponseData>();

            for(var i = 0; i < data.Count; i ++)
            {
                var d = data[i];
                var dMap = _mapper.Map<DMSearchResponseData>(d);
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

        public async Task<DMSearchResponseData> GetById(int idTree, string maDM)
        {
            if (idTree <= 0)
                return null;
            var record = await _context.DM.FirstOrDefaultAsync(x => x.IDTree == idTree && x.MaDM == maDM);
            if (record == null)
                return null;

            int maxstt = 0;
            maxstt = await _context.DM.Where(x => x.IDTree == idTree).MaxAsync(e => e.Stt) + 1;
            var response = _mapper.Map<DMSearchResponseData>(record);
            response.MaxStt = maxstt;
            return response;
        }

        public async Task<int> GetMaxStt(int idTree)
        {
            int maxstt = 0;
            maxstt = await _context.DM.Where(x => x.IDTree == idTree).MaxAsync(e => e.Stt) + 1;
            return maxstt;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DMCreateResponse> Create(DMCreateRequest request)
        {
            var response = new DMCreateResponse();

            response = Validation(request);

            request.MaDM = String.IsNullOrEmpty(request.MaDM) ? "" : request.MaDM.Trim();
            request.TenDM = String.IsNullOrEmpty(request.TenDM) ? "" : request.TenDM.Trim();

            if (response.ListError.Count ==0)
            {
                var query = _context.DM.AsQueryable();
                query = query.Where(x => x.MaDM == request.MaDM && x.IDTree == request.IdTree);
                var data = await query.OrderBy(x => x.Stt).ToListAsync();

                if(string.IsNullOrEmpty(request.Record))
                {
                    if (data.Count > 0)
                    {
                        if (request.IdTree == 82)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.ListError.Add(new ItemError { FieldError = "txtmackdoituong", Message = "Mã CK đối tượng đã tồn tại trong hệ thống." });
                        }
                        else if (request.IdTree == 83)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.ListError.Add(new ItemError { FieldError = "txtmadoituong", Message = "Mã đối tượng đã tồn tại trong hệ thống." });
                        }
                        else if (request.IdTree == 81)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.ListError.Add(new ItemError { FieldError = "txtmachucdanh", Message = "Mã chức danh đã tồn tại trong hệ thống." });
                        }
                    }
                    else
                    {
                        //Add New
                        var item = new Database.Models.DM();
                        item.MaDM = request.MaDM;
                        item.TenDM = request.TenDM;
                        item.IsActive = request.IsActive;
                        item.IDTree = (int)request.IdTree;
                        item.Createby = request.CreatorName;
                        item.Stt = request.Stt;
                        item.Createdate = DateTime.Now;
                        _context.DM.Add(item);
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Add",
                            FunctionUnique = "DM",
                            UserId = request.Creator
                        }).ConfigureAwait(false);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        if (request.IdTree == 82)
                        {
                            response.Message = "Thêm mới thông tin danh mục ck đối tượng thành công!";
                        }
                        else if (request.IdTree == 83)
                        {
                            response.Message = "Thêm mới thông tin danh mục đối tượng thành công!";
                        }
                        else if (request.IdTree == 81)
                        {
                            response.Message = "Thêm mới thông tin danh mục chức danh thành công!";
                        }
                    }
                }
                else
                {
                    //Edit
                    var item = data.FirstOrDefault();
                    item.MaDM = request.MaDM;
                    item.TenDM = request.TenDM;
                    item.IsActive = request.IsActive;
                    item.IDTree = (int)request.IdTree;
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
                        FunctionUnique = "DM",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    if (request.IdTree == 82)
                    {
                        response.Message = "Cập nhật thông tin danh mục ck đối tượng thành công!";
                    }
                    else if (request.IdTree == 83)
                    {
                        response.Message = "Cập nhật thông tin danh mục đối tượng thành công!";
                    }
                    else if (request.IdTree == 81)
                    {
                        response.Message = "Cập nhật thông tin danh mục chức danh thành công!";
                    }
                }
            }

            if(response.ListError.Count>0)
            {
                foreach(var item in response.ListError)
                {
                    response.Message = response.Message + "</br>" + item.Message;
                }

                response.Message = response.Message.Substring(5);
            }    
            
            return response;
        }

        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DMCreateResponse> Remove(DMCreateRequest request)
        {
            var response = new DMCreateResponse();

            if (string.IsNullOrEmpty(response.Message))
            {
                var query = _context.DM.AsQueryable();
                query = query.Where(x => x.MaDM == request.MaDM && x.IDTree == request.IdTree);
                var data = await query.OrderBy(x => x.Stt).ToListAsync();

                if (data.Count > 0)
                {
                    //Remove
                    var item = data.FirstOrDefault();
                    _context.DM.Remove(item);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Delete",
                        FunctionUnique = "DM",
                        UserId = request.Creator
                    }).ConfigureAwait(false);
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    if (request.IdTree == 82)
                    {
                        response.Message = "Xóa thông tin danh mục ck đối tượng thành công!";
                    }
                    else if (request.IdTree == 83)
                    {
                        response.Message = "Xóa thông tin danh mục đối tượng thành công!";
                    }
                    else if (request.IdTree == 81)
                    {
                        response.Message = "Xóa thông tin danh mục thành công!";
                    }    
                }
            }
            
            return response;
        }

        public DMCreateResponse Validation(DMCreateRequest request)
        {
            var response = new DMCreateResponse();
            response.ListError = new List<ItemError>();
            string itemText = "";
            string controlcode = "";
            string controlname = "";

            if (request.IdTree == 82)
            {
                //Danh mục hình thức chiết khấu đối tượng
                itemText = "CK đối tượng";
                controlcode = "txtmackdoituong";
                controlname = "txttenckdoituong";
            }
            else if (request.IdTree == 83)
            {
                //Danh mục đối tượng
                itemText = "đối tượng";
                controlcode = "txtmadoituong";
                controlname = "txttendoituong";
            }
            else if (request.IdTree == 81)
            {
                //Danh mục chức danh
                itemText = "chức danh";
                controlcode = "txtmachucdanh";
                controlname = "txttenchucdanh";
            }
            if (string.IsNullOrEmpty(request.MaDM))
            {   
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlcode, Message = "Mã "+ itemText + " không được bỏ trống." });
            }
            else if (request.MaDM.Contains(" "))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlcode, Message = "Mã " + itemText + " không được có khoảng trắng." });
            }
            else if (request.MaDM.Length > 10)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlcode, Message = "Mã " + itemText + " có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpeciallyFull(request.MaDM))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlcode, Message = "Mã " + itemText + " có ký tự đặc biệt." });
            }

            if (string.IsNullOrEmpty(request.TenDM))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlname, Message = "Tên " + itemText + " không được bỏ trống." });
            }
            else if (request.TenDM.Length > 200)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlname, Message = "Tên " + itemText + " có độ dài vượt quá số ký tự cho phép." });
            }
            else if (!GlobalEnums.ValidateTextSpecially(request.TenDM))
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = controlname, Message = "Tên " + itemText + " có ký tự đặc biệt." });
            }

            if (request.Stt == 0)
            {
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.ListError.Add(new ItemError { FieldError = "txtstt", Message = "Thứ tự " + itemText + " không được bỏ trống." });
            }

            return response;
        }
    }
}
