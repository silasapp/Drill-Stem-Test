using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using DST.Models.DB;
using DST.Controllers.Configurations;
using DST.Helpers;

namespace DST.Controllers.UsersManagement
{
   
    public class StaffsController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();

        public static int mydeskCount = 0;


        public StaffsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }





        /*
         * Changing portal theme for a staff.
         * 
         * id => encrypted type of theme (Light or Dark)
         * 
         */

        public JsonResult UseTheme(string id)
        {
            var theme = generalClass.Decrypt(id);
            var result = "";

            if (theme == "Error")
            {
                result = "Opps!!! Something went wrong trying to change your theme, please try again later.";
            }
            else
            {
                var user = _helpersController.getSessionUserID();

                var getUser = _context.Staff.Where(x => x.StaffId == user && x.DeleteStatus == false && x.ActiveStatus == true);

                if (getUser.Any())
                {
                    getUser.FirstOrDefault().Theme = theme;
                    getUser.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        HttpContext.Session.SetString(Authentications.AuthController.sessionTheme, generalClass.Encrypt(theme));

                        result = "Theme Changed";
                    }
                    else
                    {
                        result = "Opps!!! Something went wrong trying to update your theme, please try again later.";

                    }
                }
                else
                {
                    result = "Opps!!! Something went wrong trying to find your account. Maybe your accout has been deactivated.";
                }
            }

            _helpersController.LogMessages("Staff changing theme to : " + theme + " theme status result => " + result, _helpersController.getSessionEmail());

            return Json(result);
        }





        public IActionResult Dashboard()
        {
            int currentUser = _helpersController.getSessionUserID();

            var processingApps = _context.Applications.Where(x => x.Status == GeneralClass.Processing && x.DeletedStatus == false && x.IsProposedSubmitted == true).Count();
            
            var totalApps = _context.Applications.Where(x => x.DeletedStatus == false).Count();
            
            var totalPermits = (from p in _context.Permits.AsEnumerable()
                                join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                                where a.DeletedStatus == false
                                select new
                                {
                                    p
                                }).Count();

            var nom = _context.NominationRequest.Where(x => x.StaffId == _helpersController.getSessionUserID() && x.HasDone == false);

            ViewData["NominationRequest"] = nom.Count();
            ViewData["ProcessingApps"] = processingApps;
            ViewData["TotalApps"] = totalApps;
            ViewData["TotalPermits"] = totalPermits;

            _helpersController.LogMessages("Displaying Staff Dashboard.", _helpersController.getSessionEmail());

            return View();
        }



        /*
         * Getting the count of application on my desk
         * 
         */

        public JsonResult MyDeskCount()
        {
            var mydesk = from ad in _context.MyDesk.AsEnumerable()
                         join ap in _context.Applications.AsEnumerable() on ad.AppId equals ap.AppId
                         join cm in _context.Companies.AsEnumerable() on ap.CompanyId equals cm.CompanyId into Company
                         join ts in _context.AppTypeStage.AsEnumerable() on ap.AppTypeStageId equals ts.TypeStageId
                         join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                         join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                         join f in _context.Facilities.AsEnumerable() on ap.FacilityId equals f.FacilityId
                         orderby ad.DeskId descending
                         where ((ad.StaffId == _helpersController.getSessionUserID()) && (ad.HasWork == false) && (ap.DeletedStatus == false && ap.IsProposedSubmitted == true) && (Company.FirstOrDefault().DeleteStatus == false) && (s.DeleteStatus == false))
                         select new
                         {
                             DeskID = ad.DeskId,
                         };

            mydeskCount = mydesk.Count();
            return Json(mydesk.Count());
        }



        public JsonResult MySchduleCount()
        {
            var mysch = _context.Schdules.Where(x => x.Supervisor == _helpersController.getSessionUserID() && x.SupervisorApprove == 0 && x.DeletedStatus == false).Count();
            return Json(mysch);
        }


        public JsonResult MyNominationCount()
        {
            var count = _context.NominatedStaff.Where(x => x.StaffId == _helpersController.getSessionUserID() && x.IsActive == true && x.HasSubmitted == false);
            return Json(count.Count());
        }


    }
}
