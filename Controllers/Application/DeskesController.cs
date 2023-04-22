using DST.Controllers.Configurations;
using DST.Helpers;
using DST.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using static DST.Models.GeneralModel;

namespace BHP.Controllers.Applications
{
    [Authorize]
    public class DeskesController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();


        public DeskesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



        public IActionResult StaffDesk()
        {
            var staff = from sf in _context.Staff.AsEnumerable()
                        join fo in _context.FieldOffices.AsEnumerable() on sf.FieldOfficeId equals fo.FieldOfficeId
                        join r in _context.UserRoles.AsEnumerable() on sf.RoleId equals r.RoleId
                        where (r.RoleName != GeneralClass.SUPPORT && !r.RoleName.Contains("ADMIN"))
                        select new StaffDesk
                        {
                            AppCount = _context.MyDesk.Where(x => x.StaffId == sf.StaffId && x.HasWork == false).ToList().AsEnumerable().GroupBy(x => x.AppId).Count(),
                            AllAppCount = _context.MyDesk.Where(x => x.StaffId == sf.StaffId).ToList().AsEnumerable().GroupBy(x => x.AppId).Count(),
                            StaffName = sf.LastName + " " + sf.FirstName,
                            StaffEmail = sf.StaffEmail,
                            StaffID = sf.StaffId,
                            StaffRole = r.RoleName,
                            FieldOffice = fo.OfficeName,
                            ActiveStatus = sf.ActiveStatus == true ? "Active" : "Deactivated",
                            DeletedStatus = sf.DeleteStatus == true ? "Deleted" : "Active"
                        };

            _helpersController.LogMessages("Displaying applications with staff.",  _helpersController.getSessionEmail());

            return View(staff.ToList());
        }




        public IActionResult StaffDeskApps(string id, string option)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(option))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Staff identification link for getting applications is broken or not in correct format.") });
            }

            int staffID = 0;

            var staff_id = generalClass.Decrypt(id);

            if (staff_id == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Staff identification link for getting applications is broken or not in correct format.") });
            }
            else
            {
                staffID = Convert.ToInt32(staff_id);

                var Staffs = _context.Staff.Where(x => x.StaffId == staffID);

                var staffDesk = from d in _context.MyDesk.AsEnumerable()
                                join a in _context.Applications.AsEnumerable() on d.AppId equals a.AppId
                                join sf in _context.Staff.AsEnumerable() on d.StaffId equals sf.StaffId into Staff
                                from s in Staff.DefaultIfEmpty()
                                join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                                join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                                join st in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals st.AppStageId
                                join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId

                                where a.DeletedStatus == false && d.StaffId == staffID
                                select new MyApps
                                {
                                    DeskID = d.DeskId,
                                    AppID = a.AppId,
                                    RefNo = a.AppRefNo,
                                    Staff = s?.LastName + " " + s?.FirstName + " ("+ s?.StaffEmail + ")",
                                    HasWorked = d.HasWork,
                                    Status = a.Status,
                                    DateApplied = a.DateApplied,
                                    CompanyID = c.CompanyId,
                                    CompanyName = c.CompanyName,
                                    CompanyEmail = c.CompanyEmail,
                                    Stage = ty.TypeName + " - " + st.StageName,
                                    CompanyAddress = c.Address + ", " + c.City + ", " + c.StateName,
                                };

                if (option == "_desk")
                {
                    staffDesk = staffDesk.Where(x => x.HasWorked == false);
                }
               

                var staffDeskResult = staffDesk.GroupBy(x => x.AppID).Select(c => c.FirstOrDefault()).ToList();

                ViewData["StaffDeskDetails"] = staffDeskResult.Count() + " application(s) on " + Staffs.FirstOrDefault().LastName.ToUpper() + " " + Staffs.FirstOrDefault().FirstName.ToUpper() + "'s desk".ToLower();
                ViewData["OriginalStaffID"] = staffID;

                _helpersController.LogMessages("Displaying applications on a staff desk.",  _helpersController.getSessionEmail());

                return View(staffDeskResult);
            }
        }



        /*
        * Getting staffs to re-rout application too
        * NOTE : the new staffs must be in the same role with the previous staff
        * 
        * staff => encrypted previous staff id
        */

        public JsonResult GetRouteStaff(string staff)
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var sortColumn = HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
            var txtSearch = HttpContext.Request.Form["search[value]"][0];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            int staffID = 0;

            var staff_id = generalClass.Decrypt(staff);

            if (staff_id == "Error")
            {
                return Json("Application report link error");
            }
            else
            {
                staffID = Convert.ToInt32(staff_id);

                var previousStaff = _context.Staff.Where(x => x.StaffId == staffID);
                var staffs = _context.Staff.Where(x => x.RoleId == previousStaff.FirstOrDefault().RoleId && x.FieldOfficeId == previousStaff.FirstOrDefault().FieldOfficeId && x.LocationId == previousStaff.FirstOrDefault().LocationId && x.ActiveStatus == true && x.DeleteStatus == false && x.StaffId != previousStaff.FirstOrDefault().StaffId);

                totalRecords = staff.Count();
                var data = staffs.Skip(skip).Take(pageSize).ToList();

                _helpersController.LogMessages("Displaying list of staff to reroute application to.",  _helpersController.getSessionEmail());

                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });
            }
        }




        /*
         * Rerouting application from a previous staff to a new staff.
         * 
         * staffID => the new staff to route the application to
         * AppID => a list of all application ids of previous staff applications
         */

        public JsonResult RerouteApps(int staffID, string previousStaff, List<int> AppID)
        {
            string result = "";
            int done = 0;

            int previous_staff = generalClass.DecryptIDs(previousStaff);

            if (previous_staff == 0)
            {
                result = "Something went wrong cannot find reference for previous selected staff.";
            }
            else
            {
                foreach (var app in AppID)
                {
                    var dsk = _context.MyDesk.Where(x => x.AppId == app && x.StaffId == previous_staff);

                    if (dsk.Any())
                    {
                        foreach (var d in dsk.ToList())
                        {
                            d.StaffId = staffID;
                            d.UpdatedAt = DateTime.Now;
                        }
                    }

                    var apps = _context.Applications.Where(x => x.AppId == app && x.CurrentDeskId == previous_staff && x.DeletedStatus == false);
                   
                    if (apps.Any())
                    {
                        apps.FirstOrDefault().CurrentDeskId = staffID;
                    }

                    done += _context.SaveChanges();
                }

                if (done > 0)
                {
                    result = "1|" + done + " re-routed successfully ";
                }
                else
                {
                    result = "0|Something went wrong trying to reroute one or more application. Please try again later.";
                }
            }

            _helpersController.LogMessages(result + "Request ID : " + string.Join(",", AppID), _helpersController.getSessionEmail());

            return Json(result);
        }



    }
}