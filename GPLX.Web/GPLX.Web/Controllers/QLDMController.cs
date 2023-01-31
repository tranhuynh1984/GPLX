using Microsoft.AspNetCore.Mvc;
using GPLX.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GPLX.Web.Controllers
{
    public class QLDMController : BaseController
    {
        private readonly ILogger<QLDMController> _logger;
        private readonly IMedAuthenticateConnect _authenticateConnect;
        private readonly IConfiguration _configuration;

        public QLDMController(ILogger<QLDMController> logger, IMedAuthenticateConnect authenticateConnect, IConfiguration configuration)
        {
            _logger = logger;
            _authenticateConnect = authenticateConnect;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
       
    }
}
