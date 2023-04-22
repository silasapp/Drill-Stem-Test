using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DST.Controllers.Configurations;
using DST.Helpers;
using DST.Models.DB;
using LpgLicense.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using static DST.Models.GeneralModel;

namespace DST.Controllers.Application
{
    public class CompanyApplicationController : Controller
    {

        private readonly DST_DBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        private readonly HelpersController _helpersController;
        private readonly GeneralClass generalClass = new GeneralClass();
        private readonly RestSharpServices _restService = new RestSharpServices();
        private string fileName { get; set; }

        [Obsolete]
        private IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public CompanyApplicationController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IHostingEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = environment;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        /*
         * Viewing application details without operation control
         * 
         * 
         * id => encrypted desk id
         * option => encrypted process id
         */
        public IActionResult ViewApp(string id)
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
                    var appDocs = from sd in _context.SubmittedDocuments
                                  join ad in _context.ApplicationDocuments on sd.AppDocId equals ad.AppDocId
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

                    var temp = _context.TemplateTable.Where(x => x.AppId == app.FirstOrDefault().AppId);


                    applicationDetailsModels.Add(new ApplicationDetailsModel
                    {
                        appDocuuments = appDocs.ToList(),
                        applications = app.ToList(),
                        templates = temp.ToList()
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

        public IActionResult Apply(string id)
        {
            var ids = generalClass.DecryptIDs(id);

            if (ids == 0)
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Apply link not in correct format, please try again later.") });
            else
            {
                var stage = from a in _context.AppTypeStage.AsEnumerable()
                               join t in _context.ApplicationType.AsEnumerable() on a.AppTypeId equals t.AppTypeId
                               join s in _context.ApplicationStage.AsEnumerable() on a.AppStageId equals s.AppStageId
                               where ((a.AppTypeId == ids) && (a.DeleteStatus == false && t.DeleteStatus == false && s.DeleteStatus == false))
                               orderby a.Counter ascending
                               select new AppStage
                               {
                                   TypeStageID = a.TypeStageId,
                                   StageName = s.StageName,
                                   AppType = t.TypeName,
                                   Counter = a.Counter,
                               };

                var findTar = from p in _context.Permits.AsEnumerable()
                              join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                              join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                              join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                              join ap in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ap.TypeStageId
                              join t in _context.ApplicationType on ap.AppTypeId equals t.AppTypeId
                              join s in _context.ApplicationStage on ap.AppStageId equals s.AppStageId
                              //where ((t.TypeName == GeneralClass.MER && (s.ShortName != GeneralClass.OTAR || s.ShortName != GeneralClass.RTAR || s.ShortName != GeneralClass.TARR))  && c.CompanyId == _helpersController.getSessionUserID() && a.DeletedStatus == false && f.DeleteStatus == false)
                              where ((t.TypeName == GeneralClass.MER && (s.ShortName != GeneralClass.TARR)) && c.CompanyId == _helpersController.getSessionUserID() && a.DeletedStatus == false && f.DeleteStatus == false)
                              orderby p.PermitId ascending
                              select new PermitsInfo
                              {
                                  PermitNo = p.PermitNo,
                                  PermitDescription =   s.StageName + " => " + p.PermitNo
                              };


                var findEwt = from p in _context.Permits.AsEnumerable()
                              join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                              join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                              join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                              join ap in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ap.TypeStageId
                              join t in _context.ApplicationType on ap.AppTypeId equals t.AppTypeId
                              join s in _context.ApplicationStage on ap.AppStageId equals s.AppStageId
                              where (t.TypeName == GeneralClass.EWT &&( s.ShortName == GeneralClass.NEWT || s.ShortName == GeneralClass.EEWT) && c.CompanyId == _helpersController.getSessionUserID() && a.DeletedStatus == false && f.DeleteStatus == false)
                              orderby p.PermitId ascending
                              select new PermitsInfo
                              {
                                  PermitNo = p.PermitNo,
                                  PermitDescription = s.StageName + " => " + p.PermitNo
                              };


                if (stage.Any())
                {
                    ViewData["TypeName"] = stage.FirstOrDefault().AppType;
                    ViewData["TypeId"] = ids;
                    ViewBag.Tar = findTar.ToList();
                    ViewBag.Ewt = findEwt.ToList();
                    return View(stage.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Apply link not in correct format, please try again later.") });
                }
            }
        }

        /*
         * Generating duration for permit based on days, weeks, or months
         */

        public string GetDuration(string option, int duration)
        {
            string result = duration + " " + option;
            return result;
        }

        public FileResult downloadFile(string filePath)
        {
            IFileProvider provider = new PhysicalFileProvider(filePath);
            IFileInfo fileInfo = provider.GetFileInfo(fileName);
            var readStream = fileInfo.CreateReadStream();
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(readStream, mimeType, fileName);
        }

        [HttpGet]
        [Obsolete]
        public IActionResult DownloadTemplate()
        {
            string wwwrootPath = _hostingEnvironment.WebRootPath;
            fileName = @"Table\WellTestTemplate.xlsx";
            FileInfo file = new FileInfo(Path.Combine(wwwrootPath, fileName));
            return downloadFile(wwwrootPath);
        }

        public JsonResult GetPermitDuration(string txtApproveOption, int txtApproveDuration)
        {
            var result = GetDuration(txtApproveOption, txtApproveDuration);

            return Json(result);
        }

        public IActionResult EditApplication(string id)
        {
            var ids = generalClass.DecryptIDs(id);

            if (ids == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Apply link not in correct format, please try again later.") });
            }
            else
            {
                var apps = _context.Applications.Where(x => x.AppId == ids && x.DeletedStatus == false);



                if (apps.Any())
                {
                    var stage = from a in _context.AppTypeStage
                                join t in _context.ApplicationType on a.AppTypeId equals t.AppTypeId
                                join s in _context.ApplicationStage on a.AppStageId equals s.AppStageId
                                where ((a.TypeStageId == apps.FirstOrDefault().AppTypeStageId) && (a.DeleteStatus == false && t.DeleteStatus == false && s.DeleteStatus == false))
                                orderby a.Counter ascending
                                select new AppStage
                                {
                                    TypeStageID = a.TypeStageId,
                                    StageName = s.StageName,
                                    AppType = t.TypeName,
                                    Counter = a.Counter,
                                };

                    var template = _context.TemplateTable.Where(x => x.AppId == ids);

                    ViewBag.Template = template.ToList();

                    ViewData["RefNO"] = apps.FirstOrDefault().AppRefNo;
                    ViewData["TypeName"] = stage.FirstOrDefault().AppType;
                    ViewData["Details"] = stage.FirstOrDefault().AppType + " - " + stage.FirstOrDefault().StageName;

                    return View(apps.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, Apply link not in correct format, please try again later.") });
                }
            }
        }

        public JsonResult DeleteApplication(string AppID)
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

                var apps = _context.Applications.Where(x => x.AppId == appID && x.DeletedStatus == false);

                var companyid = apps.FirstOrDefault().CompanyId;

                var company = _context.Companies.Where(x => x.CompanyId == companyid);

                if (apps.Any())
                {
                    apps.FirstOrDefault().DeletedStatus = true;
                    apps.FirstOrDefault().DeletedAt = DateTime.Now;
                    apps.FirstOrDefault().DeletedBy = _helpersController.getSessionUserID();

                    if (_context.SaveChanges() > 0)
                    {
                        var app = _context.Applications.Where(x => x.AppId == appID);

                        string subject = "Application Deleted with Ref : " + app.FirstOrDefault().AppRefNo;
                        string content = "You have deleted an application with Refrence Number " + app.FirstOrDefault().AppRefNo + " on NUPRC's Well Test portal. Kindly find other details below.";

                        var msg = _helpersController.SaveMessage(appID, app.FirstOrDefault().CompanyId, subject, content);

                        var email = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msg);


                        result = "App Removed";
                    }
                    else
                    {
                        result = "Something went wrong trying to remove this application";
                    }
                }
                else
                {
                    result = "This application was not found. Please try again";
                }
            }

            _helpersController.LogMessages("Delete application status : " + result + " Application ID " + appID, _helpersController.getSessionEmail());

            return Json(result);
        }

        public async Task<JsonResult> UpdateFile(string AppId, IFormFile Files)
        {
            try
            {
                string result = "";
                CancellationToken cancellationToken;
                int saved = 0;

                var appid = generalClass.DecryptIDs(AppId);

                if (appid == 0)
                    result = "Something went wrong. Application reference not in correct format, please try again later.";
                else
                {

                    var apps = _context.Applications.Where(x => x.AppId == appid);
                    var temp = _context.TemplateTable.Where(x => x.AppId == appid);

                    if(temp.Any())
                    {
                        _context.TemplateTable.RemoveRange(temp);
                        _context.SaveChanges();

                        using (var stream = new MemoryStream())
                        {
                            await Files.CopyToAsync(stream, cancellationToken);

                            var templateTable = new List<TemplateTable>();

                            using (var package = new ExcelPackage(stream))
                            {
                                var currentSheet = package.Workbook.Worksheets;
                                var workSheet = currentSheet.First();
                                var rowCount = workSheet.Dimension.End.Row;

                                if (workSheet.Cells[1, 1].Value.ToString().Contains("Block") && workSheet.Cells[1, 10].Value.ToString().Trim() == "Drive Mechanism")
                                {
                                    for (int row = 2; row <= rowCount; row++)
                                    {
                                        var cell1 = workSheet.Cells[row, 1].Value;
                                        var cell2 = workSheet.Cells[row, 2].Value;
                                        var cell3 = workSheet.Cells[row, 3].Value;
                                        var cell4 = workSheet.Cells[row, 4].Value;
                                        var cell5 = workSheet.Cells[row, 5].Value;
                                        var cell6 = workSheet.Cells[row, 6].Value;
                                        var cell7 = workSheet.Cells[row, 7].Value;
                                        var cell8 = workSheet.Cells[row, 8].Value;
                                        var cell9 = workSheet.Cells[row, 9].Value;
                                        var cell10 = workSheet.Cells[row, 10].Value;

                                        if (cell1 == null || cell2 == null || cell3 == null || cell4 == null || cell5 == null || cell6 == null || cell7 == null || cell8 == null || cell9 == null || cell10 == null)
                                            result = "0|One or more excel field(s) was not filled. Please ensure to fill all the fields before uploading the excel sheet.";
                                        else
                                        {
                                            TemplateTable template = new TemplateTable
                                            {
                                                OmlOpl = cell1.ToString(),
                                                FieldName = cell3.ToString(),
                                                Reservior = cell2.ToString(),
                                                WellName = cell4.ToString(),
                                                String = cell5.ToString(),
                                                Terrian = cell6.ToString(),
                                                FluidType = cell9.ToString(),
                                                DriveMechanism = cell10.ToString(),
                                                AppId = appid,
                                                CreatedAt = DateTime.Now
                                            };

                                            if (cell7 != null)
                                                template.StartDate = Convert.ToDateTime(cell6);

                                            if (cell8 != null)
                                                template.EndDate = Convert.ToDateTime(cell7);

                                            _context.TemplateTable.Add(template);
                                            saved += _context.SaveChanges();
                                        }
                                    }

                                    if (saved > 0)
                                    {
                                        result = "1|Saved";
                                        _helpersController.LogMessages("Uploaded Excel updated successfully for with ref Application Reference : " + apps.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                    }
                                    else
                                        result = "0|Application created but something went wrong trying to save the uploaded excel sheet. Please go to my application and update this application.";
                                }
                                else
                                    result = "0|Excel columns are not equal";
                            }
                        }

                    }
                    else
                        result = "Opps... template entries not found, please try again later.";
                }

                _helpersController.LogMessages("Result from updating application details => " + result, _helpersController.getSessionEmail());

                return Json(result);

            }
            catch (Exception ex)
            {
                return Json("Error : Something went wrong. See exception message : " + ex.Message);
            }
        }

        public JsonResult SaveEdit(string AppId, string Contractor, string RigName, string RigType)
        {
            var appid = generalClass.DecryptIDs(AppId);
            var result = "";

            var apps = _context.Applications.Where(x => x.AppId == appid);

            if (appid == 0)
            {
                result = "Something went wrong. Application reference not in correct format, please try again later.";
            }
            else
            {
                if (apps.Any())
                {
                    apps.FirstOrDefault().Contractor = Contractor == "undefined" ? null : Contractor;
                    apps.FirstOrDefault().RigName = RigName == "undefined" ? null : RigName;
                    apps.FirstOrDefault().RigType = RigType == "undefined" ? null : RigType;
                    apps.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Saved";
                    }
                    else
                    {
                        result = "Opps... Something went wrong trying to update this entry, please try again later";
                    }
                }
                else
                {
                    result = "Something went wrong, this application reference was not found or not in a correct format, please try again later.";
                }
            }

            _helpersController.LogMessages("Result from updating application details for RefNo : " + apps.FirstOrDefault().AppRefNo + " => " + result, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
         * Createing Application and Facility
         */
        public async Task<JsonResult> CreateApplication(string Contractor, string RigName, string RigType, int TypeStageId, IFormFile Files)
        {
            try
            {
                string result = "";
                CancellationToken cancellationToken;
                int saved = 0;

                if (Files == null || Files.Length <= 0)
                    result = "0|file is empty please select file";
                else if (!Path.GetExtension(Files.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    result = "0|Not support file extension";
                else
                {
                    //getting company elps id from loacal DB to save to elps facility
                    var company = _context.Companies.Where(x => x.CompanyId == _helpersController.getSessionUserID());

                    var getStage = from ts in _context.AppTypeStage.AsEnumerable()
                                   join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                   join t in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals t.AppTypeId
                                   where ts.TypeStageId == TypeStageId
                                   select new
                                   {
                                       s.StageName,
                                       s.ShortName,
                                       Category = t.TypeName + " Application (" + s.StageName + ")"
                                   };


                    if (DateTime.UtcNow.AddHours(1) > new DateTime(DateTime.Now.Year, 05, 31, 23, 59, 59) && getStage.FirstOrDefault().ShortName.Equals("DST"))
                        result = "0|Deadline for DST proposal exceeded";
                    else
                    {
                        // geting elsp state id for saving facility
                        int elspStateID = generalClass.GetStatesFromCountry(company.FirstOrDefault().StateName);

                        // geting local DB state id for saving
                        var state_id = _context.States.Where(x => x.StateName.Contains(company.FirstOrDefault().StateName)).FirstOrDefault().StateId;

                        if (elspStateID == 0 || state_id == 0)
                            result = "0|Something went wrong trying to create your facility, company state not recognised.";
                        else
                        {
                            _helpersController.LogMessages("Trying to create new facility for " + company.FirstOrDefault().CompanyName, _helpersController.getSessionEmail());

                            var facName = getStage.FirstOrDefault().ShortName + "-" + generalClass.Generate_Receipt_Number();

                            Facilities _facilities = new Facilities()
                            {
                                CompanyId = _helpersController.getSessionUserID(),
                                FacilityName = facName,
                                FacilityAddress = company.FirstOrDefault().Address,
                                State = state_id,
                                Lga = company.FirstOrDefault().City,
                                City = company.FirstOrDefault().City,
                                LandMeters = 0,
                                IsPipeLine = false,
                                IsHighTention = false,
                                IsHighWay = false,
                                CreatedAt = DateTime.Now,
                                DeleteStatus = false,
                                ContactName = "NILL",
                                ContactPhone = "NILL"
                            };

                            // saving facility to local DB
                            _context.Facilities.Add(_facilities);
                            int done = _context.SaveChanges();

                            _helpersController.LogMessages("Done saving new facility " + facName, _helpersController.getSessionEmail());

                            if (done > 0)
                            {
                                int facilityID = _facilities.FacilityId;

                                Facility facility = new Facility()
                                {
                                    Name = _facilities.FacilityName,
                                    CompanyId = company.FirstOrDefault().CompanyElpsId,
                                    StreetAddress = _facilities.FacilityAddress,
                                    City = _facilities.City,
                                    FacilityType = getStage.FirstOrDefault().StageName,
                                    StateId = elspStateID,
                                    DateAdded = DateTime.Now,
                                };

                                // svaing new facility to elps
                                var response2 = _restService.Response("/api/Facility/Add/{email}/{apiHash}", null, "POST", facility);
                                var res = JsonConvert.DeserializeObject<Facility>(response2.Content);

                                // updating new epls facility id with local DB
                                var updateFacility = _context.Facilities.Where(x => x.FacilityId == facilityID).FirstOrDefault();
                                updateFacility.ElpsFacilityId = res.Id;
                                _context.SaveChanges();

                                _helpersController.LogMessages("Done saving new facility to elps " + _facilities.FacilityName, _helpersController.getSessionEmail());

                                result = "0|Facility Created but Application not created. Something went wrong";

                                // creating application on local db
                                Applications _applications = new Applications()
                                {
                                    CompanyId = _helpersController.getSessionUserID(),
                                    FacilityId = facilityID,
                                    AppTypeStageId = TypeStageId,
                                    Status = GeneralClass.PaymentPending, // change to payment pending
                                    DateApplied = DateTime.Now,
                                    AppRefNo = generalClass.Generate_Application_Number(),
                                    IsProposedSubmitted = false,
                                    IsProposedApproved = false,
                                    IsReportSubmitted = false,
                                    IsReportApproved = false,
                                    DeletedStatus = false,
                                    Contractor = Contractor == "undefined" ? null : Contractor,
                                    RigName = RigName == "undefined" ? null : RigName,
                                    RigType = RigType == "undefined" ? null : RigType,
                                };

                                _context.Applications.Add(_applications);
                                int app_saved = _context.SaveChanges();

                                if (app_saved > 0)
                                {
                                    using (var stream = new MemoryStream())
                                    {
                                        await Files.CopyToAsync(stream, cancellationToken);

                                        var templateTable = new List<TemplateTable>();

                                        using (var package = new ExcelPackage(stream))
                                        {
                                            var currentSheet = package.Workbook.Worksheets;
                                            var workSheet = currentSheet.First();
                                            var rowCount = workSheet.Dimension.End.Row;

                                            if (workSheet.Cells[1, 1].Value.ToString().Contains("Block") && workSheet.Cells[1, 10].Value.ToString().Contains("Drive Mechanism"))
                                            {
                                                for (int row = 2; row <= rowCount; row++)
                                                {
                                                    var cell1 = workSheet.Cells[row, 1].Value;
                                                    var cell2 = workSheet.Cells[row, 2].Value;
                                                    var cell3 = workSheet.Cells[row, 3].Value;
                                                    var cell4 = workSheet.Cells[row, 4].Value;
                                                    var cell5 = workSheet.Cells[row, 5].Value;
                                                    var cell6 = workSheet.Cells[row, 6].Value;
                                                    var cell7 = workSheet.Cells[row, 7].Value;
                                                    var cell8 = workSheet.Cells[row, 8].Value;
                                                    var cell9 = workSheet.Cells[row, 9].Value;
                                                    var cell10 = workSheet.Cells[row, 10].Value;



                                                    if (cell1 == null || cell2 == null || cell3 == null || cell4 == null || cell5 == null || cell6 == null || cell7 == null || cell8 == null || cell9 == null || cell10 == null)
                                                        result = "0|One or more excel field(s) was not filled. Please ensure to fill all the fields before uploading the excel sheet.";
                                                    else
                                                    {
                                                        TemplateTable template = new TemplateTable
                                                        {
                                                            OmlOpl = cell1.ToString(),
                                                            FieldName = cell3.ToString(),
                                                            Reservior = cell2.ToString(),
                                                            WellName = cell4.ToString(),
                                                            String = cell5.ToString(),
                                                            Terrian = cell6.ToString(),
                                                            FluidType = cell9.ToString(),
                                                            DriveMechanism = cell10.ToString(),
                                                            AppId = _applications.AppId,
                                                            CreatedAt = DateTime.Now
                                                        };

                                                        if (cell7 != null)
                                                            template.StartDate = Convert.ToDateTime(cell7);

                                                        if (cell8 != null)
                                                            template.EndDate = Convert.ToDateTime(cell8);

                                                        _context.TemplateTable.Add(template);
                                                        saved += _context.SaveChanges();
                                                    }
                                                }

                                                if (saved > 0)
                                                {
                                                    string app_id = _applications.AppId.ToString().Trim();
                                                    result = "1|" + generalClass.Encrypt(app_id);

                                                    string subject = "Application Initiated with Ref : " + _applications.AppRefNo;
                                                    string content = "You have initiated an application (" + getStage.FirstOrDefault().Category + ") with Refrence Number " + _applications.AppRefNo + " on NUPRC's Well Test portal. Kindly find other details below.";

                                                    var msg = _helpersController.SaveMessage(_applications.AppId, company.FirstOrDefault().CompanyId, subject, content);

                                                    var email = await _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msg);

                                                    _helpersController.LogMessages("New Application created successfully for with ref Application Reference : " + _applications.AppRefNo, _helpersController.getSessionEmail());
                                                }
                                                else
                                                    result = "0|Application created but something went wrong trying to save the uploaded excel sheet. Please go to my application and update this application.";
                                            }
                                            else
                                                result = "0|Excel columns are not equal";
                                        }
                                    }
                                }
                                else
                                    result = "0|Something went wrong trying to create your application, please try again.";
                            }
                            else
                                result = "0|Something went wrong trying to create your facility, please try again.";
                        }
                    }                    
                }

                _helpersController.LogMessages(result, _helpersController.getSessionEmail());

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("0|Error : Something went wrong. See exception message : " + ex.Message);
            }
        }

        public async Task<JsonResult> CreateNextApplicationAsync(string txtPermitNo, int LinkId)
        {
            try
            {
                var result = "";

                var getApp = from p in _context.Permits.AsEnumerable()
                             join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                             where p.PermitNo.ToUpper() == txtPermitNo.ToUpper()
                             select new
                             {
                                 a
                             };

                if (getApp.Any())
                {
                    var tables = _context.TemplateTable.Where(x => x.AppId == getApp.FirstOrDefault().a.AppId);

                    Applications applications = new Applications()
                    {
                        FacilityId = getApp.FirstOrDefault().a.FacilityId,
                        CompanyId = getApp.FirstOrDefault().a.CompanyId,
                        PreviousAppId = getApp.FirstOrDefault().a.PreviousAppId,
                        AppTypeStageId = LinkId,
                        AppRefNo = generalClass.Generate_Application_Number(),
                        Status = GeneralClass.PaymentPending,
                        DateApplied = DateTime.Now,
                        IsProposedSubmitted = false,
                        IsProposedApproved = false,
                        IsReportSubmitted = false,
                        IsReportApproved = false,
                        DeletedStatus = false,
                        Contractor = getApp.FirstOrDefault().a.Contractor,
                        RigName = getApp.FirstOrDefault().a.RigName,
                        RigType = getApp.FirstOrDefault().a.RigType,
                        Comment = getApp.FirstOrDefault().a.Comment
                    };

                    _context.Applications.Add(applications);

                    _context.SaveChanges();

                    List<TemplateTable> table = new List<TemplateTable>();

                    foreach (var a in tables.ToList())
                    {
                        table.Add(new TemplateTable
                        {
                            AppId = applications.AppId,
                            DriveMechanism = a.DriveMechanism,
                            FluidType = a.FluidType,
                            String = a.String,
                            WellName = a.WellName,
                            Reservior = a.Reservior,
                            FieldName = a.FieldName,
                            OmlOpl = a.OmlOpl,
                            Terrian = a.Terrian,
                            StartDate = a.StartDate,
                            EndDate = a.EndDate,
                            CreatedAt = DateTime.Now
                        });
                    }

                    _context.TemplateTable.AddRange(table);

                    if (_context.SaveChanges() > 0)
                    {
                        result = "1|" + generalClass.Encrypt(applications.AppId.ToString());

                        var getStage = from ts in _context.AppTypeStage.AsEnumerable()
                                       join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                       join t in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals t.AppTypeId
                                       where ts.TypeStageId == LinkId
                                       select new
                                       {
                                           s.StageName,
                                           s.ShortName,
                                           Category = t.TypeName + " Application (" + s.StageName + ")"
                                       };

                        var company = _context.Companies.Where(x => x.CompanyId == _helpersController.getSessionUserID());

                        string subject = "Application Initiated with Ref : " + applications.AppRefNo;
                        string content = "You have initiated an application (" + getStage.FirstOrDefault().Category + ") with Refrence Number " + applications.AppRefNo + " on NUPRC's Well Test portal. Kindly find other details below.";

                        var msg = _helpersController.SaveMessage(applications.AppId, company.FirstOrDefault().CompanyId, subject, content);

                        var email = await _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msg);

                        _helpersController.LogMessages("New Application created successfully for with ref Application Reference : " + applications.AppRefNo, _helpersController.getSessionEmail());

                    }
                    else
                    {
                        result = "Something went wrong trying to create this application, please try again later.";
                    }
                }
                else
                {
                    result = "This application reference number was not found, please try again later.";
                }

                _helpersController.LogMessages(result, _helpersController.getSessionEmail());

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("Error : Something went wrong. See exception message : " + ex.Message);
            }
        }

        public IActionResult ViewSubmission(string id)
        {
            var app_id = generalClass.DecryptIDs(id);

            if (app_id == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, application reference is incorrect. Please try agin later") });
            }
            else
            {
                List<ViewSubmit> submits = new List<ViewSubmit>();

                var apps = from app in _context.Applications.AsEnumerable()
                           join f in _context.Facilities.AsEnumerable() on app.FacilityId equals f.FacilityId into Facility
                           join c in _context.Companies.AsEnumerable() on app.CompanyId equals c.CompanyId into Company
                           join st in _context.States.AsEnumerable() on Facility.FirstOrDefault().State equals st.StateId into State
                           join ts in _context.AppTypeStage.AsEnumerable() on app.AppTypeStageId equals ts.TypeStageId
                           join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                           join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                           where app.AppId == app_id && app.DeletedStatus == false
                           select new ApplicationDetails
                           {
                               RefNo = app.AppRefNo,
                               AppId = app.AppId,
                               ContractorName = app.Contractor,
                               RigName = app.RigName,
                               RigType = app.RigType,
                               CompanyName = Company.FirstOrDefault().CompanyName,
                               CompanyAddress = Company.FirstOrDefault().Address,
                               CompanyEmail = Company.FirstOrDefault().CompanyEmail,
                               Stage = ty.TypeName + " - " + s.StageName,
                               Status = app.Status,
                               DateApplied = app.DateApplied,
                           };

                var template = _context.TemplateTable.Where(x => x.AppId == app_id);

                if (apps.Any() && template.Any())
                {
                    submits.Add(new ViewSubmit
                    {
                        details = apps.ToList(),
                        templates = template.ToList()
                    });

                    ViewData["AppRef"] = apps.FirstOrDefault().RefNo;

                    _helpersController.LogMessages("Showing new created application for ref no : " + apps.FirstOrDefault().RefNo, _helpersController.getSessionEmail());

                    return View(submits.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, application reference or template reference not in correct format. Please try agin later") });
                }
            }
        }

        public IActionResult ApplicationPayment(string id) // application id
        {
            try
            {
                var app_id = generalClass.DecryptIDs(id);

                if (app_id == 0)
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, request ID is incorrect. Please try agin later") });
                }
                else
                {
                    ViewData["PaymentResponse"] = "false";
                    ViewData["PaymentDescriptioin"] = "";

                    //var countTemplate = _context.TemplateTable.Where(x => x.AppId == app_id).Count();


                    var pay = (from app in _context.Applications.AsEnumerable()
                               join f in _context.Facilities.AsEnumerable() on app.FacilityId equals f.FacilityId into Facility
                               join c in _context.Companies.AsEnumerable() on app.CompanyId equals c.CompanyId into Company
                               join st in _context.States.AsEnumerable() on Facility.FirstOrDefault().State equals st.StateId into State
                               join ts in _context.AppTypeStage.AsEnumerable() on app.AppTypeStageId equals ts.TypeStageId
                               join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                               join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                               where (app.AppId == app_id && (app.DeletedStatus == false && Facility.FirstOrDefault().DeleteStatus == false && Company.FirstOrDefault().DeleteStatus == false && State.FirstOrDefault().DeleteStatus == false && s.DeleteStatus == false))
                               select new PaymentDetailsSubmit()
                               {
                                   AppID = app.AppId,
                                   RefNo = app.AppRefNo,
                                   FacName = Facility.FirstOrDefault().FacilityName,
                                   FacID = Facility.FirstOrDefault().FacilityId,
                                   FacLocation = State.FirstOrDefault().StateName + " (" + Facility.FirstOrDefault().Lga + ")",
                                   Status = app.Status,
                                   AppStage = ty.TypeName + " - " + s.StageName,
                                   ShortName = s.ShortName,
                                   CompanyName = Company.FirstOrDefault().CompanyName,
                                   CompanyId = Company.FirstOrDefault().CompanyId,
                                   Amount = (s.Amount),
                                   TotalAmount = ((s.Amount)) ,
                                   ServiceCharge = s.ServiceCharge
                               });

                    var countTemplate = 0;
                    int fields = 0; 
                    var templates = _context.TemplateTable.Where(x => x.AppId == app_id).ToList();

                    if (pay.FirstOrDefault().ShortName.Equals(GeneralClass.DST))
                        countTemplate = templates.GroupBy(x => x.Reservior).Count();
                    else if (pay.FirstOrDefault().ShortName.Equals(GeneralClass.OCMER)
                        || pay.FirstOrDefault().ShortName.Equals(GeneralClass.RMER))
                        countTemplate = templates.GroupBy(x => x.FieldName).Count();
                    else if (pay.FirstOrDefault().ShortName == GeneralClass.OTAR || pay.FirstOrDefault().ShortName == GeneralClass.RTAR
                        || pay.FirstOrDefault().ShortName == GeneralClass.TARR)
                        countTemplate = 1;


                    if (pay.Count() < 1)
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Payment details was not found for this application, kindly contact support for") });
                    else
                    {
                        var company = _context.Companies.Where(x => x.CompanyId == pay.FirstOrDefault().CompanyId);

                        var trans = _context.Transactions.Where(x => x.AppId == app_id);

                        int amount_due = 0;
                        int totalAmt = 0;
                        string des = "";

                        if (pay.FirstOrDefault().ShortName == GeneralClass.NEWT || pay.FirstOrDefault().ShortName == GeneralClass.EEWT)
                        {
                            des = "Dollar Payment for " + countTemplate + " items " + pay.FirstOrDefault().AppStage;
                            totalAmt = 0;
                        }
                        //else if (pay.FirstOrDefault().ShortName == GeneralClass.OTAR || pay.FirstOrDefault().ShortName == GeneralClass.RTAR)
                        //{
                        //    des = "Naira Payment for " + pay.FirstOrDefault().AppStage;
                        //    totalAmt = (int)(pay.FirstOrDefault().TotalAmount + pay.FirstOrDefault().ServiceCharge) * countTemplate;
                        //}
                        else
                        {
                            des = pay.FirstOrDefault().ShortName == GeneralClass.OCMER || pay.FirstOrDefault().ShortName == GeneralClass.RMER 
                                ? pay.FirstOrDefault().ShortName == GeneralClass.OTAR || pay.FirstOrDefault().ShortName == GeneralClass.TARR
                                || pay.FirstOrDefault().ShortName == GeneralClass.RTAR ? "Naira Payment for " + pay.FirstOrDefault().AppStage : "Naira Payment for " + countTemplate + " Field(s) " + pay.FirstOrDefault().AppStage
                                : "Naira Payment for " + countTemplate + " Reservoir(s) " + pay.FirstOrDefault().AppStage;
                            totalAmt = (int)(pay.FirstOrDefault().TotalAmount + pay.FirstOrDefault().ServiceCharge) * countTemplate;
                        }

                        ViewData["PaymentDescriptioin"] = des;

                        List<PaymentDetailsSubmit> paymentDetails = new List<PaymentDetailsSubmit>();

                        paymentDetails.Add(new PaymentDetailsSubmit
                        {
                            AppID = pay.FirstOrDefault().AppID,
                            RefNo = pay.FirstOrDefault().RefNo,
                            FacName = pay.FirstOrDefault().FacName,
                            FacID = pay.FirstOrDefault().FacID,
                            FacLocation = pay.FirstOrDefault().FacLocation,
                            Status = pay.FirstOrDefault().Status,
                            AppType = pay.FirstOrDefault().AppType,
                            AppStage = pay.FirstOrDefault().AppStage,
                            ShortName = pay.FirstOrDefault().ShortName,
                            CompanyName = pay.FirstOrDefault().CompanyName,
                            Amount = pay.FirstOrDefault().Amount,
                            TotalAmount = totalAmt,
                            ServiceCharge = pay.FirstOrDefault().ServiceCharge,
                            Description = des
                        });

                        //calculating amount due for payment
                        amount_due = (int)(paymentDetails.FirstOrDefault().Amount);

                        // updating transaction
                        if (trans.Any())
                        {
                            trans.FirstOrDefault().AmtPaid = amount_due;
                            trans.FirstOrDefault().TotalAmt = paymentDetails.FirstOrDefault().TotalAmount;
                            trans.FirstOrDefault().ServiceCharge = paymentDetails.FirstOrDefault().ServiceCharge;
                            trans.FirstOrDefault().Description = paymentDetails.FirstOrDefault().Description;
                            _context.SaveChanges();
                        }
                        else
                        {
                            // saving transactions
                            Models.DB.Transactions transactions = new Models.DB.Transactions()
                            {
                                AppId = app_id,
                                TransactionType = "Await",
                                TransactionStatus = GeneralClass.PaymentPending,
                                TransactionDate = DateTime.Now,
                                AmtPaid = amount_due,
                                TotalAmt = paymentDetails.FirstOrDefault().TotalAmount,
                                ServiceCharge = paymentDetails.FirstOrDefault().ServiceCharge,
                                TransRef = generalClass.Generate_Receipt_Number(),
                                Description = paymentDetails.FirstOrDefault().Description
                            };

                            _context.Transactions.Add(transactions);
                            _context.SaveChanges();
                        }


                        var paramData = _restService.parameterData("compemail", generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionEmail")));
                        var response = _restService.Response("/api/company/{compemail}/{email}/{apiHash}", paramData); // GET

                        if (response.ErrorException != null || string.IsNullOrWhiteSpace(response.Content))
                        {
                            return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, this can be a netwrok related or an error. Please try agin later") });
                        }
                        else
                        {
                            // checck company
                            var res = JsonConvert.DeserializeObject<CompanyDetail>(response.Content);

                            _helpersController.LogMessages("Generating payment for application : " + paymentDetails.FirstOrDefault().RefNo, generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionEmail")));

                            var values = new JObject();
                            values.Add("serviceTypeId", _configuration.GetSection("RemitaSplit").GetSection("ServicTypeID").Value.ToString());
                            values.Add("categoryName", _configuration.GetSection("RemitaSplit").GetSection("CategoryName").Value.ToString());
                            values.Add("totalAmount", totalAmt.ToString());
                            values.Add("payerName", pay.FirstOrDefault().CompanyName);
                            values.Add("payerEmail", res.user_Id);
                            values.Add("serviceCharge", "0"); // application.ServiceCharge.ToString("#"));
                            values.Add("amountDue", amount_due.ToString());
                            values.Add("payerPhone", res.contact_Phone);
                            values.Add("orderId", paymentDetails.FirstOrDefault().RefNo);
                            values.Add("returnSuccessUrl", Url.Action("PaymentSuccess", "CompanyApplication", new { id = paymentDetails.FirstOrDefault().RefNo }, Request.Scheme));
                            values.Add("returnFailureUrl", Url.Action("PaymentFail", "CompanyApplication", new { id = paymentDetails.FirstOrDefault().RefNo }, Request.Scheme));
                            values.Add("returnBankPaymentUrl", Url.Action("PaymentBank", "CompanyApplication"));

                            JArray lineItems = new JArray();
                            JObject lineItem1 = new JObject();
                            lineItem1.Add("lineItemsId", "1");
                            lineItem1.Add("beneficiaryName", _configuration.GetSection("RemitaSplit").GetSection("BeneficiaryName").Value.ToString());
                            lineItem1.Add("beneficiaryAccount", _configuration.GetSection("RemitaSplit").GetSection("BeneficiaryAccount").Value.ToString());
                            lineItem1.Add("bankCode", _configuration.GetSection("RemitaSplit").GetSection("BankCode").Value.ToString());
                            lineItem1.Add("beneficiaryAmount", paymentDetails.FirstOrDefault().TotalAmount.ToString());
                            lineItem1.Add("deductFeeFrom", "1");

                            lineItems.Add(lineItem1);
                            values.Add("lineItems", lineItems);

                            JArray appItems = new JArray();
                            JObject appItem1 = new JObject();

                            appItem1.Add("name", _configuration.GetSection("RemitaSplit").GetSection("CategoryName").Value.ToString());
                            appItem1.Add("description", _configuration.GetSection("RemitaSplit").GetSection("Description").Value.ToString() + pay.FirstOrDefault().Description);
                            appItem1.Add("group", _configuration.GetSection("RemitaSplit").GetSection("Group").Value.ToString());
                            appItems.Add(appItem1);
                            values.Add("applicationItems", appItems);

                            JArray customFields = new JArray();
                            JObject customFields1 = new JObject
                            {
                                { "name", "STATE" },
                                { "value", company.FirstOrDefault().StateName + " State" },
                                { "type", "ALL" }
                            };

                            JObject customFields2 = new JObject
                            {
                                { "name", "COMPANY BRANCH" },
                                { "value", company.FirstOrDefault().CompanyName },
                                { "type", "ALL" }
                            };

                            JObject customFields3 = new JObject
                            {
                                { "name", "FACILITY ADDRESS" },
                                { "value", company.FirstOrDefault().Address },
                                { "type", "ALL" }
                            };

                            JObject customFields4 = new JObject
                            {
                                { "name", "Field/Zonal Office" },
                                { "value", "HEAD OFFICE" },
                                { "type", "ALL" }
                            };

                            customFields.Add(customFields1);
                            customFields.Add(customFields2);
                            customFields.Add(customFields3);
                            customFields.Add(customFields4);

                            values.Add("customFields", customFields);

                            _helpersController.LogMessages("Done Generating payment for application : " + paymentDetails.FirstOrDefault().RefNo + " Posting to remita", generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionEmail")));

                            var paramDatas = _restService.parameterData("CompId", res.id.ToString());
                            var resp = _restService.Response("/api/Payments/{CompId}/{email}/{apiHash}", paramDatas, "POST", values);

                            var resz = JsonConvert.DeserializeObject<JObject>(resp.Content);

                            if (resz != null && resp.Content != null)
                            {
                                if (resz.GetValue("rrr").ToString() == null)
                                {
                                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Oops...! Something went wrong, Remita RRR not generated, please try again later. ") });
                                }
                                else
                                {
                                    var apps = _context.Applications.Where(x => x.AppId == app_id && x.DeletedStatus == false).ToList();

                                    if (apps.Any())
                                    {
                                        _helpersController.LogMessages("Payment RRR for application : " + paymentDetails.FirstOrDefault().RefNo + " generated successfully. RRR => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());

                                        var tranz = _context.Transactions.Where(x => x.AppId == app_id);
                                        trans.FirstOrDefault().ElpsTransId = Convert.ToInt32(resz.GetValue("transactionId").ToString());
                                        trans.FirstOrDefault().Rrr = resz.GetValue("rrr").ToString();
                                        _context.SaveChanges();

                                        paymentDetails.FirstOrDefault().rrr = resz.GetValue("rrr").ToString();

                                        var paramDatas2 = _restService.parameterData("CompId", res.id.ToString());

                                        _helpersController.LogMessages("Checking if generated RRR has already been paid. RRR => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());

                                        var rrrCheck = _restService.Response("/api/Payments/BankPaymentInfo/{CompId}/{email}/{apiHash}/" + resz.GetValue("rrr").ToString(), paramDatas, "GET");

                                        var rrrResp = JsonConvert.DeserializeObject<JObject>(rrrCheck.Content);

                                        if (pay.FirstOrDefault().ShortName == GeneralClass.NEWT || pay.FirstOrDefault().ShortName == GeneralClass.EEWT)
                                        {
                                            ViewData["PaymentResponse"] = "true";
                                            _helpersController.LogMessages("Awaiting RRR payment confirmation => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());
                                        }
                                        else if (rrrCheck.Content != null && rrrResp != null && (rrrResp.GetValue("statusMessage").ToString() == "Approved" || rrrResp.GetValue("status")?.ToString() == "01" || rrrResp.GetValue("status")?.ToString() == "00"))
                                        {
                                            var transs = _context.Transactions.Where(x => x.AppId == app_id);
                                            transs.FirstOrDefault().TransactionStatus = GeneralClass.PaymentCompleted;
                                            transs.FirstOrDefault().TransactionType = "Online";
                                            transs.FirstOrDefault().TransactionDate = Convert.ToDateTime(rrrResp.GetValue("transactiontime").ToString());
                                            _context.SaveChanges();

                                            paymentDetails.FirstOrDefault().Status = GeneralClass.PaymentCompleted;

                                            ViewData["PaymentResponse"] = "true";
                                            _helpersController.LogMessages("Paid RRR => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());

                                        }
                                        else if (rrrResp != null && (rrrResp.GetValue("statusMessage").ToString() == "Transaction Pending"))
                                        {
                                            ViewData["PaymentResponse"] = "false";
                                            _helpersController.LogMessages("Transaction Pending RRR => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());
                                        }
                                        else
                                        {
                                            ViewData["PaymentResponse"] = "false";
                                            _helpersController.LogMessages("Not Paid RRR => " + resz.GetValue("rrr").ToString(), _helpersController.getSessionEmail());
                                        }

                                        _helpersController.UpdateElpsApplication(apps.ToList());

                                        return View(paymentDetails);
                                    }
                                    else
                                    {
                                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Oops...! Something went wrong, Application not found or have been deleted.") });
                                    }
                                }
                            }
                            else
                            {
                                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Oops...! Something went wrong, Remita RRR not generated, please try again later.") });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Oops...! Something went wrong, Remita RRR not generated, please try again later. " + ex.InnerException.Message) });
            }
        }

        public async Task<IActionResult> PaymentSuccess(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            var apps = _context.Applications.Where(x => x.AppRefNo == id);

            if (apps.Any())
            {
                apps.FirstOrDefault().Status = GeneralClass.PaymentCompleted;
                apps.FirstOrDefault().UpdatedAt = DateTime.Now;

                if (_context.SaveChanges() > 0)
                {
                    var app = _context.Applications.Where(x => x.AppRefNo == id); 
                    
                    // getting application type and stage 
                    var stage = from ts in _context.AppTypeStage.AsEnumerable()
                                join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                                where ts.TypeStageId == app.FirstOrDefault().AppTypeStageId
                                select new
                                {
                                    StageName = ty.TypeName + " - " + s.StageName
                                };

                    var company = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId);

                    var trans = _context.Transactions.Where(x => x.AppId == app.FirstOrDefault().AppId);

                    trans.FirstOrDefault().TransactionStatus = GeneralClass.PaymentCompleted;
                    trans.FirstOrDefault().TransactionType = "Online";
                    trans.FirstOrDefault().TransactionDate = DateTime.Now;
                    _context.SaveChanges();
                    var app_id = generalClass.Encrypt(app.FirstOrDefault().AppId.ToString());

                    string subject = "Application Payment Made Successfully with Ref : " + app.FirstOrDefault().AppRefNo;
                    string content = "Your payment for application (" + stage.FirstOrDefault().StageName + ") with Refrence Number " + app.FirstOrDefault().AppRefNo + " on NUPRC's Well Test portal has been made successfully. Kindly find other details below.";

                    var msg = _helpersController.SaveMessage(app.FirstOrDefault().AppId, app.FirstOrDefault().CompanyId, subject, content);

                    var email = await _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msg);

                    _helpersController.LogMessages("Result from Payment application for " + id + " Payment Status : " + GeneralClass.PaymentCompleted, _helpersController.getSessionEmail());

                    return RedirectToAction("ApplicationPayment", "CompanyApplication", new { id = app_id });
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong updating your payment status. Go to My Application and continue your application. If the problem continue kindly contact support.") });
                }


            }
            else
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or have been deleted. Kindly contact support.") });
            }
        }

        public async Task<IActionResult> PaymentFail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            var app = _context.Applications.Where(x => x.AppRefNo == id);

            if (app.Any())
            {
                // getting application type and stage 
                var stage = from ts in _context.AppTypeStage.AsEnumerable()
                            join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                            join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                            where ts.TypeStageId == app.FirstOrDefault().AppTypeStageId
                            select new
                            {
                                StageName = ty.TypeName + " - "+ s.StageName
                            };

                var company = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId);

                string subject = "Application Payment FAILED with Ref : " + app.FirstOrDefault().AppRefNo;
                string content = "Your payment for application (" + stage.FirstOrDefault().StageName + ") with Refrence Number " + app.FirstOrDefault().AppRefNo + " on NUPRC's Well Test portal FAILED, Please try again later. Kindly find other details below.";

                var msg = _helpersController.SaveMessage(app.FirstOrDefault().AppId, app.FirstOrDefault().CompanyId, subject, content);

                var email = await _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msg);

                _helpersController.LogMessages("Application payment failed. Application Reference => " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                return View(app.ToList());
            }
            else
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or have been deleted. Kindly contact support.") });
            }
        }

        /*
         * Upload company's document first time
         * 
         * id => encrypted application id
         */

        public IActionResult UploadDocument(string id) // application id
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            int app_id = Convert.ToInt32(generalClass.Decrypt(id));

            var appDetails = from app in _context.Applications.AsEnumerable()
                             join fac in _context.Facilities.AsEnumerable() on app.FacilityId equals fac.FacilityId into facility
                             join com in _context.Companies.AsEnumerable() on app.CompanyId equals com.CompanyId into company
                             join ts in _context.AppTypeStage.AsEnumerable() on app.AppTypeStageId equals ts.TypeStageId
                             join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                             join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                             join aps in _context.AppStageDocuments.AsEnumerable() on s.AppStageId equals aps.AppStageId
                             join doc in _context.ApplicationDocuments.AsEnumerable() on aps.AppDocId equals doc.AppDocId
                             where ((app.DeletedStatus == false && aps.DeleteStatus == false && doc.DeleteStatus == false && s.DeleteStatus == false && facility.FirstOrDefault().DeleteStatus == false && company.FirstOrDefault().DeleteStatus == false) && app.AppId == app_id)
                             select new
                             {
                                 AppID = app.AppId,
                                 AppRef = app.AppRefNo,
                                 FacilityName = facility.FirstOrDefault().FacilityName,
                                 LocalFacilityID = facility.FirstOrDefault().FacilityId,
                                 ElpsFacilityID = facility.FirstOrDefault().ElpsFacilityId,
                                 LocalCompanyID = company.FirstOrDefault().CompanyId,
                                 ElpsCompanyID = company.FirstOrDefault().CompanyElpsId,
                                 AppDocID = doc.AppDocId,
                                 EplsDocTypeID = doc.ElpsDocTypeId,
                                 DocName = doc.DocName,
                                 docType = doc.DocType,
                                 AppStage = ty.TypeName + " - " + s.StageName,

                             };

            List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
            List<MissingDocument> missingDocuments = new List<MissingDocument>();
            List<PresentDocuments> presentDocuments2 = new List<PresentDocuments>();
            List<MissingDocument> missingDocuments2 = new List<MissingDocument>();
            List<BothDocuments> bothDocuments = new List<BothDocuments>();

            if (appDetails.Any())
            {
                ViewData["FacilityName"] = appDetails.FirstOrDefault().FacilityName;
                ViewData["AppStage"] = appDetails.FirstOrDefault().AppStage;
                ViewData["AppID"] = appDetails.FirstOrDefault().AppID;
                ViewData["CompanyElpsID"] = appDetails.FirstOrDefault().ElpsCompanyID;
                ViewData["FacilityElpsID"] = appDetails.FirstOrDefault().ElpsFacilityID;
                ViewData["AppReference"] = appDetails.FirstOrDefault().AppRef;

                List<LpgLicense.Models.Document> companyDoc = generalClass.getCompanyDocuments(appDetails.FirstOrDefault().ElpsCompanyID.ToString());
                List<LpgLicense.Models.FacilityDocument> facilityDoc = generalClass.getFacilityDocuments(appDetails.FirstOrDefault().ElpsFacilityID.ToString());

                if (companyDoc == null || facilityDoc == null)
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("A network related problem. Please connect to a network.") });
                }

                foreach (var appDoc in appDetails.ToList())
                {
                    if (appDoc.docType == "Company")
                    {
                        foreach (var cDoc in companyDoc)
                        {
                            if (cDoc.document_type_id == appDoc.EplsDocTypeID.ToString())
                            {
                                presentDocuments.Add(new PresentDocuments
                                {
                                    Present = true,
                                    FileName = cDoc.fileName,
                                    Source = cDoc.source,
                                    CompElpsDocID = cDoc.id,
                                    DocTypeID = Convert.ToInt32(cDoc.document_type_id),
                                    LocalDocID = appDoc.AppDocID,
                                    DocType = appDoc.docType,
                                    TypeName = cDoc.documentTypeName
                                });
                            }

                        }
                    }
                    else
                    {
                        foreach (var fDoc in facilityDoc)
                        {
                            if (fDoc.Document_Type_Id == appDoc.EplsDocTypeID)
                            {
                                presentDocuments.Add(new PresentDocuments
                                {
                                    Present = true,
                                    FileName = fDoc.Name,
                                    Source = fDoc.Source,
                                    CompElpsDocID = fDoc.Id,
                                    DocTypeID = fDoc.Document_Type_Id,
                                    LocalDocID = appDoc.AppDocID,
                                    DocType = appDoc.docType,
                                    TypeName = appDoc.DocName

                                });
                            }
                        }
                    }
                }

                var result = appDetails.AsEnumerable().Where(x => !presentDocuments.Any(x2 => x2.LocalDocID == x.AppDocID));

                foreach (var r in result)
                {
                    missingDocuments.Add(new MissingDocument
                    {
                        Present = false,
                        DocTypeID = r.EplsDocTypeID,
                        LocalDocID = r.AppDocID,
                        DocType = r.docType,
                        TypeName = r.DocName
                    });
                }

                presentDocuments = presentDocuments.AsEnumerable().GroupBy(x => x.TypeName).Select(c => c.FirstOrDefault()).ToList();


                var allAppsDoc = _context.ApplicationDocuments.AsEnumerable().Where(x => x.DeleteStatus == false);
                var excludedDocs = allAppsDoc.AsEnumerable().Where(x => !appDetails.AsEnumerable().Any(c => c.AppDocID == x.AppDocId)).ToList();

                var appDetails2 = from app in _context.Applications.AsEnumerable()
                                  join fac in _context.Facilities.AsEnumerable() on app.FacilityId equals fac.FacilityId into facility
                                  join com in _context.Companies.AsEnumerable() on app.CompanyId equals com.CompanyId into company
                                  join ts in _context.AppTypeStage.AsEnumerable() on app.AppTypeStageId equals ts.TypeStageId
                                  join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                                  join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                                  join subd in _context.SubmittedDocuments.AsEnumerable() on app.AppId equals subd.AppId
                                  join doc in _context.ApplicationDocuments.AsEnumerable() on subd.AppDocId equals doc.AppDocId
                                  where (app.AppId == app_id && (subd.CompElpsDocId == null || subd.IsAddictional == true))
                                  select new
                                  {
                                      AppID = app.AppId,
                                      AppRef = app.AppRefNo,
                                      FacilityName = facility.FirstOrDefault().FacilityName,
                                      LocalFacilityID = facility.FirstOrDefault().FacilityId,
                                      ElpsFacilityID = facility.FirstOrDefault().ElpsFacilityId,
                                      LocalCompanyID = company.FirstOrDefault().CompanyId,
                                      ElpsCompanyID = company.FirstOrDefault().CompanyElpsId,
                                      AppDocID = doc.AppDocId,
                                      EplsDocTypeID = doc.ElpsDocTypeId,
                                      DocName = doc.DocName,
                                      docType = doc.DocType,
                                      AppStage = ty.TypeName + " - " +s.StageName,
                                      SubmitDocID = subd.SubDocId
                                  };


                if (appDetails2.Any())
                {
                    List<LpgLicense.Models.Document> companyDoc2 = generalClass.getCompanyDocuments(appDetails2.FirstOrDefault().ElpsCompanyID.ToString());
                    List<LpgLicense.Models.FacilityDocument> facilityDoc2 = generalClass.getFacilityDocuments(appDetails2.FirstOrDefault().ElpsFacilityID.ToString());

                    if (companyDoc2 == null || facilityDoc2 == null)
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("A network related problem. Please connect to a network.") });
                    }

                    foreach (var appDoc in appDetails2.ToList())
                    {
                        if (appDoc.docType == "Company")
                        {
                            foreach (var cDoc in companyDoc2)
                            {
                                if (cDoc.document_type_id == appDoc.EplsDocTypeID.ToString())
                                {
                                    presentDocuments2.Add(new PresentDocuments
                                    {
                                        SubmitDocID = appDoc.SubmitDocID,
                                        Present = true,
                                        FileName = cDoc.fileName,
                                        Source = cDoc.source,
                                        CompElpsDocID = cDoc.id,
                                        DocTypeID = Convert.ToInt32(cDoc.document_type_id),
                                        LocalDocID = appDoc.AppDocID,
                                        DocType = appDoc.docType,
                                        TypeName = cDoc.documentTypeName
                                    });
                                }
                            }
                        }
                        else
                        {
                            foreach (var fDoc in facilityDoc2)
                            {
                                if (fDoc.Document_Type_Id == appDoc.EplsDocTypeID)
                                {
                                    presentDocuments2.Add(new PresentDocuments
                                    {
                                        SubmitDocID = appDoc.SubmitDocID,
                                        Present = true,
                                        FileName = fDoc.Name,
                                        Source = fDoc.Source,
                                        CompElpsDocID = fDoc.Id,
                                        DocTypeID = fDoc.Document_Type_Id,
                                        LocalDocID = appDoc.AppDocID,
                                        DocType = appDoc.docType,
                                        TypeName = appDoc.DocName

                                    });
                                }
                            }
                        }
                    }

                    var result2 = appDetails2.AsEnumerable().Where(x => !presentDocuments2.Any(x2 => x2.LocalDocID == x.AppDocID));

                    foreach (var r in result2)
                    {
                        missingDocuments2.Add(new MissingDocument
                        {
                            SubmitDocID = r.SubmitDocID,
                            Present = false,
                            DocTypeID = r.EplsDocTypeID,
                            LocalDocID = r.AppDocID,
                            DocType = r.docType,
                            TypeName = r.DocName
                        });
                    }

                    presentDocuments2 = presentDocuments2.AsEnumerable().GroupBy(x => x.TypeName).Select(c => c.FirstOrDefault()).ToList();
                }

                bothDocuments.Add(new BothDocuments
                {
                    missingDocuments = missingDocuments,
                    presentDocuments = presentDocuments,
                    presentDocuments2 = presentDocuments2,
                    missingDocuments2 = missingDocuments2,
                    AdditionalDoc = excludedDocs
                });
            }

