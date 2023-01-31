using GPLX.Core.Contracts;
using GPLX.Core.DTO.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contracts.Groups;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Core.DTO.Request.Unit;
using GPLX.Core.DTO.Response;
using GPLX.Core.DTO.Response.Users;
using GPLX.Core.Enum;
using GPLX.Database.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMedAuthenticateConnect _medAuthenticateConnect;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitRepository _unitRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IConfiguration _configuration;

        public AccountController(IMedAuthenticateConnect medAuthenticateConnect,
                                 IConfiguration configuration,
                                 IUserRepository userRepository,
                                 ILogger<AccountController> logger,
                                 IUnitRepository unitRepository,
                                 IGroupsRepository groupsRepository
                                 )
        {

            _userRepository = userRepository;
            _logger = logger;
            _unitRepository = unitRepository;
            _groupsRepository = groupsRepository;
            _medAuthenticateConnect = medAuthenticateConnect;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var token = await _medAuthenticateConnect.GetAccessTokenAsync(_configuration.GetSection("Authentication:AccessToken").Value, new MedAccesstokenRequest
                {
                    grant_type = _configuration.GetSection("AuthenSecret:grant_type").Value,
                    client_id = _configuration.GetSection("AuthenSecret:client_id").Value,
                    scope = _configuration.GetSection("AuthenSecret:scope").Value,
                    client_secret = _configuration.GetSection("AuthenSecret:client_secret").Value
                });

                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError(string.Empty, "Lỗi hệ thống, đăng nhập không thành công");
                    Log.Error("fetch GetAccessTokenAsync failed");
                }
                else
                {
                    //var result = await _medAuthenticateConnect.LoginAsync(token, request.UserName, request.Password, _configuration.GetSection("Authentication:Login").Value);

                    //Todo
                    var result = new MedApiResponse<UserSync>
                    {
                        code = 999,
                        items = new[]
                        {
                            new UserSync
                            {
                                UserId = request.UserName
                            },
                        }
                    };

                    var user = result.items?.FirstOrDefault();

                    //if (result.code == 999 && user != null)
                    //{
                    //    // check đơn vị là đơn vị trong hay ngoài đơn vị y tế
                    //    var checkUserIsSub = result.items?.FirstOrDefault(x => !string.IsNullOrEmpty(x.OfficesSub));

                    //    var dbUser = await _userRepository.GetUserByUserIdAsync(user.UserId);
                    //    if (dbUser == null)
                    //        ModelState.AddModelError(string.Empty, "Thông tin tài khoản chưa được đồng bộ sang hệ thống, vui lòng liên hệ quản trị viên!");
                    //    else
                    //    {
                    //        var oUnit = await _unitRepository.GetByIdAsync(dbUser.UnitId ?? 0);
                    //        var oGroups = await _groupsRepository.GetGroupsOurUser(dbUser.Id);
                    //        var sGroups = "0";
                    //        var positions = new List<PositionModel>();

                    //        if (oGroups != null)
                    //        {
                    //            foreach (var oGroup in oGroups)
                    //            {
                    //                positions.Add(new PositionModel
                    //                {
                    //                    PositionCode = oGroup.GroupCode,
                    //                    PositionId = oGroup.Id,
                    //                    PositionLevel = oGroup.Order,
                    //                    PositionName = oGroup.Name
                    //                });
                    //            }
                    //            sGroups = string.Join(";", oGroups.Select(x => x.Id.ToString()).ToArray());
                    //        }

                    //        var sessionUser = new UsersSessionModel
                    //        {
                    //            Positions = positions,
                    //            Unit = new UnitModel
                    //            {
                    //                UnitId = oUnit?.Id ?? 0,
                    //                UnitName = oUnit?.OfficesDesc,
                    //                UnitCode = oUnit?.OfficesCode,
                    //                UnitType = checkUserIsSub != null ? GlobalEnums.UnitIn : GlobalEnums.UnitOut,
                    //                IsSub = !string.IsNullOrEmpty(oUnit?.OfficesSub)
                    //            },
                    //            UserName = dbUser.UserId,
                    //            UserId = dbUser.Id,
                    //            Department = new DepartmentModel
                    //            {
                    //                DepartmentName = dbUser.DepartmentName,
                    //                DepartmentId = dbUser.DepartmentId ?? 0
                    //            }
                    //        };
                    //        var claims = new List<Claim>
                    //        {
                    //            new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
                    //            new Claim(ClaimTypes.Name, dbUser.UserName),
                    //            new Claim(ClaimTypes.GroupSid, sGroups),
                    //            new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(sessionUser))
                    //        };
                    //        var claimsIdentity = new ClaimsIdentity(
                    //            claims,
                    //            CookieAuthenticationDefaults.AuthenticationScheme);

                    //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    //        //await _unitRepository.SetInOut(new UnitSetTypeRequest { UnitCode = oUnit?.OfficesCode, Type = checkUserIsSub != null ? GlobalEnums.UnitIn : GlobalEnums.UnitOut });
                    //        return RedirectToAction("Index", "Home");
                    //    }
                    //}
                    //else
                    //    ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng!");

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                ModelState.AddModelError(string.Empty, "Lỗi hệ thống, đăng nhập không thành công");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
