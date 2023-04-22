using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DST.Models.DB;
using Newtonsoft.Json;
using DST.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using DST.Controllers.Configurations;
using LpgLicense.Models;
using RestSharp;
using Microsoft.AspNetCore.Authorization;
using static DST.Models.GeneralModel;
using DST.Controllers.Authentications;

namespace DST.Controllers.Company
{
   
    public class CompaniesController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();
        public static List<ApplicationType> _applicationType;


        public CompaniesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
            var appType = _context.ApplicationType.Where(a => a.DeleteStatus == false);
            _applicationType = appType.ToList();
        }


       


        [Authorize(Roles = "COMPANY")]
        public IActionResult Index()
        {
            var paramData = _restService.parameterData("compemail", _helpersController.getSessionEmail());
            var response = _restService.Response("/api/company/{compemail}/{email}/{apiHash}", paramData); // GET

            if (response.ErrorException != null || string.IsNullOrWhiteSpace(response.Content))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong, this can be a netwrok related or an error. Please try agin later") });
            }
            else
            {
                // checck company
                var res = JsonConvert.DeserializeObject<CompanyDetail>(response.Content);

                if (string.IsNullOrWhiteSpace(res.contact_FirstName) || string.IsNullOrWhiteSpace(res.contact_LastName.ToString()) || (res.registered_Address_Id == null && res.operational_Address_Id == null))
                {
                    return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please complete company's profile, addresses before proceeding.") });
                }

                else
                {
                    // checking for directors

                    var paramData2 = _restService.parameterData("CompId", generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionElpsID")));
                    var response2 = _restService.Response("/api/Directors/{CompId}/{email}/{apiHash}", paramData2);

                    if (response2.ErrorException != null || string.IsNullOrWhiteSpace(response2.Content))
                    {
                        return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please complete company's profile, addresses before proceeding.") });
                    }
                    else
                    {
                        var res2 = SimpleJson.DeserializeObject<List<Directors>>(response2.Content);

                        if (res2.Any())
                        {
                            // saving company code 

                            string address = "";

                            if (res.registered_Address_Id != null || res.registered_Address_Id != "0")
                            {
                                address = res.registered_Address_Id;
                            }
                            else if (res.operational_Address_Id != null || res.operational_Address_Id != "0")
                            {
                                address = res.operational_Address_Id;
                            }

                            var paramDatas = _restService.parameterData("Id", address);
                            var responses = _restService.Response("/api/Address/ById/{Id}/{email}/{apiHash}", paramDatas); // GET

                            if (responses != null)
                            {
                                var getCom = _context.Companies.Where(x => x.CompanyId ==  _helpersController.getSessionUserID());

                                if (getCom.Any())
                                {
                                    var CODE = getCom.FirstOrDefault().CompanyId;
                                        var com = JsonConvert.DeserializeObject<Address>(responses.Content);

                                    if (com != null)
                                    {
                                        var code = generalClass.GetStateShortName(com.stateName.ToUpper(), "00" + CODE);

                                        getCom.FirstOrDefault().IdentificationCode = code;
                                        getCom.FirstOrDefault().Address = com.address_1;
                                        getCom.FirstOrDefault().City = com.city;
                                        getCom.FirstOrDefault().StateName = com.stateName.ToUpper();
                                        getCom.FirstOrDefault().UpdatedAt = DateTime.Now;

                                        if (_context.SaveChanges() > 0)
                                        {
                                            return RedirectToAction("Dashboard", "Companies");
                                        }
                                        else
                                        {
                                            return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please complete company's profile, addresses before proceeding.") });
                                        }
                                    }
                                    else
                                    {
                                        return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please complete company's profile, addresses before proceeding.") });
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please complete company's profile, addresses before proceeding.") });
                                }
                            }
                            else
                            {
                                return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Company address not found, please add a registered address or operational address.") });
                            }
                        }
                        else
                        {
                            return RedirectToAction("CompanyInformation", "Companies", new { message = generalClass.Encrypt("Please add or update director's information before proceeding.") });
                        }
                    }
                }
            }
        }






        [AllowAnonymous]
        [Authorize(Roles = "COMPANY")]
        public IActionResult LegalStuff(string id)
        {
            if (id == null || string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Error", "Home", new { message = generalClass.Encrypt("Opps... No company was found or passed") });
            }
            else
            {
                ViewData["cid"] = id;
                return View();
            }
        }




        [AllowAnonymous]
        [Authorize(Roles = "COMPANY")]
        public IActionResult AcceptLegal(string id)
        {
            if (id == null || string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Error", "Home", new { message = generalClass.Encrypt("Opps... This company was not found and cannot accept tearms and condictions. Please try again.") });
            }
            else
            {
                int cmid = Convert.ToInt32(generalClass.Decrypt(id));

                var company = (from c in _context.Companies where c.CompanyId == cmid select c);

                if (company.Any())
                {
                    company.FirstOrDefault().IsFirstTime = false;
                    company.FirstOrDefault().UpdatedAt = DateTime.Now;

                    string content = "Hello " + company.FirstOrDefault().CompanyName + ", Welcome to Drill Stem Test (DST) Portal.";

                    if (_context.SaveChanges() > 0)
                    {
                        _helpersController.LogMessages("Company accepted legal condictions", company.FirstOrDefault().CompanyEmail);
                        return RedirectToAction("UserAuth", "Auth", new { email = company.FirstOrDefault().CompanyEmail });
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { message = generalClass.Encrypt("Opps... Something went wrong trying to accept your tearms and condictions. Please try again.") });
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { message = generalClass.Encrypt("Opps... This company was not found and cannot accept tearms and condictions. Please try again.") });
                }
            }
        }




        [Authorize(Roles = "COMPANY")]
        public IActionResult CompanyInformation(string message = null)
        {
            var msg = "";

            if (message != null)
            {
                msg = generalClass.Decrypt(message);
            }

            ViewData["Message"] = msg;

            return View();
        }



        public IActionResult MyApplications()
        {
            int companyID = _helpersController.getSessionUserID();

            var myApps = from a in _context.Applications
                         join f in _context.Facilities on a.FacilityId equals f.FacilityId
                         join c in _context.Companies on a.CompanyId equals c.CompanyId
                         join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                         join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                         join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                         orderby a.AppId descending
                         where ((a.DeletedStatus == false) && (c.CompanyId == companyID && c.ActiveStatus == true && c.DeleteStatus == false) && (f.DeleteStatus == false) && (s.DeleteStatus == false))
                         select new MyApps
                         {
                             AppID = a.AppId,
                             RefNo = a.AppRefNo,
                             FacilityName = f.FacilityName,
                             FacilityAddress = f.FacilityAddress,
                             FacilityID = f.FacilityId,
                             Category =  ty.TypeName + " - " +s.StageName ,
                             Status = a.Status,
                             DateApplied = a.DateApplied,
                             DateSubmitted = a.DateSubmitted,
                             IsProposedSubmitted = a.IsProposedSubmitted,
                             IsProposedApproved = a.IsProposedApproved,
                             IsReportApproved = a.IsReportApproved,
                             IsReportSubmitted = a.IsReportSubmitted,
                             CompanyName = c.CompanyName,
                             CompanyID = c.CompanyId,
                             DeletedStatus = a.DeletedStatus,
                         };

            _helpersController.LogMessages("Displaying company's application list...", _helpersController.getSessionEmail());

            return View(myApps.ToList());
        }





        [Authorize(Roles = "COMPANY")]
        public IActionResult Dashboard()
        {
            var appType = _context.ApplicationType.Where(a => a.DeleteStatus == false);
            _applicationType = appType.ToList();

            var companyID =  _helpersController.getSessionUserID();

            var messages = _context.Messages.Where(x => x.CompanyId == companyID).OrderByDescending(x => x.MessageId).Take(10);

            var all_apps = from a in _context.Applications.AsEnumerable()
                           join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                           where c.CompanyId == companyID && (a.DeletedStatus == false)
                           select new
                           {
                               a
                           };

            var unfinished = _context.Applications.Where(x => x.CompanyId == companyID && x.IsProposedSubmitted == false && (x.DeletedStatus == false)).Count();

            var all_permits = from p in _context.Permits.AsEnumerable()
                              join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                              join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                              where c.CompanyId == companyID && a.DeletedStatus == false
                              select new
                              {
                                  p
                              };

            int expiryCount = 0;

            var permits = from p in all_permits
                          select new Models.DB.Permits
                          {
                              PermitNo = p.p.PermitNo,
                              PermitId = p.p.PermitId
                          };


            foreach (var pr in all_permits.ToList())
            {
                var check = pr.p.ExpireDate.AddDays(-10);
                var now = DateTime.Now;

                if (check <= now && pr.p.ExpireDate > now)
                {
                    expiryCount++;
                }
                else
                {
                    // Do nothin as the permit is still valid
                }
            }

            var notify = _context.Applications.Where(x => x.CompanyId == companyID && (x.Status != GeneralClass.Approved && x.Status != GeneralClass.Processing && x.Status != GeneralClass.DISAPPROVE) && x.DeletedStatus == false).Count();
            
            var uploadReport = _context.Applications.Where(x => x.CompanyId == companyID && (x.IsProposedApproved == true && x.IsReportSubmitted == false) && x.DeletedStatus == false).Count();

            var schedule = from a in _context.Applications.AsEnumerable()
                           join s in _context.Schdules.AsEnumerable() on a.AppId equals s.AppId
                           where  a.CompanyId == companyID && a.DeletedStatus == false && s.SupervisorApprove == 1 && (s.CustomerAccept == null || s.CustomerAccept == 0) && s.DeletedStatus == false && s.SchduleDate > DateTime.Now
                           select a;

           

            ViewData["Notify"] = notify;
            ViewData["UploadReport"] = uploadReport;
            ViewData["ScheduleNotify"] = schedule.Count();
            ViewData["Unfinished"] = unfinished;
            ViewData["AllCompanyApps"] = all_apps.Count();
            ViewData["AllCompanyPermit"] = all_permits.Count();
            ViewBag.Permits = permits.ToList();
            ViewData["AllProcessingApps"] = all_apps.Where(x => x.a.Status == GeneralClass.Processing).Count();
            ViewData["RejectedApps"] = all_apps.Where(x => x.a.Status == GeneralClass.Rejected).Count();
            ViewData["AppExpiring"] = expiryCount;

            _helpersController.LogMessages("Displaying company's dashboard...", _helpersController.getSessionEmail());

            return View(messages.ToList());
        }



        /*
         * Getting company profile information
         */

        public IActionResult GetCompanyProfile(string CompanyId)
        {
            var paramData = _restService.parameterData("id", CompanyId);
            var result = generalClass.RestResult("company/{id}", "GET", paramData, null); // GET

            //generalClass.LogMessage("Displaying company's profile.", CompanyEmail);
            return Json(result.Value);
        }



        /*
         * Updating full a company's profile
         */

        public JsonResult UpdateCompanyProfile(CompanyDetail companyDetail)
        {
            string results = "";

            bool emailChange = false;

            var company = _context.Companies.Where(x => x.CompanyElpsId == companyDetail.id);

            if (company.Any())
            {
                if (company.FirstOrDefault().CompanyEmail == companyDetail.user_Id)
                {
                    emailChange = false;
                }
                else
                {
                    emailChange = true;
                }
            }

            CompanyChangeModel companyChange = new CompanyChangeModel()
            {
                Name = companyDetail.name,
                RC_Number = companyDetail.rC_Number,
                Business_Type = companyDetail.business_Type,
                emailChange = emailChange,
                CompanyId = companyDetail.id,
                NewEmail = companyDetail.user_Id
            };

            CompanyInformationModel companyDetails = new CompanyInformationModel();
            companyDetails.company = companyDetail;
            var result = generalClass.RestResult("company/Edit", "PUT", null, companyDetails, "Company Updated"); // PUT
            var result2 = generalClass.RestResult("Accounts/ChangeEmail", "POST", null, companyChange, "Company Updated"); // PUT

            if (result2.Value.ToString() == "Company Updated")
            {
                if (company.Any())
                {
                    company.FirstOrDefault().CompanyName = companyDetail.name;
                    company.FirstOrDefault().CompanyEmail = companyDetail.user_Id;
                    company.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        results = "Company Updated";
                    }
                    else
                    {
                        results = "Company profile successfully updated on ELPS but not on Drill Stem Test (DST) Portal. Please try again later.";
                    }
                }
                else
                {
                    results = "Company profile updated on ELPS but not on Drill Stem Test (DST) Portal. Please try again later.";
                }
            }
            else
            {
                results = result.Value.ToString();
            }

            _helpersController.LogMessages("Updating company's profile... Result : " + result, _helpersController.getSessionEmail());

            return Json(results);
        }



        /*
         * Createing a company's address
         */
        public IActionResult CreateCompanyAddress(string CompanyId, List<Address> address)
        {
            var paramData = _restService.parameterData("CompId", CompanyId);
            var result = generalClass.RestResult("Address/{CompId}", "POST", paramData, address, "Created Address"); // POST

            _helpersController.LogMessages("Creating new company's address...." + result, _helpersController.getSessionEmail());
            return Json(result.Value);
        }


        /*
         * Updating a company's address
         */
        public IActionResult UpdateCompanyAddress(List<Address> address)
        {
            var result = generalClass.RestResult("Address", "PUT", null, address, "Address Updated"); // PUT
            _helpersController.LogMessages("Updating company's address...", _helpersController.getSessionEmail());
            return Json(result.Value);
        }



        /*
         * Getting Directors Names
         */
        public IActionResult GetDirectorsNames(string CompanyId)
        {
            var paramData = _restService.parameterData("CompId", CompanyId);
            var result = generalClass.RestResult("Directors/{CompId}", "GET", paramData, null, null); // GET
            _helpersController.LogMessages("Getting company's directors name", _helpersController.getSessionEmail());
            return Json(result.Value);
        }


        /*
         * Creating company directors
         */
        public IActionResult CreateCompanyDirectors(string CompanyId, List<Directors> directors)
        {
            var paramData = _restService.parameterData("CompId", CompanyId);
            var result = generalClass.RestResult("Directors/{CompId}", "POST", paramData, directors, "Director Created"); // POST
            _helpersController.LogMessages("Creating company's directors...", _helpersController.getSessionEmail());
            return Json(result.Value);
        }


        /*
         * Retriving a list of a particular company directors
         */
        public IActionResult GetDirectors(string DirectorID)
        {
            var paramData = _restService.parameterData("Id", DirectorID);
            var result = generalClass.RestResult("Directors/ById/{Id}", "GET", paramData, null, null); // GET
            _helpersController.LogMessages("Getting single company's director details...", _helpersController.getSessionEmail());
            return Json(result.Value);
        }


        /*
         * Updating company's director information
         */

        public IActionResult UpdateCompanyDirectors(List<Directors> directors)
        {
            var result = generalClass.RestResult("Directors", "PUT", null, directors, "Director Updated"); // PUT
            _helpersController.LogMessages("Updating company's director...", _helpersController.getSessionEmail());
            return Json(result.Value);
        }



        /*
         *  view full company information for Admin.
         */
     
        public IActionResult FullCompanyProfile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }

            var company = generalClass.Decrypt(id);

            if (company == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Application not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var com = _context.Companies.Where(x => x.CompanyEmail == company).FirstOrDefault();

                CompanyDetail companyModels = new CompanyDetail();

                 var paramData = _restService.parameterData("id", com.CompanyElpsId.ToString());
                var response = _restService.Response("/api/company/{id}/{email}/{apiHash}", paramData, "GET", null); // GET

                if (response.IsSuccessful == false)
                {
                    var comp = _context.Companies.Where(x => x.CompanyEmail == company).FirstOrDefault();
                    companyModels.user_Id = comp.CompanyEmail;
                    companyModels.name = comp.CompanyName;
                    companyModels.id = comp.CompanyElpsId;

                    ViewData["CompanyName"] = null;
                }
                else
                {
                    companyModels = JsonConvert.DeserializeObject<CompanyDetail>(response.Content);
                    ViewData["CompanyName"] = companyModels.name;
                }

                _helpersController.LogMessages("Displaying full company's profile for admin...", _helpersController.getSessionEmail());

                return View(companyModels);
            }
        }




        [Authorize(Roles = "COMPANY")]
        public IActionResult MySchedule()
        {
            int companyID = _helpersController.getSessionUserID();

            var sch = from sh in _context.Schdules
                      join a in _context.Applications on sh.AppId equals a.AppId
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                      join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                      join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId

                      where c.CompanyId == companyID && sh.SupervisorApprove == 1 && sh.DeletedStatus == false
                      select new SchdulesList
                      {
                          Schedule = sh,
                          Stage = s,
                          Apps = a
                      };

            _helpersController.LogMessages("Displaying company's schedule...", _helpersController.getSessionEmail());

            return View(sch.ToList());
        }



        /*
         * Getting notification count for company
         */
       
        public JsonResult GetScheduleCount()
        {
            var getSch = from sh in _context.Schdules
                         join a in _context.Applications on sh.AppId equals a.AppId
                         where a.CompanyId == _helpersController.getSessionUserID() && (sh.CustomerAccept == 0 || sh.CustomerAccept == null) && a.DeletedStatus == false && sh.DeletedStatus == false && sh.SupervisorApprove == 1 && sh.SchduleDate > DateTime.Now
                         select new
                         {
                             sh
                         };

            return Json(getSch.Count());
        }




        /*
         * Viewing a list of all meaages for a company
         * 
         * id => encrypted company id
         */
        [Authorize(Roles = "COMPANY")]
        public IActionResult Messages()
        {
            var messages = _context.Messages.Where(x => x.CompanyId ==  _helpersController.getSessionUserID()).OrderByDescending(x => x.MessageId);
            _helpersController.LogMessages("Displaying company's messages as list...", _helpersController.getSessionEmail());
            return View(messages.ToList());
        }



        /*
         * Viewing a single message for a company 
         * 
         * id  => encrypted company id,
         * option => encrypted message id
         */
        public IActionResult Message(string id, string option)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(option))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Message not found or not in correct format. Kindly contact support.") });
            }

            int comp_id = 0;
            int msg_id = 0;

            var c_id = generalClass.Decrypt(id);
            var m_id = generalClass.Decrypt(option);

            if (c_id == "Error" || m_id == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Message not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                comp_id = Convert.ToInt32(c_id);
                msg_id = Convert.ToInt32(m_id);

                var msg = _context.Messages.Where(x => x.MessageId == msg_id);

                msg.FirstOrDefault().Seen = true;
                _context.SaveChanges();

                var message = from m in _context.Messages.AsEnumerable()
                              join a in _context.Applications.AsEnumerable() on m.AppId equals a.AppId
                              join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                              join c in _context.Companies.AsEnumerable() on f.CompanyId equals c.CompanyId into comp
                              join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                              join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                              join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId

                              join tr in _context.Transactions.AsEnumerable() on a.AppId equals tr.AppId into trans
                              from t in trans.DefaultIfEmpty()
                              where m.CompanyId == comp_id && m.MessageId == msg_id
                              select new AppMessage
                              {
                                  Subject = m.Subject,
                                  Content = m.MesgContent,
                                  RefNo = a.AppRefNo,
                                  FacilityDetails = f.FacilityName + " (" + f.FacilityAddress + ")",
                                  Stage = ty.TypeName + " - " + s.StageName,
                                  Status = a.Status,
                                  TotalAmount = t?.TotalAmt,
                                  Seen = m.Seen,
                                  CompanyDetails = comp.FirstOrDefault().CompanyName,
                                  DateApplied = a.DateApplied.ToLongDateString(),
                                  DateSubmitted = a.DateSubmitted.ToString(),
                              };

                ViewData["MessageTitle"] = "";

                if (message.Any())
                {
                    ViewData["MessageTitle"] = message.FirstOrDefault().Subject;
                }


                _helpersController.LogMessages("Displaying single company's message...", _helpersController.getSessionEmail());

                return View(message.ToList());
            }
        }



        [Authorize(Roles = "COMPANY")]
        public IActionResult MyPermits()
        {
            int companyID = Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionUserID)).Trim());

            var myPermits = from p in _context.Permits
                            join a in _context.Applications on p.AppId equals a.AppId
                            join c in _context.Companies on a.CompanyId equals c.CompanyId
                            join f in _context.Facilities on a.FacilityId equals f.FacilityId
                            join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                            join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                            join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId

                            where (c.CompanyId == companyID)
                            select new MyPermit
                            {
                                PermitID = p.PermitId,
                                AppId = p.AppId,
                                PermitNo = p.PermitNo,
                                RefNo = a.AppRefNo,
                                IssuedDate = p.IssuedDate,
                                ExpireDate = p.ExpireDate,
                                isPrinted = p.Printed,
                                CompanyName = c.CompanyName,
                                CompanyID = c.CompanyId,
                                CompanyEmail = c.CompanyEmail,
                                WellDetails = f.FacilityName,
                                StageName = ty.TypeName + " - "+ s.StageName,
                                ShortName = s.ShortName

                            };

            _helpersController.LogMessages("Displaying company's permits/license list...", _helpersController.getSessionEmail());

            return View(myPermits.ToList());
        }





        [Authorize(Roles = "COMPANY")]
        public IActionResult DocumentsLibrary()
        {
            List<PresentDocuments> presentDocuments = new List<PresentDocuments>();

            var doc = _context.ApplicationDocuments;

            var getCom = _context.Companies.Where(x => x.CompanyId ==  _helpersController.getSessionUserID());

            if (getCom.Any())
            {
                ViewData["CompanyName"] = "All available documents for " + getCom.FirstOrDefault().CompanyName;

                List<LpgLicense.Models.Document> companyDoc = generalClass.getCompanyDocuments(getCom.FirstOrDefault().CompanyElpsId.ToString());

                if (companyDoc == null)
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("A network related problem. Please connect to a network.") });
                }
                else
                {
                    foreach (var appDoc in doc.ToList())
                    {
                        if (appDoc.DocType == "Company")
                        {
                            foreach (var cDoc in companyDoc)
                            {
                                if (cDoc.document_type_id == appDoc.ElpsDocTypeId.ToString())
                                {
                                    presentDocuments.Add(new PresentDocuments
                                    {
                                        Present = true,
                                        FileName = cDoc.fileName,
                                        Source = cDoc.source,
                                        CompElpsDocID = cDoc.id,
                                        DocTypeID = Convert.ToInt32(cDoc.document_type_id),
                                        LocalDocID = appDoc.AppDocId,
                                        DocType = appDoc.DocType,
                                        TypeName = cDoc.documentTypeName
                                    });
                                }
                            }
                        }

                        presentDocuments = presentDocuments.GroupBy(x => x.TypeName).Select(c => c.FirstOrDefault()).OrderBy(x => x.TypeName).ToList();

                    }
                }
            }
            else
            {
                ViewData["CompanyName"] = "No available documents for ";
                //return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("No document found for this company") });
            }

            _helpersController.LogMessages("Displaying company's documents...", _helpersController.getSessionEmail());

            return View(presentDocuments);
        }


    }

   
}