            else
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Trying to find Application(Type, Stage or documents). Kindly contact support.") });
            }

            _helpersController.LogMessages("Displaying company upload documents for " + ViewData["FacilityName"], _helpersController.getSessionEmail());

            return View(bothDocuments.ToList());
        }

        /*
         * Removing already submitted document for the first upload, and not reuploading.
         * 
         */
        public JsonResult RemoveAddictionalDocuments(int AppID, int SubmitDocID)
        {
            var result = "";

            var checkDoc = _context.SubmittedDocuments.Where(x => x.AppId == AppID && x.SubDocId == SubmitDocID);

            if (checkDoc.Any())
            {
                _context.SubmittedDocuments.Remove(checkDoc.FirstOrDefault());

                if (_context.SaveChanges() > 0)
                {
                    result = "Done";
                }
                else
                {
                    result = "Something went wrong trying to remove this document.";
                }
            }
            else
            {
                result = "The document you want to remove was not found in the initial added documents. Please try again.";
            }

            _helpersController.LogMessages("Trying to remove document for uploads.... Status :  " + result + ", Submited Document ID : " + SubmitDocID + ", Application ID : " + AppID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
        * Customer adding more documents for uploads 
        * 
        * 
        */
        public JsonResult AddMoreDocuments(int AppID, int AppDocID)
        {
            string result = "";

            var checkDoc = _context.SubmittedDocuments.Where(x => x.AppDocId == AppDocID && x.AppId == AppID);

            if (checkDoc.Any())
            {
                result = "This document has already been added to your uploads.";
            }
            else
            {
                SubmittedDocuments submitDoc = new SubmittedDocuments()
                {
                    AppId = AppID,
                    AppDocId = AppDocID,
                    IsAddictional = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.SubmittedDocuments.Add(submitDoc);

                if (_context.SaveChanges() > 0)
                {
                    result = "Done";
                }
                else
                {
                    result = "Something went wrong trying to add this documents.";
                }
            }

            _helpersController.LogMessages("Trying to add more document for uploads.... Status :  " + result + ", Document ID : " + AppDocID + ", Application ID : " + AppID, _helpersController.getSessionEmail());

            return Json(result);
        }

        /*
        * Submitting application documents for the first time
        * AppID => encrypted application ID 
        * SubmittedDocuments => a list of documents to be submitted
        */

        public IActionResult SubmitDocuments(int AppID, List<SubmitDoc> SubmittedDocuments)
        {
            var result = "";
            foreach (var item in SubmittedDocuments)
            {
                var check_doc = _context.SubmittedDocuments.Where(x => x.AppId == AppID && x.AppDocId == item.LocalDocID);

                if (check_doc.Count() <= 0)
                {
                    SubmittedDocuments submitDocs = new SubmittedDocuments()
                    {
                        AppId = AppID,
                        AppDocId = item.LocalDocID,
                        CompElpsDocId = item.CompElpsDocID,
                        CreatedAt = DateTime.Now,
                        DeleteStatus = false,
                        DocSource = item.DocSource
                    };
                    _context.SubmittedDocuments.Add(submitDocs);
                }
                else
                {
                    check_doc.FirstOrDefault().AppId = AppID;
                    check_doc.FirstOrDefault().AppDocId = item.LocalDocID;
                    check_doc.FirstOrDefault().CompElpsDocId = item.CompElpsDocID;
                    check_doc.FirstOrDefault().DocSource = item.DocSource;
                    check_doc.FirstOrDefault().UpdatedAt = DateTime.Now;
                }
            }

            int done = _context.SaveChanges();

            if (done > 0)
            {
                //updating application status
                var get = _context.Applications.Where(x => x.AppId == AppID && x.DeletedStatus == false);

                if (get.Any())
                {
                    get.FirstOrDefault().Status = GeneralClass.DocumentsUploaded;
                    get.FirstOrDefault().UpdatedAt = DateTime.Now;

                    int saved = _context.SaveChanges();

                    if (saved > 0)
                    {
                        result = "1|" + generalClass.Encrypt(AppID.ToString());
                        _helpersController.LogMessages("All documents uploaded successfully", _helpersController.getSessionEmail());
                    }
                    else
                    {
                        result = "0|Something went wrong trying to update application status.";
                    }
                }
                else
                {
                    result = "0|Something went wrong trying to update application status.";
                }
            }
            else
            {
                result = "0|Something went wrong trying to save submitted documents.";
            }

            _helpersController.LogMessages(result, _helpersController.getSessionEmail());
            return Json(result);
        }

        /*
         * Getting the list of all document request by staff to be 
         * submitted by the customer.
         * 
         * id => encrypted application id.
         */

        public IActionResult ReUploadDocument(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            int app_id = Convert.ToInt32(generalClass.Decrypt(id));

            var appDetails2 = from app in _context.Applications.AsEnumerable()
                              join dk in _context.MyDesk.AsEnumerable() on app.DeskId equals dk.DeskId into Desk
                              join fac in _context.Facilities.AsEnumerable() on app.FacilityId equals fac.FacilityId into facility
                              join com in _context.Companies.AsEnumerable() on app.CompanyId equals com.CompanyId into company
                              join ts in _context.AppTypeStage.AsEnumerable() on app.AppTypeStageId equals ts.TypeStageId
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              join subd in _context.SubmittedDocuments.AsEnumerable() on app.AppId equals subd.AppId
                              join doc in _context.ApplicationDocuments.AsEnumerable() on subd.AppDocId equals doc.AppDocId
                              where (app.AppId == app_id && app.DeletedStatus == false)
                              select new
                              {
                                  AppID = app.AppId,
                                  AppRef = app.AppRefNo,
                                  FacilityName = facility.FirstOrDefault().FacilityName,
                                  LocalFacilityID = facility.FirstOrDefault().FacilityId,
                                  ElpsFacilityID = facility.FirstOrDefault().ElpsFacilityId,
                                  LocalCompanyID = company.FirstOrDefault().CompanyId,
                                  ElpsCompanyID = company.FirstOrDefault().CompanyElpsId,
                                  AppDocID = doc.AppDocId,
                                  EplsDocTypeID = doc.ElpsDocTypeId,
                                  DocName = doc.DocName,
                                  docType = doc.DocType,
                                  AppStage = ty.TypeName + " - " + s.StageName,
                                  DeskComment = Desk.FirstOrDefault()?.Comment,
                                  SubmitDocID = subd.SubDocId
                              };

            List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
            List<MissingDocument> missingDocuments = new List<MissingDocument>();
            List<BothDocuments> bothDocuments = new List<BothDocuments>();

            if (appDetails2.Any())
            {

                ViewData["FacilityName"] = appDetails2.FirstOrDefault().FacilityName;
                ViewData["AppStage"] = appDetails2.FirstOrDefault().AppStage;
                ViewData["AppID"] = appDetails2.FirstOrDefault().AppID;
                ViewData["CompanyElpsID"] = appDetails2.FirstOrDefault().ElpsCompanyID;
                ViewData["FacilityElpsID"] = appDetails2.FirstOrDefault().ElpsFacilityID;
                ViewData["AppReference"] = appDetails2.FirstOrDefault().AppRef;
                ViewData["DeskComment"] = appDetails2.FirstOrDefault().DeskComment;


                List<LpgLicense.Models.Document> companyDoc2 = generalClass.getCompanyDocuments(appDetails2.FirstOrDefault().ElpsCompanyID.ToString());
                List<LpgLicense.Models.FacilityDocument> facilityDoc2 = generalClass.getFacilityDocuments(appDetails2.FirstOrDefault().ElpsFacilityID.ToString());

                if (companyDoc2 == null || facilityDoc2 == null)
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("A network related problem. Please connect to a network.") });
                }

                foreach (var appDoc in appDetails2.ToList())
                {
                    if (appDoc.docType == "Company")
                    {
                        foreach (var cDoc in companyDoc2)
                        {
                            if (cDoc.document_type_id == appDoc.EplsDocTypeID.ToString())
                            {
                                presentDocuments.Add(new PresentDocuments
                                {
                                    SubmitDocID = appDoc.SubmitDocID,
                                    Present = true,
                                    FileName = cDoc.fileName,
                                    Source = cDoc.source,
                                    CompElpsDocID = cDoc.id,
                                    DocTypeID = Convert.ToInt32(cDoc.document_type_id),
                                    LocalDocID = appDoc.AppDocID,
                                    DocType = appDoc.docType,
                                    TypeName = cDoc.documentTypeName
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var fDoc in facilityDoc2)
                        {
                            if (fDoc.Document_Type_Id == appDoc.EplsDocTypeID)
                            {
                                presentDocuments.Add(new PresentDocuments
                                {
                                    SubmitDocID = appDoc.SubmitDocID,
                                    Present = true,
                                    FileName = fDoc.Name,
                                    Source = fDoc.Source,
                                    CompElpsDocID = fDoc.Id,
                                    DocTypeID = fDoc.Document_Type_Id,
                                    LocalDocID = appDoc.AppDocID,
                                    DocType = appDoc.docType,
                                    TypeName = appDoc.DocName

                                });
                            }
                        }
                    }
                }

                var result2 = appDetails2.AsEnumerable().Where(x => !presentDocuments.Any(x2 => x2.LocalDocID == x.AppDocID));

                foreach (var r in result2)
                {
                    missingDocuments.Add(new MissingDocument
                    {
                        SubmitDocID = r.SubmitDocID,
                        Present = false,
                        DocTypeID = r.EplsDocTypeID,
                        LocalDocID = r.AppDocID,
                        DocType = r.docType,
                        TypeName = r.DocName
                    });
                }

                presentDocuments = presentDocuments.AsEnumerable().GroupBy(x => x.TypeName).Select(c => c.FirstOrDefault()).ToList();

                bothDocuments.Add(new BothDocuments
                {
                    missingDocuments = missingDocuments,
                    presentDocuments = presentDocuments,
                });
            }
            else
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            _helpersController.LogMessages("Displaying company Reupload documents for " + ViewData["FacilityName"], _helpersController.getSessionEmail());

            return View(bothDocuments.ToList());
        }

        /*
         * Re-submting company's documents
         */

        public JsonResult ReSubmitDocuments(int AppID, List<SubmitDoc> ReSubmittedDocuments)
        {
            var result = "";

            foreach (var item in ReSubmittedDocuments)
            {
                var check_doc = _context.SubmittedDocuments.Where(x => x.AppId == AppID && x.AppDocId == item.LocalDocID);

                if (check_doc.Any())
                {
                    check_doc.FirstOrDefault().AppId = AppID;
                    check_doc.FirstOrDefault().AppDocId = item.LocalDocID;
                    check_doc.FirstOrDefault().CompElpsDocId = item.CompElpsDocID;
                    check_doc.FirstOrDefault().UpdatedAt = DateTime.Now;
                    check_doc.FirstOrDefault().DocSource = item.DocSource;
                    check_doc.FirstOrDefault().DeleteStatus = false;
                }
            }

            int done = _context.SaveChanges();

            if (done > 0)
            {
                //updating application status
                var get = _context.Applications.Where(x => x.AppId == AppID && x.DeletedStatus == false);
                var company = _context.Companies.Where(x => x.CompanyId == get.FirstOrDefault().CompanyId && x.DeleteStatus == false);

                if (get.Any() && company.Any())
                {
                    // getting application type and stage 
                    var stage = from ts in _context.AppTypeStage.AsEnumerable()
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              where ts.TypeStageId == get.FirstOrDefault().AppTypeStageId
                              select new
                              {
                                  StageName = ty.TypeName + " - " + s.StageName
                              };


                    // returning application back to officer
                    var desk = _context.MyDesk.Where(x => x.AppId == AppID && x.DeskId == get.FirstOrDefault().DeskId);

                    int staffID = desk.FirstOrDefault().StaffId;

                    get.FirstOrDefault().Status = GeneralClass.Processing;
                    get.FirstOrDefault().UpdatedAt = DateTime.Now;

                    desk.FirstOrDefault().HasWork = false;
                    desk.FirstOrDefault().UpdatedAt = DateTime.Now;

                    int saved = _context.SaveChanges();

                    if (saved > 0)
                    {
                        var user = _context.Staff.Where(x => x.StaffId == staffID).FirstOrDefault();

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(user.RoleId, user.StaffId);


                        _helpersController.SaveHistory(get.FirstOrDefault().AppId, actionFrom, actionTo, "Moved", "Application was re-submitted to staff =>");
                        result = "Resubmitted";

                        // Saving Messages
                        string subject = stage.FirstOrDefault().StageName + " Application Resubmitted with Ref : " + get.FirstOrDefault().AppRefNo;
                        string content = "You have resubmitted your application (" + stage.FirstOrDefault().StageName + ") with Refrence Number " + get.FirstOrDefault().AppRefNo + " for processing on NUPRC Well Test portal. Kindly find other details below.";

                        var emailMsg = _helpersController.SaveMessage(get.FirstOrDefault().AppId, _helpersController.getSessionUserID(), subject, content);

                        var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                        var getApps = _context.Applications.Where(x => x.AppId == get.FirstOrDefault().AppId);

                        string subj = "Application (" + getApps.FirstOrDefault().AppRefNo + ") Resubmitted and Awaiting your response.";
                        string cont = "Application with reference number " + getApps.FirstOrDefault().AppRefNo + " has been resubmitted for processing.";

                        var staff = _context.Staff.Where(x => x.StaffId == staffID);

                        var send = _helpersController.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                    }
                    else
                    {
                        result = "Something went wrong trying to update application status.";
                    }
                }
                else
                {
                    result = "Something went wrong. Application and Company was not found or has been deleted.";
                }
            }
            else
            {
                result = "Something went wrong trying to save re-submitted documents.";
            }

            _helpersController.LogMessages("Resubmit application =>" + result, _helpersController.getSessionEmail());

            return Json(result);
        }

        public IActionResult ReportDocument(string id)
        {
            var appid = generalClass.DecryptIDs(id);

            if (appid == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var app = from a in _context.Applications.AsEnumerable()
                          join fac in _context.Facilities.AsEnumerable() on a.FacilityId equals fac.FacilityId into facility
                          join com in _context.Companies.AsEnumerable() on a.CompanyId equals com.CompanyId into company
                          join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          where ((a.DeletedStatus == false && a.IsProposedApproved == true && s.DeleteStatus == false && facility.FirstOrDefault().DeleteStatus == false && company.FirstOrDefault().DeleteStatus == false) && (a.AppId == appid))
                          select new
                          {
                              AppID = a.AppId,
                              AppRef = a.AppRefNo,
                              FacilityName = facility.FirstOrDefault().FacilityName,
                              LocalFacilityID = facility.FirstOrDefault().FacilityId,
                              ElpsFacilityID = facility.FirstOrDefault().ElpsFacilityId,
                              LocalCompanyID = company.FirstOrDefault().CompanyId,
                              CompanyElpsID = company.FirstOrDefault().CompanyElpsId,
                              AppStage = ty.TypeName + " - " + s.StageName,
                              Status = a.Status,
                              AppDeskID = a.DeskId
                          };

                ViewData["RejectionComment"] = "";

                if (app.Any())
                {
                    var getDocuments = _context.ApplicationDocuments.Where(x => x.DocType == "Facility" && x.DocName.Trim().Contains(GeneralClass.end_of_welltest_fac_doc) && x.DeleteStatus == false);

                    List<PresentDocuments> presentDocuments = new List<PresentDocuments>();
                    List<MissingDocument> missingDocuments = new List<MissingDocument>();
                    List<BothDocuments> bothDocuments = new List<BothDocuments>();

                    if (getDocuments.Any())
                    {
                        ViewData["FacilityElpsID"] = app.FirstOrDefault().ElpsFacilityID;
                        ViewData["CompanyElpsID"] = app.FirstOrDefault().CompanyElpsID;
                        ViewData["FacilityName"] = app.FirstOrDefault().FacilityName;
                        ViewData["AppStage"] = app.FirstOrDefault().AppStage;
                        ViewData["AppID"] = app.FirstOrDefault().AppID;
                        ViewData["AppRefNo"] = app.FirstOrDefault().AppRef;
                        ViewData["AppStatus"] = app.FirstOrDefault().Status;

                        if (app.FirstOrDefault().AppDeskID != 0 || app.FirstOrDefault().AppDeskID != null)
                        {
                            var rejComment = _context.MyDesk.Where(x => x.DeskId == app.FirstOrDefault().AppDeskID);

                            if (rejComment.Any())
                            {
                                ViewData["RejectionComment"] = rejComment.FirstOrDefault().Comment;
                            }
                        }

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

                            _helpersController.LogMessages("Loading facility information and document for report upload : " + app.FirstOrDefault().AppRef, _helpersController.getSessionEmail());

                            _helpersController.LogMessages("Displaying/Viewing more application details.  Reference : " + app.FirstOrDefault().AppRef, _helpersController.getSessionEmail());
                        }

                        return View(bothDocuments.ToList());
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong trying fetch well documents, please try again later.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong trying to get this application, please try again later. If the problem continues, kindly contact support.") });
                }
            }
        }

        public JsonResult SubmitReportDocuments(int AppID, List<SubmitDoc> SubmittedDocuments)
        {
            var result = "";
            foreach (var item in SubmittedDocuments)
            {
                var check_doc = _context.SubmittedDocuments.Where(x => x.AppId == AppID && x.AppDocId == item.LocalDocID);

                if (check_doc.Count() <= 0)
                {
                    SubmittedDocuments submitDocs = new SubmittedDocuments()
                    {
                        AppId = AppID,
                        AppDocId = item.LocalDocID,
                        CompElpsDocId = item.CompElpsDocID,
                        CreatedAt = DateTime.Now,
                        DeleteStatus = false,
                        DocSource = item.DocSource
                    };
                    _context.SubmittedDocuments.Add(submitDocs);
                }
                else
                {
                    check_doc.FirstOrDefault().AppId = AppID;
                    check_doc.FirstOrDefault().AppDocId = item.LocalDocID;
                    check_doc.FirstOrDefault().CompElpsDocId = item.CompElpsDocID;
                    check_doc.FirstOrDefault().DocSource = item.DocSource;
                    check_doc.FirstOrDefault().UpdatedAt = DateTime.Now;
                }
            }

            int done = _context.SaveChanges();

            if (done > 0)
            {
                //check if rejected
                var gets = _context.Applications.Where(x => x.AppId == AppID && x.Status == GeneralClass.Rejected && x.IsReportApproved == false && x.IsProposedApproved == true && x.DeletedStatus == false);

                if (gets.Any())
                {
                    var desk = _context.MyDesk.Where(x => x.AppId == AppID && x.DeskId == gets.FirstOrDefault().DeskId);
                    var company = _context.Companies.Where(x => x.CompanyId == gets.FirstOrDefault().CompanyId && x.DeleteStatus == false);

                    // resubmitting rejected report
                    gets.FirstOrDefault().Status = GeneralClass.Processing;
                    gets.FirstOrDefault().UpdatedAt = DateTime.Now;

                    desk.FirstOrDefault().HasWork = false;
                    desk.FirstOrDefault().UpdatedAt = DateTime.Now;

                    int staffID = desk.FirstOrDefault().StaffId;

                    int saved = _context.SaveChanges();

                    if (saved > 0)
                    {
                        var getStaff = _context.Staff.Where(x => x.StaffId == staffID);
                        var get = _context.Applications.Where(x => x.AppId == AppID);
                        var companys = _context.Companies.Where(x => x.CompanyId == get.FirstOrDefault().CompanyId);

                        // Saving Messages
                        string subject = "Application Report Resubmitted with Ref : " + get.FirstOrDefault().AppRefNo;
                        string content = "You have resubmitted your application report with Refrence Number " + get.FirstOrDefault().AppRefNo + " for processing on NUPRC's Well Test Portal. Kindly find other details below.";
                        var emailMsg = _helpersController.SaveMessage(AppID, _helpersController.getSessionUserID(), subject, content);
                        var sendEmail = _helpersController.SendEmailMessageAsync(companys.FirstOrDefault().CompanyEmail, companys.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                        string n_content = "A rejected application report with reference number : " + get.FirstOrDefault().AppRefNo + " has been resubmitted to you for processsing. Kindly logo to you desk and act on it.";

                        var sendStaffEmail = _helpersController.SendEmailMessageAsync(getStaff.FirstOrDefault().StaffEmail, getStaff.FirstOrDefault().LastName + " " + getStaff.FirstOrDefault().FirstName, subject, n_content, GeneralClass.STAFF_NOTIFY, null);

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                        _helpersController.SaveHistory(AppID, actionFrom, actionTo, "Moved", "Application report was re-submitted to staff.");

                        result = "Resubmitted";
                    }
                    else
                    {
                        result = "Something went wrong trying to resubmit your application report, please try again later.";
                    }
                }
                else
                {
                    // submitting report

                    var getapp = _context.Applications.Where(x => x.AppId == AppID);

                    var stg = from ts in _context.AppTypeStage.AsEnumerable()
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              where ts.TypeStageId == getapp.FirstOrDefault().AppTypeStageId
                              select new
                              {
                                  AppStageId = s.AppStageId
                              };

                    int AppDropStaffID = _helpersController.ApplicationDropStaff(stg.FirstOrDefault().AppStageId, 0, GeneralClass.BEGIN);
                    List<ApplicationProccess> process = _helpersController.GetAppProcess(stg.FirstOrDefault().AppStageId, 0, 0, GeneralClass.BEGIN);

                    if (AppDropStaffID > 0 && process.Any())
                    {
                        var checkdesk = _context.MyDesk.Where(x => x.AppId == AppID && x.ProcessId == process.FirstOrDefault().ProccessId && x.Sort == process.FirstOrDefault().Sort && x.HasWork == false);

                        if (checkdesk.Any())
                        {
                            result = "Sorry, this application is already on a staff desk.";
                        }
                        else
                        {
                            MyDesk desk = new MyDesk()
                            {
                                ProcessId = process.FirstOrDefault().ProccessId,
                                AppId = AppID,
                                StaffId = AppDropStaffID,
                                HasWork = false,
                                CreatedAt = DateTime.Now,
                                HasPushed = false,
                                Sort = process.FirstOrDefault().Sort,
                            };

                            _context.MyDesk.Add(desk);

                            if (_context.SaveChanges() > 0)
                            {
                                var apps = _context.Applications.Where(x => x.AppId == AppID && x.DeletedStatus == false);

                                if (apps.Any())
                                {
                                    apps.FirstOrDefault().UpdatedAt = DateTime.Now;
                                    apps.FirstOrDefault().CurrentDeskId = AppDropStaffID;
                                    apps.FirstOrDefault().Status = GeneralClass.Processing;
                                    apps.FirstOrDefault().IsReportSubmitted = true;

                                    if (_context.SaveChanges() > 0)
                                    {
                                        var Req = _context.Applications.Where(x => x.AppId == AppID && x.DeletedStatus == false);

                                        var company = _context.Companies.Where(x => x.CompanyId == Req.FirstOrDefault().CompanyId && x.DeleteStatus == false);
                                        string subject = "Well Test Application Report submitted with Ref : " + Req.FirstOrDefault().AppRefNo;
                                        string content = "You have submitted your Well Test application report with Refrence Number " + Req.FirstOrDefault().AppRefNo + " for processing on NUPRC's Well Rest Portal. Kindly find other details below.";
                                        var emailMsg = _helpersController.SaveMessage(AppID, _helpersController.getSessionUserID(), subject, content);
                                        var sendEmail = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, emailMsg);

                                        string n_content = "A Well Test application with reference number : " + Req.FirstOrDefault().AppRefNo + " has landed on your desk for processsing. Kindly logo to you desk and act on it.";

                                        var getStaff = _context.Staff.Where(x => x.StaffId == AppDropStaffID);

                                        var sendStaffEmail = _helpersController.SendEmailMessageAsync(getStaff.FirstOrDefault().StaffEmail, getStaff.FirstOrDefault().LastName + " " + getStaff.FirstOrDefault().FirstName, subject, n_content, GeneralClass.STAFF_NOTIFY, null);

                                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                        var actionTo = _helpersController.getActionHistory(getStaff.FirstOrDefault().RoleId, getStaff.FirstOrDefault().StaffId);

                                        _helpersController.SaveHistory(AppID, actionFrom, actionTo, "Moved", "Application landed on staff desk");

                                        result = "Submitted";

                                    }
                                    else
                                    {
                                        result = "Something went wrong trying to update status of your report. Please try again later.";
                                    }
                                }
                                else
                                {
                                    result = "Opps!!! Something went wrong, application not found or not in correct format. Please try again later.";
                                }
                            }
                            else
                            {
                                result = "Something went wrong trying to submit your application. Please try again later.";
                            }
                        }
                    }
                    else
                    {
                        result = "No user or process was found to send application to.";
                    }
                }
            }
            else
            {
                result = "Something went wrong trying to save submitted documents.";
            }

            _helpersController.LogMessages("Result from submitting report : => " + result, _helpersController.getSessionEmail());
            return Json(result);
        }

        /*
        * Submiting an application
        * 
        * id => encrypted application ID
        */
        public IActionResult SubmitApplication(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
            }

            var app_id = Convert.ToInt32(generalClass.Decrypt(id));

            if (app_id <= 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var apps = _context.Applications.Where(x => x.AppId == app_id && x.DeletedStatus == false);

                if (apps.Any())
                {
                    return View(apps.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found. Kindly contact support.") });
                }
            }
        }

        public JsonResult SubmitApp(string AppID)
        {
            string result = "";

            var id = generalClass.DecryptIDs(AppID);

            if (id == 0)
            {
                result = "Application reference not in correct format";
            }
            else
            {
                var apps = _context.Applications.Where(x => x.AppId == id && x.DeletedStatus == false);

                if (apps.Any())
                {

                    var stg = from ts in _context.AppTypeStage.AsEnumerable()
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                              where ts.TypeStageId == apps.FirstOrDefault().AppTypeStageId
                              select new
                              {
                                  AppStageId = s.AppStageId
                              };

                    int AppDropStaffID = _helpersController.ApplicationDropStaff(stg.FirstOrDefault().AppStageId, 1);

                    List<ApplicationProccess> process = _helpersController.GetAppProcess(stg.FirstOrDefault().AppStageId, 0, 1);

                    if (AppDropStaffID > 0 && process.Any())
                    {
                        var staff = _context.Staff.Where(x => x.StaffId == AppDropStaffID).FirstOrDefault();

                        var checkdesk = _context.MyDesk.Where(x => x.AppId == apps.FirstOrDefault().AppId && x.ProcessId == process.FirstOrDefault().ProccessId && x.Sort == process.FirstOrDefault().Sort && x.HasWork == false);

                        if (checkdesk.Any())
                        {
                            result = "Sorry, this application is already on a staff desk.";
                        }
                        else
                        {
                            MyDesk desk = new MyDesk()
                            {
                                ProcessId = process.FirstOrDefault().ProccessId,
                                AppId = apps.FirstOrDefault().AppId,
                                StaffId = AppDropStaffID,
                                HasWork = false,
                                CreatedAt = DateTime.Now,
                                HasPushed = false,
                                Sort = process.FirstOrDefault().Sort,
                            };

                            _context.MyDesk.Add(desk);

                            if (_context.SaveChanges() > 0)
                            {
                                string subj = "Well Test Application (" + apps.FirstOrDefault().AppRefNo + ") is on your desk";
                                string cont = "A Well Test application with reference number : " + apps.FirstOrDefault().AppRefNo + " has been submitted on your desk for processing.";

                                var msg = _helpersController.SendEmailMessageAsync(staff.StaffEmail, staff.LastName + " " + staff.FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                                apps.FirstOrDefault().UpdatedAt = DateTime.Now;
                                apps.FirstOrDefault().CurrentDeskId = AppDropStaffID;
                                apps.FirstOrDefault().DeskId = desk.DeskId;
                                apps.FirstOrDefault().Status = GeneralClass.Processing;
                                apps.FirstOrDefault().IsProposedSubmitted = true;
                                apps.FirstOrDefault().DateSubmitted = DateTime.Now;

                                if (_context.SaveChanges() > 0)
                                {
                                    var user = _context.Staff.Where(x => x.StaffId == AppDropStaffID).FirstOrDefault();

                                    var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                                    var actionTo = _helpersController.getActionHistory(user.RoleId, user.StaffId);

                                    _helpersController.SaveHistory(id, actionFrom, actionTo, "Moved", "Application completed & submitted; landed on staff desk");

                                    var app = _context.Applications.Where(x => x.AppId == id);

                                    var company = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId);

                                    string subject = "Application Submitted with Ref : " + app.FirstOrDefault().AppRefNo;
                                    string content = "You have submitted your application with Refrence Number " + app.FirstOrDefault().AppRefNo + " from processing on NUPRC Well Test portal. Kindly find other details below.";

                                    var msgs = _helpersController.SaveMessage(id, app.FirstOrDefault().CompanyId, subject, content);

                                    var email = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msgs);

                                    _helpersController.LogMessages("Application submitted successfully with reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                                    result = "Submitted";

                                    _helpersController.UpdateElpsApplication(app.ToList());

                                }
                                else
                                {
                                    result = "Something went wrong trying to update your application status";
                                }

                            }
                            else
                            {
                                result = "Something went wrong trying to submit your application.";
                            }
                        }
                    }
                    else
                    {
                        result = "Something went wrong trying to submit your application - Staff not found. Please try again later.";
                    }
                }
                else
                {
                    result = "Application not found. Please try again later.";
                }
            }

            _helpersController.LogMessages("Result from submitting your application : " + result, _helpersController.getSessionEmail());

            return Json(result);
        }

        public JsonResult WithdrawApplication(string AppID)
        {
            string result = "";

            var id = generalClass.DecryptIDs(AppID);

            if (id == 0)
            {
                result = "Application reference not in correct format";
            }
            else
            {
                var getApp = _context.Applications.Where(x => x.AppId == id);

                if(getApp.Any())
                {
                    var deskid = getApp.FirstOrDefault().DeskId;

                    var getDesk = _context.MyDesk.Where(x => x.AppId == getApp.FirstOrDefault().AppId).AsEnumerable();

                    foreach(var d in getDesk.ToList())
                    {
                        d.HasPushed = true;
                        d.HasWork = true;
                    }
                    getApp.FirstOrDefault().Status = GeneralClass.Withdrawn;
                    getApp.FirstOrDefault().UpdatedAt = DateTime.Now;
                    getApp.FirstOrDefault().CurrentDeskId = getDesk.LastOrDefault().StaffId;
                    getApp.FirstOrDefault().DeskId = getDesk.LastOrDefault().DeskId;

                    if (_context.SaveChanges() > 0)
                    {
                        var app = _context.Applications.Where(x => x.AppId == id);

                        var company = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId);

                        _helpersController.UpdateElpsApplication(app.ToList());

                        string subject = "Application Withdrawn with Ref : " + app.FirstOrDefault().AppRefNo;
                        string content = "You have withdrawn your application with Refrence Number " + app.FirstOrDefault().AppRefNo + " from futher processing on Well Test portal. Please note that you can resubmit your application at any time you want.  Kindly find other details below.";

                        var msgs = _helpersController.SaveMessage(id, app.FirstOrDefault().CompanyId, subject, content);
                        var email = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msgs);

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());

                        _helpersController.SaveHistory(id, actionFrom, actionTo, "Withdrawn", "Application was withdrawn from this current staff by the applicant (Company).");

                        _helpersController.LogMessages("Application withdrawn successfully with reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                        result = "Withdrawn";
                    }
                    else
                    {
                        result = "Something went wrong trying to withdraw this application, please try again later.";
                    }
                }
                else
                {
                    result = "Something went wrong, this application was not found. Please try again later.";
                }


            }
            return Json(result);
        }

        public JsonResult ResubmitApplication(string AppID)
        {
            string result = "";

            var id = generalClass.DecryptIDs(AppID);

            if (id == 0)
            {
                result = "Application reference not in correct format";
            }
            else
            {
                var getApp = _context.Applications.Where(x => x.AppId == id);

                if (getApp.Any())
                {
                    var getDesk = _context.MyDesk.Where(x => x.DeskId == getApp.FirstOrDefault().DeskId);

                    getApp.FirstOrDefault().Status = GeneralClass.Processing;
                    getApp.FirstOrDefault().UpdatedAt = DateTime.Now;
                   
                    getDesk.FirstOrDefault().HasWork = false;
                    getDesk.FirstOrDefault().HasPushed = false;
                    getDesk.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        var app = _context.Applications.Where(x => x.AppId == id);

                        var company = _context.Companies.Where(x => x.CompanyId == app.FirstOrDefault().CompanyId);

                        _helpersController.UpdateElpsApplication(app.ToList());

                        string subject = "Application Resubmitted with Ref : " + app.FirstOrDefault().AppRefNo;
                        string content = "You have Resubmitted your application with Refrence Number " + app.FirstOrDefault().AppRefNo + " for futher processing on NUPRC Well Test portal. Please note that you can resubmit your application at any time you want.  Kindly find other details below.";

                        var msgs = _helpersController.SaveMessage(id, app.FirstOrDefault().CompanyId, subject, content);
                        var email = _helpersController.SendEmailMessageAsync(company.FirstOrDefault().CompanyEmail, company.FirstOrDefault().CompanyName, subject, content, GeneralClass.COMPANY_NOTIFY, msgs);


                        string subj = "Application (" + app.FirstOrDefault().AppRefNo + ") Resubmitted Back to You.";
                        string cont = "Application with reference number " + app.FirstOrDefault().AppRefNo + " has been resubmitted back to you for processing.";
                        var staff = _context.Staff.Where(x => x.StaffId == app.FirstOrDefault().CurrentDeskId);

                        var send = _helpersController.SendEmailMessageAsync(staff.FirstOrDefault().StaffEmail, staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName, subj, cont, GeneralClass.STAFF_NOTIFY, null);

                        var actionFrom = _helpersController.getActionHistory(_helpersController.getSessionRoleID(), _helpersController.getSessionUserID());
                        var actionTo = _helpersController.getActionHistory(staff.FirstOrDefault().RoleId, staff.FirstOrDefault().StaffId);

                        _helpersController.SaveHistory(id, actionFrom, actionTo, "Moved", "Application was Resubmitted for futher processing");

                        _helpersController.LogMessages("Application Resubmitted successfully with reference : " + app.FirstOrDefault().AppRefNo, _helpersController.getSessionEmail());

                        result = "Resubmitted";
                    }
                    else
                    {
                        result = "Something went wrong trying to Resubmit this application, please try again later.";
                    }
                }
                else
                {
                    result = "Something went wrong, this application was not found. Please try again later.";
                }


            }
            return Json(result);
        }
    }
}
