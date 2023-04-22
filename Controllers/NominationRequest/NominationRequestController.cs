using DST.Controllers.Configurations;
using DST.Helpers;
using DST.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using static DST.Models.GeneralModel;

namespace DST.Controllers.NominationRequest
{
    [Authorize]

    public class NominationRequestController : Controller
    {

        private readonly DST_DBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        private readonly HelpersController _helpersController;
        private readonly GeneralClass generalClass = new GeneralClass();
        public RestSharpServices _restService = new RestSharpServices();

        public NominationRequestController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



       
        public IActionResult Requests(string id)
        {
            List<NominationRequestList> requestLists = new List<NominationRequestList>();

            var nom = (from n in _context.NominationRequest.AsEnumerable()
                      join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                      join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                      join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                      select new NominationRequestList
                      {
                          StaffName = s.LastName + " " + s.FirstName,
                          Email = s.StaffEmail,
                          StaffId = s.StaffId,
                          NominationRequestId = n.RequestId,
                          Role = r.RoleName,
                          RefNo = a.AppRefNo,
                          AppId = a.AppId,
                          isDone = n.HasDone,
                          Comment = n.Comment
                      }).ToList();

            if(id == "_desk")
            {
                requestLists = nom.Where(x => x.StaffId == _helpersController.getSessionUserID() && x.isDone == false).ToList();

                if (requestLists.Any())
                {
                    ViewData["NominationName"] = "Pending Nomination request for " + requestLists.FirstOrDefault().StaffName.ToString();
                }
                else
                {
                    ViewData["NominationName"] = "No record to show";
                }
            }
            else if(id == "_self")
            {
                requestLists = nom.Where(x => x.StaffId == _helpersController.getSessionUserID()).ToList();

                if (requestLists.Any())
                {
                    ViewData["NominationName"] = "All Nomination request for " + requestLists.FirstOrDefault().StaffName.ToString();
                }
                else
                {
                    ViewData["NominationName"] = "No record to show";
                }
            }
            else
            {
                ViewData["NominationName"] = "Showing all request";
                requestLists = nom.ToList();
            }

            return View(requestLists.ToList());
        }


        public JsonResult GetNominationRequest()
        {
            var nom = _context.NominationRequest.Where(x => x.StaffId == _helpersController.getSessionUserID() && x.HasDone == false);
            return Json(nom.Count());
        }

    }
}
