using System;
using GPLX.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contants;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Unit;
using GPLX.Core.Contracts.User;
using GPLX.Database.Models;
using GPLX.Web.Filters;
using GPLX.Web.Models.Users;
using Serilog;
using Functions = GPLX.Core.Contants.Functions;

namespace GPLX.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;


        public HomeController(IUserRepository userRepository, IUnitRepository unitRepository)
        {
            _userRepository = userRepository;
            _unitRepository = unitRepository;
        }

        [Route("index.html")]
        [AuthorizeUser(Module = Functions.Home, Permission = PermissionConstant.VIEW)]

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
