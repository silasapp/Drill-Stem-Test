using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using DST.Helpers;
using DST.Models.DB;
using DST.Controllers.Configurations;
using DST.RecycleModels;
using static DST.Models.GeneralModel;

namespace DST.Controllers.Recyclebin
{
    
    public class Recyclebin : Controller
    {
        private readonly DST_DBContext _context;
        GeneralClass generalClass = new GeneralClass();
        HelpersController helpers;
        IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;

        public Recyclebin(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            helpers = new HelpersController(_context, _configuration, _httpContextAccessor);
        }


        /*
         * Getting deleted application documents
         */
       
        public IActionResult DeletedApplicationDocuments()
        {
            var del = from ad in _context.ApplicationDocuments
                      join s in _context.Staff on ad.DeletedBy equals s.StaffId
                      where ad.DeleteStatus == true
                      select new RecycleAppDocs
                      {
                          _appDocs = ad,
                          _staffs = s
                      };

            helpers.LogMessages("Displaying Deleted Application documents", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
         * Restoring deleted Application documents 
         * AppDocID => encryted application document id
         */
        public JsonResult RestorAppDoc(string AppDocID)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(AppDocID);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.ApplicationDocuments.Where(x => x.AppDocId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application document, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application documents. Status : "+ result + " Application document ID : "+ ID, helpers.getSessionEmail());

            return Json(result);
        }




        /*
         * Getting all deleted application processes
         */
       
        public IActionResult DeletedApplicationProcess()
        {
            var get = from ap in _context.ApplicationProccess.AsEnumerable()
                      join sf in _context.Staff.AsEnumerable() on ap.CreatedBy equals sf.StaffId into Staff
                      join sfu in _context.Staff.AsEnumerable() on ap.UpdatedBy equals sfu.StaffId into StaffU
                      join s in _context.ApplicationStage.AsEnumerable() on ap.StageId equals s.AppStageId
                      join r in _context.UserRoles.AsEnumerable() on ap.RoleId equals r.RoleId into Role
                      join ar in _context.UserRoles.AsEnumerable() on ap.OnAcceptRoleId equals ar.RoleId into AcceptRole
                      join rr in _context.UserRoles.AsEnumerable() on ap.OnRejectRoleId equals rr.RoleId into RejectRole
                      join l in _context.Location.AsEnumerable() on ap.LocationId equals l.LocationId into Location
                      join ds in _context.Staff.AsEnumerable() on ap.DeletedBy equals ds.StaffId
                      where ap.DeleteStatus == true
                      select new RecycleAppProcess
                      {
                          ProcessID = ap.ProccessId,
                          LinkName = s.StageName,
                          RoleID = Role.FirstOrDefault()?.RoleId,
                          RoleName = Role.FirstOrDefault()?.RoleName,
                          LocationID = Location.FirstOrDefault()?.LocationId,
                          LocationName = Location.FirstOrDefault()?.LocationName,
                          Sort = ap.Sort,
                          AcceptRole = AcceptRole.FirstOrDefault()?.RoleName,
                          RejectRole = RejectRole.FirstOrDefault()?.RoleName,
                          Process = ap.Process,
                          CanPush = ap.CanPush == true ? "YES" : "NO",
                          CanWork = ap.CanWork == true ? "YES" : "NO",
                          CanAccept = ap.CanAccept == true ? "YES" : "NO",
                          CanReject = ap.CanReject == true ? "YES" : "NO",
                          CanReport = ap.CanReport == true ? "YES" : "NO",
                          CanInspect = ap.CanInspect == true ? "YES" : "NO",
                          CanSchdule = ap.CanSchdule == true ? "YES" : "NO",
                          CreatedAt = ap.CreatedAt.ToString(),
                          DeletedBy = ds.LastName + " " + ds.FirstName,
                          DeletedAt = ap.DeletedAt.ToString()
                      };

            helpers.LogMessages("Displaying Deleted Application Process", helpers.getSessionEmail());

            return View(get.ToList());
        }


        /*
        * Restoring deletd  Application process 
        * id => encryted application process id
        */
        public JsonResult RestoreProcess(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.ApplicationProccess.Where(x => x.ProccessId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application process, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application process. Status : " + result + " Application process ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }




        /*
         * Getting all deleted applications 
         */
       
        public IActionResult DeletedApplications()
        {
            var apps = from a in _context.Applications.AsEnumerable()
                       join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                       join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                       join st in _context.States.AsEnumerable() on f.State equals st.StateId
                      join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                       join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                       
                       join at in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals at.AppTypeId
                       where a.DeletedStatus == true
                       select new MyApp
                       {
                           AppID = a.AppId,
                           RefNo = a.AppRefNo,
                           Type = at.TypeName,
                           Stage = s.StageName,
                           States = st.StateName,
                           Lga = f.Lga,
                           Facility = f.FacilityName + " (" + f.FacilityAddress + ")",
                           FacilityName = f.FacilityName,
                           CompanyName = c.CompanyName,
                           CompanyEmail = c.CompanyEmail,
                           Status = a.Status,
                           DateApplied = a.DateApplied == null ? "" : a.DateApplied.ToString(),
                           DateSubmitted = a.DateSubmitted == null ? "" : a.DateSubmitted.ToString(),
                           DeletedAt = a.DeletedAt,
                       };
            
            helpers.LogMessages("Displaying Deleted Application", helpers.getSessionEmail());

            return View(apps.AsEnumerable().GroupBy(x => x.AppID).Select(x => x.FirstOrDefault()).OrderByDescending(x => x.AppID).ToList());
        }



        /*
       * Restoring deleted Application 
       * id => encryted application process id
       */

        public JsonResult RestoreApplication(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Applications.Where(x => x.AppId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeletedStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application process, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application. Status : " + result + " Application ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted Application Stage
         */
        
        public IActionResult DeletedApplicationStage()
        {
            var del = from st in _context.ApplicationStage
                      join s in _context.Staff on st.DeletedBy equals s.StaffId
                      where st.DeleteStatus == true
                      select new RecycleAppStage
                      {
                          StageID = st.AppStageId,
                          StageName = st.StageName,
                          ShortName = st.ShortName,
                          Amount = st.Amount,
                          ServiceCharge = st.ServiceCharge,
                          CreatedAt = st.CreatedAt,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = st.DeletedAt,
                          DeletedStatus = st.DeleteStatus
                      };
            helpers.LogMessages("Displaying Deleted Application Stage", helpers.getSessionEmail());

            return View(del.ToList());
        }



        /*
         * Restoring deleted application stages
         * 
         * id => encrypted application stage id
         */
        public JsonResult RestoreApplicationStage(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.ApplicationStage.Where(x => x.AppStageId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application stage, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application Stage. Status : " + result + " Application Stage ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
        * Getting all deleted Application type
        */
        
        public IActionResult DeletedApplicationType()
        {
            var del = from t in _context.ApplicationType
                      join s in _context.Staff on t.DeletedBy equals s.StaffId
                      where t.DeleteStatus == true
                      select new RecycleAppType
                      {
                          TypeID = t.AppTypeId,
                          TypeName = t.TypeName,
                          CreatedAt = t.CreatedAt,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = t.DeletedAt,
                          DeletedStatus = t.DeleteStatus
                      };

            helpers.LogMessages("Displaying Deleted Application Type", helpers.getSessionEmail());

            return View(del.ToList());
        }



        /*
         * Restoring deleted application types
         * 
         * id => encrypted application stage id
         */
        public JsonResult RestoreApplicationType(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.ApplicationType.Where(x => x.AppTypeId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application type, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted Application Type. Status : " + result + " Application Type ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted application stage documents 
         */
        
        public IActionResult DeletedApplicationStageDocuments()
        {
            var del = from ts in _context.AppStageDocuments
                      join st in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals st.AppStageId
                      join d in _context.ApplicationDocuments on ts.AppDocId equals d.AppDocId
                      join sf in _context.Staff on ts.DeletedBy equals sf.StaffId
                      where ts.DeleteStatus == true
                      select new RecycleStageDoc
                      {
                          StageDocID = ts.StageDocId,
                          DocName = d.DocName,
                          StageName = st.StageName,
                          DocType = d.DocType,
                          DeletedAt = ts.DeletedAt.ToString(),
                          CreatedAt = ts.CreatedAt.ToString(),
                          DeletedBy = sf.FirstName + " " + sf.LastName,
                          DeletedStatus = ts.DeleteStatus
                      };

            helpers.LogMessages("Displaying Deleted Application Stage Documents", helpers.getSessionEmail());

            return View(del.ToList());
        }



        /*
         * Restoring deleted application stage documents
         * 
         * id => encrypted application stage document id
         */
        public JsonResult RestoreApplicationStageDocuments(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.AppStageDocuments.Where(x => x.StageDocId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application stage documents, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application Stage Document. Status : " + result + " Application Stage Document ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted application Tupe and stage 
         */
       
        public IActionResult DeletedApplicationTypeStage()
        {
            var del = from ts in _context.AppTypeStage
                      join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                      join d in _context.ApplicationType on ts.AppTypeId equals d.AppTypeId
                      join sf in _context.Staff on ts.DeletedBy equals sf.StaffId
                      where ts.DeleteStatus == true
                      select new RecycleStageDoc
                      {
                          TypeStageID = ts.TypeStageId,
                          StageName = s.StageName,
                          TypeName = d.TypeName,
                          Counter = ts.Counter,
                          DeletedAt = ts.DeletedAt.ToString(),
                          CreatedAt = ts.CreatedAt.ToString(),
                          DeletedBy = sf.FirstName + " " + sf.LastName,
                          DeletedStatus = ts.DeleteStatus
                      };
            helpers.LogMessages("Displaying Deleted Application Type Stage", helpers.getSessionEmail());
            return View(del.ToList());
        }


        /*
         * Restoring deleted application Type and stage
         * 
         * id => encrypted application type and stage id
         */
        public JsonResult RestoreApplicationTypeStage(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.AppTypeStage.Where(x => x.TypeStageId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this application type and stage documents, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted Application Type Stage. Status : " + result + " Application Type Stage ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }




        /*
         * Getting all deleted companies 
         */
        
        public IActionResult DeletedCompanies()
        {
            var del = from c in _context.Companies
                      join s in _context.Staff on c.DeletedBy equals s.StaffId
                      where c.DeleteStatus == true
                      select new RecycleCompany
                      {
                          CompanyID = c.CompanyId,
                          CompanyName = c.CompanyName,
                          CompanyEmail = c.CompanyEmail,
                          Address = c.Address,
                          City = c.City,
                          StateName = c.StateName,
                          CreatedAt = c.CreatedAt,
                          DeletedStatus = c.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = c.DeletedAt,
                          CompanyCode = c.IdentificationCode
                      };
            helpers.LogMessages("Displaying Deleted companies", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
         * Restoring deleted companies 
         * 
         * id => encrypted company id
         */
        public JsonResult RestoreCompany(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Companies.Where(x => x.CompanyId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this company, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted companies. Status : " + result + " Company ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted countries
         */
        
        public IActionResult DeletedCountries()
        {
            var del = from c in _context.Countries
                      join s in _context.Staff on c.DeletedBy equals s.StaffId
                      where c.DeleteStatus == true
                      select new RecycleCountry
                      {
                          CountryID = c.CountryId,
                          CountryName = c.CountryName,
                          CreatedAt = c.CreatedAt,
                          DeletedStatus = c.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = c.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted countries", helpers.getSessionEmail());

            return View(del.ToList());
        }



        /*
        * Restoring deleted countries 
        * 
        * id => encrypted country id
        */
        public JsonResult RestoreCountry(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Countries.Where(x => x.CountryId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this country, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted country. Status : " + result + " country ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }


        /*
         * Getting all deleted field office
         */
       
        public IActionResult DeletedFieldOffice()
        {
            var del = from f in _context.FieldOffices
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      where f.DeleteStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.FieldOfficeId,
                          Name = f.OfficeName,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };
            helpers.LogMessages("Displaying Deleted field office", helpers.getSessionEmail());

            return View(del.ToList());
        }



        /*
        * Restoring deleted field office 
        * 
        * id => encrypted field office id
        */
        public JsonResult RestoreFieldOffice(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.FieldOffices.Where(x => x.FieldOfficeId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this field office, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted field office. Status : " + result + " field office ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted location
         */
       
        public IActionResult DeletedLocation()
        {
            var del = from f in _context.Location
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      where f.DeleteStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.LocationId,
                          Name = f.LocationName,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted Location", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
       * Restoring deleted location
       * 
       * id => encrypted location id
       */
        public JsonResult RestoreLocation(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Location.Where(x => x.LocationId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this location, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted location. Status : " + result + " locatioin ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted reports
         */
        
        public IActionResult DeletedReport()
        {
            var del = from f in _context.Reports
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      join ss in _context.Staff on f.StaffId equals ss.StaffId
                      where f.DeletedStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.ReportId,
                          Subject = f.Subject,
                          Comment = f.Comment,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = (bool)f.DeletedStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          CreatedBy = ss.FirstName + " " + ss.LastName,
                          DeletedAt = f.DeletedAt,
                          DocSource = f.DocSource
                      };
            helpers.LogMessages("Displaying Deleted reports", helpers.getSessionEmail());
            return View(del.ToList());
        }


        /*
       * Restoring deleted report
       * 
       * id => encrypted report id
       */
        public JsonResult RestoreReport(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Reports.Where(x => x.ReportId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeletedStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this report, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted reports. Status : " + result + " report ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted schedule
         */
        
        public IActionResult DeletedSchedule()
        {
            var del = from f in _context.Schdules
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      join ss in _context.Staff on f.SchduleBy equals ss.StaffId
                      join a in _context.Applications on f.AppId equals a.AppId
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      where f.DeletedStatus == true
                      select new RecycleSchedule
                      {
                          ID = f.SchduleId,
                          Comment = f.Comment,
                          ScheduleDate = f.SchduleDate,
                          SchduleType = f.SchduleType,
                          SchduleLocation = f.SchduleLocation,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = (bool)f.DeletedStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          CreatedBy = ss.FirstName + " " + ss.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted schedule", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
       * Restoring deleted schedule
       * 
       * id => encrypted schedule id
       */
        public JsonResult RestoreSchedule(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Schdules.Where(x => x.SchduleId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeletedStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this schedule, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted schedule. Status : " + result + " schedule ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted staff
         */
        
        public IActionResult DeletedStaff()
        {
            var del = from f in _context.Staff
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      join fd in _context.FieldOffices on f.FieldOfficeId equals fd.FieldOfficeId
                      join r in _context.UserRoles on f.RoleId equals r.RoleId
                      where f.DeleteStatus == true
                      select new RecycleStaff
                      {
                          ID = f.StaffId,
                          StaffEmail = f.StaffEmail,
                          StaffName = f.LastName + " " + f.FirstName,
                          Office = fd.OfficeName,
                          Role = r.RoleName,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted staff", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
       * Restoring deleted staff
       * 
       * id => encrypted staff id
       */
        public JsonResult RestoreStaff(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.Staff.Where(x => x.StaffId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this staff, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted staff. Status : " + result + " staff ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
        * Getting all deleted state
        */
       
        public IActionResult DeletedState()
        {
            var del = from f in _context.States
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      join c in _context.Countries on f.CountryId equals c.CountryId
                      where f.DeleteStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.StateId,
                          Name = f.StateName + " (" + c.CountryName + ")",
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted state", helpers.getSessionEmail());
            
            return View(del.ToList());
        }


        /*
       * Restoring deleted state
       * 
       * id => encrypted state id
       */
        public JsonResult RestoreState(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.States.Where(x => x.StateId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this state, please try again later.";
                    }
                }
            }

            helpers.LogMessages("Restored Deleted state. Status : " + result + " State ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }



        /*
         * Getting all deleted roles
         */
       
        public IActionResult DeletedRoles()
        {
            var del = from f in _context.UserRoles
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      where f.DeleteStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.RoleId,
                          Name = f.RoleName,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted Roles", helpers.getSessionEmail());
            return View(del.ToList());
        }


        /*
       * Restoring deleted roles
       * 
       * id => encrypted roles id
       */
        public JsonResult RestoreRole(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.UserRoles.Where(x => x.RoleId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this role, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted Roles. Status : " + result + " Role ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }


        /*
         * Getting all deleted zones
         */
       
        public IActionResult DeletedZone()
        {
            var del = from f in _context.ZonalOffice
                      join s in _context.Staff on f.DeletedBy equals s.StaffId
                      where f.DeleteStatus == true
                      select new RecycleAnonymous
                      {
                          ID = f.ZoneId,
                          Name = f.ZoneName,
                          CreatedAt = f.CreatedAt,
                          DeletedStatus = f.DeleteStatus,
                          DeletedBy = s.FirstName + " " + s.LastName,
                          DeletedAt = f.DeletedAt,
                      };

            helpers.LogMessages("Displaying Deleted Zones", helpers.getSessionEmail());

            return View(del.ToList());
        }


        /*
       * Restoring deleted zones
       * 
       * id => encrypted zones id
       */
        public JsonResult RestoreZone(string id)
        {
            string result = "";

            int ID = 0;

            var sID = generalClass.Decrypt(id);

            if (sID == "Error")
            {
                result = "Application link error";
            }
            else
            {
                ID = Convert.ToInt32(sID);

                var del = _context.ZonalOffice.Where(x => x.ZoneId == ID);

                if (del.Any())
                {
                    del.FirstOrDefault().DeleteStatus = false;
                    del.FirstOrDefault().DeletedAt = null;
                    del.FirstOrDefault().DeletedBy = null;
                    del.FirstOrDefault().UpdatedAt = DateTime.Now;

                    if (_context.SaveChanges() > 0)
                    {
                        result = "Restored";
                    }
                    else
                    {
                        result = "Something went wrong trying to restore this zone, please try again later.";
                    }
                }
            }
            helpers.LogMessages("Restored Deleted Zones. Status : " + result + " Zonal ID : " + ID, helpers.getSessionEmail());

            return Json(result);
        }




    }
}
