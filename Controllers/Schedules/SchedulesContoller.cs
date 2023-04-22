using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using DST.Controllers.Application;
using DST.Helpers;
using DST.Models.DB;
using DST.Controllers.Configurations;
using static DST.Models.GeneralModel;

namespace DST.Controllers.Schedules
{

    public class SchedulesController : Controller
    {
        public DST_DBContext _context;
        public IConfiguration _configuration;
        GeneralClass generalClass = new GeneralClass();
        HelpersController helpers;
        IHttpContextAccessor _httpContextAccessor;
        ApplicationsController applicationsController;


        public SchedulesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            helpers = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



        /*
         * id => encryted staff ID
         */
        // GET: SchedulesContoller
        public IActionResult Index(string id)
        {
            int staff_id = 0;
            var staffID = generalClass.Decrypt(id);

            var sch = from s in _context.Schdules
                      join a in _context.Applications on s.AppId equals a.AppId
                      join sf in _context.Staff on s.SchduleBy equals sf.StaffId
                      join sf2 in _context.Staff on s.Supervisor equals sf2.StaffId
                      join f in _context.Facilities on a.FacilityId equals f.FacilityId
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      join st in _context.States on f.State equals st.StateId
                      where s.DeletedStatus == false
                      select new MySchdule
                      {
                          ScheduleID = s.SchduleId,
                          FacilityID = f.FacilityId,
                          FacilityName = f.FacilityName,
                          FacilityAddress = f.FacilityAddress + ", " + st.StateName + " (" + f.Lga + ")",
                          ContactName = f.ContactName,
                          ContactPhone = f.ContactPhone,
                          ScheduleDate = s.SchduleDate.ToString(),
                          ScheduleBy = sf.LastName + " " + sf.FirstName,
                          CompanyName = c.CompanyName,
                          CompanyID = c.CompanyId,
                          staffID = s.SchduleBy,
                          CustomerResponse = s.CustomerAccept,
                          StaffComment = s.Comment,
                          SupervisorComment = s.SupervisorComment,
                          CustomerComment = s.CustomerComment,
                          ScheduleType = s.SchduleType,
                          ScheduleLocation = s.SchduleLocation,
                          CreatedAt = s.CreatedAt,
                          UpdatedAt = s.UpdatedAt,
                          Supervisor = s.Supervisor,
                          SupervisorApproved = s.SupervisorApprove,
                          ApprovedBy = sf2.LastName + " " + sf2.FirstName
                      };

            ViewData["ScheduleTitle"] = "All Schedules";
            ViewData["ScheduleStaffID"] = "";

            if (!string.IsNullOrWhiteSpace(id))
            {
                if (staffID == "Error")
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Schedule not found or not in correct format. Kindly contact support.") });
                }
                else
                {
                    staff_id = Convert.ToInt32(staffID);
                    sch = sch.Where(x => x.staffID == staff_id || x.Supervisor == staff_id);
                    ViewData["ScheduleTitle"] = "My Schedule";
                    ViewData["ScheduleStaffID"] = staffID;
                }
            }

            helpers.LogMessages("Displaying schedule for " + ViewData["ScheduleTitle"], helpers.getSessionEmail());

            return View(sch.ToList());
        }



        /*
         * id => encryted staff ID
         */
        // GET: SchedulesContoller
        public JsonResult ScheduleCalendar(string id)
        {
            var sch = from s in _context.Schdules
                      join a in _context.Applications on s.AppId equals a.AppId
                      join sf in _context.Staff on s.SchduleBy equals sf.StaffId
                      join f in _context.Facilities on a.FacilityId equals f.FacilityId
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      join st in _context.States on f.State equals st.StateId
                      where s.DeletedStatus == false
                      select new MySchdule
                      {
                          ScheduleID = s.SchduleId,
                          FacilityID = f.FacilityId,
                          FacilityName = f.FacilityName,
                          FacilityAddress = f.FacilityAddress + ", " + st.StateName + " (" + f.Lga + ")",
                          ContactName = f.ContactName,
                          ContactPhone = f.ContactPhone,
                          ScheduleDate = s.SchduleDate.ToString(),
                          ScheduleBy = sf.LastName + " " + sf.FirstName,
                          CompanyName = c.CompanyName,
                          CompanyID = c.CompanyId,
                          staffID = s.SchduleBy,
                          CustomerResponse = s.CustomerAccept,
                          StaffComment = s.Comment,
                          SupervisorComment = s.SupervisorComment,
                          CustomerComment = s.CustomerComment,
                          ScheduleType = s.SchduleType,
                          ScheduleLocation = s.SchduleLocation,
                          CreatedAt = s.CreatedAt,
                          UpdatedAt = s.UpdatedAt,
                          Supervisor = s.Supervisor,
                          SupervisorApproved = s.SupervisorApprove
                      };

            var calendar = from s in sch
                           select new
                           {
                               id = s.ScheduleID,
                               title = s.ScheduleType.ToUpper(),
                               start = Convert.ToDateTime(s.ScheduleDate),
                               company = s.CompanyName.ToUpper(),
                               facility = s.FacilityName.ToUpper() + "(" + s.FacilityAddress.ToLower() + ")",
                               location = s.ScheduleLocation,
                               contact = s.ContactName + " - " + s.ContactPhone,
                               customerResponse = s.CustomerResponse == 1 ? "Accepted" : s.CustomerResponse == 2 ? "Rejected" : "Awaiting Response",
                               customerComment = s.CustomerComment,
                               schedule = s.ScheduleBy,
                               staffComment = s.StaffComment,
                               supervisorResponse = s.SupervisorApproved == 1 ? "Accepted" : s.SupervisorApproved == 2 ? "Rejected" : "Awaiting Response",
                               supervisorComment = s.SupervisorComment,
                               allDay = false,
                               supervisor = s.Supervisor,
                               staff_id = s.staffID,
                           };

            if (!string.IsNullOrEmpty(id))
            {
                calendar = calendar.Where(x => x.supervisor == Convert.ToInt32(id) || x.staff_id == Convert.ToInt32(id));
            }

            return Json(calendar.ToList());
        }



