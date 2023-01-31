using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Department;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Request.Signature;
using GPLX.Core.DTO.Request.Users;
using GPLX.Core.DTO.Response.Users;
using GPLX.Core.Enum;
using GPLX.Core.Extensions;
using GPLX.Infrastructure.Constants;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Filters;
using GPLX.Web.Models;
using GPLX.Web.Models.Users;
using GPLX.Web.Signature;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string _defaultRootFolder;
        private readonly string _fileHostView;
        private readonly string _physicalFolder;
        public UsersController(IUserRepository userRepository,
            ILogger<UsersController> logger, IGroupsRepository groupsRepository,
            IUnitRepository unitRepository, IDepartmentRepository departmentRepository, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _logger = logger;
            _groupsRepository = groupsRepository;
            _unitRepository = unitRepository;
            _departmentRepository = departmentRepository;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _defaultRootFolder = configuration.GetValue<string>("DefaultRootFolder");
            _fileHostView = configuration.GetValue<string>("FileHosting");
            _physicalFolder = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, _defaultRootFolder);
        }

        [AuthorizeUser(Module = Functions.UsersView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> List()
        {
            var model = new UsersListModel();
            try
            {

                string publicKey = UsersConst.PublicKey;
                var allUnits = await _unitRepository.GetAllAsync(string.Empty, 0, 1000);
                var allDeparts = await _departmentRepository.GetAll((int)GlobalEnums.StatusDefaultEnum.Active);
                var allGroups = await _groupsRepository.GetAllActiveGroups();

                var selectUnits = model.CreateDefaults("Chọn đơn vị");
                var selectUnitConcurrently = model.CreateDefaults("Chọn đơn vị");
                var selectDeparts = model.CreateDefaults("Chọn phòng ban");
                var selectGroups = model.CreateDefaults("Chọn chức vụ");
                foreach (var u in allUnits)
                {
                    // if (u.OfficesCode.Equals(GetUserUnit().UnitCode, StringComparison.CurrentCultureIgnoreCase))
                    //     continue;
                    selectUnits.Add(new SelectListItem { Text = $"[{u.OfficesCode}] {u.OfficesName}", Value = u.Id.ToString().StringAesEncryption(publicKey) });
                    selectUnitConcurrently.Add(new SelectListItem { Text = $"[{u.OfficesCode}] {u.OfficesName}", Value = u.OfficesCode });
                }
                foreach (var d in allDeparts)
                    selectDeparts.Add(new SelectListItem { Text = d.Name, Value = d.Id.ToString().StringAesEncryption(publicKey) });
                foreach (var g in allGroups)
                    selectGroups.Add(new SelectListItem { Text = $"[{g.GroupCode}]-{g.Name}", Value = g.Id.ToString().StringAesEncryption(publicKey) });

                model.Groups = selectGroups;
                model.Departments = selectDeparts;
                model.Units = selectUnits;
                model.UnitChanges = selectUnitConcurrently;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return View(model);
        }

        [AuthorizeUser(Module = Functions.UsersView, Permission = PermissionConstant.VIEW)]

        public async Task<IActionResult> Search(int length, int start, UsersSearchRequest @base)
        {
            var response = new UsersSearchResponse
            {
                Draw = (Request.Form["draw"].Count > 0 ? Request.Form["draw"][0] : "0").ToInt32()
            };
            try
            {
                @base.RequestPage = UsersConst.PublicKey;
                response = await _userRepository.Search(start, length, @base);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.NoContentMessage;
            }
            return Json(response);
        }
        [AuthorizeUser(Module = Functions.UsersView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> Settings(string record)
        {
            var model = new UsersListModel();
            try
            {
                string publicKey = UsersConst.PublicKey;
                var allUnits = await _unitRepository.GetAllAsync(string.Empty, 0, 1000);
                var allDeparts = await _departmentRepository.GetAll((int)GlobalEnums.StatusDefaultEnum.Active);
                var allGroups = await _groupsRepository.GetAllActiveGroups();
                var oUser = await _userRepository.GetUserByIdAsync(
                    int.TryParse(record.StringAesDecryption(UsersConst.PublicKey, true), out var i) ? i : -1);
                var oUnitManager = await _userRepository.GetUserUnitManages(oUser?.Id ?? 0);
                var oUserGroup = await _groupsRepository.GetGroupsOurUser(oUser?.Id ?? 0);
                var selectUnits = new List<SelectListItem>();
                var selectChanges = new List<SelectListItem>();
                var selectDeparts = model.CreateDefaults("Chọn phòng ban");
                var selectGroups = model.CreateDefaults("Chọn chức vụ");
                foreach (var u in allUnits)
                    selectUnits.Add(new SelectListItem
                    {
                        Text = $"[{u.OfficesCode}]-{u.OfficesName}",
                        Value = u.OfficesCode,
                        Selected = oUnitManager?.Any(x => x.OfficeCode.Equals(u.OfficesCode)) ?? false
                    });

                foreach (var u in allUnits)
                    selectChanges.Add(new SelectListItem
                    {
                        Text = $"[{u.OfficesCode}]-{u.OfficesName}",
                        Value = u.OfficesCode,
                        Selected = u.Id == oUser?.UnitId
                    });

                foreach (var d in allDeparts)
                    selectDeparts.Add(new SelectListItem { Text = d.Name, Value = d.Id.ToString().StringAesEncryption(publicKey), Selected = oUser?.DepartmentId == d.Id });
                foreach (var g in allGroups)
                    selectGroups.Add(new SelectListItem
                    {
                        Text = $"[{g.GroupCode}]-{g.Name}",
                        Value = g.Id.ToString().StringAesEncryption(publicKey),
                        Selected = oUserGroup?.Any(x => x.Id == g.Id) ?? false
                    });

                model.Groups = selectGroups;
                model.Departments = selectDeparts;
                model.Units = selectUnits;
                model.UnitChanges = selectChanges;
                model.CurrentlySetting = await _userRepository.GetUserConcurrently(oUser?.Id ?? 0);
                if (!string.IsNullOrEmpty(oUser?.PathSignature))
                    oUser.PathSignature = $"{_fileHostView}{oUser.PathSignature}";
                ViewBag.oUser = oUser;
                ViewBag.Record = record;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
            }
            return PartialView("_Settings", model);
        }
        [AuthorizeUser(Module = Functions.UsersView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnSetting(UsersConfigRequest request)
        {
            var response = new UsersConfigResponse();
            try
            {
                var sessionUser = GetUsersSessionModel();
                if (Request.Form.Files.Count > 0)
                {
                    var excelFile = Request.Form.Files[0];
                    string ownerFolder = sessionUser.UserName.CreateDatePhysicalPathFileToUser();
                    var fileCreateName = $"{Path.GetRandomFileName()}{Path.GetExtension(excelFile.FileName)}";
                    var folder = Path.Combine(_physicalFolder, ownerFolder);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    var file = Path.Combine(folder, fileCreateName);

                    await using var fileStream = new FileStream(file, FileMode.Create);
                    await excelFile.CopyToAsync(fileStream);
                    await fileStream.DisposeAsync();
                    fileStream.Close();

                    request.PathSignature = file.Replace(_physicalFolder, string.Empty).NormalizePath();
                }

                request.RequestPage = UsersConst.PublicKey;
                response = await _userRepository.Configs(request);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }

            return Json(response);
        }
        [AuthorizeUser(Module = Functions.UsersView, Permission = PermissionConstant.EDIT)]

        public async Task<IActionResult> OnSync()
        {
            var response = new EmptySearchResponseModel();
            try
            {
                var token = await _authenticateConnect.GetAccessTokenAsync(_configuration.GetSection("Authentication:AccessToken").Value, new MedAccesstokenRequest
                {
                    grant_type = _configuration.GetSection("AuthenSecret:grant_type").Value,
                    client_id = _configuration.GetSection("AuthenSecret:client_id").Value,
                    scope = _configuration.GetSection("AuthenSecret:scope").Value,
                    client_secret = _configuration.GetSection("AuthenSecret:client_secret").Value
                });
                var concurrentUsers = new ConcurrentBag<UserSync>();
                //var dataOnLoop = new List<UserSync>();
                var counters = new List<int>();
                for (int i = 0; i <= 20; i++)
                    counters.Add(i);

                Parallel.ForEach(counters, new ParallelOptions { MaxDegreeOfParallelism = 10 }, msg =>
                  {
                      var requestCallOnLoop =
                          _authenticateConnect.GetUsersAsync(_configuration.GetSection("Authentication:Users").Value, token, msg, 100).Result;
                      if (requestCallOnLoop.code == 999)
                      {
                          foreach (var u in requestCallOnLoop.items)
                              concurrentUsers.Add(u);
                          Log.Information("[{0}]: {1}", requestCallOnLoop.code, $"http://medid.medcom.vn/api/Account/GetLisAccountByPageSync?page={msg}&pageSize=100");
                      }
                      else
                      {
                          Log.Information("[{0}]: {1}", requestCallOnLoop.code, $"http://medid.medcom.vn/api/Account/GetLisAccountByPageSync?page={msg}&pageSize=100");
                      }
                  });
                if (concurrentUsers.Any())
                {

                    var dataGroup = concurrentUsers.GroupBy(x => x.UserId, (key, val) => new
                    {
                        k = key,
                        v = val.First()
                    }).Select(x => x.v).ToArray();

                    var syncOk = await _userRepository.AddRangeAsync(dataGroup);
                    if (!syncOk)
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                        response.Message = "Đồng bộ dữ liệu thành viên không thành công!";
                    }
                    else
                    {
                        response.Code = (int)GlobalEnums.ResponseCodeEnum.Success;
                        response.Message = "Đồng bộ dữ liệu thành viên thành công!";
                    }
                }
                else
                {
                    response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                    response.Message = "Đồng bộ dữ liệu thành viên không thành công!";
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                response.Code = (int)GlobalEnums.ResponseCodeEnum.Error;
                response.Message = GlobalEnums.ErrorMessage;
            }
            return Json(response);
        }

        public async Task<IActionResult> SwitchUnit(SwitchUnitRequest request)
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                request.UserId = sessionUser.UserId;
                var response = await _userRepository.SwitchUnit(request);
                return Json(response);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new SwitchUnitResponse
                {
                    Message = "Chuyển đơn vị không thành công!",
                    Code = (int)GlobalEnums.ResponseCodeEnum.Error
                });
            }
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                var sessionUser = GetUsersSessionModel();
                return View(sessionUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }

        public async Task<IActionResult> OnUpdateProfile(UserSignatureConfigRequest request)
        {
            try
            {
                request.UserId = GetUserId();

                var signature = new SignatureConnect();
                string acc = signature.GetAccess(new GetTokenBody
                {
                    client_id = _configuration.GetSection("Signature:APP_ID").Value,
                    client_secret = _configuration.GetSection("Signature:APP_SECRET").Value,
                    password = request.SignaturePass,
                    username = request.SignatureAcc
                }, _configuration.GetSection("Signature:SERVICE_GET_TOKENURL").Value);

                if (string.IsNullOrEmpty(acc))
                    return Json(new UserSignatureConfigResponse { Code = (int)GlobalEnums.ResponseCodeEnum.Error, Msg = "Tài khoản hoặc mật khẩu ký số không đúng!" });

                string enc = request.SignaturePass.StringAesEncryption(UsersConst.PublicKey);
                request.SignaturePass = enc;
                var req = await _userRepository.ConfigSignature(request);
                return Json(req);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return Json(new UserSignatureConfigResponse());
            }
        }
    }
}
