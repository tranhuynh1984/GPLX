using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contants;
using GPLX.Core.Contracts.Department;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Users;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.User
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IGroupsRepository _groupsRepository;


        public UserRepository(Context context, IUnitRepository unitRepository, IMapper mapper, ILogger<UserRepository> logger, IDepartmentRepository departmentRepository, IGroupsRepository groupsRepository)
        {
            this._context = context;
            this._unitRepository = unitRepository;
            _mapper = mapper;
            _logger = logger;
            _departmentRepository = departmentRepository;
            _groupsRepository = groupsRepository;
        }

        public async Task<bool> AddRangeAsync(UserSync[] users)
        {
            try
            {
                var transaction = _context.Database.BeginTransaction();
                await transaction.CreateSavepointAsync("Before");
                try
                {
                    var listUsers = await _context.Users.ToListAsync();
                    var lstInsert = users.Where(a => listUsers.All(x => !x.UserId.Equals(a.UserId, StringComparison.OrdinalIgnoreCase))).ToList();
                    var lstMaps = new List<Users>();
                    foreach (var x in lstInsert)
                    {
                        var u = _mapper.Map<Users>(x);
                        var oUnit = await _unitRepository.GetByCodeAsync(x.OfficesCode);
                        u.Id = 0;
                        u.UnitId = oUnit?.Id;
                        u.UnitName = oUnit?.OfficesName;
                        lstMaps.Add(u);
                    }
                    await _context.Users.AddRangeAsync(lstMaps);
                    await _context.SaveChangesAsync();

                    var listOlds = users.Where(a => listUsers.Any(x => x.UserId.Equals(a.UserId, StringComparison.OrdinalIgnoreCase))).ToList();
                    var listUpdates = new List<Users>();
                    foreach (var old in listOlds)
                    {
                        var u = _mapper.Map<Users>(old);
                        var dbUser = listUsers.First(x => x.UserId.Equals(old.UserId, StringComparison.CurrentCultureIgnoreCase));
                        var oUnit = await _unitRepository.GetByCodeAsync(old.OfficesCode);
                        dbUser.UnitId = oUnit?.Id;
                        dbUser.UnitName = oUnit?.OfficesName;
                        dbUser.UnitName = oUnit?.OfficesName;
                        dbUser.Status = u.Status;
                        dbUser.UserPhone = u.UserPhone;
                        dbUser.UserEmail = u.UserEmail;
                        dbUser.UserImage = u.UserImage;
                        dbUser.AccountGuid = u.AccountGuid;
                        dbUser.UserCode = u.UserCode;
                        listUpdates.Add(dbUser);
                    }
                    _context.UpdateRange(listUpdates);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackToSavepointAsync("Before");
                    _logger.Log(LogLevel.Error, e, e.Message);
                    Log.Error(e, e.Message);

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }
        }

        public async Task<Users> GetUserByUserIdAsync(string userId)
        {
            try
            {
                var obj = await _context.Users.FirstOrDefaultAsync(a => a.UserId.ToLower().Equals(userId.ToLower()));
                return obj;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<IList<UserUnitsManages>> GetUserUnitManages(int userId)
        {
            try
            {
                var q = await _context.UserUnitsManages.Where(x => x.UserId == userId).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<bool> AddUserToUnit(int userId, int unitId)
        {
            try
            {
                var group = await this._unitRepository.GetByIdAsync(unitId);
                var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == userId);
                if (user != null && group != null)
                {
                    user.UnitId = group.Id;
                    user.UnitName = group.OfficesShortName;
                    _context.SaveChanges();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return false;
            }

        }

        public async Task<IList<Users>> GetAllAsync(string name, int offset, int limit)
        {
            try
            {
                var result = _context.Users.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                    result = result.Where(a => a.UserName.Contains(name));

                return (await result.ToListAsync()).Skip(offset).Take(limit).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return new List<Users>();
            }

        }

        public async Task<Users> GetUserByIdAsync(int id)
        {
            try
            {
                var obj = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
                return obj;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return null;
            }
        }

        public async Task<int> GetProcessRoleByUserIdAsync(int userid)
        {
            try
            {
                int processRoleId = 0;

                var query = _context.UserGroups.AsQueryable()
                .LeftJoin(_context.Groups, u => u.GroupId, g => g.Id, (u, g) => new
                {
                    Userid = u.UserId,
                    GroupCode = g.GroupCode
                });

                query = query.Where(x => x.Userid == userid);
                var data = await query.OrderByDescending(x => x.Userid).ToListAsync();

                if(data.Count > 0)
                {
                    string groupCode = data.FirstOrDefault().GroupCode;
                    var queryprocess = _context.ProcessMapRole.AsQueryable();
                    queryprocess = queryprocess.Where(x => x.GroupCode == groupCode);
                    var dataprocess = await queryprocess.OrderByDescending(x => x.GroupCode).ToListAsync();
                    if(dataprocess.Count>0)
                    {
                        processRoleId = dataprocess.FirstOrDefault().IDRole;
                    }
                }
                
                return processRoleId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return 0;
            }
        }


        public async Task<UsersSearchResponse> Search(int skip, int length, UsersSearchRequest request)
        {
            var response = new UsersSearchResponse();
            try
            {
                var query = _context.Users.GroupJoin(_context.UserGroups, w => w.Id, y => y.UserId,
                    (x, z) => new { Users = x, U = z }).SelectMany(

                    x => x.U.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        Users = x.Users,
                        G = y.GroupId
                    }
                );


                if (!string.IsNullOrEmpty(request.DepartmentRecord))
                {
                    int searchDepartmentId = int.TryParse(request.DepartmentRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchDepartmentId < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.Users.DepartmentId == searchDepartmentId);
                }

                if (!string.IsNullOrEmpty(request.GroupRecord))
                {
                    int searchGroupId = int.TryParse(request.GroupRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchGroupId < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.G == searchGroupId);
                }

                if (!string.IsNullOrEmpty(request.UnitRecord))
                {
                    int searchUnit = int.TryParse(request.UnitRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchUnit < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.Users.UnitId == searchUnit);

                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.Users.UserEmail.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserName.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserCode.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserId.ToLower().Contains(request.Keywords.ToLower())
                                             );
                var data = await query.Select(x => x.Users).Distinct().ToListAsync();
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;

                var dataResponse = new List<UsersSearchResponseData>();
                foreach (var u in data.Skip(skip).Take(length))
                {
                    var item = _mapper.Map<UsersSearchResponseData>(u);
                    item.Record = u.Id.ToString().StringAesEncryption(request.RequestPage);
                    dataResponse.Add(item);
                }

                response.Data = dataResponse;
                response.Draw = request.Draw;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                response.Draw = request.Draw;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }
            return response;
        }

        public async Task<UsersSearchResponse> SearchAll(UsersSearchRequest request)
        {
            var response = new UsersSearchResponse();
            try
            {
                var query = _context.Users.GroupJoin(_context.UserGroups, w => w.Id, y => y.UserId,
                    (x, z) => new { Users = x, U = z }).SelectMany(

                    x => x.U.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        Users = x.Users,
                        G = y.GroupId
                    }
                );


                if (!string.IsNullOrEmpty(request.DepartmentRecord))
                {
                    int searchDepartmentId = int.TryParse(request.DepartmentRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchDepartmentId < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.Users.DepartmentId == searchDepartmentId);
                }

                if (!string.IsNullOrEmpty(request.GroupRecord))
                {
                    int searchGroupId = int.TryParse(request.GroupRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchGroupId < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.G == searchGroupId);
                }

                if (!string.IsNullOrEmpty(request.UnitRecord))
                {
                    int searchUnit = int.TryParse(request.UnitRecord.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                    if (searchUnit < 0)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.NoContent;
                        response.Message = GlobalEnums.NoContentMessage;
                    }
                    else
                        query = query.Where(x => x.Users.UnitId == searchUnit);

                }

                if (!string.IsNullOrEmpty(request.Keywords))
                    query = query.Where(x => x.Users.UserEmail.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserName.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserCode.ToLower().Contains(request.Keywords.ToLower())
                                             || x.Users.UserId.ToLower().Contains(request.Keywords.ToLower())
                                             );
                var data = await query.Select(x => x.Users).Distinct().ToListAsync();
                response.RecordsFiltered = data.Count;
                response.RecordsTotal = data.Count;

                var dataResponse = new List<UsersSearchResponseData>();
                foreach (var u in data)
                {
                    var item = _mapper.Map<UsersSearchResponseData>(u);
                    item.Record = u.Id.ToString().StringAesEncryption(request.RequestPage);
                    dataResponse.Add(item);
                }

                response.Data = dataResponse;
                response.Draw = request.Draw;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                response.Draw = request.Draw;
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }
            return response;
        }

        public async Task<UsersConfigResponse> Configs(UsersConfigRequest request)
        {
            UsersConfigResponse response = new UsersConfigResponse();
            try
            {
                var oUser = await GetUserByIdAsync(request.RawId);
                if (oUser == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = GlobalEnums.NoContentMessage;
                }
                else
                {
                    var transaction = await _context.Database.BeginTransactionAsync();
                    await transaction.CreateSavepointAsync("Before");
                    try
                    {
                        #region Phòng ban / chức vụ
                        int settingDepartment = 0;
                        if (!string.IsNullOrEmpty(request.DepartmentRecord))
                        {
                            int iDepartment = int.TryParse(request.DepartmentRecord.StringAesDecryption(request.RequestPage, true), out var i) ? i : -1;
                            if (iDepartment < 0)
                            {
                                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                response.Message = "Dữ liệu không hợp lệ!";
                                return response;
                            }

                            settingDepartment = iDepartment;
                        }

                        var allOlder = await _context.UserGroups.Where(x => x.UserId == request.RawId).ToListAsync();
                        if (!string.IsNullOrEmpty(request.GroupRecords))
                        {
                            var groupIds = new List<int>();
                            foreach (var requestGroupRecord in request.GroupRecords.Split(","))
                            {
                                int iGroup = int.TryParse(requestGroupRecord.StringAesDecryption(request.RequestPage, true), out var i) ? i : -1;
                                if (iGroup < 0)
                                {
                                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                    response.Message = "Dữ liệu không hợp lệ!";
                                    return response;
                                }
                                groupIds.Add(iGroup);
                            }

                            var listAdd = groupIds.Where(x => allOlder.All(c => c.GroupId != x)).ToList();
                            var listDelete = allOlder.Where(x => groupIds.All(c => c != x.GroupId)).ToList();
                            if (listAdd.Count > 0)
                            {
                                var listUGroupsAdd = new List<UserGroups>();
                                listAdd.ForEach(c =>
                                {
                                    listUGroupsAdd.Add(new UserGroups
                                    {
                                        UserId = request.RawId,
                                        GroupId = c,
                                        Id = Guid.NewGuid()
                                    });
                                });
                                await _context.UserGroups.AddRangeAsync(listUGroupsAdd);
                                await _context.SaveChangesAsync();
                            }

                            if (listDelete.Count > 0)
                            {
                                _context.RemoveRange(listDelete);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else if (allOlder?.Count > 0)
                        {
                            _context.RemoveRange(allOlder);
                            await _context.SaveChangesAsync();
                        }

                        var oDepartment = await _departmentRepository.GetById(settingDepartment);

                        if (string.IsNullOrEmpty(request.GroupRecords) || oDepartment == null)
                        {
                            await transaction.ReleaseSavepointAsync("Before");

                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message = "Dữ liệu không hợp lệ!";
                            return response;
                        }

                        oUser.DepartmentId = settingDepartment;
                        oUser.DepartmentName = oDepartment.Name;
                        if (!string.IsNullOrEmpty(request.PathSignature))
                            oUser.PathSignature = request.PathSignature;
                        #endregion

                        #region Đơn vị được giao quản lý
                        var oOlderManageUnits =
                            await _context.UserUnitsManages.Where(x => request.RawId == x.UserId).ToListAsync();

                        if (string.IsNullOrEmpty(request.Units) && oOlderManageUnits.Count > 0)
                        {
                            _context.RemoveRange(oOlderManageUnits);
                            await _context.SaveChangesAsync();
                        }
                        else if (!string.IsNullOrEmpty(request.Units))
                        {
                            var unitSeparators = request.Units.Split(",");
                            var listAdd =
                                unitSeparators.Where(c => oOlderManageUnits.All(x => !x.OfficeCode.Equals(c))).ToList();
                            var listDelete = oOlderManageUnits.Where(c => unitSeparators.All(x => !c.OfficeCode.Equals(x))).ToList();
                            if (listDelete.Count > 0)
                                _context.RemoveRange(listDelete);
                            if (listAdd.Count > 0)
                            {
                                var units = await _context.Units.Where(x => unitSeparators.Contains(x.OfficesCode))
                                    .ToListAsync();
                                var unitAdd = units.Where(x => listAdd.Contains(x.OfficesCode))
                                    .Select(x => new UserUnitsManages
                                    {
                                        Id = Guid.NewGuid(),
                                        OfficeCode = x.OfficesCode,
                                        OfficeId = x.Id,
                                        UserId = request.RawId
                                    }).ToList();
                                await _context.AddRangeAsync(unitAdd);
                            }

                            await _context.SaveChangesAsync();
                        }
                        #endregion

                        #region Đơn vị kiêm nhiệm

                        var oUserUnit = await _unitRepository.GetByIdAsync(oUser.UnitId ?? 0);
                        // nếu set đơn vị kiêm nhiệm
                        // là đơn vị hiện tại của user
                        // không hợp lệ
                        if (oUserUnit != null && request.CurrentlySettings?.Any(c =>
                            c.UnitCode.Equals(oUserUnit.OfficesCode, StringComparison.CurrentCultureIgnoreCase)) == true)
                        {
                            response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                            response.Message = "Không thể chọn đơn vị hiện tại của thành viên làm đơn vị kiêm nhiệm!";
                            return response;
                        }


                        var oCurrentlySettings = await _context.UserConcurrently.Where(x => x.UserId == request.RawId).ToListAsync();
                        if (request.CurrentlySettings == null ||
                            request.CurrentlySettings.Count == 0 && oCurrentlySettings.Count > 0)
                        {
                            _context.RemoveRange(oCurrentlySettings);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var allUnits = await _unitRepository.GetAllAsync(string.Empty, 0, 1000);
                            var allGroups = await _groupsRepository.GetAllActiveGroups();
                            var listAdd = request.CurrentlySettings.Where(x =>
                                oCurrentlySettings.All(c => !c.UnitCode.Equals(x.UnitCode, StringComparison.CurrentCultureIgnoreCase))).ToList();

                            var listDelete = oCurrentlySettings.Where(x =>
                                request.CurrentlySettings.All(c => !c.UnitCode.Equals(x.UnitCode, StringComparison.CurrentCultureIgnoreCase))).ToList();

                            var listUpdate = oCurrentlySettings.Where(x =>
                                request.CurrentlySettings.Any(c => c.UnitCode.Equals(x.UnitCode, StringComparison.CurrentCultureIgnoreCase))).ToList();

                            if (listDelete.Count > 0)
                                _context.RemoveRange(listDelete);
                            if (listAdd.Count > 0)
                            {
                                var listDataAdd = new List<UserConcurrently>();
                                foreach (var groupOnConcur in listAdd)
                                {
                                    int iGroup = int.TryParse(groupOnConcur.GroupId.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                                    if (iGroup < 0)
                                    {
                                        await transaction.ReleaseSavepointAsync("Before");
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                        response.Message = "Dữ liệu không hợp lệ!";
                                        return response;
                                    }

                                    var oGroup = allGroups.FirstOrDefault(x => x.Id == iGroup);
                                    if (oGroup == null)
                                    {
                                        await transaction.ReleaseSavepointAsync("Before");
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                        response.Message = "Dữ liệu không hợp lệ!";
                                        return response;
                                    }
                                    var oUnit = allUnits.FirstOrDefault(x =>
                                        x.OfficesCode.Equals(groupOnConcur.UnitCode,
                                            StringComparison.CurrentCultureIgnoreCase));
                                    if (oUnit == null)
                                    {
                                        await transaction.ReleaseSavepointAsync("Before");
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                        response.Message = "Dữ liệu không hợp lệ!";
                                        return response;
                                    }
                                    listDataAdd.Add(new UserConcurrently
                                    {
                                        Id = Guid.NewGuid(),
                                        UserId = request.RawId,
                                        UnitCode = groupOnConcur.UnitCode,
                                        GroupId = iGroup,
                                        UnitId = oUnit.Id,
                                        UnitName = oUnit.OfficesName,
                                        GroupCode = oGroup.GroupCode
                                    });
                                    await _context.AddRangeAsync(listDataAdd);
                                }
                            }

                            if (listUpdate.Count > 0)
                            {
                                foreach (var u in listUpdate)
                                {
                                    var update = request.CurrentlySettings.FirstOrDefault(c =>
                                        c.UnitCode.Equals(u.UnitCode, StringComparison.CurrentCultureIgnoreCase));

                                    int iGroup = int.TryParse(update?.GroupId.StringAesDecryption(request.RequestPage), out var i) ? i : -1;
                                    if (iGroup < 0)
                                    {
                                        await transaction.ReleaseSavepointAsync("Before");
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                        response.Message = "Dữ liệu không hợp lệ!";
                                        return response;
                                    }

                                    var oGroup = allGroups.FirstOrDefault(x => x.Id == iGroup);
                                    if (oGroup == null)
                                    {
                                        await transaction.ReleaseSavepointAsync("Before");
                                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                                        response.Message = "Dữ liệu không hợp lệ!";
                                        return response;
                                    }
                                    if (u.GroupId != iGroup)
                                    {
                                        u.GroupId = oGroup.Id;
                                        u.GroupCode = oGroup.GroupCode;
                                    }
                                }
                                _context.UpdateRange(listUpdate);
                            }
                        }
                        #endregion

                        _context.Update(oUser);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Cập nhật thông tin thành công!";
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackToSavepointAsync("Before");
                        Log.Error(e, "{0}", request);
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Cập nhật thông tin không thành công!";
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
                return response;
            }
        }

        public async Task<IList<UserConcurrently>> GetUserConcurrently(int userId)
        {
            try
            {
                var q = await _context.UserConcurrently.Where(x => x.UserId == userId).ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return null;
            }
        }

        public async Task<SwitchUnitResponse> SwitchUnit(SwitchUnitRequest request)
        {
            try
            {
                var response = new SwitchUnitResponse();
                var oUser = await GetUserByIdAsync(request.UserId);
                if (oUser == null)
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Chuyển đơn vị không thành công!!!";
                }
                var oUserConcurrently = await GetUserConcurrently(request.UserId);
                var selectedOnConcurrently = oUserConcurrently.Any(x =>
                    x.UnitCode.Equals(request.UnitCode, StringComparison.CurrentCultureIgnoreCase));

                //nếu chọn đơn vị không nằm trong đơn vị kiêm nhiệm
                // --> đang chọn đơn vị gốc
                if (!selectedOnConcurrently && oUserConcurrently.Count > 0)
                {
                    foreach (var userConcurrently in oUserConcurrently)
                        userConcurrently.Selected = false;
                    _context.UserConcurrently.UpdateRange(oUserConcurrently);
                }
                else
                {
                    foreach (var userConcurrently in oUserConcurrently)
                        userConcurrently.Selected = userConcurrently.UnitCode.Equals(request.UnitCode, StringComparison.OrdinalIgnoreCase);
                    _context.UserConcurrently.UpdateRange(oUserConcurrently);
                }

                await _context.SaveChangesAsync();
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                response.Message = "Chuyển đơn vị thành công!";
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error");
                return new SwitchUnitResponse
                {
                    Code = (int)GlobalEnums.ResponseCodeEnum.Success,
                    Message = "Chuyển đơn vị thành công!"
                };
            }
        }

        public async Task<UserSignatureConfigResponse> ConfigSignature(UserSignatureConfigRequest rq)
        {
            try
            {
                var oUser = await GetUserByIdAsync(rq.UserId);
                if (oUser != null)
                {
                    oUser.AccDigitalSignature = rq.SignatureAcc;
                    oUser.PasswordDigitalSignature = rq.SignaturePass;

                    _context.Update(oUser);
                    await _context.SaveChangesAsync();
                    return new UserSignatureConfigResponse
                    {
                        Msg = "Cập nhật thông tin tài khoản ký số thành công!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Success
                    };
                }
                else
                {
                    return new UserSignatureConfigResponse
                    {
                        Msg = "Cập nhật thông tin tài khoản ký số không thành công!",
                        Code = (int)GlobalEnums.ResponseCodeEnum.Error
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return new UserSignatureConfigResponse
                {
                    Msg = GlobalEnums.ErrorMessage,
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                };
            }
        }
    }
}