        /*
         * Supervisor approving schedule
         * 
         * ScheduleID => encrypted schedule id
         */
        public JsonResult ApproveSchedule(string ScheduleID, string SupervisorComment)
        {
            string result = "";

            int scheduleID = 0;

            var sID = generalClass.Decrypt(ScheduleID);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                scheduleID = Convert.ToInt32(sID);

                var schedule = _context.Schdules.Where(x => x.SchduleId == scheduleID);

                var appid = schedule.FirstOrDefault().AppId;
                var date = schedule.FirstOrDefault().SchduleDate.ToString();
                var type = schedule.FirstOrDefault().SchduleType;
                var supervisor = schedule.FirstOrDefault().Supervisor;

                var app = from a in _context.Applications
                          join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                          join c in _context.Companies on a.CompanyId equals c.CompanyId
                          where a.AppId == appid
                          select new
                          {
                              CompanyID = c.CompanyId,
                              AppRefNo = a.AppRefNo,
                              CompanyName = c.CompanyName,
                              CompanyEmail = c.CompanyEmail,
                              StageID = s.AppStageId
                          };

                var stage = _context.ApplicationStage.Where(x => x.AppStageId == app.FirstOrDefault().StageID);

                schedule.FirstOrDefault().SupervisorApprove = 1;
                schedule.FirstOrDefault().SupervisorComment = SupervisorComment;
                schedule.FirstOrDefault().CustomerAccept = null;
                schedule.FirstOrDefault().CustomerComment = null;
                schedule.FirstOrDefault().UpdatedAt = DateTime.Now;

                if (_context.SaveChanges() > 0)
                {
                    result = "Schedule Approved";

                    var getApps = _context.Applications.Where(x => x.AppId == appid);

                    var getStaff = _context.Staff.Where(x => x.StaffId == (int)supervisor);

                    var actionFrom = helpers.getActionHistory(helpers.getSessionRoleID(), helpers.getSessionUserID());
                    var actionTo = helpers.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                    helpers.SaveHistory(getApps.FirstOrDefault().AppId, actionFrom, actionTo, "Schedule Approved", "Schedule approved by supervisor");
                    
                    // Saving Messages
                    string subject = stage.FirstOrDefault().StageName + " Application Schedule with Ref : " + getApps.FirstOrDefault().AppRefNo;
                    string content = "You have been schedule for a " + type + " on " + date + " with comment -: " + SupervisorComment + " Kindly find other details below.";
                    
                    var emailMsg = helpers.SaveMessage(getApps.FirstOrDefault().AppId, getApps.FirstOrDefault().CompanyId, subject, content);
                    
                    var sendEmail = helpers.SendEmailMessageAsync(app.FirstOrDefault().CompanyEmail, app.FirstOrDefault().CompanyName, subject,content, GeneralClass.COMPANY_NOTIFY, emailMsg);
                    // send email to marketer.
                }
                else
                {
                    result = "Something went wrong trying to approve your schedule.";
                }
            }

            helpers.LogMessages("Schedule Status " + result + ". Schedule ID : " + scheduleID, helpers.getSessionEmail());

