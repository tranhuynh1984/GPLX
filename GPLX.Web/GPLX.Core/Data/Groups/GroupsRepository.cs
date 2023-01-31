using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.DTO.Entities;
using GPLX.Core.DTO.Request.Groups;
using GPLX.Core.DTO.Response.Groups;
using GPLX.Core.DTO.Response.Permission;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.Groups
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly Context _context;
        private readonly ILogger<GroupsRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IActionLogsRepository _actionLogsRepository;

        public GroupsRepository(Context context, ILogger<GroupsRepository> logger, IMapper mapper, IActionLogsRepository actionLogsRepository)
        {
            this._context = context;
            _logger = logger;
            _mapper = mapper;
            _actionLogsRepository = actionLogsRepository;
        }

        public async Task<GroupsCreateResponse> DeleteAsync(GroupsCreateRequest request)
        {
            var response = new GroupsCreateResponse();
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
                        var record = await _context.Groups.FirstOrDefaultAsync(x => x.Id == request.RawId);
                        if (record == null)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = GlobalEnums.NoContentMessage;
                            return response;
                        }

                        record.Status = (int)GlobalEnums.StatusDefaultEnum.Deleted;
                        record.StatusName = string.Empty;
                        record.GroupCode = string.Empty;
                        _context.Update(record);

                        var dMap = _mapper.Map<GroupsSearchResponseData>(record);
                        dMap.Record = request.Record;
                        response.Data = dMap;
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Delete",
                            FunctionUnique = "Groups",
                            UserId = request.Creator
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Xóa chức vụ thành công!";
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
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return response;
        }

        public async Task<bool> IsAuthorize(List<int> groups, string module, int permission)
        {
            try
            {
                var func = await _context.Functions.Where(a => a.Unique == module).ToListAsync();
                if (func == null)
                    return false;
                var listIds = func.Select(x => x.Id);
                var objs = await _context.GroupsPermission.Where(a => groups.Contains(a.GroupId) && listIds.Contains(a.FunctionId)).ToListAsync();
                if (objs == null)
                    return false;

                bool isAuthorized = false;
                foreach (var obj in objs)
                {
                    if ((obj.Permission & permission) == permission)
                    {
                        isAuthorized = true;
                        break;
                    }
                }
                return isAuthorized;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }
        }

        public async Task<Database.Models.Groups> GetByIdAsync(int id)
        {
            try
            {
                var obj = await _context.Groups.FirstOrDefaultAsync(a => a.Id == id);
                return obj;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<GroupsSearchResponseData> GetByIdAsyncView(int groups)
        {
            try
            {
                var obj = await _context.Groups.FirstOrDefaultAsync(a => a.Id == groups);
                var data = _mapper.Map<GroupsSearchResponseData>(obj);
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<bool> IsAuthorize(int groups, string module, int permission)
        {
            try
            {
                var func = await _context.Functions.Where(a => a.Unique == module).ToListAsync();
                if (func == null)
                    return false;
                var listIds = func.Select(x => x.Id);
                var objs = await _context.GroupsPermission.Where(a => a.GroupId == groups && listIds.Contains(a.FunctionId)).ToListAsync();
                if (objs == null)
                    return false;

                bool isAuthorized = false;
                foreach (var obj in objs)
                {
                    if ((obj.Permission & permission) == permission)
                    {
                        isAuthorized = true;
                        break;
                    }
                }
                return isAuthorized;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }
        }

        public async Task<bool> EditPermission(int groupId, int permission, int functionId)
        {
            try
            {
                var per = await _context.GroupsPermission.FirstOrDefaultAsync(a => a.GroupId == groupId && a.FunctionId == functionId);

                if (per == null)
                {
                    _context.GroupsPermission.Add(new GroupsPermission
                    {
                        Permission = permission,
                        FunctionId = functionId,
                        GroupId = groupId
                    });
                }
                else
                {
                    per.Permission = permission;
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, ex.Message);
                Log.Error(ex, ex.Message);
                return false;
            }


        }
        /// <summary>
        /// Tìm kiếm
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="size"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GroupsSearchResponse> SearchAsync(int skip, int size, GroupsSearchRequest request)
        {
            var response = new GroupsSearchResponse { Draw = request.Draw };
            try
            {
                var query = _context.Groups.Where(x =>
                    x.Status == request.Status || request.Status == (int)GlobalEnums.StatusDefaultEnum.All);
                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.Name.ToLower().Contains(request.Keywords.Trim().ToLower()));
                var data = await query.OrderBy(x => x.Order).ToListAsync();

                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;
                var dataResponse = new List<GroupsSearchResponseData>();

                foreach (var d in data.Skip(skip).Take(size))
                {
                    var dMap = _mapper.Map<GroupsSearchResponseData>(d);
                    dMap.Record = d.Id.ToString().StringAesEncryption(request.RequestPage);
                    dataResponse.Add(dMap);
                }
                response.Data = dataResponse;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }

            return response;
        }
        /// <summary>
        /// Thêm sửa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GroupsCreateResponse> Create(GroupsCreateRequest request)
        {
            var response = new GroupsCreateResponse();
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




                        var record = await _context.Groups.FirstOrDefaultAsync(x => x.Id == request.RawId);
                        if (record == null)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = GlobalEnums.NoContentMessage;
                            return response;
                        }

                        var checkDuplicate = await _context.Groups.FirstOrDefaultAsync(x =>
                            x.Name.ToLower().Equals(request.Name.ToLower()) ||
                            x.GroupCode.ToLower().Equals(request.GroupCode.ToLower()));

                        if (checkDuplicate != null)
                        {
                            if (checkDuplicate.Id != record.Id)
                                return new GroupsCreateResponse
                                {
                                    Message = "Tên hoặc mã chức vụ đã tồn tại!",
                                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                                };
                        }

                        record.Name = request.Name;
                        record.Order = request.Order;
                        _context.Update(record);

                        var dMap = _mapper.Map<GroupsSearchResponseData>(record);
                        dMap.Record = request.Record;
                        response.Data = dMap;
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "Edit",
                            FunctionUnique = "Groups",
                            UserId = request.Creator
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật thông tin chức vụ thành công!";
                    }
                }
                else
                {
                    var checkDuplicate = await _context.Groups.FirstOrDefaultAsync(x =>
                        x.Name.ToLower().Equals(request.Name.ToLower()) ||
                        x.GroupCode.ToLower().Equals(request.GroupCode.ToLower()));

                    if (checkDuplicate != null)
                    {
                        return new GroupsCreateResponse
                        {
                            Message = "Tên hoặc mã chức vụ đã tồn tại!",
                            Code = (int)GlobalEnums.ResponseCodeEnum.Error
                        };
                    }

                    var create = new Database.Models.Groups
                    {
                        CreatedDate = DateTime.Now,
                        Creator = request.Creator,
                        CreatorName = request.CreatorName,
                        Name = request.Name,
                        Status = (int)GlobalEnums.StatusDefaultEnum.Active,
                        StatusName = GlobalEnums.OtherStatusNames[(int)GlobalEnums.StatusDefaultEnum.Active],
                        Order = request.Order,
                        GroupCode = request.GroupCode
                    };
                    _context.Groups.Add(create);
                    await _context.SaveChangesAsync();
                    await _actionLogsRepository.AddLogAsync(new ActionLogs
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        UserName = request.CreatorName,
                        Content = JsonConvert.SerializeObject(request),
                        Action = "Add",
                        FunctionUnique = "Groups",
                        UserId = request.Creator
                    });


                    var dMap = _mapper.Map<GroupsSearchResponseData>(create);
                    dMap.Record = create.Id.ToString().StringAesEncryption(request.RequestPage);
                    response.Data = dMap;
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                    response.Message = "Thêm mới chức vụ thành công!";
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
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
        public async Task<GroupsCreateResponse> ChangeStatus(GroupsCreateRequest request)
        {
            var response = new GroupsCreateResponse();
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
                        var record = await _context.Groups.FirstOrDefaultAsync(x => x.Id == request.RawId);
                        if (record == null)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                            response.Message = GlobalEnums.NoContentMessage;
                            return response;
                        }

                        record.Status = request.Status;
                        record.StatusName = GlobalEnums.OtherStatusNames[request.Status];
                        _context.Update(record);

                        var dMap = _mapper.Map<GroupsSearchResponseData>(record);
                        dMap.Record = request.Record;
                        response.Data = dMap;
                        await _context.SaveChangesAsync();
                        await _actionLogsRepository.AddLogAsync(new ActionLogs
                        {
                            Id = Guid.NewGuid(),
                            CreatedDate = DateTime.Now,
                            UserName = request.CreatorName,
                            Content = JsonConvert.SerializeObject(request),
                            Action = "UpdateStatus",
                            FunctionUnique = "Groups",
                            UserId = request.Creator
                        });
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật trạng thái chức vụ thành công!";
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
                Log.Error(e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return response;
        }

        public async Task<IList<Database.Models.Groups>> GetAllActiveGroups()
        {
            try
            {
                var query = _context.Groups.Where(x => x.Status == (int)GlobalEnums.StatusDefaultEnum.Active);
                var data = await query.OrderBy(x => x.Order).ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return new List<Database.Models.Groups>();
            }
        }

        public bool IsAuthorize(int groups, string module, int permission, IList<GroupsPermission> allGroups, IList<Functions> allFunctions)
        {
            try
            {
                if (allFunctions == null || allGroups == null)
                    return false;
                var func = allFunctions.FirstOrDefault(a => a.Unique == module);
                if (func == null)
                    return false;
                var obj = allGroups.FirstOrDefault(a => a.GroupId == groups && a.FunctionId == func.Id);
                if (obj == null)
                    return false;

                if ((obj.Permission & permission) == permission)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public bool IsAuthorize(List<int> groups, string module, int permission, IList<GroupsPermission> allGroups, IList<Functions> allFunctions)
        {
            try
            {
                if (allFunctions == null || allGroups == null)
                    return false;
                var func = allFunctions.FirstOrDefault(a => a.Unique == module);
                if (func == null)
                    return false;
                var obj = allGroups.Where(a => groups.Contains(a.GroupId) && a.FunctionId == func.Id).ToList();
                if (obj.Count == 0)
                    return false;
                foreach (var o in obj)
                {
                    if ((o.Permission & permission) == permission)
                        return true;
                }
               
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public bool IsAuthorizePath(int groups, int fcId, int permission, IList<GroupsPermission> allGroups, IList<Functions> allFunctions)
        {
            try
            {
                if (allFunctions == null || allGroups == null)
                    return false;
                var func = allFunctions.FirstOrDefault(a => a.Id == fcId);
                if (func == null)
                    return false;
                var obj = allGroups.FirstOrDefault(a => a.GroupId == groups && a.FunctionId == func.Id);
                if (obj == null)
                    return false;

                if ((obj.Permission & permission) == permission)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool IsAuthorizePath(List<int> groups, int fcId, int permission, IList<GroupsPermission> allGroups, IList<Functions> allFunctions)
        {
            try
            {
                if (allFunctions == null || allGroups == null)
                    return false;
                var func = allFunctions.FirstOrDefault(a => a.Id == fcId);
                if (func == null)
                    return false;
                var obj = allGroups.Where(a => groups.Contains(a.GroupId) && a.FunctionId == func.Id).ToList();
                if (obj.Count == 0)
                    return false;
                foreach (var o in obj)
                {
                    if ((o.Permission & permission) == permission)
                        return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IList<GroupsPermission>> GetAllPermission()
        {
            try
            {
                try
                {
                    return await _context.GroupsPermission.ToListAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IList<PermissionGrantResponse>> GetGrants(int groupId)
        {
            try
            {
                var listGrant = new List<PermissionGrantResponse>();
                var all = await GetFunctionsAsync();
                var allPermission = await GetAllPermission();
                var root = all.Where(x => x.ParentId == 0).ToList();
                foreach (var a in root)
                {
                    listGrant.Add(new PermissionGrantResponse
                    {
                        FuncName = a.Name,
                        FuncId = a.Id,
                        Children = GetChild(all, a.Id, groupId, allPermission),
                        //Checked = IsCheckedPermission(groupId, a.Id, allPermission),
                        Order = a.Order,
                        Unique = $"root:{a.Id}",
                        Url = a.Url
                    });
                }
                SetChecked(listGrant, allPermission, groupId);
                return listGrant;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);
                return null;
            }
        }

        public async Task<bool> DeleteFuncInPermission(List<PermissionUpdate> updates, int g)
        {
            try
            {
                var allOlder = await _context.GroupsPermission.Where(x => x.GroupId == g).ToListAsync();
                var listDelete = allOlder.Where(x => updates.All(c => c.Id != x.FunctionId)).ToList();
                if (listDelete.Any())
                {
                    _context.RemoveRange(listDelete);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", g);
                return false;
            }
        }

        public IList<PermissionGrantResponse> GetChild(IList<Functions> all, int parent, int groupId, IList<GroupsPermission> permission)
        {
            return all.Where(x => x.ParentId == parent).Select(x => new PermissionGrantResponse
            {
                FuncName = x.Name,
                FuncId = x.Id,
                Children = GetChild(all, x.Id, groupId, permission),
                //Checked = IsCheckedPermission(groupId, x.Id, permission),
                Order = x.Order,
                Unique = $"child:{x.Id}",
                Url = x.Url
            }).ToList();
        }

        public void SetChecked(IList<PermissionGrantResponse> all, IList<GroupsPermission> perm, int groupId)
        {
            foreach (var permissionGrantResponse in all)
            {
                if (permissionGrantResponse.Children.Count == 0 && !string.IsNullOrEmpty(permissionGrantResponse.Unique))
                {
                    foreach (var grantResponse in GlobalEnums.PermissionNames)
                    {
                        permissionGrantResponse.Children.Add(new PermissionGrantResponse
                        {
                            Unique = $"action:{permissionGrantResponse.FuncId}:{grantResponse.Key}",
                            FuncName = grantResponse.Value,
                            Checked = perm.Any(x => x.FunctionId == permissionGrantResponse.FuncId && (x.Permission & grantResponse.Key) == grantResponse.Key && x.GroupId == groupId)
                        });
                    }
                }
                else if (permissionGrantResponse.Children.Count > 0)
                    SetChecked(permissionGrantResponse.Children, perm, groupId);
            }
        }

        public async Task<IList<Functions>> GetFunctionsAsync()
        {
            try
            {
                return await _context.Functions.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<IList<GroupsPermission>> GetPermissionAsync(int groupId)
        {
            try
            {
                return await _context.GroupsPermission.Where(a => a.GroupId == groupId).ToListAsync();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<Database.Models.Groups> GetByCode(string code)
        {
            try
            {
                var q = await _context.Groups.FirstOrDefaultAsync(x => x.GroupCode.ToLower().Equals(code));
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0}", code);
                return null;
            }
        }

        public async Task<IList<Database.Models.Groups>> GetGroupsOurUser(int user)
        {
            try
            {
                var q = await (from g in _context.Groups
                               join ug in _context.UserGroups on g.Id equals ug.GroupId
                               where ug.UserId == user
                               select g).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }
    }
}
