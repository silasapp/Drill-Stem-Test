using System;
using DST.Controllers.Authentications;
using DST.Controllers.Configurations;
using DST.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DST.Controllers.Authentications
{

    [Authorize]
    public class SessionController : Controller
    {

        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        public DST_DBContext _context;
        HelpersController _helpersController;

        public SessionController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }


        /*
       * To check if user is still logged in
       */
        [AllowAnonymous]
        public JsonResult CheckSession()
        {
            try
            {
                var session = _helpersController.getSessionEmail();

                string result = "";

                if (session == null || session == "Error" || session == "")
                {
                    result = "true";
                }
                return Json(result);

            }
            catch (Exception)
            {
                return Json("true");
            }
        }

    }
}