            return Json(result);
        }


        /*
         * Supervisor rejecting schedule
         * 
         * ScheduleID => encrypted schedule id
         */

        public JsonResult RejectSchedule(string ScheduleID, string SupervisorComment)
        {
            string result = "";

            int scheduleID = 0;

            var sID = generalClass.Decrypt(ScheduleID);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                scheduleID = Convert.ToInt32(sID);

                var schedule = _context.Schdules.Where(x => x.SchduleId == scheduleID);

                schedule.FirstOrDefault().SupervisorApprove = schedule.FirstOrDefault().CustomerAccept = 2;
                schedule.FirstOrDefault().SupervisorComment = SupervisorComment;
                schedule.FirstOrDefault().UpdatedAt = DateTime.Now;
                schedule.FirstOrDefault().CustomerComment = "rejected";

                if (_context.SaveChanges() > 0)
                {
                    result = "Schedule Rejected";

                    var getStaff = _context.Staff.Where(x => x.StaffId == (int)schedule.FirstOrDefault().Supervisor);

                    var actionFrom = helpers.getActionHistory(helpers.getSessionRoleID(), helpers.getSessionUserID());
                    var actionTo = helpers.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                    helpers.SaveHistory(schedule.FirstOrDefault().AppId, actionFrom, actionTo, "Schedule Rejected", "Schedule rejected by supervisor");

                    var getApps = _context.Applications.Where(x => x.AppId == schedule.FirstOrDefault().AppId);

                    string subj = "Schedule for application (" + getApps.FirstOrDefault().AppRefNo + ") Rejecyted by Supervisor and Awaiting your response.";
                    string cont = "Schedule for application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been rejected by supervisor. Your action is required.";

                    var staff = _context.Staff.Where(x => x.StaffId == schedule.FirstOrDefault().SchduleBy);

                    var send = helpers.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                }
                else
                {
                    result = "Something went wrong trying to reject your schedule.";
                }
            }

            helpers.LogMessages("Schedule Status " + result + ". Schedule ID : " + scheduleID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Customer approving schedule
         * 
         * ScheduleID => encrypted schedule id
         */

        public JsonResult CustomerAcceptSchedule(string ScheduleID, string txtComment)
        {
            string result = "";

            int scheduleID = 0;

            var sID = generalClass.Decrypt(ScheduleID);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                scheduleID = Convert.ToInt32(sID);

                var sch = _context.Schdules.Where(x => x.SchduleId == scheduleID && x.DeletedStatus == false);


                if (sch.Any())
                {
                    int appid = sch.FirstOrDefault().AppId;
                    int scheduleBy = sch.FirstOrDefault().SchduleBy;

                    sch.FirstOrDefault().CustomerAccept = 1;
                    sch.FirstOrDefault().CustomerComment = txtComment;
                    sch.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        var app = _context.Applications.Where(x => x.AppId == appid);

                        var getStaff = _context.Staff.Where(x => x.StaffId == scheduleBy);

                        var actionFrom = helpers.getActionHistory(helpers.getSessionRoleID(), helpers.getSessionUserID());
                        var actionTo = helpers.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                        helpers.SaveHistory(appid, actionFrom, actionTo, "Schedule Approved", "Customer accepted the Schedule");

                        result = "Schedule Accepted";

                        var getApps = _context.Applications.Where(x => x.AppId == appid);

                        string subj = "Schedule for application (" + getApps.FirstOrDefault().AppRefNo + ") Approved by the Marketer and Awaiting your response.";
                        string cont = "Schedule for application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been approved by the Marketer. Your action is required.";
                        
                        var staff = _context.Staff.Where(x => x.StaffId == scheduleBy);

                        var send = helpers.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                       
                        helpers.SaveMessage(appid, (int)app.FirstOrDefault().CompanyId, subj, cont);
                    }
                    else
                    {
                        result = "Something went wrong trying to approve this schedule. Please try again later.";
                    }
                }
                else
                {
                    result = "Something went wrong. Your schedule was not found.";
                }
            }

            helpers.LogMessages("Schedule Status " + result + ". Schedule ID : " + scheduleID, helpers.getSessionEmail());

            return Json(result);
        }







        public JsonResult CustomerRejectSchedule(string ScheduleID, string txtComment)
        {
            string result = "";

            int scheduleID = 0;

            var sID = generalClass.Decrypt(ScheduleID);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                scheduleID = Convert.ToInt32(sID);

                var sch = _context.Schdules.Where(x => x.SchduleId == scheduleID && x.DeletedStatus == false);

                int appid = sch.FirstOrDefault().AppId;
                int scheduleBy = sch.FirstOrDefault().SchduleBy;

                if (sch.Any())
                {
                    sch.FirstOrDefault().CustomerAccept = 2;
                    sch.FirstOrDefault().CustomerComment = txtComment;
                    sch.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        var app = _context.Applications.Where(x => x.AppId == appid);

                        result = "Schedule Rejected";

                        var getStaff = _context.Staff.Where(x => x.StaffId == scheduleBy);

                        var actionFrom = helpers.getActionHistory(helpers.getSessionRoleID(), helpers.getSessionUserID());
                        var actionTo = helpers.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                        helpers.SaveHistory(appid, actionFrom, actionTo, "Schedule Rejected", "Customer acceopted the Schdule");

                        var getApps = _context.Applications.Where(x => x.AppId == appid);

                        string subj = "Schedule for application (" + getApps.FirstOrDefault().AppRefNo + ") Rejected by the Marketer";
                        string cont = "Schedule for application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been rejected by the Marketer. Your action is required.";
                        
                        var staff = _context.Staff.Where(x => x.StaffId == scheduleBy);

                        var send = helpers.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                        helpers.SaveMessage(appid, (int)app.FirstOrDefault().CompanyId, subj, cont);

                    }
                    else
                    {
                        result = "Something went wrong trying to reject this schedule. Please try again later.";
                    }
                }
                else
                {
                    result = "Something went wrong. Your schedule was not found.";
                }
            }

            helpers.LogMessages("Schedule Status " + result + ". Schedule ID : " + scheduleID, helpers.getSessionEmail());

            return Json(result);
        }

    }
}
