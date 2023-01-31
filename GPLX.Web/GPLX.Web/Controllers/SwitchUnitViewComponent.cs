using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Database.Models;
using GPLX.Infrastructure.Extensions;
using GPLX.Web.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GPLX.Web.Controllers
{
    public class SwitchUnitViewComponent : ViewComponent
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;

        public SwitchUnitViewComponent(IUnitRepository unitRepository, IUserRepository userRepository)
        {
            _unitRepository = unitRepository;
            _userRepository = userRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new UserConcurrentlyModel();

            try
            {
                int sessionUserId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    sessionUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToInt32() ?? 0;
                    if (sessionUserId == 0)
                        return View("_SwitchUnit", model); ;
                }

                var oUser = await _userRepository.GetUserByIdAsync(sessionUserId);
                var allConcurrently = await _userRepository.GetUserConcurrently(sessionUserId);
                var oUnit = await _unitRepository.GetByIdAsync(oUser?.UnitId ?? 0);
                model.AnyConcurrently = allConcurrently?.Count > 0;

                if (allConcurrently?.Count > 0)
                {
                    foreach (var userConcurrently in allConcurrently)
                        userConcurrently.UnitName = $"{userConcurrently.UnitName}";
                    if (oUnit != null)
                    {
                        allConcurrently.Add(new UserConcurrently
                        {
                            UnitId = oUnit.Id,
                            UnitName = $"{oUnit.OfficesShortName ?? oUnit.OfficesName}",
                            UnitCode = oUnit.OfficesCode,
                            Selected = !allConcurrently.Any(x => x.Selected)
                        });
                    }

                    model.UserConcurrently = allConcurrently;
                }
                else
                {
                   model.UserConcurrently = new List<UserConcurrently>
                    {
                        new UserConcurrently
                        {
                            UnitId = oUnit.Id,
                            UnitName = $"{oUnit.OfficesShortName ?? oUnit.OfficesName}",
                            UnitCode = oUnit.OfficesCode,
                            Selected = true
                        }
                    };
                    model.AnyConcurrently = true;
                }
            }
            catch (System.Exception e)
            {
                Log.Error(e, "Error");
            }
            return View("_SwitchUnit", model);
        }
    }
}
