using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DST.Models.DB;
using DST.Helpers;
using Microsoft.Extensions.Configuration;
using DST.Controllers.Configurations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using static DST.Models.GeneralModel;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace DST.Controllers.Application
{
    public class ApplicationsController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();

        public ApplicationsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        /*
        * Fetching all applications for viewing
        * 
        * Id => encrypted company id
        * option => the type of application report to display (ALL/Company)
        */
        public IActionResult Index(string id, string option)
        {
            var apps = from a in _context.Applications.AsEnumerable()
                       join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                       join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                       join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                       join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                       join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                       where a.DeletedStatus == false
                       select new
                       {
                           AppID = a.AppId,
                           RefNo = a.AppRefNo,
                           Stage = ty.TypeName + " - " + s.StageName,
                           CompanyName = c.CompanyName,
                           CompanyAddress = c.Address + ", " + c.City + ", " + c.StateName,
                           CompanyEmail = c.CompanyEmail,
                           CompanyID = a.CompanyId,
                           Status = a.Status,
                           DateApplied = a.DateApplied == null ? "" : a.DateApplied.ToString(),
                           DateSubmitted = a.DateSubmitted == null ? "" : a.DateSubmitted.ToString(),
                           DeletedStatus = a.DeletedStatus,
                           isProposalSubmitted = a.IsProposedSubmitted == true ? "YES" : "NO",
                           isReportSubmitted = a.IsReportSubmitted == true ? "YES" : "NO",
                           isProposalApproved = a.IsProposedApproved == true ? "YES" : "NO",
                           isReportApproved = a.IsReportApproved == true ? "YES" : "NO",
                           WellDetails = f.FacilityName,
                       };

            List<MyApp> myApps = new List<MyApp>();

            foreach (var a in apps)
            {
                var dks = _context.MyDesk.Where(x => x.AppId == a.AppID).OrderByDescending(x => x.DeskId);

                if (dks.Any())
                {
                    myApps.Add(new MyApp
                    {
                        AppID = a.AppID,
                        RefNo = a.RefNo,
                        CompanyName = a.CompanyName,
                        CompanyAddress = a.CompanyAddress,
                        CompanyEmail = a.CompanyEmail,
                        CompanyID = a.CompanyID,
                        Stage = a.Stage,
                        Status = a.Status,
                        Rate = a.Status == GeneralClass.Approved ? "100%" :
                                a.Status == GeneralClass.Rejected ? "0%" :
                                  dks.FirstOrDefault().Sort == 1 ? "20%" :
                                  dks.FirstOrDefault().Sort == 2 ? "76%" :
                                  dks.FirstOrDefault().Sort == 3 ? "98%" :
                                  dks.FirstOrDefault().Sort == 4 ? "20%" :
                                  dks.FirstOrDefault().Sort == 5 ? "76%" :
                                  dks.FirstOrDefault().Sort == 6 ? "98%" : "XX%",
                        DateApplied = a.DateApplied,
                        DateSubmitted = a.DateSubmitted,
                        isSubmitted = a.isProposalSubmitted,
                        isReportSubmitted = a.isReportSubmitted,
                        isProposalApproved = a.isProposalApproved,
                        isReportApproved = a.isReportApproved,
                        DeletedStatus = a.DeletedStatus,
                        WellDetails = a.WellDetails
                    });
                }
                else
                {
                    myApps.Add(new MyApp
                    {
                        AppID = a.AppID,
                        RefNo = a.RefNo,
                        CompanyName = a.CompanyName,
                        CompanyAddress = a.CompanyAddress,
                        CompanyEmail = a.CompanyEmail,
                        Stage = a.Stage,
                        CompanyID = a.CompanyID,
                        Status = a.Status,
                        Rate = a.Status == GeneralClass.Approved ? "100%" :
                                a.Status == GeneralClass.Rejected ? "0%" :
                                  dks.FirstOrDefault()?.Sort == 1 ? "20%" :
                                  dks.FirstOrDefault()?.Sort == 2 ? "76%" :
                                  dks.FirstOrDefault()?.Sort == 3 ? "98%" :
                                  dks.FirstOrDefault()?.Sort == 4 ? "20%" :
                                  dks.FirstOrDefault()?.Sort == 5 ? "76%" :
                                  dks.FirstOrDefault()?.Sort == 6 ? "98%" : "XX%",

                        DateApplied = a.DateApplied,
                        DateSubmitted = a.DateSubmitted,
                        isSubmitted = a.isProposalSubmitted,
                        isReportSubmitted = a.isReportSubmitted,
                        isProposalApproved = a.isProposalApproved,
                        isReportApproved = a.isReportApproved,
                        DeletedStatus = a.DeletedStatus,
                        WellDetails = a.WellDetails
                    });
                }

            }

            ViewData["ClassifyApp"] = "All Applications";

            if (apps.Any())
            {
                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(option))
                {
                    int ids = 0;
                    var type = generalClass.Decrypt(id);
                    var general_id = generalClass.Decrypt(option);

                    ids = Convert.ToInt32(general_id);

                    if (type == "_company")
                    {
                        myApps = myApps.Where(x => x.CompanyID == ids).ToList();

                        ViewData["ClassifyApp"] = "All applications for " + myApps.FirstOrDefault()?.CompanyName + " Company";

                    }
                    else if(type == "_report")
                    {
                        myApps = myApps.Where(x => x.isReportSubmitted == "YES").ToList();

                        ViewData["ClassifyApp"] = "All submitted applications reports";
                    }
                }
                _helpersController.LogMessages("Displaying all record for " + ViewData["ClassifyApp"], _helpersController.getSessionEmail());
                return View(myApps);
            }
            else
            {
                return View(myApps);
            }
        }

        /*
        * Getting all companies
        */
        public IActionResult AllCompanies()
        {
            var com = _context.Companies;
            _helpersController.LogMessages("Displaying all companies...", _helpersController.getSessionEmail());
            return View(com.ToList());
        }

        public IActionResult AllNominations()
        {
            var get = from n in _context.NominatedStaff
                      join a in _context.Applications on n.AppId equals a.AppId
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                      join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                      join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                      join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                      join st in _context.Staff on n.StaffId equals st.StaffId
                      join fo in _context.FieldOffices on st.FieldOfficeId equals fo.FieldOfficeId
                      select new NominatedList
                      {
                          AppRef = a.AppRefNo,
                          CreatedAt = n.CreatedAt,
                          hasSubmitted = n.HasSubmitted,
                          isActive = n.IsActive,
                          CompanyName = c.CompanyName,
                          StaffName = st.LastName + " " + st.FirstName,
                          AppID = n.AppId,
                          NominationID = n.NominateId,
                          AppId = n.AppId,
                          FieldOffice = fo.OfficeName,
                          WellDetails = f.FacilityName,
                      };

            return View(get.ToList());
        }

        public IActionResult MyDesk()
        {
            return View();
        }

        /*
        * Displaying all application on a particular staff desk
        */
        public JsonResult GetMyDeskApps()
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

            var staff = _helpersController.getSessionUserID();


            var get = from ad in _context.MyDesk.AsEnumerable()
                      join a in _context.Applications.AsEnumerable() on ad.AppId equals a.AppId
                      join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                      join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                      join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                      join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                      join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                      orderby ad.DeskId descending
                      where ((ad.StaffId == staff && ad.HasWork == false) && (a.DeletedStatus == false && a.IsProposedSubmitted == true) && (c.DeleteStatus == false))
                      select new
                      {
                          DeskID = ad.DeskId,
                          AppId = a.AppId,
                          HasWork = ad.HasWork,
                          HasPushed = ad.HasPushed,
                          ProcessID = ad.ProcessId,
                          RefNo = a.AppRefNo,
                          CompanyName = c.CompanyName,
                          WellDetails = f.FacilityName,
                          DateApplied = a.DateApplied.ToString(),
                          Stage = ty.TypeName + " - " + s.StageName,
                          DateSubmitted = a.DateSubmitted.ToString(),
                          UpdatedAt = ad.UpdatedAt.ToString(),
                          Status = a.Status,
                          ProposalApproved = a.IsProposedApproved == true ? "YES" : "NO",
                          ReportApproved = a.IsReportApproved == true ? "YES" : "NO",
                          Activity = ad.Sort == 1 ? "(20%) Application Verification; Work (Create Schedule and Fill Form); Approve/Reject" :
                                 ad.Sort == 2 ? "(76%) Application Verification; Work (Create Schedule and Fill Form); Approve/Reject" :
                                 ad.Sort == 3 ? "(98%) Application Verification; Approve/Reject" :
                                 ad.Sort == 4 ? "(20%) Report Verification; Work (Create Schedule and Fill Form); Approve/Reject" :
                                 ad.Sort == 5 ? "(76%) Report Verification; Work (Create Schedule and Fill Form); Approve/Reject" :
                                 ad.Sort == 6 ? "(98%) Report Approve/Reject (Verify Again)"
                                 : "Application Report Verification; Work (Create Schedule and Fill Form); Approve/Reject"
                      };


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    get = sortColumn == "refNo" ? get.OrderByDescending(c => c.RefNo) :
                               sortColumn == "companyName" ? get.OrderByDescending(c => c.CompanyName) :
                               sortColumn == "dateApplied" ? get.OrderByDescending(c => c.DateApplied) :
                               sortColumn == "dateSubmitted" ? get.OrderByDescending(c => c.DateSubmitted) :
                               get.OrderByDescending(c => c.DeskID + " " + sortColumnDir);
                }
                else
                {
                    get = sortColumn == "refNo" ? get.OrderBy(c => c.RefNo) :
                                sortColumn == "companyName" ? get.OrderBy(c => c.CompanyName) :
                               sortColumn == "dateApplied" ? get.OrderBy(c => c.DateApplied) :
                               sortColumn == "dateSubmitted" ? get.OrderBy(c => c.DateSubmitted) :
                               get.OrderBy(c => c.DeskID + " " + sortColumnDir);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                get = get.Where(c => c.RefNo.Contains(txtSearch.ToUpper()) || c.CompanyName.Contains(txtSearch.ToUpper()));
            }

            totalRecords = get.Count();
            var data = get.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying list of applications on staff desk.", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });
        }

        /*
        * Viewing application details with operation control
        * 
        * 
        * id => encrypted desk id
        * option => encrypted process id
        */
        public IActionResult ViewApplication(string id, string option)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(option))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }

            var deskID = generalClass.DecryptIDs(id);
            var processID = generalClass.DecryptIDs(option);

            if (deskID == 0 || processID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                List<ApplicationDetailsModel> applicationDetailsModels = new List<ApplicationDetailsModel>();

                var staff = _helpersController.getSessionUserID();

                var app = from d in _context.MyDesk.AsEnumerable()
                          join r in _context.Applications.AsEnumerable() on d.AppId equals r.AppId
                          join f in _context.Facilities.AsEnumerable() on r.FacilityId equals f.FacilityId
                          join ts in _context.AppTypeStage.AsEnumerable() on r.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join c in _context.Companies.AsEnumerable() on r.CompanyId equals c.CompanyId
                          join t in _context.Transactions.AsEnumerable() on r.AppId equals t.AppId into trans
                          from tr in trans.DefaultIfEmpty()
                          where ((d.StaffId == staff && d.HasWork == false && d.DeskId == deskID && d.ProcessId == processID) && (r.DeletedStatus == false) && (c.DeleteStatus == false))
                          select new ApplicationDetails
                          {
                              CompanyID = r.CompanyId,
                              FacilityID = f.FacilityId,
                              ElpsFacilityID = f.ElpsFacilityId,
                              CompanyElpsID = c.CompanyElpsId,
                              DeskId = d.DeskId,
                              RefNo = r.AppRefNo,
                              AppId = r.AppId,
                              ContractorName = r.Contractor,
                              RigName = r.RigName,
                              RigType = r.RigType,
                              CompanyName = c.CompanyName,
                              CompanyAddress = c.Address,
                              CompanyEmail = c.CompanyEmail,
                              TotalAmount = tr?.TotalAmt,
                              RRR = tr?.Rrr,
                              Volume = r?.Volume,
                              TransType = tr?.TransactionType,
                              AmountPaid = tr?.AmtPaid,
                              ServiceCharge = tr?.ServiceCharge,
                              TransDate = tr?.TransactionDate,
                              TransDescription = tr?.Description,
                              TransStatus = tr?.TransactionStatus,
                              Stage = ty.TypeName + " - " + s.StageName,
                              Status = r.Status,
                              ShortName = s.ShortName,
                              DateApplied = r.DateApplied,
                              DateSubmitted = r.DateSubmitted,
                              ReportApproved = r.IsReportApproved == true ? "YES" : "NO",
                              ProposalApproved = r.IsProposedApproved == true ? "YES" : "NO",
                          };

                if (app.Any())
                {

                    var appHistory = from h in _context.AppDeskHistory.AsEnumerable()
                                     orderby h.HistoryId descending
                                     where h.AppId == app.FirstOrDefault().AppId
                                     select new History
                                     {
                                         Status = h.Status,
                                         Comment = h.Comment,
                                         ActionFrom = h.ActionFrom,
                                         ActionTo = h.ActionTo,
                                         HistoryDate = h.CreatedAt.ToString()
                                     };

                    var appDocs = from sd in _context.SubmittedDocuments.AsEnumerable()
                                  join ad in _context.ApplicationDocuments.AsEnumerable() on sd.AppDocId equals ad.AppDocId
                                  where sd.AppId == app.FirstOrDefault().AppId && sd.DeleteStatus == false && ad.DeleteStatus == false && sd.CompElpsDocId != null
                                  select new AppDocuument
                                  {
                                      LocalDocID = sd.AppDocId,
                                      DocName = ad.DocName,
                                      EplsDocTypeID = ad.ElpsDocTypeId,
                                      CompanyDocID = (int)sd.CompElpsDocId,
                                      DocType = ad.DocType,
                                      DocSource = sd.DocSource,
                                      isAddictional = sd.IsAddictional,
                                  };


                    var appReport = from r in _context.Reports.AsEnumerable()
                                    join s in _context.Staff.AsEnumerable() on r.StaffId equals s.StaffId
                                    orderby r.ReportId descending
                                    where r.AppId == app.FirstOrDefault().AppId && r.DeletedStatus == false
                                    select new AppReport
                                    {
                                        ReportID = r.ReportId,
                                        Staff = s.StaffEmail,
                                        StaffID = r.StaffId,
                                        Comment = r.Comment,
                                        Subject = r.Subject,
                                        CreatedAt = r.CreatedAt.ToString(),
                                        UpdatedAt = r.UpdatedAt == null ? "" : r.UpdatedAt.ToString()
                                    };

                    var nominationRequest = from r in _context.NominationRequest.AsEnumerable()
                                            join s in _context.Staff.AsEnumerable() on r.StaffId equals s.StaffId
                                            join ro in _context.UserRoles.AsEnumerable() on s.RoleId equals ro.RoleId
                                            where r.AppId == app.FirstOrDefault().AppId
                                            select new NominationRequests
                                            {
                                                Staff = s.LastName + " " + s.FirstName + "(" + ro.RoleName + ")",
                                                hasDone = r.HasDone == false ? "NO" : "YES",
                                                CreatedAt = r.CreatedAt,
                                                ReminderedAt = r.ReminderDate,
                                                UpdatedAt = r.UpdatedAt,
                                                Comment = r.Comment
                                            };


                    var appSchdule = from s in _context.Schdules.AsEnumerable()
                                     join sby in _context.Staff.AsEnumerable() on s.SchduleBy equals sby.StaffId
                                     orderby s.SchduleId descending
                                     where s.AppId == app.FirstOrDefault().AppId && s.DeletedStatus == false
                                     select new AppSchdule
                                     {
                                         SchduleID = s.SchduleId,
                                         SchduleByID = s.SchduleBy,
                                         SchduleByEmail = sby.StaffEmail,
                                         SchduleType = s.SchduleType,
                                         SchduleLocation = s.SchduleLocation,
                                         SchduleDate = s.SchduleDate.ToString(),
                                         cResponse = s.CustomerAccept == 1 ? "Accepted" : s.CustomerAccept == 2 ? "Rejected" : "Awaiting Response",
                                         sResponse = s.SupervisorApprove == 1 ? "Accepted" : s.SupervisorApprove == 2 ? "Rejected" : "Awaiting Response",
                                         SchduleComment = s.Comment,
                                         CustomerComment = s.CustomerComment,
                                         SupervisorComment = s.SupervisorComment,
                                         CreatedAt = s.CreatedAt.ToString(),
                                         UpdatedAt = s.UpdatedAt == null ? "" : s.UpdatedAt.ToString()
                                     };

                    List<ApplicationProccess> appProcess = _context.ApplicationProccess.Where(x => x.ProccessId == processID && x.DeleteStatus == false).ToList();

                    var currentDesk = from a in _context.Applications.AsEnumerable()
                                      join s in _context.Staff.AsEnumerable() on a.CurrentDeskId equals s.StaffId
                                      join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                      where a.AppId == app.FirstOrDefault().AppId
                                      select new CurrentDesk
                                      {
                                          Staff = s.LastName + " " + s.FirstName + " (" + r.RoleName + " -- " + s.StaffEmail + ")"
                                      };

                    var staffList = from s in _context.Staff.AsEnumerable()
                                    join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                    join f in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals f.FieldOfficeId
                                    join zf in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zf.FieldOfficeId
                                    join z in _context.ZonalOffice.AsEnumerable() on zf.ZoneId equals z.ZoneId
                                    //where ((s.ActiveStatus == true && s.DeleteStatus == false) && (r.RoleName == GeneralClass.ZOPSCON || r.RoleName == GeneralClass.OPSCON))
                                    where s.ActiveStatus == true && s.DeleteStatus == false && (!r.RoleName.Equals(GeneralClass.SUPER_ADMIN) 
                                    && !r.RoleName.Equals(GeneralClass.SUPPORT) && !r.RoleName.Equals(GeneralClass.DIRECTOR) 
                                    && !r.RoleName.Equals(GeneralClass.ICT_ADMIN) && !r.RoleName.Equals(GeneralClass.ADMIN) 
                                    && !r.RoleName.Equals(GeneralClass.COMPANY))
                                    select new StaffNomination
                                    {
                                        FullName = s.LastName + " " + s.FirstName,
                                        ZonalOffice = z.ZoneName,
                                        StaffId = s.StaffId,
                                        StaffEmail = s.StaffEmail,
                                        FieldOffice = f.OfficeName,
                                        RoleName = r.RoleName
                                    };

                    var nomination = from n in _context.NominatedStaff.AsEnumerable()
                                     join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                     join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                     join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                                     where a.AppId == app.FirstOrDefault().AppId
                                     select new Nomination
                                     {
                                         NominationID = n.NominateId,
                                         StaffName = s.LastName + " " + s.FirstName,
                                         UserRoles = r.RoleName,
                                         StaffEmail = s.StaffEmail.Trim(),
                                         CreatedBy = n.CreatedBy,
                                         hasSubmitted = n.HasSubmitted,
                                         AppId = n.AppId
                                     };

                    var getDocuments = _context.ApplicationDocuments.Where(x => x.DocType == "Facility" && x.DocName.Trim().Contains(GeneralClass.staff_application_report_fac_doc) && x.DeleteStatus == false);

                    List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
                    List<MissingDocument> missingDocuments = new List<MissingDocument>();
                    List<BothDocuments> bothDocuments = new List<BothDocuments>();


                    if (getDocuments.Any())
                    {
                        ViewData["FacilityElpsID"] = app.FirstOrDefault().ElpsFacilityID;
                        ViewData["CompanyElpsID"] = app.FirstOrDefault().CompanyElpsID;

                        List<LpgLicense.Models.FacilityDocument> facilityDoc = generalClass.getFacilityDocuments(app.FirstOrDefault().ElpsFacilityID.ToString());

                        if (facilityDoc != null)
                        {

                            foreach (var fDoc in facilityDoc)
                            {
                                if (fDoc.Document_Type_Id == getDocuments.FirstOrDefault().ElpsDocTypeId)
                                {
                                    presentDocuments.Add(new PresentDocuments
                                    {
                                        Present = true,
                                        FileName = fDoc.Name,
                                        Source = fDoc.Source,
                                        CompElpsDocID = fDoc.Id,
                                        DocTypeID = fDoc.Document_Type_Id,
                                        LocalDocID = getDocuments.FirstOrDefault().AppDocId,
                                        DocType = getDocuments.FirstOrDefault().DocType,
                                        TypeName = getDocuments.FirstOrDefault().DocName

                                    });
                                }
                            }

                            var result = getDocuments.AsEnumerable().Where(x => !presentDocuments.AsEnumerable().Any(x2 => x2.LocalDocID == x.AppDocId));

                            foreach (var r in result)
                            {
                                missingDocuments.Add(new MissingDocument
                                {
                                    Present = false,
                                    DocTypeID = r.ElpsDocTypeId,
                                    LocalDocID = r.AppDocId,
                                    DocType = r.DocType,
                                    TypeName = r.DocName
                                });
                            }

                            bothDocuments.Add(new BothDocuments
                            {
                                missingDocuments = missingDocuments,
                                presentDocuments = presentDocuments,
                            });

                            _helpersController.LogMessages("Loading facility information and document for report upload : " + app.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                            _helpersController.LogMessages("Displaying/Viewing more application details.  Reference : " + app.FirstOrDefault().RefNo, _helpersController.getSessionEmail());
                        }

                        List<OtherDocuments> otherDocuments = new List<OtherDocuments>();

                        var allDoc = _context.ApplicationDocuments.AsEnumerable().Where(x => x.DeleteStatus == false && !x.DocName.Contains(GeneralClass.staff_application_report_fac_doc));

                        var othrDoc = allDoc.AsEnumerable().Where(x => !appDocs.Any(s => s.LocalDocID == x.AppDocId)).ToList();

                        foreach (var r in othrDoc)
                        {
                            otherDocuments.Add(new OtherDocuments
                            {
                                LocalDocID = r.AppDocId,
                                DocName = r.DocName
                            });
                        }

                        applicationDetailsModels.Add(new ApplicationDetailsModel
                        {
                            currentDesks = currentDesk.ToList(),
                            applicationProccesses = appProcess.ToList(),
                            appSchdules = appSchdule.ToList(),
                            appReports = appReport.ToList(),
                            histories = appHistory.Take(3).ToList(),
                            appDocuuments = appDocs.ToList(),
                            staffs = staffList.ToList(),
                            nominations = nomination.ToList(),
                            applications = app.ToList(),
                            bothDocuments = bothDocuments.ToList(),
                            otherDocuments = otherDocuments.ToList(),
                            NominationRequest = nominationRequest.ToList()
                        });

                        ViewData["AppRefNo"] = app.FirstOrDefault().RefNo;

                        return View(applicationDetailsModels);
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong trying get staff report for application. Kindly contact support.") });
                    }

                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
                }
            }
        }

        /*
       * Viewing application details without operation control
       * 
       * 
       * id => encrypted desk id
       * option => encrypted process id
       */
        public IActionResult Apps(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }

            var appid = generalClass.DecryptIDs(id);

            if (appid == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                List<ApplicationDetailsModel> applicationDetailsModels = new List<ApplicationDetailsModel>();

                var staff = _helpersController.getSessionUserID();

                var app =
                          from r in _context.Applications.AsEnumerable()
                          join ts in _context.AppTypeStage.AsEnumerable() on r.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join c in _context.Companies.AsEnumerable() on r.CompanyId equals c.CompanyId
                          join f in _context.Facilities.AsEnumerable() on r.FacilityId equals f.FacilityId
                          join t in _context.Transactions.AsEnumerable() on r.AppId equals t.AppId into trans
                          from tr in trans.DefaultIfEmpty()
                          where ((r.AppId == appid))
                          select new ApplicationDetails
                          {
                              CompanyID = r.CompanyId,
                              FacilityID = f.FacilityId,
                              ElpsFacilityID = f.ElpsFacilityId,
                              CompanyElpsID = c.CompanyElpsId,
                              RefNo = r.AppRefNo,
                              AppId = r.AppId,
                              ContractorName = r.Contractor,
                              RigName = r.RigName,
                              RigType = r.RigType,
                              CompanyName = c.CompanyName,
                              CompanyAddress = c.Address,
                              CompanyEmail = c.CompanyEmail,
                              TotalAmount = tr?.TotalAmt,
                              RRR = tr?.Rrr,
                              TransType = tr?.TransactionType,
                              AmountPaid = tr?.AmtPaid,
                              ServiceCharge = tr?.ServiceCharge,
                              TransDate = tr?.TransactionDate,
                              TransDescription = tr?.Description,
                              TransStatus = tr?.TransactionStatus,
                              Stage = ty.TypeName + " - "+ s.StageName,
                              Volume = r?.Volume,
                              ShortName = s.ShortName,
                              Status = r.Status,
                              DateApplied = r.DateApplied,
                              DateSubmitted = r.DateSubmitted,
                              ReportApproved = r.IsReportApproved == true ? "YES" : "NO",
                              ProposalApproved = r.IsProposedApproved == true ? "YES" : "NO",
                          };

                if (app.Any())
                {

                    var appHistory = from h in _context.AppDeskHistory
                                     orderby h.HistoryId descending
                                     where h.AppId == app.FirstOrDefault().AppId
                                     select new History
                                     {
                                         Status = h.Status,
                                         Comment = h.Comment,
                                         ActionFrom = h.ActionFrom,
                                         ActionTo = h.ActionTo,
                                         HistoryDate = h.CreatedAt.ToString()
                                     };


                    var appDocs = from sd in _context.SubmittedDocuments.AsEnumerable()
                                  join ad in _context.ApplicationDocuments.AsEnumerable() on sd.AppDocId equals ad.AppDocId
                                  where sd.AppId == app.FirstOrDefault().AppId && sd.DeleteStatus == false && ad.DeleteStatus == false && sd.CompElpsDocId != null
                                  select new AppDocuument
                                  {
                                      LocalDocID = sd.AppDocId,
                                      DocName = ad.DocName,
                                      EplsDocTypeID = ad.ElpsDocTypeId,
                                      CompanyDocID = (int)sd.CompElpsDocId,
                                      DocType = ad.DocType,
                                      DocSource = sd.DocSource,
                                  };


                    var nominationRequest = from r in _context.NominationRequest.AsEnumerable()
                                            join s in _context.Staff.AsEnumerable() on r.StaffId equals s.StaffId
                                            join ro in _context.UserRoles.AsEnumerable() on s.RoleId equals ro.RoleId
                                            where r.AppId == app.FirstOrDefault().AppId
                                            select new NominationRequests
                                            {
                                                Staff = s.LastName + " " + s.FirstName + "(" + ro.RoleName + ")",
                                                hasDone = r.HasDone == false ? "NO" : "YES",
                                                CreatedAt = r.CreatedAt,
                                                ReminderedAt = r.ReminderDate,
                                                UpdatedAt = r.UpdatedAt,
                                                Comment = r.Comment
                                            };



                    var appReport = from r in _context.Reports.AsEnumerable()
                                    join s in _context.Staff.AsEnumerable() on r.StaffId equals s.StaffId
                                    orderby r.ReportId descending
                                    where r.AppId == app.FirstOrDefault().AppId && r.DeletedStatus == false
                                    select new AppReport
                                    {
                                        ReportID = r.ReportId,
                                        Staff = s.StaffEmail,
                                        StaffID = r.StaffId,
                                        Comment = r.Comment,
                                        Subject = r.Subject,
                                        CreatedAt = r.CreatedAt.ToString(),
                                        UpdatedAt = r.UpdatedAt == null ? "" : r.UpdatedAt.ToString()
                                    };


                    var appSchdule = from s in _context.Schdules.AsEnumerable()
                                     join sby in _context.Staff.AsEnumerable() on s.SchduleBy equals sby.StaffId
                                     orderby s.SchduleId descending
                                     where s.AppId == app.FirstOrDefault().AppId && s.DeletedStatus == false
                                     select new AppSchdule
                                     {
                                         SchduleID = s.SchduleId,
                                         SchduleByID = s.SchduleBy,
                                         SchduleByEmail = sby.StaffEmail,
                                         SchduleType = s.SchduleType,
                                         SchduleLocation = s.SchduleLocation,
                                         SchduleDate = s.SchduleDate.ToString(),
                                         cResponse = s.CustomerAccept == 1 ? "Accepted" : s.CustomerAccept == 2 ? "Rejected" : "Awaiting Response",
                                         sResponse = s.SupervisorApprove == 1 ? "Accepted" : s.SupervisorApprove == 2 ? "Rejected" : "Awaiting Response",
                                         SchduleComment = s.Comment,
                                         CustomerComment = s.CustomerComment,
                                         SupervisorComment = s.SupervisorComment,
                                         CreatedAt = s.CreatedAt.ToString(),
                                         UpdatedAt = s.UpdatedAt == null ? "" : s.UpdatedAt.ToString()
                                     };

                    var currentDesk = from a in _context.Applications.AsEnumerable()
                                      join s in _context.Staff.AsEnumerable() on a.CurrentDeskId equals s.StaffId
                                      join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                      where a.AppId == app.FirstOrDefault().AppId
                                      select new CurrentDesk
                                      {
                                          Staff = s.LastName + " " + s.FirstName + " (" + r.RoleName + " -- " + s.StaffEmail + ")"
                                      };

                    var nomination = from n in _context.NominatedStaff.AsEnumerable()
                                     join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                     join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                     join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                                     where a.AppId == app.FirstOrDefault().AppId
                                     select new Nomination
                                     {
                                         NominationID = n.NominateId,
                                         StaffName = s.LastName + " " + s.FirstName,
                                         UserRoles = r.RoleName,
                                         StaffEmail = s.StaffEmail.Trim(),
                                         CreatedBy = n.CreatedBy,
                                         hasSubmitted = n.HasSubmitted,
                                         AppId = n.AppId
                                     };

                    applicationDetailsModels.Add(new ApplicationDetailsModel
                    {
                        currentDesks = currentDesk.ToList(),
                        appSchdules = appSchdule.ToList(),
                        appReports = appReport.ToList(),
                        histories = appHistory.Take(3).ToList(),
                        appDocuuments = appDocs.ToList(),
                        nominations = nomination.ToList(),
                        applications = app.ToList(),
                        NominationRequest = nominationRequest.ToList()
                    });

                    ViewData["AppRefNo"] = app.FirstOrDefault().RefNo;

                    return View(applicationDetailsModels);

                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
                }
            }
        }

        public IActionResult ViewTemplate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application template referenc not found or not in correct format. Kindly contact support.") });
            }

            var appid = generalClass.DecryptIDs(id);

            if (appid == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application template referenc not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var temp = _context.TemplateTable.Where(x => x.AppId == appid);

                if(temp.Count() <= 0)
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application template not found. Kindly contact support.") });
                }
                else
                {
                    var apps = _context.Applications.Where(x => x.AppId == appid);

                    ViewData["RefNo"] = apps.FirstOrDefault().AppRefNo;
                    return View(temp.ToList());
                }
            }
        }

        /*
        * Viewing an application report created by a staff
        * 
        * id => encrypted report ID
        * 
        */
        public IActionResult ViewReport(string id)
        {
            var report_id = generalClass.DecryptIDs(id.Trim());

            if (report_id == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, No link was found for this application history.") });
            }
            else
            {
                var Request = from re in _context.Reports.AsEnumerable()
                              join r in _context.Applications.AsEnumerable() on re.AppId equals r.AppId
                              join st in _context.Staff.AsEnumerable() on re.StaffId equals st.StaffId
                              join f in _context.Facilities.AsEnumerable() on r.FacilityId equals f.FacilityId
                              join ts in _context.AppTypeStage.AsEnumerable() on r.AppTypeStageId equals ts.TypeStageId
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              join c in _context.Companies.AsEnumerable() on r.CompanyId equals c.CompanyId
                              join t in _context.Transactions.AsEnumerable() on r.AppId equals t.AppId into trans
                              from tr in trans.DefaultIfEmpty()
                              where ((re.ReportId == report_id && re.DeletedStatus == false))
                              select new ReportViewModel
                              {
                                  RefNo = r.AppRefNo,
                                  CompanyName = c.CompanyName,
                                  CompanyAddress = c.Address,
                                  CompanyEmail = c.CompanyEmail,
                                  Stage = ty.TypeName + " - "+s.StageName,
                                  Status = r.Status,
                                  DateApplied = r.DateApplied,
                                  DateSubmitted = r.DateSubmitted,
                                  Staff = st.LastName + " " + st.FirstName + " (" + st.StaffEmail + ")",
                                  ReportDate = re.CreatedAt,
                                  UpdatedAt = re.UpdatedAt,
                                  Subject = re.Subject,
                                  Comment = re.Comment,
                                  DocSource = re.DocSource,
                                  WellName = f.FacilityName,
                              };

                ViewData["AppRef"] = Request.FirstOrDefault()?.RefNo;

                _helpersController.LogMessages("Displaying application report for Application reference : " + Request.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                return View(Request.ToList());
            }
        }

        /*
         * Saving application report
         * 
         * AppID => encrypted application id
         * txtReport => the comment for the report
         */
        public JsonResult SaveReport(string AppID, string txtReport, string txtReportTitle, List<SubmitDoc> SubmittedDocuments)
        {
            string result = "";

            int appID = 0;

            var appId = generalClass.Decrypt(AppID.ToString().Trim());

            if (appId == "Error")
            {
                result = "Application link error";
            }
            else
            {
                appID = Convert.ToInt32(appId);

                Models.DB.Reports reports = new Models.DB.Reports()
                {
                    AppId = appID,
                    StaffId = _helpersController.getSessionUserID(),
                    Subject = txtReportTitle.ToUpper(),
                    ElpsDocId = SubmittedDocuments.FirstOrDefault()?.CompElpsDocID,
                    DocSource = SubmittedDocuments.FirstOrDefault()?.DocSource,
                    AppDocId = SubmittedDocuments.FirstOrDefault()?.LocalDocID,
                    Comment = txtReport,
                    CreatedAt = DateTime.Now,
                    DeletedStatus = false
                };

                _context.Reports.Add(reports);

                if (_context.SaveChanges() > 0)
                {
                    var apps = _context.Applications.Where(x => x.AppId == appID);

                    result = "Report Saved";
                    _helpersController.LogMessages("Saving report for application : " + apps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                    var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                    var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                    _helpersController.SaveHistory(apps.FirstOrDefault().AppId, actionFrom, actionTo, "Report", "A report has been added to this application.");

                }
                else
                {
                    result = "Something went wrong trying to save your report";
                }
            }
            _helpersController.LogMessages("Operation result from saving report : " + result, _helpersController.getSessionEmail());
            return Json(result);
        }

        /*
         * Getting application report for editing
         * 
         * ReportID => encrypted report id
         */
        public JsonResult GetReport(string ReportID)
        {
            string result = "";

            int rID = 0;

            var rid = generalClass.Decrypt(ReportID.Trim());

            if (rid == "Error")
            {
                result = "0|Application report link error";
            }
            else
            {
                rID = Convert.ToInt32(rid);

                var get = _context.Reports.Where(x => x.ReportId == rID);

                if (get.Any())
                {
                    result = "1|" + get.FirstOrDefault().Comment + "|" + get.FirstOrDefault().Subject;
                }
                else
                {
                    result = "0|Error... cannot find this application report."
    ;
                }
            }

            _helpersController.LogMessages("Displaying report. Report ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Editing application report
         * 
         * ReportID => encrypted report id
         * txtReport => comment for the report
         */
        public JsonResult EditReport(string ReportID, string txtReport, string txtReportTitle, List<SubmitDoc> SubmittedDocuments)
        {
            string result = "";

            int rID = 0;

            var reportId = generalClass.Decrypt(ReportID.ToString().Trim());

            if (reportId == "Error")
            {
                result = "Application link error";
            }
            else
            {
                rID = Convert.ToInt32(reportId);

                var get = _context.Reports.Where(x => x.ReportId == rID);

                if (get.Any())
                {
                    int appid = get.FirstOrDefault().AppId;

                    get.FirstOrDefault().Comment = txtReport;
                    get.FirstOrDefault().UpdatedAt = DateTime.Now;
                    get.FirstOrDefault().Subject = txtReportTitle.ToUpper();

                    if (SubmittedDocuments.FirstOrDefault()?.CompElpsDocID != 0)
                    {
                        get.FirstOrDefault().ElpsDocId = SubmittedDocuments.FirstOrDefault()?.CompElpsDocID;
                    }

                    if (SubmittedDocuments.FirstOrDefault()?.LocalDocID != 0)
                    {
                        get.FirstOrDefault().AppDocId = SubmittedDocuments.FirstOrDefault()?.LocalDocID;
                    }

                    if (SubmittedDocuments.FirstOrDefault()?.DocSource != "NILL")
                    {
                        get.FirstOrDefault().DocSource = SubmittedDocuments.FirstOrDefault()?.DocSource;
                    }


                    if (_context.SaveChanges() > 0)
                    {
                        result = "Report Edited";
                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                        _helpersController.SaveHistory(appid, actionFrom, actionTo, "Edit Report", "A report has been updated to this application.");
                    }
                    else
                    {
                        result = "Something went wrong trying to save your report";
                    }
                }
                else
                {
                    result = "Something went wrong trying to find this report.";
                }
            }

            _helpersController.LogMessages("Report update status :" + result + " Report ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
       * Deleting application report
       * 
       * ReportID => encryted report id
       */
        public JsonResult DeleteReport(string ReportID)
        {
            string result = "";

            int rID = 0;

            var reportId = generalClass.Decrypt(ReportID.ToString().Trim());

            if (reportId == "Error")
            {
                result = "Application link error";
            }
            else
            {
                rID = Convert.ToInt32(reportId);
            }

            var get = _context.Reports.Where(x => x.ReportId == rID);

            if (get.Any())
            {
                int appid = get.FirstOrDefault().AppId;

                get.FirstOrDefault().DeletedBy = _helpersController.getSessionUserID();
                get.FirstOrDefault().DeletedStatus = true;
                get.FirstOrDefault().DeletedAt = DateTime.Now;

                if (_context.SaveChanges() > 0)
                {
                    result = "Report Deleted";

                    var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                    var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                    _helpersController.SaveHistory(appid, actionFrom, actionTo, "Deleted Report", "A report has been deleted on this application.");

                }
                else
                {
                    result = "Something went wrong trying to delete your report";
                }
            }
            else
            {
                result = "Something went wrong trying to find this report.";
            }

            _helpersController.LogMessages("Report delete status :" + result + " Report ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Creating schdule for application
         */
        public JsonResult CreateSchdule(string AppID, string DeskID, string SchduleDate, string SchduleComment, string SchduleLocation, string SchduleType)
        {
            string result = "";

            int aID = 0;
            int dID = 0;

            var date = DateTime.Parse(SchduleDate.Trim());

            var appID = generalClass.Decrypt(AppID.Trim());
            var deskID = generalClass.Decrypt(DeskID.Trim());

            if (appID == "Error" || deskID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                aID = Convert.ToInt32(appID);
                dID = Convert.ToInt32(deskID);

                var apps = _context.Applications.Where(x => x.AppId == aID && x.DeletedStatus == false);

                if (apps.Any())
                {

                    var stage = from ts in _context.AppTypeStage.AsEnumerable()
                                join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                join t in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals t.AppTypeId
                                where ts.TypeStageId == apps.FirstOrDefault().AppTypeStageId
                                select new
                                {
                                    s.StageName,
                                    AppStageId = s.AppStageId,
                                    Category = t.TypeName + " Application (" + s.StageName + ")"
                                };

                    var app = from a in _context.Applications
                              join f in _context.Facilities on a.FacilityId equals f.FacilityId
                              join c in _context.Companies on a.CompanyId equals c.CompanyId
                              where a.AppId == aID
                              select new
                              {
                                  CompanyID = c.CompanyId,
                                  AppRefNo = a.AppRefNo,
                                  CompanyEmail = c.CompanyEmail,
                                  CompanyName = c.CompanyName
                              };

                    var myDesk = _context.MyDesk.Where(x => x.DeskId == dID);

                    var schdule = _context.Schdules.Where(x => x.AppId == aID && x.SchduleType.Trim() == SchduleType.Trim() && x.DeletedStatus == false);

                    int AppDropStaffID = _helpersController.ApplicationDropStaff(stage.FirstOrDefault().AppStageId, 2);


                    if (AppDropStaffID > 0)
                    {
                        var staff = _context.Staff.Where(x => x.StaffId == AppDropStaffID).FirstOrDefault();

                        var all_staff = from s in _context.Staff
                                        join r in _context.UserRoles on s.RoleId equals r.RoleId
                                        join l in _context.Location on s.LocationId equals l.LocationId
                                        where r.RoleName == GeneralClass.TEAM && l.LocationName == "HQ"
                                        select new
                                        {
                                            Fullname = s.LastName + " " + s.FirstName,
                                            Email = s.StaffEmail
                                        };

                        if (schdule.Any())
                        {
                            var startDate = DateTime.Parse(schdule.FirstOrDefault().CreatedAt.ToString());
                            var expDate = startDate.AddDays(3);

                            if (DateTime.Now < expDate) // schdule has not expired
                            {
                                result = "A " + schdule.FirstOrDefault().SchduleType + " has already been created for " + schdule.FirstOrDefault().SchduleDate.ToString();
                            }
                            else
                            {
                                if (_helpersController.getSessionRoleName() == GeneralClass.TEAM)
                                {
                                    /*
                                     * Send schdule to supervisor for approval
                                     */
                                    Schdules schdules = new Schdules()
                                    {
                                        AppId = aID,
                                        SchduleBy = _helpersController.getSessionUserID(),
                                        SchduleType = SchduleType,
                                        SchduleLocation = SchduleLocation,
                                        SchduleDate = date,
                                        Supervisor = AppDropStaffID,
                                        SupervisorApprove = 0,
                                        Comment = SchduleComment,
                                        CreatedAt = DateTime.Now,
                                        DeletedStatus = false,
                                        IsDone = false
                                    };
                                    _context.Schdules.Add(schdules);

                                    if (_context.SaveChanges() > 0)
                                    {
                                        var getApps = _context.Applications.Where(x => x.AppId == aID);

                                        string subj = SchduleType + " Schedule for application (" + getApps.FirstOrDefault().AppRefNo + ") Createded and Awaiting your response.";
                                        string cont = SchduleType + " Schedule for application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been created. Your response is required.";

                                       foreach(var s in all_staff.ToList())
                                        {
                                            var msgs = _helpersController.SendEmailMessageAsync(s.Email, s.Fullname, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                                        }

                                        var msg = _helpersController.SendEmailMessageAsync(staff.StaffEmail, staff.LastName + " " + staff.FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                                        result = "Schdule Created";

                                        _helpersController.LogMessages("Schedule created successfully with application reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                        var actionTo = _helpersController.getActionHistory(staff.RoleId, staff.StaffId);

                                        _helpersController.SaveHistory(aID, actionFrom, actionTo, "Schedule", "An Application " + SchduleType + " schedule was created ");

                                    }
                                    else
                                    {
                                        result = "Something went wrong trying to create your schedule.";
                                    }
                                }
                                else
                                {
                                    Schdules schdules = new Schdules()
                                    {
                                        AppId = aID,
                                        SchduleBy = _helpersController.getSessionUserID(), // session staff id
                                        SchduleType = SchduleType,
                                        SchduleLocation = SchduleLocation,
                                        SchduleDate = date,
                                        Supervisor = _helpersController.getSessionUserID(),
                                        SupervisorApprove = 1,
                                        Comment = SchduleComment,
                                        SupervisorComment = SchduleComment,
                                        CreatedAt = DateTime.Now,
                                        DeletedStatus = false,
                                        IsDone = false

                                    };

                                    _context.Schdules.Add(schdules);

                                    if (_context.SaveChanges() > 0)
                                    {
                                        result = "Schdule Created";

                                        _helpersController.LogMessages("Schedule created successfully with application reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                        var user = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyID).FirstOrDefault();

                                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                        var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);

                                        // change to staff session id
                                        _helpersController.SaveHistory(aID, actionFrom, actionTo, "Schedule", "An Application " + SchduleType + " schedule was created ");


                                        // Saving Messages
                                        string subject = stage.FirstOrDefault().StageName + " Application Schedule with Ref : " + app.FirstOrDefault().AppRefNo;
                                        string content = "You have been schedule for a " + schdules.SchduleType + " on " + schdules.SchduleDate.ToString() + " with comment -: " + schdules.Comment + " Kindly find other details below.";
                                        var emailMsg = _helpersController.SaveMessage(aID, app.FirstOrDefault().CompanyID, subject, content);
                                        var sendEmail = _helpersController.SendEmailMessageAsync(app.FirstOrDefault().CompanyEmail, app.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);


                                        string subj = SchduleType + " Schedule for application (" + app.FirstOrDefault().AppRefNo + ") Createded and Awaiting your response.";
                                        string cont = SchduleType + " Schedule for application with reference number " + app.FirstOrDefault().AppRefNo + " has been created. Your response is required.";

                                        foreach (var s in all_staff.ToList())
                                        {
                                            var msgs = _helpersController.SendEmailMessageAsync(s.Email, s.Fullname, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                                        }
                                    }
                                    else
                                    {
                                        result = "Something went wrong trying to create your schedule.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (GeneralClass.TEAM == _helpersController.getSessionRoleName())
                            {
                                Schdules schdules = new Schdules()
                                {
                                    AppId = aID,
                                    SchduleBy = _helpersController.getSessionUserID(),
                                    SchduleType = SchduleType,
                                    SchduleLocation = SchduleLocation,
                                    SchduleDate = date,
                                    Supervisor = AppDropStaffID,
                                    SupervisorApprove = 0,
                                    Comment = SchduleComment,
                                    CreatedAt = DateTime.Now,
                                    DeletedStatus = false,
                                    IsDone = false
                                };

                                _context.Schdules.Add(schdules);

                                if (_context.SaveChanges() > 0)
                                {
                                    _helpersController.LogMessages("Schedule created successfully with application reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                    var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                    var actionTo = _helpersController.getActionHistory(staff.RoleId, staff.StaffId);
                                    
                                    // change to staff session id
                                    _helpersController.SaveHistory(aID, actionFrom, actionTo, "Schedule", "An Application " + SchduleType + " schedule was created ");

                                    var getApps = _context.Applications.Where(x => x.AppId == aID);

                                    string subj = SchduleType + " Schedule for application (" + getApps.FirstOrDefault().AppRefNo + ") Createded and Awaiting your response.";
                                    string cont = SchduleType + " schedule for application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been created. Your response is required.";

                                    var msg = _helpersController.SendEmailMessageAsync(staff.StaffEmail, staff.LastName + " " + staff.FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                                    foreach (var s in all_staff.ToList())
                                    {
                                        var msgs = _helpersController.SendEmailMessageAsync(s.Email, s.Fullname, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                                    }

                                    result = "Schdule Created";
                                }
                                else
                                {
                                    result = "Something went wrong trying to create your schedule.";
                                }
                            }
                            else
                            {
                                Schdules schdules = new Schdules()
                                {
                                    AppId = aID,
                                    SchduleBy = _helpersController.getSessionUserID(),
                                    SchduleType = SchduleType,
                                    SchduleLocation = SchduleLocation,
                                    SchduleDate = date,
                                    Supervisor = _helpersController.getSessionUserID(),
                                    SupervisorApprove = 1,
                                    Comment = SchduleComment,
                                    SupervisorComment = SchduleComment,
                                    CreatedAt = DateTime.Now,
                                    DeletedStatus = false,
                                    IsDone = false
                                };

                                _context.Schdules.Add(schdules);

                                if (_context.SaveChanges() > 0)
                                {
                                    _helpersController.LogMessages("Schedule created successfully with application reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                    var user = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyID).FirstOrDefault();

                                    var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                    var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);

                                    // change to staff session id
                                    _helpersController.SaveHistory(aID, actionFrom, actionTo, "Schedule", "An Application " + SchduleType + " schedule was created ");


                                    // Saving Messages
                                    string subject = stage.FirstOrDefault().StageName + " Application Schedule with Ref : " + app.FirstOrDefault().AppRefNo;
                                    string content = "You have been schedule for a " + schdules.SchduleType + " on " + schdules.SchduleDate.ToString() + " with comment -: " + schdules.Comment + " Kindly find other details below.";
                                    var emailMsg = _helpersController.SaveMessage(aID, app.FirstOrDefault().CompanyID, subject, content);
                                    var sendEmail = _helpersController.SendEmailMessageAsync(app.FirstOrDefault().CompanyEmail, app.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                                    string subj = SchduleType + " Schedule for application (" + app.FirstOrDefault().AppRefNo + ") Createded and Awaiting your response.";
                                    string cont = SchduleType + " Schedule for application with reference number " + app.FirstOrDefault().AppRefNo + " has been created. Your response is required.";

                                    foreach (var s in all_staff.ToList())
                                    {
                                        var msgs = _helpersController.SendEmailMessageAsync(s.Email, s.Fullname, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                                    }

                                    result = "Schdule Created";
                                }
                                else
                                {
                                    result = "Something went wrong trying to create your schedule.";
                                }
                            }
                        }
                    }
                    else
                    {
                        result = "Supervisor not found to approve your inspection schedule. Please try again later or contact support";
                    }
                }
                else
                {
                    result = "Application not found, please try again later.";
                }
            }

            _helpersController.LogMessages("Schdule status :" + result, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Get schdule for editing 
         */
        public JsonResult GetSchdule(string schduleID)
        {
            string result = "";

            int rID = 0;

            var rid = generalClass.Decrypt(schduleID.Trim());

            if (rid == "Error")
            {
                result = "0|Application report link error";
            }
            else
            {
                rID = Convert.ToInt32(rid);

                var get = _context.Schdules.Where(x => x.SchduleId == rID);

                if (get.Any())
                {
                    result = "1|" + get.FirstOrDefault().Comment + "|" + get.FirstOrDefault().SchduleDate.ToString();
                }
                else
                {
                    result = "0|Error... cannot find this application schedule.";
                }
            }

            _helpersController.LogMessages("Displaying schedule : " + result + ". Schedule ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Editing a schdule
         */
        public JsonResult EditSchdule(string schduleID, string txtComment, string txtSchduleDate, string txtSchduleLoaction, string txtSchduleType)
        {
            string result = "";

            int rID = 0;

            var schid = generalClass.Decrypt(schduleID.ToString().Trim());

            if (schid == "Error")
            {
                result = "Application link error";
            }
            else
            {
                rID = Convert.ToInt32(schid);

                var get = _context.Schdules.Where(x => x.SchduleId == rID);

                if (get.Any())
                {
                    var app = _context.Applications.Where(x => x.AppId == get.FirstOrDefault().AppId);

                    if (app.Any())
                    {
                        var stage = from ts in _context.AppTypeStage.AsEnumerable()
                                    join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                    join t in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals t.AppTypeId
                                    where ts.TypeStageId == app.FirstOrDefault().AppTypeStageId
                                    select new
                                    {
                                        s.StageName,
                                        AppStageId = s.AppStageId,
                                        Category = t.TypeName + " Application (" + s.StageName + ")"
                                    };

                        var apps = from a in _context.Applications
                                   join f in _context.Facilities on a.FacilityId equals f.FacilityId
                                   join c in _context.Companies on a.CompanyId equals c.CompanyId
                                   where a.AppId == get.FirstOrDefault().AppId
                                   select new
                                   {
                                       CompanyID = c.CompanyId,
                                       AppRefNo = a.AppRefNo,
                                       CompanyEmail = c.CompanyEmail,
                                       CompanyName = c.CompanyName
                                   };

                        if (get.Any())
                        {
                            get.FirstOrDefault().Comment = txtComment;
                            get.FirstOrDefault().SchduleLocation = txtSchduleLoaction;
                            get.FirstOrDefault().SchduleType = txtSchduleType;
                            get.FirstOrDefault().SchduleDate = DateTime.Parse(txtSchduleDate.Trim());
                            get.FirstOrDefault().UpdatedAt = DateTime.Now;

                            if (_context.SaveChanges() > 0)
                            {
                                result = "Schdule Edited";

                                var user = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId).FirstOrDefault();

                                var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);


                                _helpersController.SaveHistory(get.FirstOrDefault().AppId, actionFrom, actionTo, "Schedule", "Application " + txtSchduleType + " schedule was edited ");
                            }
                            else
                            {
                                result = "Something went wrong trying to save your schedule";
                            }
                        }
                        else
                        {
                            result = "Something went wrong trying to find this schedule.";
                        }
                    }
                    else
                    {
                        result = "Application not found, please try again later.";
                    }
                }
                else
                {
                    result = "This schedule was not found, please try again later.";
                }
            }

            _helpersController.LogMessages("Schedule Status : " + result + ". Schedule ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Delete Application schdule
         */
        public JsonResult DeleteSchdule(string schduleID)
        {
            string result = "";

            int rID = 0;

            var reportId = generalClass.Decrypt(schduleID.ToString().Trim());

            if (reportId == "Error")
            {
                result = "Application link error";
            }
            else
            {
                rID = Convert.ToInt32(reportId);

                var get = _context.Schdules.Where(x => x.SchduleId == rID);

                if (get.Any())
                {
                    get.FirstOrDefault().DeletedBy = Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionUserID")));
                    get.FirstOrDefault().DeletedStatus = true;
                    get.FirstOrDefault().DeletedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Schdule Deleted";


                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                        _helpersController.SaveHistory(get.FirstOrDefault().AppId, actionFrom, actionTo, "Schedule", "An Application " + get.FirstOrDefault().SchduleType + " schdule was removed (ID) => " + get.FirstOrDefault().SchduleId);

                    }
                    else
                    {
                        result = "Something went wrong trying to delete your schedule";
                    }
                }
                else
                {
                    result = "Something went wrong trying to find this schedule.";
                }
            }

            _helpersController.LogMessages("Schedule Status : " + result + ". Schedule ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Geetting the histories of a single application
         * 
         * id => encrypted app id
         */
        public IActionResult ApplicationHistory(string id)
        {
            int AppID = 0;

            var app_id = generalClass.Decrypt(id.Trim());

            if (app_id == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, No link was found for this application history.") });
            }
            else
            {
                AppID = Convert.ToInt32(app_id);

                var apps = from a in _context.Applications.AsEnumerable()
                           join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId into Facility
                           join c in _context.Companies.AsEnumerable().AsEnumerable() on a.CompanyId equals c.CompanyId into Company
                           join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                           join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                           join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                           where ((a.AppId == AppID))
                           select new ApplicationDetails
                           {
                               AppId = a.AppId,
                               RefNo = a.AppRefNo,
                               Status = a.Status,
                               DateApplied = a.DateApplied,
                               DateSubmitted = a.DateSubmitted,
                               CompanyName = Company.FirstOrDefault().CompanyName,
                               CompanyID = Company.FirstOrDefault().CompanyId,
                               WellName = Facility.FirstOrDefault().FacilityName,
                               Stage = ty.TypeName + " - " + s.StageName
                           };

                var appHistory = from h in _context.AppDeskHistory
                                 orderby h.HistoryId descending
                                 where h.AppId == AppID
                                 select new History
                                 {
                                     Status = h.Status,
                                     Comment = h.Comment,
                                     ActionFrom = h.ActionFrom,
                                     ActionTo = h.ActionTo,
                                     HistoryDate = h.CreatedAt.ToString()
                                 };


                List<HistoryInformation> historyInformation = new List<HistoryInformation>();

                historyInformation.Add(new HistoryInformation
                {
                    applicationDetails = apps.ToList(),
                    histories = appHistory.ToList(),
                });

                _helpersController.LogMessages("Displaying application histories. Application reference : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                return View(historyInformation);
            }
        }

        /*
        * A application rejection process/method
        * 
        * DeskID => encrypted application desk ID
        * txtComment => rejection comment
        * RequiredDocs => a list of document id for rejection
        * 
        */
        public JsonResult Rejection(string DeskID, string txtComment, List<int> RequiredDocs)
        {
            string result = "";

            var deskID = generalClass.DecryptIDs(DeskID);
            int previousSort = 0;

            var refNo = "";

            if (deskID == 0)
            {
                result = "Something went wrong, Desk reference not in correct format.";
            }
            else
            {
                var getDesk = _context.MyDesk.Where(x => x.DeskId == deskID);

                var getApps = _context.Applications.Where(x => x.AppId == getDesk.FirstOrDefault().AppId && x.DeletedStatus == false);

                // finding rejection role of a staff
                var getProcess = from ap in _context.ApplicationProccess.AsEnumerable()
                                 join r in _context.UserRoles.AsEnumerable() on ap.OnRejectRoleId equals r.RoleId
                                 where ap.ProccessId == getDesk.FirstOrDefault().ProcessId && ap.DeleteStatus == false && r.DeleteStatus == false
                                 select new
                                 {
                                     ap,
                                     r
                                 };

                 // getting application type and stage 
                var stage = from ts in _context.AppTypeStage.AsEnumerable()
                            join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                            join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                            where ts.TypeStageId == getApps.FirstOrDefault().AppTypeStageId
                            select new
                            {
                                StageName = ty.TypeName + " - "+ s.StageName
                            };

                var company = _context.Companies.Where(x => x.CompanyId == getApps.FirstOrDefault().CompanyId);


                if (getDesk.Any() && getApps.Any() && getProcess.Any() && stage.Any() && company.Any())
                {

                    refNo = getApps.FirstOrDefault().AppRefNo;

                    // Planning or reviewer staff rejecting with no documents to attached back to customer
                    if (getProcess.FirstOrDefault().r.RoleName == GeneralClass.COMPANY && RequiredDocs.Count() <= 0)
                    {
                        getApps.FirstOrDefault().DeskId = getDesk.FirstOrDefault().DeskId;
                        getApps.FirstOrDefault().CurrentDeskId = getDesk.FirstOrDefault().StaffId;
                        getApps.FirstOrDefault().UpdatedAt = DateTime.Now;
                        getApps.FirstOrDefault().Status = GeneralClass.Rejected;

                        getDesk.FirstOrDefault().HasWork = true;
                        getDesk.FirstOrDefault().UpdatedAt = DateTime.Now;
                        getDesk.FirstOrDefault().Comment = txtComment;

                        if (_context.SaveChanges() > 0)
                        {
                            result = "External Rejection";

                            _helpersController.LogMessages("Staff rejected customer's application. Application reference : " + getApps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());
                            
                            var user = _context.Companies.Where(x => x.CompanyId == getApps.FirstOrDefault().CompanyId).FirstOrDefault();

                            var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                            var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);

                            _helpersController.SaveHistory(getDesk.FirstOrDefault().AppId, actionFrom, actionTo, "Rejected", "Staff rejected customer's application => " + txtComment);

                            // Saving Messages
                            string subject = stage.FirstOrDefault().StageName + " Application REJECTED with Ref : " + getApps.FirstOrDefault().AppRefNo;
                            string content = "Your application have been Rejected with comment -: " + txtComment + " Kindly find other details below.";
                            var emailMsg = _helpersController.SaveMessage(getApps.FirstOrDefault().AppId, company.FirstOrDefault().CompanyId, subject, content);
                            var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                            _helpersController.UpdateElpsApplication(getApps.ToList());
                        }
                        else
                        {
                            result = "Something went wrong trying to update your desk.";
                        }
                    }
                    // reviewer or inspector rejecting to customer with documents to attached
                    else if (getProcess.FirstOrDefault().r.RoleName == GeneralClass.COMPANY && RequiredDocs.Any())
                    {
                        if (RequiredDocs != null)
                        {
                            foreach (var r in RequiredDocs)
                            {
                                var checkDocs = _context.SubmittedDocuments.Where(x => x.AppDocId == r && x.AppId == getDesk.FirstOrDefault().AppId);

                                if (checkDocs.Count() <= 0)
                                {
                                    SubmittedDocuments submitDoc = new SubmittedDocuments()
                                    {
                                        AppId = getDesk.FirstOrDefault().AppId,
                                        AppDocId = r,
                                        IsAddictional = true,
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now
                                    };
                                    _context.SubmittedDocuments.Add(submitDoc);
                                }
                            }
                            _context.SaveChanges();
                        }

                        getApps.FirstOrDefault().DeskId = getDesk.FirstOrDefault().DeskId;
                        getApps.FirstOrDefault().CurrentDeskId = getDesk.FirstOrDefault().StaffId;
                        getApps.FirstOrDefault().UpdatedAt = DateTime.Now;
                        getApps.FirstOrDefault().Status = GeneralClass.Rejected;

                        getDesk.FirstOrDefault().HasWork = true;
                        getDesk.FirstOrDefault().UpdatedAt = DateTime.Now;
                        getDesk.FirstOrDefault().Comment = txtComment;

                        if (_context.SaveChanges() > 0)
                        {
                            result = "External Rejection";

                            _helpersController.LogMessages("Staff rejected customer's application. Application reference : " + getApps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                            var user = _context.Companies.Where(x => x.CompanyId == getApps.FirstOrDefault().CompanyId).FirstOrDefault();

                            var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                            var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);


                            _helpersController.SaveHistory(getDesk.FirstOrDefault().AppId, actionFrom, actionTo, "Rejected", "Staff rejected customer's application => " + txtComment);

                            // Saving Messages
                            string subject = stage.FirstOrDefault().StageName + " Application REJECTED with Ref : " + getApps.FirstOrDefault().AppRefNo;
                            string content = "Your application have been Rejected with comment -: " + txtComment + " Kindly find other details below.";
                            var emailMsg = _helpersController.SaveMessage(getApps.FirstOrDefault().AppId, company.FirstOrDefault().CompanyId, subject, content);
                            var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                            _helpersController.UpdateElpsApplication(getApps.ToList());
                        }
                        else
                        {
                            result = "Something went wrong trying to update your desk.";
                        }
                    }
                    else // supervisor and the staff above rejecting back to staff
                    {
                        while (getDesk.Any())
                        {
                            previousSort++;

                            var getPreviousStaff = from d in _context.MyDesk.AsEnumerable()
                                                   join pr in _context.ApplicationProccess.AsEnumerable() on d.ProcessId equals pr.ProccessId
                                                   join s in _context.Staff.AsEnumerable() on d.StaffId equals s.StaffId
                                                   join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                                   where r.RoleName == getProcess.FirstOrDefault().r.RoleName && (d.Sort == (getDesk.FirstOrDefault().Sort - previousSort) && d.AppId == getDesk.FirstOrDefault().AppId && s.ActiveStatus == true && s.DeleteStatus == false && pr.DeleteStatus == false && r.DeleteStatus == false)
                                                   select new
                                                   {
                                                       d,
                                                       pr,
                                                   };

                            if (getPreviousStaff.Any())
                            {
                                var prevDesk = _context.MyDesk.Where(x => x.AppId == getDesk.FirstOrDefault().AppId && x.StaffId == getPreviousStaff.FirstOrDefault().d.StaffId && x.HasWork == true).OrderByDescending(c => c.DeskId).Take(1);

                                if (prevDesk.Any())
                                {
                                    MyDesk desk = new MyDesk()
                                    {
                                        ProcessId = prevDesk.FirstOrDefault().ProcessId,
                                        AppId = prevDesk.FirstOrDefault().AppId,
                                        StaffId = prevDesk.FirstOrDefault().StaffId,
                                        HasWork = false,
                                        CreatedAt = DateTime.Now,
                                        HasPushed = true,
                                        Sort = prevDesk.FirstOrDefault().Sort
                                    };

                                    _context.MyDesk.Add(desk);

                                    if (_context.SaveChanges() > 0)
                                    {
                                        getDesk.FirstOrDefault().HasWork = true;
                                        getDesk.FirstOrDefault().UpdatedAt = DateTime.Now;
                                        getDesk.FirstOrDefault().Comment = txtComment;

                                        var app = _context.Applications.Where(x => x.AppId == getDesk.FirstOrDefault().AppId);

                                        app.FirstOrDefault().CurrentDeskId = desk.StaffId;
                                        app.FirstOrDefault().UpdatedAt = DateTime.Now;
                                        app.FirstOrDefault().Status = GeneralClass.Processing;

                                        if (_context.SaveChanges() > 0)
                                        {
                                            string subj = "Application (" + getApps.FirstOrDefault().AppRefNo + ") Rejected Back to You.";
                                            string cont = "Application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been rejected internally back to you for processing. Kindly see rejection comment => " + txtComment;

                                            var staff = _context.Staff.Where(x => x.StaffId == desk.StaffId);

                                            var send = _helpersController.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);
                                            
                                            var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                            var actionTo = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);

                                            _helpersController.SaveHistory(getDesk.FirstOrDefault().AppId, actionFrom, actionTo, "In-Reject", "Application was rejected (Internally) back to staff with comment => " + txtComment);
                                            _helpersController.SaveHistory(getDesk.FirstOrDefault().AppId, actionFrom, actionTo, "Moved", "Application landed on staff desk");

                                            result = "Internal Rejection";
                                            break;
                                        }
                                        else
                                        {
                                            result = "Something went wrong trying to update your desk";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        result = "Something went wrong trying to reject this application. Try again.";
                                        break;
                                    }
                                }
                                else
                                {
                                    result = "Sorry, could not find previouse processing staff to reject application back.";
                                    break;
                                }
                            }
                            else
                            {
                                result = "Cannot find previouse processing staff. Please try again later.";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    result = "Something went wrong trying to find either desk application reference, application reference, processing reference, stage reference or company profile";
                }
            }

            _helpersController.LogMessages("Result from application rejection : " + result + ". Ref No : " + refNo, _helpersController.getSessionEmail());


            return Json(result);
        }

        /*
         * All application approval process/method
         * 
         * txtComment => Not encrypted; comment for approval.
         * txtDeskID => Encrypted desk id
         * txtAppID => Encrypted application id
         * txtApproveOption => option to take weeks, days or months
         * txtApproveDuration => a number for how long the permit will last
         */
        public JsonResult Approval(string txtComment, string txtDeskID, string txtAppID, int txtVolume)
        {
            string result = "";

            int deskID = generalClass.DecryptIDs(txtDeskID);
            int appID = generalClass.DecryptIDs(txtAppID);
            int staff_id = _helpersController.getSessionUserID();

            var start = "";

            if (deskID == 0 || appID == 0)
                result = "Something went wrong, Application or Desk reference not in correct format. Please refreash the page.";
            else
            {
                // check for alreday approvd trucks and barges with still valide permit before this approval.

                // current process
                var getDesk = from d in _context.MyDesk.AsEnumerable()
                              join p in _context.ApplicationProccess.AsEnumerable() on d.ProcessId equals p.ProccessId
                              join r in _context.UserRoles.AsEnumerable() on p.OnAcceptRoleId equals r.RoleId
                              join a in _context.Applications.AsEnumerable() on d.AppId equals a.AppId
                              join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                              join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              where d.DeskId == deskID && a.AppId == appID && d.HasWork == false && a.DeletedStatus == false
                              select new
                              {
                                  d,
                                  p,
                                  a,
                                  s,
                                  c,
                                  ty
                              };

                if (getDesk.Any())
                {
                    var appDetails = _helpersController.GetAppDetails(getDesk.FirstOrDefault().a.AppId);

                    start = getDesk.FirstOrDefault().p.Process;

                    if (getDesk.FirstOrDefault().p.Process == GeneralClass.START || getDesk.FirstOrDefault().p.Process == GeneralClass.NEXT || getDesk.FirstOrDefault().p.Process == GeneralClass.BEGIN)
                    {
                        // getting staff to drop application on 
                        int AppDropStaffID = _helpersController.ApplicationDropStaff(getDesk.FirstOrDefault().s.AppStageId, (getDesk.FirstOrDefault().d.Sort + 1));
                        List<ApplicationProccess> process = _helpersController.GetAppProcess(getDesk.FirstOrDefault().s.AppStageId, 0, (getDesk.FirstOrDefault().d.Sort + 1));

                        if (AppDropStaffID > 0 && process.Any())
                        {
                            var checkdesk = _context.MyDesk.Where(x => x.AppId == appID && x.ProcessId == process.FirstOrDefault().ProccessId && x.Sort == process.FirstOrDefault().Sort && x.HasWork == false);

                            if (checkdesk.Any())
                                result = "Sorry, this application is already on a staff desk.";
                            else
                            {
                                var nominated_staff = _context.NominatedStaff.Where(x => x.AppId == getDesk.FirstOrDefault().a.AppId);

                                if (nominated_staff.Count() <= 0 && _helpersController.getSessionRoleName() == GeneralClass.SECTION_HEAD && !appDetails.FirstOrDefault().ShortName.Contains("TAR"))
                                    result = "You need to nominate a staff for witnessing";
                                else
                                {
                                    if (getDesk.FirstOrDefault().ty.TypeName == GeneralClass.EWT)
                                    {
                                        var trans = _context.Transactions.Where(x => x.AppId == appID);

                                        if (trans.Any())
                                        {
                                            trans.FirstOrDefault().TransactionStatus = GeneralClass.PaymentCompleted;
                                            trans.FirstOrDefault().UpdatedAt = DateTime.Now;
                                        }
                                    }

                                    MyDesk desk = new MyDesk()
                                    {
                                        ProcessId = process.FirstOrDefault().ProccessId,
                                        AppId = appID,
                                        StaffId = AppDropStaffID,
                                        HasWork = false,
                                        CreatedAt = DateTime.Now,
                                        HasPushed = true,
                                        Sort = process.FirstOrDefault().Sort,
                                    };

                                    _context.MyDesk.Add(desk);

                                    if (_context.SaveChanges() > 0)
                                    {
                                        var desks = _context.MyDesk.Where(x => x.DeskId == deskID);
                                        var apps = _context.Applications.Where(x => x.AppId == appID);

                                        desks.FirstOrDefault().HasWork = true;
                                        desks.FirstOrDefault().UpdatedAt = DateTime.Now;
                                        desks.FirstOrDefault().Comment = txtComment;

                                        apps.FirstOrDefault().UpdatedAt = DateTime.Now;
                                        apps.FirstOrDefault().CurrentDeskId = AppDropStaffID;
                                        apps.FirstOrDefault().DeskId = desk.DeskId;
                                        apps.FirstOrDefault().Volume = txtVolume;

                                        if (_context.SaveChanges() > 0)
                                        {
                                            var user = _context.Staff.Where(x => x.StaffId == AppDropStaffID).FirstOrDefault();

                                            var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                            var actionTo = _helpersController.getActionHistory(user.RoleId, user.StaffId);

                                            _helpersController.SaveHistory(appID, actionFrom, actionTo, "Recommended", "Application recommendation approval sent and landed on staff desk  =>" + txtComment);

                                            var getStaff = _context.Staff.Where(x => x.StaffId == AppDropStaffID);

                                            var subject = "APPLICATION ON YOUR DESK : REF : " + apps.FirstOrDefault().AppRefNo;
                                            var content = "An application with REF. " + apps.FirstOrDefault().AppRefNo + " has landed on you desk.";
                                            var sendEmail = _helpersController.SendEmailMessageAsync(getStaff.FirstOrDefault().StaffEmail, getStaff.FirstOrDefault().LastName + " " + getStaff.FirstOrDefault().FirstName, subject, content, GeneralClass.STAFF_NOTIFY, null);

                                            result = "Approved Next";
                                        }
                                        else
                                            result = "Something went wrong trying to update your desk";
                                    }
                                    else
                                        result = "Something went wrong trying to push this application to the next processing officer.";
                                }
                            }
                        }
                        else
                            result = "Sorry, could not find staff to push to or process reference. Please try again later.";
                    }
                    else if (getDesk.FirstOrDefault().p.Process == GeneralClass.DONE) // for approve report
                    {
                        var myDesk = _context.MyDesk.Where(x => x.DeskId == deskID);
                        var getApps = _context.Applications.Where(x => x.AppId == appID);

                        myDesk.FirstOrDefault().HasWork = true;
                        myDesk.FirstOrDefault().HasPushed = true;
                        myDesk.FirstOrDefault().UpdatedAt = DateTime.Now;
                        myDesk.FirstOrDefault().Comment = txtComment;

                        getApps.FirstOrDefault().Status = GeneralClass.Approved;
                        getApps.FirstOrDefault().UpdatedAt = DateTime.Now;
                        getApps.FirstOrDefault().IsReportApproved = true;
                        getApps.FirstOrDefault().Volume = txtVolume;

                        if (_context.SaveChanges() > 0)
                        {
                            var getApp = _context.Applications.Where(x => x.AppId == appID);
                            var company = _context.Companies.Where(x => x.CompanyId == getApp.FirstOrDefault().CompanyId);

                            var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                            var actionTo = _helpersController.getActionHistory(company.FirstOrDefault().RoleId, company.FirstOrDefault().CompanyId);

                            _helpersController.SaveHistory(appID, actionFrom, actionTo, "Report Approval", "Application Report has been approved (Report APPROVAL) => " + txtComment);

                            string subject = "Application Report Approved With Ref : " + getApp.FirstOrDefault().AppRefNo;
                            string content = "Your application report has been APPROVED successfully. Kindly find other details below. You can login to the NUPRC's Well test portal to view.";
                            var emailMsg = _helpersController.SaveMessage(appID, company.FirstOrDefault().CompanyId, subject, content);
                            var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);
                            _helpersController.UpdateElpsApplication(getApp.ToList());

                            result = "Report Approved";
                        }
                        else
                            result = "Something went wrong trying to update applicatioin report status.";
                    }
                    else // END Generate Permit
                    {
                        // check if permit already generated
                        var checkPermit = _context.Permits.Where(x => x.AppId == appID);

                        if (checkPermit.Any())
                            result = "Permit has already been generated for this application";
                        else
                        {
                            var checkNominate = _context.NominatedStaff.Where(x => x.AppId == appID);

                            if (checkNominate.Any() || (!checkNominate.Any() && appDetails.FirstOrDefault().ShortName.Contains("TAR")))
                            {
                                var PermitNO = _helpersController.GeneratePermitNumber(appID);
                                var expiaryDate = _helpersController.PermitExpiry(appID);
                                var seq = _context.Permits.Count();

                                Models.DB.Permits permits = new Models.DB.Permits()
                                {
                                    AppId = appID,
                                    PermitNo = PermitNO,
                                    Printed = false,
                                    IsRenewed = false,
                                    IssuedDate = DateTime.Now,
                                    ExpireDate = expiaryDate,
                                    CreatedAt = DateTime.Now,
                                    ApprovedBy = staff_id,
                                    PermitSequence = (seq + 1)
                                };

                                _context.Permits.Add(permits);

                                if (_context.SaveChanges() > 0)
                                {
                                    var myDesk = _context.MyDesk.Where(x => x.DeskId == deskID);
                                    var getApps = _context.Applications.Where(x => x.AppId == appID);
                                    var getNomination = _context.NominatedStaff.Where(x => x.AppId == appID);

                                    myDesk.FirstOrDefault().HasWork = true;
                                    myDesk.FirstOrDefault().HasPushed = true;
                                    myDesk.FirstOrDefault().UpdatedAt = DateTime.Now;
                                    myDesk.FirstOrDefault().Comment = txtComment;

                                    getApps.FirstOrDefault().Status = GeneralClass.Approved;
                                    getApps.FirstOrDefault().UpdatedAt = DateTime.Now;
                                    getApps.FirstOrDefault().IsProposedApproved = true;
                                    getApps.FirstOrDefault().Volume = txtVolume;

                                    if(appDetails.FirstOrDefault().Type == GeneralClass.MER)
                                    {
                                        getApps.FirstOrDefault().IsReportApproved = true;
                                        getApps.FirstOrDefault().IsReportSubmitted = true;
                                    }

                                    foreach (var n in getNomination.ToList())
                                    {
                                        n.IsActive = true;
                                        n.HasSubmitted = false;
                                        n.UpdatedAt = DateTime.Now;
                                    }

                                    if (_context.SaveChanges() > 0)
                                    {
                                        string additional_content = "";

                                        var getApp = _context.Applications.Where(x => x.AppId == appID);
                                        var company = _context.Companies.Where(x => x.CompanyId == getApp.FirstOrDefault().CompanyId);
                                        var facility = _context.Facilities.Where(x => x.FacilityId == getApp.FirstOrDefault().FacilityId);

                                        if(appDetails.FirstOrDefault().Type == GeneralClass.DSTs)
                                        {
                                            additional_content += "; Also, you will also find the request letter to conduct End of well test. from the portal.";
                                        }

                                        // Send Company email
                                        string subject =  appDetails.FirstOrDefault().Stage + " Application Approved with Permit NO : " + permits.PermitNo;
                                        string content = "Your application  with Permit Number " + permits.PermitNo + ", for the application of " + appDetails.FirstOrDefault().Type + " - "+ appDetails.FirstOrDefault().Stage + " has been APPROVED. "+ additional_content +" Please, find other details below. You can login to the NUPRC's Well test portal and download your approval letters.";
                                        var emailMsg = _helpersController.SaveMessage(appID, company.FirstOrDefault().CompanyId, subject, content);
                                        var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                                        // send nomination mail to staff
                                        var nominated = _context.NominatedStaff.Where(x => x.AppId == appID && x.HasSubmitted == false);
  
                                        var n_subject = "RE: REQUEST TO CONDUCT WITNESSING ON " + facility.FirstOrDefault().FacilityName.ToUpper();

                                        if (nominated.Any())
                                        {
                                            foreach (var n in nominated.ToList())
                                            {
                                                var NominationLink = ElpsServices.link + "/Applications/NominationLink/" + generalClass.Encrypt(n.NominateId.ToString());

                                              
                                                var n_content = "A request to conduct (witness) for " + appDetails.FirstOrDefault().Type + " - " + appDetails.FirstOrDefault().Stage + " on " + facility.FirstOrDefault().FacilityName.ToUpper() + " has been sent successfully to you. You are hereby nominated to participate in the exercise. Kindly logon to the NUPRC's Well test portal and see the attached nomination letter. Also, you can respond to this nomination by clicking <a href='" + NominationLink + "'>(RESPOND HERE)</a> stating your avialability here. ";

                                                var staffs = _context.Staff.Where(x => x.StaffId == n.StaffId);

                                                var sendNomination = _helpersController.SendEmailMessageAsync(staffs.FirstOrDefault()?.StaffEmail, staffs.FirstOrDefault().LastName + " " + staffs.FirstOrDefault().FirstName, n_subject, n_content, GeneralClass.STAFF_NOTIFY, null);

                                                var actionFro = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                                var actionToo = _helpersController.getActionHistory(staffs.FirstOrDefault().RoleId, staffs.FirstOrDefault().StaffId);

                                                _helpersController.SaveHistory(appID, actionFro, actionToo, "Nomination", "Nomination letter successfully sent to staff");
                                            }
                                        }


                                        // send zopscon mail and umr zone head

                                        var getZoneStaff = from r in _context.NominationRequest.AsEnumerable()
                                                           join s in _context.Staff.AsEnumerable() on r.StaffId equals s.StaffId
                                                           join ro in _context.UserRoles.AsEnumerable() on s.RoleId equals ro.RoleId
                                                           join f in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals f.FieldOfficeId
                                                           join zfo in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zfo.FieldOfficeId
                                                           join z in _context.ZonalOffice.AsEnumerable() on zfo.ZoneId equals z.ZoneId
                                                           join zs in _context.ZoneStates.AsEnumerable() on z.ZoneId equals zs.ZoneId
                                                           join st in _context.States.AsEnumerable() on zs.StateId equals st.StateId
                                                           where ((ro.RoleName == GeneralClass.ZOPSCON || ro.RoleName == GeneralClass.UMR_ZONE_HEAD) && r.AppId == appID && f.FieldOfficeId == s.FieldOfficeId)
                                                           select new
                                                           {
                                                               ZospconName = s.LastName + " " + s.FirstName,
                                                               ZospconEmail = s.StaffEmail,
                                                               ZoneHeadName = s.LastName + " " + s.FirstName,
                                                               ZoneHeadEmail = s.StaffEmail,
                                                               ZoneRole = ro.RoleName,
                                                               StaffID = s.StaffId,
                                                               RoleId = s.RoleId
                                                           };

                                        if (getZoneStaff.Any())
                                        {
                                            var n_content = "A request to conduct (witness) for " + appDetails.FirstOrDefault().Type + " - " + appDetails.FirstOrDefault().Stage + " on " + facility.FirstOrDefault().FacilityName.ToUpper() + " has been sent successfully. Kindly logon to the NUPRC's well test portal and see the attached nomination letter.";

                                            foreach (var n in getZoneStaff.ToList())
                                            {
                                                var sendToZone = _helpersController.SendEmailMessageAsync(n?.ZospconEmail, n?.ZospconName, n_subject, n_content, GeneralClass.STAFF_NOTIFY, null);

                                                var actionF = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                                var actionT = _helpersController.getActionHistory(n.RoleId, n.StaffID);

                                                _helpersController.SaveHistory(appID, actionF, actionT, "Nomination Sent", "Nomination letter successfully sent to staff (" + n?.ZoneRole + ")");
                                            }
                                        }

                                        _helpersController.UpdateElpsApplication(getApp.ToList());

                                        // posting permits to elps
                                        var posted = _helpersController.PostPermitToElps(permits.PermitId, permits.PermitNo, getApp.FirstOrDefault().AppRefNo, company.FirstOrDefault().CompanyElpsId, permits.IssuedDate, permits.ExpireDate, (bool)permits.IsRenewed);

                                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                        var actionTo = _helpersController.getActionHistory(company.FirstOrDefault().RoleId, company.FirstOrDefault().CompanyId);

                                        _helpersController.SaveHistory(appID, actionFrom, actionTo, "Final Approval", "Application has been approved (FINAL APPROVAL) => " + txtComment);

                                        result = "Approved";
                                    }
                                    else
                                        result = "Application approved but Something went wrong trying to update applicatioin status.";
                                }
                                else
                                    result = "Something went wrong trying to save this permit. Please try again later";
                            }
                            else
                                result = "You need to nominate one or more staff to witness the survey.";
                        }
                    }
                }
                else
                    result = "Opps!!! something went wrong trying to fetch your desk. Please try again later.";
            }

            _helpersController.LogMessages("Result from Application Approval process : " + result, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Creating nomination staff
         */
        //public JsonResult SaveNominatedStaff(int txtAppId, int RequestId, List<int> StaffId)
        //{
        //    try
        //    {
        //        string result = "";

        //        var appid = txtAppId;
        //        var requestid = RequestId;
        //        int done = 0;

        //        var request = _context.NominationRequest.Where(x => x.RequestId == RequestId);


        //        if (appid == 0 && request.Any())
        //        {
        //            result = "Something went wrong, request reference not in correct format.";
        //        }
        //        else
        //        {
        //            int staff_id = request.FirstOrDefault().StaffId;

        //            var removeStaff = _context.NominatedStaff.Where(x => x.AppId == appid && x.HasSubmitted == false && x.IsActive == false);

        //            _context.NominatedStaff.RemoveRange(removeStaff);
        //            _context.SaveChanges();

        //            foreach (var p in StaffId)
        //            {
        //                if (p != 0)
        //                {
        //                    var checkStaff = _context.NominatedStaff.Where(x => x.StaffId == p && x.AppId == appid);

        //                    if (checkStaff.Count() <= 0)
        //                    {
        //                        NominatedStaff nominated = new NominatedStaff()
        //                        {
        //                            StaffId = p,
        //                            AppId = appid,
        //                            Designation = "UMR",
        //                            CreatedAt = DateTime.Now,
        //                            CreatedBy = staff_id,
        //                            HasSubmitted = false,
        //                            IsActive = false
        //                        };

        //                        _context.NominatedStaff.Add(nominated);
        //                        done += _context.SaveChanges();
        //                    }
        //                }
        //            }

        //            if (done > 0)
        //            {
        //                var NominationRequest = _context.NominationRequest.Where(x => x.AppId == appid);

        //                NominationRequest.FirstOrDefault().HasDone = true;
        //                NominationRequest.FirstOrDefault().UpdatedAt = DateTime.Now;

        //                _context.SaveChanges();

        //                var staff = _context.Staff.Where(x => x.StaffId == staff_id);

        //                var actionFrom = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);
        //                var actionTo = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);

        //                _helpersController.SaveHistory(appid, actionFrom, actionTo, "Nomination", "Saff has been save for this nomination");

        //                _helpersController.LogMessages("Result from Nominating a staff : " + result, staff.FirstOrDefault().StaffEmail);

        //                result = "Saved";
        //            }
        //            else
        //            {
        //                result = "Something went wrong trying to save this nominated staff. Please try again later.";
        //            }
        //        }



        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json("Error " + ex.Message);
        //    }

        //}
        public JsonResult SaveNominatedStaff(string txtAppId, List<int> StaffId, string txtComment)
        {
            try
            {
                string result = "";
                var appid = generalClass.DecryptIDs(txtAppId);

                if (appid == 0)
                    result = "Something went wrong, request reference not in correct format.";
                else
                {
                    int staff_id = _helpersController.getSessionUserID();

                    int done = 0;
                    var removeStaff = _context.NominatedStaff.Where(x => x.AppId == appid && x.HasSubmitted == false && x.IsActive == false);

                    _context.NominatedStaff.RemoveRange(removeStaff);
                    _context.SaveChanges();

                    foreach (var p in StaffId)
                    {
                        if (p != 0)
                        {
                            var checkStaff = _context.NominatedStaff.Where(x => x.StaffId == p && x.AppId == appid);

                            if (checkStaff.Count() <= 0)
                            {
                                NominatedStaff nominated = new NominatedStaff()
                                {
                                    StaffId = p,
                                    AppId = appid,
                                    Designation = "UMR",
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = staff_id,
                                    HasSubmitted = false,
                                    IsActive = true
                                };

                                _context.NominatedStaff.Add(nominated);
                                done += _context.SaveChanges();
                            }
                        }
                    }

                    if (done > 0)
                    {
                        //var NominationRequest = _context.NominationRequest.Where(x => x.AppId == appid);

                        //NominationRequest.FirstOrDefault().HasDone = true;
                        //NominationRequest.FirstOrDefault().UpdatedAt = DateTime.Now;

                        //_context.SaveChanges();

                        var staff = _context.Staff.Where(x => x.StaffId == staff_id);

                        var actionFrom = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);
                        var actionTo = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);

                        _helpersController.SaveHistory(appid, actionFrom, actionTo, "Nomination", "Saff has been save for this nomination");

                        _helpersController.LogMessages("Result from Nominating a staff : " + result, staff.FirstOrDefault().StaffEmail);

                        result = "Saved";
                    }
                    else
                        result = "Something went wrong trying to save this nominated staff. Please try again later.";
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("Error " + ex.Message);
            }

        }

        /*
         * Sending Nomination request to a zopscon
         * 
         */ 
        public async Task<JsonResult> SendNominationRequest(string txtAppId, List<string> txtStaffEmail, string txtComment)
        {
            try
            {
                string result = "";

                var appid = generalClass.DecryptIDs(txtAppId);

                if (appid == 0)
                    result = "Something went wrong, request reference not in correct format.";
                else
                {
                    var staff = _context.Staff.Where(x => txtStaffEmail.Contains(x.StaffEmail) && x.ActiveStatus == true && x.DeleteStatus == false).Select(x => x).ToList();

                    if (staff.Any())
                    {
                        //var checkStaff = _context.NominationRequest.Where(x => !staff.Any(y => y.StaffId == x.StaffId) && x.AppId == appid && x.HasDone == false);
                        var checkStaff = (from n in _context.NominationRequest.AsEnumerable()
                                          join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                          where s.StaffId == n.StaffId && n.AppId == appid && !n.HasDone
                                          select s.StaffEmail);

                        //if (checkStaff.Any())
                        //    result = "This nomination request has already been sent to this staff.";
                        //else
                        //{
                        var stafflist = staff.Where(x => !checkStaff.Contains(x.StaffEmail));
                        foreach (var st in stafflist)
                        {
                            Models.DB.NominationRequest nominated = new Models.DB.NominationRequest()
                            {
                                StaffId = st.StaffId,
                                AppId = appid,
                                Comment = txtComment,
                                HasDone = false,
                                CreatedAt = DateTime.Now,
                            };

                            _context.NominationRequest.Add(nominated);

                            if (_context.SaveChanges() > 0)
                            {
                                result = "Saved";

                                var apps = _context.Applications.Where(x => x.AppId == appid);

                                var user = _context.Staff.Where(x => x.StaffId == st.StaffId);

                                var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                var actionTo = _helpersController.getActionHistory(user.FirstOrDefault().RoleId, user.FirstOrDefault().StaffId);

                                //var NominationLink = ElpsServices.link + "Applications/NominationRequest/" + generalClass.Encrypt(nominated.RequestId.ToString());
                                var NominationLink = ElpsServices.link;

                                var subject = "NOMINATION REQUEST (" + apps.FirstOrDefault().AppRefNo + ")";
                                //var content = "You have a request to nominate some staff that will take part in the witnessing of an application with reference number : " + apps.FirstOrDefault().AppRefNo + ". Please click on this link to select staff => <a href='" + NominationLink + "'>(RESPOND HERE)</a> <hr> <b>See Comment </b> <br> " + txtComment;
                                var content = "You have been nominated to take part in the witnessing of an application with reference number : " + apps.FirstOrDefault().AppRefNo + ". Please click on this link to view nomination => <a href='" + NominationLink + "'>(RESPOND HERE)</a> <hr> <b>See Comment </b> <br> " + txtComment;
                                var sendEmail = _helpersController.SendEmailMessageAsync(user.FirstOrDefault().StaffEmail, user.FirstOrDefault().LastName + " " + user.FirstOrDefault().FirstName, subject, content, GeneralClass.STAFF_NOTIFY, null);
                                _helpersController.SaveHistory(appid, actionFrom, actionTo, "Nomination", "Request has been sent to nominated satff for this application");
                            }
                            else
                                result = "Something went wrong trying to send this nomination request to staff. Please try again later.";
                        }
                            
                        //}
                    }
                    else
                        result = "Opps!!! this staff was not found, please try again later.";
                }

                _helpersController.LogMessages("Result from sending nomination to staff : " + result, _helpersController.getSessionEmail());

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("Error " + ex.Message);
            }
        }

        public IActionResult NominatedStaff()
        {
            var get = from n in _context.NominatedStaff.AsEnumerable()
                      join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                      join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                      join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                      join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                      join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                      join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                      join st in _context.Staff.AsEnumerable() on n.StaffId equals st.StaffId
                      where n.StaffId == _helpersController.getSessionUserID()
                      select new NominatedList
                      {
                          AppRef = a.AppRefNo,
                          CreatedAt = n.CreatedAt,
                          hasSubmitted = n.HasSubmitted,
                          isActive = n.IsActive,
                          CompanyName = c.CompanyName,
                          StaffName = st.LastName + " " + st.FirstName,
                          AppID = n.AppId,
                          NominationID = n.NominateId,
                          WellDetails = f.FacilityName,
                      };

            return View(get.ToList());
        }

        public IActionResult ViewNominationReport(string id)
        {
            var norm_id = generalClass.DecryptIDs(id.Trim());

            if (norm_id == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, No link was found for this application history.") });
            }
            else
            {
                var report = from re in _context.NominatedStaff.AsEnumerable()
                              join r in _context.Applications.AsEnumerable() on re.AppId equals r.AppId
                              join st in _context.Staff.AsEnumerable() on re.StaffId equals st.StaffId
                             join ts in _context.AppTypeStage.AsEnumerable() on r.AppTypeStageId equals ts.TypeStageId
                             join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                             join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                             join f in _context.Facilities.AsEnumerable() on r.FacilityId equals f.FacilityId
                             join c in _context.Companies.AsEnumerable() on r.CompanyId equals c.CompanyId
                              join t in _context.Transactions.AsEnumerable() on r.AppId equals t.AppId into trans
                              from tr in trans.DefaultIfEmpty()
                              where (re.NominateId == norm_id)
                              select new ReportViewModel
                              {
                                  RefNo = r.AppRefNo,
                                  CompanyName = c.CompanyName,
                                  CompanyAddress = c.Address,
                                  CompanyEmail = c.CompanyEmail,
                                  Stage = ty.TypeName + " - " + s.StageName,
                                  Status = r.Status,
                                  DateApplied = r.DateApplied,
                                  DateSubmitted = r.DateSubmitted,
                                  Staff = st.LastName + " " + st.FirstName + " (" + st.StaffEmail + ")",
                                  ReportDate = re.CreatedAt,
                                  UpdatedAt = re.UpdatedAt,
                                  Subject = re.Title,
                                  Comment = re.Contents,
                                  DocSource = re.DocSource,
                                  WellName = f.FacilityName,

                              };

                ViewData["AppRef"] = report.FirstOrDefault()?.RefNo;

                _helpersController.LogMessages("Displaying application nomination report for Application reference : " + report.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                return View(report.ToList());
            }
        }

        public IActionResult AddReport(string id)
        {
            int NormID = 0;

            var norm_id = generalClass.Decrypt(id.Trim());

            if (norm_id == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, No link was found for this application history.") });
            }
            else
            {
                NormID = Convert.ToInt32(norm_id);

                List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
                List<MissingDocument> missingDocuments = new List<MissingDocument>();
                List<BothDocuments> bothDocuments = new List<BothDocuments>();
                List<NominationReport> nominationReports = new List<NominationReport>();

                var apps = from n in _context.NominatedStaff.AsEnumerable()
                           join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                           join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId into Facility
                           join c in _context.Companies.AsEnumerable().AsEnumerable() on a.CompanyId equals c.CompanyId into Company
                           join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                           join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                           join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                           where ((n.NominateId == NormID))
                           select new ApplicationDetails
                           {
                               AppId = a.AppId,
                               RefNo = a.AppRefNo,
                               Status = a.Status,
                               AppStage =ty.TypeName + " - " + s.StageName,
                               DateApplied = a.DateApplied,
                               DateSubmitted = a.DateSubmitted,
                               ElpsFacilityID = Facility.FirstOrDefault().ElpsFacilityId,
                               CompanyElpsID = Company.FirstOrDefault().CompanyElpsId,
                               CompanyName = Company.FirstOrDefault().CompanyName,
                               CompanyID = Company.FirstOrDefault().CompanyId,
                               CompanyEmail = Company.FirstOrDefault().CompanyEmail,
                               CompanyAddress = Company.FirstOrDefault().Address,
                               NominationID = n.NominateId,
                               WellName = Facility.FirstOrDefault().FacilityName,
                           };

                if (apps.Any())
                {

                    var getDocuments = _context.ApplicationDocuments.Where(x => x.DocType == "Facility" && x.DocName.Trim().Contains(GeneralClass.staff_exercise_report_fac_doc) && x.DeleteStatus == false);

                    if (getDocuments.Any())
                    {
                        ViewData["FacilityElpsID"] = apps.FirstOrDefault().ElpsFacilityID;
                        ViewData["CompanyElpsID"] = apps.FirstOrDefault().CompanyElpsID;
                        ViewData["FacilityName"] = apps.FirstOrDefault().WellName;
                        ViewData["AppStage"] = apps.FirstOrDefault().AppStage;
                        ViewData["AppID"] = apps.FirstOrDefault().AppId;
                        ViewData["AppRefNo"] = apps.FirstOrDefault().RefNo;
                        ViewData["AppStatus"] = apps.FirstOrDefault().Status;
                        ViewData["NominationID"] = apps.FirstOrDefault().NominationID;

                        List<LpgLicense.Models.FacilityDocument> facilityDoc = generalClass.getFacilityDocuments(apps.FirstOrDefault().ElpsFacilityID.ToString());

                        if (facilityDoc != null)
                        {
                            foreach (var fDoc in facilityDoc)
                            {
                                if (fDoc.Document_Type_Id == getDocuments.FirstOrDefault().ElpsDocTypeId)
                                {
                                    presentDocuments.Add(new PresentDocuments
                                    {
                                        Present = true,
                                        FileName = fDoc.Name,
                                        Source = fDoc.Source,
                                        CompElpsDocID = fDoc.Id,
                                        DocTypeID = fDoc.Document_Type_Id,
                                        LocalDocID = getDocuments.FirstOrDefault().AppDocId,
                                        DocType = getDocuments.FirstOrDefault().DocType,
                                        TypeName = getDocuments.FirstOrDefault().DocName

                                    });
                                }
                            }

                            var result = getDocuments.AsEnumerable().Where(x => !presentDocuments.AsEnumerable().Any(x2 => x2.LocalDocID == x.AppDocId));

                            foreach (var r in result)
                            {
                                missingDocuments.Add(new MissingDocument
                                {
                                    Present = false,
                                    DocTypeID = r.ElpsDocTypeId,
                                    LocalDocID = r.AppDocId,
                                    DocType = r.DocType,
                                    TypeName = r.DocName
                                });
                            }

                            bothDocuments.Add(new BothDocuments
                            {
                                missingDocuments = missingDocuments,
                                presentDocuments = presentDocuments,
                            });


                            nominationReports.Add(new NominationReport
                            {
                                applicationDetails = apps.ToList(),
                                bothDocuments = bothDocuments.ToList()
                            });

                            _helpersController.LogMessages("Loading facility information and document for nominated staff report upload : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                            _helpersController.LogMessages("Displaying/Viewing more application details.  Reference : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());
                        }

                        return View(nominationReports.ToList());
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong trying fetch well documents, please try again later.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Application was not found, please try again later.") });
                }
            }
        }

        public IActionResult EditNominationReport(string id)
        {
            int NormID = 0;

            var norm_id = generalClass.Decrypt(id.Trim());

            if (norm_id == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, No link was found for this application history.") });
            }
            else
            {
                NormID = Convert.ToInt32(norm_id);

                List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
                List<MissingDocument> missingDocuments = new List<MissingDocument>();
                List<BothDocuments> bothDocuments = new List<BothDocuments>();
                List<NominationReport> nominationReports = new List<NominationReport>();

                var apps = from n in _context.NominatedStaff.AsEnumerable()
                           join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                           join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId into Facility
                           join c in _context.Companies.AsEnumerable().AsEnumerable() on a.CompanyId equals c.CompanyId into Company
                           join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                           join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                           join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                           where ((n.NominateId == NormID))
                           select new ApplicationDetails
                           {
                               AppId = a.AppId,
                               RefNo = a.AppRefNo,
                               Status = a.Status,
                               AppStage = ty.TypeName + " - " + s.StageName,
                               DateApplied = a.DateApplied,
                               DateSubmitted = a.DateSubmitted,
                               ElpsFacilityID = Facility.FirstOrDefault().ElpsFacilityId,
                               CompanyElpsID = Company.FirstOrDefault().CompanyElpsId,
                               CompanyName = Company.FirstOrDefault().CompanyName,
                               CompanyID = Company.FirstOrDefault().CompanyId,
                               CompanyEmail = Company.FirstOrDefault().CompanyEmail,
                               CompanyAddress = Company.FirstOrDefault().Address,
                               Titile = n.Title,
                               Content = n.Contents,
                               SubmittedAt = n.SubmittedAt,
                               NominationID = n.NominateId,
                               WellName = Facility.FirstOrDefault().FacilityName,
                           };

                if (apps.Any())
                {

                    var getDocuments = _context.ApplicationDocuments.Where(x => x.DocType == "Facility" && x.DocName.Trim().Contains(GeneralClass.staff_exercise_report_fac_doc) && x.DeleteStatus == false);

                    if (getDocuments.Any())
                    {
                        ViewData["FacilityElpsID"] = apps.FirstOrDefault().ElpsFacilityID;
                        ViewData["CompanyElpsID"] = apps.FirstOrDefault().CompanyElpsID;
                        ViewData["FacilityName"] = apps.FirstOrDefault().WellName;
                        ViewData["AppStage"] = apps.FirstOrDefault().AppStage;
                        ViewData["AppID"] = apps.FirstOrDefault().AppId;
                        ViewData["AppRefNo"] = apps.FirstOrDefault().RefNo;
                        ViewData["AppStatus"] = apps.FirstOrDefault().Status;
                        ViewData["NominationID"] = apps.FirstOrDefault().NominationID;

                        List<LpgLicense.Models.FacilityDocument> facilityDoc = generalClass.getFacilityDocuments(apps.FirstOrDefault().ElpsFacilityID.ToString());

                        if (facilityDoc != null)
                        {
                            foreach (var fDoc in facilityDoc)
                            {
                                if (fDoc.Document_Type_Id == getDocuments.FirstOrDefault().ElpsDocTypeId)
                                {
                                    presentDocuments.Add(new PresentDocuments
                                    {
                                        Present = true,
                                        FileName = fDoc.Name,
                                        Source = fDoc.Source,
                                        CompElpsDocID = fDoc.Id,
                                        DocTypeID = fDoc.Document_Type_Id,
                                        LocalDocID = getDocuments.FirstOrDefault().AppDocId,
                                        DocType = getDocuments.FirstOrDefault().DocType,
                                        TypeName = getDocuments.FirstOrDefault().DocName

                                    });
                                }
                            }

                            var result = getDocuments.AsEnumerable().Where(x => !presentDocuments.AsEnumerable().Any(x2 => x2.LocalDocID == x.AppDocId));

                            foreach (var r in result)
                            {
                                missingDocuments.Add(new MissingDocument
                                {
                                    Present = false,
                                    DocTypeID = r.ElpsDocTypeId,
                                    LocalDocID = r.AppDocId,
                                    DocType = r.DocType,
                                    TypeName = r.DocName
                                });
                            }

                            bothDocuments.Add(new BothDocuments
                            {
                                missingDocuments = missingDocuments,
                                presentDocuments = presentDocuments,
                            });


                            nominationReports.Add(new NominationReport
                            {
                                applicationDetails = apps.ToList(),
                                bothDocuments = bothDocuments.ToList()
                            });

                            _helpersController.LogMessages("Loading facility information and document for nominated staff report upload : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                            _helpersController.LogMessages("Displaying/Viewing more application details.  Reference : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());
                        }

                        return View(nominationReports.ToList());
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong trying fetch well documents, please try again later.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Application was not found, please try again later.") });
                }
            }
        }

        /*
        * Saving nominated staff application report
        * 
        * AppID => encrypted application id
        * txtReport => the comment for the report
        */
        public JsonResult SaveNominationReport(string NormID, string txtReport, string txtReportTitle, List<SubmitDoc> SubmittedDocuments)
        {
            string result = "";

            int norm_id = 0;

            var normid = generalClass.Decrypt(NormID.ToString().Trim());

            if (normid == "Error")
            {
                result = "Application link error";
            }
            else
            {
                norm_id = Convert.ToInt32(normid);

                var getNorm = _context.NominatedStaff.Where(x => x.NominateId == norm_id).ToList();

                if (getNorm.Any())
                {
                    int appid = getNorm.FirstOrDefault().AppId;

                    getNorm.FirstOrDefault().Title = txtReportTitle.ToUpper();
                    getNorm.FirstOrDefault().ElpsDocId = SubmittedDocuments.FirstOrDefault()?.CompElpsDocID;
                    getNorm.FirstOrDefault().DocSource = SubmittedDocuments.FirstOrDefault()?.DocSource;
                    getNorm.FirstOrDefault().AppDocId = SubmittedDocuments.FirstOrDefault()?.LocalDocID;
                    getNorm.FirstOrDefault().Contents = txtReport;
                    getNorm.FirstOrDefault().SubmittedAt = DateTime.Now;
                    getNorm.FirstOrDefault().UpdatedAt = DateTime.Now;
                    getNorm.FirstOrDefault().HasSubmitted = true;

                    if (_context.SaveChanges() > 0)
                    {
                        var apps = _context.Applications.Where(x => x.AppId == appid);

                        result = "Report Saved";

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                        _helpersController.LogMessages("Saving report for application : " + apps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());
                        _helpersController.SaveHistory(apps.FirstOrDefault().AppId, actionFrom, actionTo, "Witness Report", "A report has been added for the witnessing of WELL Test.");

                    }
                    else
                    {
                        result = "Something went wrong trying to save your report";
                    }
                }
            }
            _helpersController.LogMessages("Operation result from saving report : " + result, _helpersController.getSessionEmail());
            return Json(result);
        }

        public JsonResult EditNominationReports(string NormID, string txtReport, string txtReportTitle, List<SubmitDoc> SubmittedDocuments)
        {
            string result = "";

            int rID = 0;

            var id = generalClass.Decrypt(NormID.ToString().Trim());

            if (id == "Error")
            {
                result = "Application link error";
            }
            else
            {
                rID = Convert.ToInt32(id);

                var get = _context.NominatedStaff.Where(x => x.NominateId == rID);

                if (get.Any())
                {
                    int appid = get.FirstOrDefault().AppId;

                    get.FirstOrDefault().Contents = txtReport;
                    get.FirstOrDefault().UpdatedAt = DateTime.Now;
                    get.FirstOrDefault().Title = txtReportTitle.ToUpper();

                    if (SubmittedDocuments.FirstOrDefault()?.CompElpsDocID != 0)
                    {
                        get.FirstOrDefault().ElpsDocId = SubmittedDocuments.FirstOrDefault()?.CompElpsDocID;
                    }

                    if (SubmittedDocuments.FirstOrDefault()?.LocalDocID != 0)
                    {
                        get.FirstOrDefault().AppDocId = SubmittedDocuments.FirstOrDefault()?.LocalDocID;
                    }

                    if (SubmittedDocuments.FirstOrDefault()?.DocSource != "NILL")
                    {
                        get.FirstOrDefault().DocSource = SubmittedDocuments.FirstOrDefault()?.DocSource;
                    }


                    if (_context.SaveChanges() > 0)
                    {
                        var apps = _context.Applications.Where(x => x.AppId == appid);

                        result = "Report Edited";

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                        _helpersController.LogMessages("Saving witnessing report for application : " + apps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());
                        _helpersController.SaveHistory(appid, actionFrom, actionTo, "Edit Report", "A nomination witnesss report has been updated to this application.");
                    }
                    else
                    {
                        result = "Something went wrong trying to save your report";
                    }
                }
                else
                {
                    result = "Something went wrong trying to find this report.";
                }
            }

            _helpersController.LogMessages("Report update status :" + result + " Report ID : " + rID, _helpersController.getSessionEmail());

            return Json(result);
        }

        public JsonResult DenyApplication(string DeskID, string txtComment)
        {
            string result = "";

            int deskID = 0;
            var deskId = generalClass.Decrypt(DeskID.Trim());

            if (deskId == "Error")
            {
                result = "Application report link error";
            }
            else
            {
                deskID = Convert.ToInt32(deskId);

                var desk = _context.MyDesk.Where(x => x.DeskId == deskID);

                var app = from a in _context.Applications
                          join f in _context.Facilities on a.FacilityId equals f.FacilityId
                          join c in _context.Companies on a.CompanyId equals c.CompanyId
                          where a.AppId == desk.FirstOrDefault().AppId
                          select new
                          {
                              CompanyID = c.CompanyId,
                              AppRefNo = a.AppRefNo,
                              CompanyEmail = c.CompanyEmail,
                              CompanyName = c.CompanyName
                          };


                if (desk.Any())
                {
                    var apps = _context.Applications.Where(x => x.AppId == desk.FirstOrDefault().AppId && x.DeletedStatus == false && x.Status != GeneralClass.DISAPPROVE);

                    if (apps.Any())
                    {
                        apps.FirstOrDefault().Status = GeneralClass.DISAPPROVE;
                        apps.FirstOrDefault().UpdatedAt = DateTime.Now;
                        apps.FirstOrDefault().CurrentDeskId = desk.FirstOrDefault().StaffId;
                        apps.FirstOrDefault().DeskId = desk.FirstOrDefault().DeskId;

                        if (_context.SaveChanges() > 0)
                        {
                            desk.FirstOrDefault().HasWork = true;
                            desk.FirstOrDefault().UpdatedAt = DateTime.Now;
                            desk.FirstOrDefault().Comment = txtComment;

                            if (_context.SaveChanges() > 0)
                            {
                                var _apps = _context.Applications.Where(x => x.AppId == desk.FirstOrDefault().AppId);

                                result = "Denied";

                                _helpersController.LogMessages("AD Disapprove company's application. Application reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());
                                
                                var user = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyID).FirstOrDefault();

                                var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                var actionTo = _helpersController.getActionHistory(user.RoleId, user.CompanyId);

                                _helpersController.SaveHistory(desk.FirstOrDefault().AppId, actionFrom, actionTo, "Dismissed", "Staff rejected & dismissed customer's application => " + txtComment);

                                // Saving Messages
                                string subject = "Application DISAPPROVED with Ref : " + app.FirstOrDefault().AppRefNo;
                                string content = "Your application have been DISAPPROVED with comment -: " + txtComment + " Kindly find other details below.";
                                var emailMsg = _helpersController.SaveMessage(desk.FirstOrDefault().AppId, app.FirstOrDefault().CompanyID, subject, content);
                                var sendEmail = _helpersController.SendEmailMessageAsync(app.FirstOrDefault().CompanyEmail, app.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                                _helpersController.UpdateElpsApplication(_apps.ToList());
                            }
                            else
                            {
                                result = "Something went wrong trying to update your desk.";
                            }
                        }
                        else
                        {
                            result = "Something went wrong trying to reject application to client.";
                        }
                    }
                    else
                    {
                        result = "This application is not found.";
                    }
                }
                else
                {
                    result = "Something went wrong. This application was not found on your desk";
                }
            }
            return Json(result);
        }

        [AllowAnonymous]
        public IActionResult NominationLink(string id)
        {
            var norm_id = generalClass.DecryptIDs(id.Trim());

            if (norm_id == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Nomination link reference not in correct format, please try again later.") });
            }
            else
            {
                var get = from n in _context.NominatedStaff
                          join a in _context.Applications on n.AppId equals a.AppId
                          join c in _context.Companies on a.CompanyId equals c.CompanyId
                          join s in _context.Staff on n.StaffId equals s.StaffId
                          where n.NominateId == norm_id && n.HasSubmitted == false && n.IsActive == true
                          select new
                          {
                              AppRef = a.AppRefNo,
                              AppID = a.AppId,
                              StaffName = s.LastName + " " + s.FirstName + " (" + s.StaffEmail + ")",
                              CompanyName = c.CompanyName
                          };

                if(get.Any())
                {
                    ViewData["RefNo"] = get.FirstOrDefault().AppRef;
                    ViewData["AppID"] = get.FirstOrDefault().AppID;
                    ViewData["StaffName"] = get.FirstOrDefault().StaffName;
                    ViewData["CompanyName"] = get.FirstOrDefault().CompanyName;
                    ViewData["NominationID"] = norm_id;

                    return View();
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Nomination not found, please try again later.") });
                }
            }
        }

        [AllowAnonymous]
        public IActionResult NominationRequest(string id)
        {
            var norm_id = generalClass.DecryptIDs(id.Trim());

            if (norm_id == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Nomination link reference not in correct format, please try again later.") });
            }
            else
            {
                var get = from n in _context.NominationRequest
                          join a in _context.Applications on n.AppId equals a.AppId
                          join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join c in _context.Companies on a.CompanyId equals c.CompanyId
                          join st in _context.Staff on n.StaffId equals st.StaffId
                          join r in _context.UserRoles on st.RoleId equals r.RoleId
                          join f in _context.FieldOffices.AsEnumerable() on st.FieldOfficeId equals f.FieldOfficeId
                          join zf in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zf.FieldOfficeId
                          join z in _context.ZonalOffice.AsEnumerable() on zf.ZoneId equals z.ZoneId
                          where n.RequestId == norm_id
                          select new
                          {
                              AppRef = a.AppRefNo,
                              AppID = a.AppId,
                              StaffName = st.LastName + " " + st.FirstName + " (" + r.RoleName + ")",
                              Stage = ty.TypeName + " - " + s.StageName,
                              CompanyName = c.CompanyName,
                              CompanyAddress = c.Address + ", " + c.City + ", " + c.StateName,
                              CompanyEmail = c.CompanyEmail,
                              hasDone = n.HasDone == false ? "NO" : "YES",
                              CreatedAt = n.CreatedAt,
                              Comment = n.Comment,
                              ZonalId = z.ZoneId,
                              StaffId =  st.StaffId
                          };

                if (get.Any())
                {
                    ViewData["RefNo"] = get.FirstOrDefault().AppRef;
                    ViewData["AppID"] = get.FirstOrDefault().AppID;
                    ViewData["StaffName"] = get.FirstOrDefault().StaffName;
                    ViewData["CompanyName"] = get.FirstOrDefault().CompanyName;
                    ViewData["CompanyAddress"] = get.FirstOrDefault().CompanyAddress;
                    ViewData["RequestId"] = norm_id;
                    ViewData["Stage"] = get.FirstOrDefault().Stage;
                    ViewData["HasDone"] = get.FirstOrDefault().hasDone;
                    ViewData["CreatedAt"] = get.FirstOrDefault().CreatedAt;
                    ViewData["Comment"] = get.FirstOrDefault().Comment;

                    var staffList = from s in _context.Staff.AsEnumerable()
                                    join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                    join f in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals f.FieldOfficeId
                                    join zf in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zf.FieldOfficeId
                                    join z in _context.ZonalOffice.AsEnumerable() on zf.ZoneId equals z.ZoneId
                                    where ((s.ActiveStatus == true && s.DeleteStatus == false && z.ZoneId == get.FirstOrDefault().ZonalId &&  s.StaffId != get.FirstOrDefault().StaffId))
                                    select new StaffNomination
                                    {
                                        FullName = s.LastName + " " + s.FirstName + " (" + s.StaffEmail + ")",
                                        RoleName = r.RoleName,
                                        StaffId = s.StaffId,
                                        FieldOffice = f.OfficeName,
                                        ZonalOffice = z.ZoneName
                                    };

                    return View(staffList.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Nomination not found, please try again later.") });
                }
            }
        }

        public JsonResult NominationAction(int id, string option, string comment)
        {
            var status = "";
            var result = "";

            if(option == "Accept")
            {
                status = "YES";
            }
            else
            {
                status = "NO";
            }

            var get = _context.NominatedStaff.Where(x => x.NominateId == id);

            if(get.Any())
            {
                var appid = get.FirstOrDefault().AppId;
                var staffid = get.FirstOrDefault().StaffId;
                var createdby = get.FirstOrDefault().CreatedBy;

                get.FirstOrDefault().RespondStatus = status;
                get.FirstOrDefault().RespondComment = comment;
                get.FirstOrDefault().UpdatedAt = DateTime.Now;

                if(_context.SaveChanges() > 0)
                {
                    var staff = _context.Staff.Where(x => x.StaffId == staffid);

                    var actionFrom = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);
                    var actionTo = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);

                    _helpersController.SaveHistory(appid,actionFrom, actionTo, "Nomination " + option, "Nomination has been " + option + "ed by staff with comment : " + comment);

                    var subject = "Nomination " + option.ToUpper() + "ED BY STAFF";
                    var content = "Nomination has been " + option + "ed by staff with comment : " + comment;

                    var getStaff = _context.Staff.Where(x => x.StaffId == createdby).FirstOrDefault();

                    var send = _helpersController.SendEmailMessageAsync(getStaff?.StaffEmail, getStaff?.LastName + " "+ getStaff?.FirstName, subject, content, GeneralClass.STAFF_NOTIFY, null);

                    result = "Done";
                }
                else
                {
                    result = "Something went wrong trying to " + option + " this nomination, please try again later";
                }
            }
            else
            {
                result = "nomination link not found, please try again later.";
            }

            return Json(result);
        }

        /*
         * Get the info about a particular permit for a company application (Renewal or supplementry)
         */
        public JsonResult GetPermitInfo(string PermitNO, int typeId, int TypeStageId)
        {
            string result = "";

            var getApps = from p in _context.Permits.AsEnumerable()
                          join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                          join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                          join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                          where p.PermitNo == PermitNO.Trim()
                          select new
                          {
                              AppID = a.AppId,
                              RefNo = a.AppRefNo,
                              MyCompanyDetails = c.CompanyName + " (" + c.Address + ", " + c.City + ", " + c.StateName + ")",
                              FacilityDetails = f.FacilityName,
                              Status = a.Status,
                              Type = ty.TypeName,
                              Stage = s.StageName,
                              ShortName = s.ShortName,
                              CreatedAt = a.DateApplied.ToString(),
                              PermitNO = p.PermitNo,
                              issuedDate = p.IssuedDate.ToShortDateString(),
                              expireDate = p.ExpireDate.ToShortDateString()
                          };

            if(getApps.Any())
            {
                var getType = _context.ApplicationType.Where(x => x.AppTypeId == typeId);

                if(getType.Any())
                {
                    if(getType.FirstOrDefault().TypeName == getApps.FirstOrDefault().Type)
                    {
                        var checkStage = from ts in _context.AppTypeStage.AsEnumerable()
                                         join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                         where ts.TypeStageId == TypeStageId && ts.DeleteStatus == false
                                         select new
                                         {
                                             ShortName = s.ShortName
                                         };

                        if(checkStage.Any())
                        {
                            if(checkStage.FirstOrDefault().ShortName == GeneralClass.RTAR)
                            {
                                if(getApps.FirstOrDefault().ShortName == GeneralClass.RMER)
                                {
                                    return Json(getApps.ToList());
                                }
                                else
                                {
                                    result = "Please select the permit number for Routine TAR";
                                }
                            }
                            else if (checkStage.FirstOrDefault().ShortName == GeneralClass.OTAR)
                            {
                                if (getApps.FirstOrDefault().ShortName == GeneralClass.OCMER)
                                {
                                    return Json(getApps.ToList());
                                }
                                else
                                {
                                    result = "Please select the permit number for Off-cycle TAR";
                                }
                            }
                            else
                            {
                                return Json(getApps.ToList());
                            }
                        }
                        else
                        {
                            result = "Oops! Application type and stage was not found for your selection, please try again later";
                        }
                    }
                    else
                    {
                        result = "Oops!, Application type selected and the approval number does not match. Please try the correct approval number.";
                    }
                }
                else
                {
                    result = "Oops! Something went wrong, application type not found. Try again later.";
                }
            }
            else
            {
                result = "Oops! Something went wrong, cannot find this particular permit.";
            }

            return Json(result);
        }

    }
}
