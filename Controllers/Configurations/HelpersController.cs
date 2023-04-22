using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using DST.Controllers.Authentications;
using DST.Helpers;
using DST.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using static DST.Models.GeneralModel;

namespace DST.Controllers.Configurations
{
    [Authorize]
    public class HelpersController : Controller
    {

        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        public DST_DBContext _context;

        RestSharpServices restSharpServices = new RestSharpServices();
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();



        public HelpersController(DST_DBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public string getSessionEmail()
        {
            try
            {
                return generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionEmail));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string getSessionRoleName()
        {
            try
            {
                return generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionRoleName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public int getSessionUserID()
        {
            try
            {
                return Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionUserID)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int getSessionRoleID()
        {
            try
            {
                return Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionRoleID)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string getSessionUserName()
        {
            try
            {
                return generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionUserName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string getSessionTheme()
        {
            try
            {
                return generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionTheme));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public int getSessionElpsID()
        {
            try
            {
                return Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionElpsID)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int getSessionLogin()
        {
            try
            {
                return Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionLogin)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public JsonResult GetAllCountries(int countryID = 0, string countryName = null, bool deletedStatus = false)
        {
            var countries = (from c in _context.Countries
                             where c.DeleteStatus == deletedStatus
                             select c);

            countries = countryID != 0 ? countries.Where(c => c.CountryId == countryID) :
                        countryName != null ? countries.Where(c => c.CountryName == countryName.ToUpper()) :
                        countries.Where(c => c.DeleteStatus == deletedStatus);

            return Json(countries.ToList());
        }



        public void SaveHistory(int appid, string actionFrom, string actionTo, string status, string comment)
        {
            AppDeskHistory appDeskHistory = new AppDeskHistory()
            {
                AppId = appid,
                ActionFrom = actionFrom,
                ActionTo = actionTo,
                Comment = comment,
                Status = status,
                CreatedAt = DateTime.Now
            };

            _context.AppDeskHistory.Add(appDeskHistory);
            _context.SaveChanges();
        }



        public void LogMessages(string message, string user_id = null)
        {
            AuditTrail auditTrail = new AuditTrail()
            {
                CreatedAt = DateTime.Now,
                UserId = user_id,
                AuditAction = message
            };

            _context.AuditTrail.Add(auditTrail);
            _context.SaveChanges();
        }


        public void SavePermitHistory(int permitID, string type)
        {
            var userRole = generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            var userName = generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserName));
            var userEmail = generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));

            PermitHistory permitHistory = new PermitHistory();

            if (type == "Download")
            {
                permitHistory.DownloadedAt = DateTime.Now;
                permitHistory.ViewType = type;
                permitHistory.PermitId = permitID;
                permitHistory.UserDetails = userRole + " - " + userName + " (" + userEmail + ")";
            }
            else if (type == "Preview")
            {
                permitHistory.PreviewedAt = DateTime.Now;
                permitHistory.ViewType = type;
                permitHistory.PermitId = permitID;
                permitHistory.UserDetails = userRole + " - " + userName + " (" + userEmail + ")";
            }

            _context.PermitHistory.Add(permitHistory);
            _context.SaveChanges();
        }




        public List<AppMessage> SaveMessage(int appid, int companyid, string subject, string content)
        {

            Messages messages = new Messages()
            {
                AppId = appid,
                CompanyId = companyid,
                Subject = subject,
                MesgContent = content,
                Seen = false,
                CreatedAt = DateTime.Now
            };
            _context.Messages.Add(messages);
            _context.SaveChanges();

            var msg = GetMessage(messages.MessageId, messages.CompanyId);
            return msg;

        }




        public List<AppMessage> GetMessage(int msg_id, int comp_id)
        {
            var message = from m in _context.Messages.AsEnumerable()
                          join a in _context.Applications.AsEnumerable() on m.AppId equals a.AppId into Apps
                          from ap in Apps.DefaultIfEmpty()
                          join c in _context.Companies.AsEnumerable() on ap?.CompanyId equals c.CompanyId into Com
                          from cm in Com.DefaultIfEmpty()
                          join fc in _context.Facilities on ap?.FacilityId equals fc.FacilityId into Fac
                          from f in Fac.DefaultIfEmpty()
                          join ts in _context.AppTypeStage.AsEnumerable() on ap?.AppTypeStageId equals ts.TypeStageId
                          join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                          join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                          join tr in _context.Transactions.AsEnumerable() on ap?.AppId equals tr.AppId into trans
                          from t in trans.DefaultIfEmpty()
                          where m.CompanyId == comp_id && m.MessageId == msg_id
                          select new AppMessage
                          {
                              Subject = m.Subject,
                              Content = m.MesgContent,
                              RefNo = ap == null ? "" : ap.AppRefNo,
                              Stage = ty.TypeName + " - "+ s.StageName,
                              Status = ap == null ? "" : ap.Status,
                              TotalAmount = t?.TotalAmt,
                              Seen = m.Seen,
                              DateApplied = ap == null ? "" : ap.DateApplied.ToLongDateString(),
                              DateSubmitted = ap == null ? "" : ap.DateSubmitted?.ToLongDateString(),
                              CompanyDetails = cm == null ? "" : cm.CompanyName + " (" + cm.Address + ", " + cm.City + ", " + cm.StateName + ")",
                              FacilityDetails = f == null ? "" : f.FacilityName,
                          };
            return message.ToList();
        }




        public string MessageTemplate(string subject, string content)
        {
            string body = "<div>";
            body += "<div style='width: 700px; background-color: #ece8d4; padding: 5px 0 5px 0;'><img style='width: 98%; height: 120px; display: block; margin: 0 auto;' src='https://ROMS.dpr.gov.ng/Content/Images/mainlogo.png' alt='Logo'/></div>";
            body += "<div class='text-left' style='background-color: #ece8d4; width: 700px; min-height: 200px;'>";
            body += "<div style='padding: 10px 30px 30px 30px;'>";
            body += "<h5 style='text-align: center; font-weight: 300; padding-bottom: 10px; border-bottom: 1px solid #ddd;'>" + subject.ToUpper() + "</h5>";
            body += "<p>Dear Sir/Madam,</p>";
            body += "<p style='line-height: 30px; text-align: justify;'>" + content + "</p>";
            body += "<br>";
            body += "<p>Kindly go to <a href='https://welltest.NUPRC.gov.ng/'> Well Test Portal (CLICK HERE)</a> link to view more details </p>";
            body += "<p>Nigerian Upstream Petroleum Regulator Commission<br/> <small> (Well Test) </small></p> </div>";
            body += "<p style='padding:10px 0 10px; 10px; background-color:#888; color:#f9f9f9; width:700px;'> &copy; " + DateTime.Now.Year + " Nigerian Upstream Petroleum Regulator Commission &minus; NUPRC Nigeria</p></div></div>";
            return body;
        }




        public string StaffMessageTemplate(string subject, string content)
        {
            string body = "<div>";
            body += "<div style='width: 700px; background-color: #ece8d4; padding: 5px 0 5px 0;'><img style='width: 98%; height: 120px; display: block; margin: 0 auto;' src='https://ROMS.dpr.gov.ng/Content/Images/mainlogo.png' alt='Logo'/></div>";
            body += "<div class='text-left' style='background-color: #ece8d4; width: 700px; min-height: 200px;'>";
            body += "<div style='padding: 10px 30px 30px 30px;'>";
            body += "<h5 style='text-align: center; font-weight: 300; padding-bottom: 10px; border-bottom: 1px solid #ddd;'>" + subject + "</h5>";
            body += "<p>Dear Sir/Madam,</p>";
            body += "<p style='line-height: 30px; text-align: justify;'>" + content + "</p>";
            body += "<br>";
            body += "<p>Kindly go to <a href='https://welltest.dpr.gov.ng/'> Well Test Portal (CLICK HERE)</a> link and process application on your desk. </p>";
            body += "<p>Nigerian Upstream Petroleum Regulator Commission<br/> <small> (Well Test) </small></p> </div>";
            body += "<div style='padding:10px 0 10px; 10px; background-color:#888; color:#f9f9f9; width:700px;'> &copy; " + DateTime.Now.Year + " Nigerian Upstream Petroleum Regulator Commission &minus; NUPRC Nigeria</div></div></div>";
            return body;
        }



      

        /*
         * Email HTML template
         */
        public string CompanyMessageTemplate(List<AppMessage> appMessages)
        {
            var msg = appMessages.FirstOrDefault();

            string body = "<div>";
            body += "<div style='width: 800px; background-color: #ece8d4; padding: 5px 0 5px 0;'><img style='width: 98%; height: 120px; display: block; margin: 0 auto;' src='https://ROMS.dpr.gov.ng/Content/Images/mainlogo.png' alt='Logo'/></div>";
            body += "<div class='text-left' style='background-color: #ece8d4; width: 800px; min-height: 200px;'>";
            body += "<div style='padding: 10px 30px 30px 30px;'>";
            body += "<h5 style='text-align: center; font-weight: 300; padding-bottom: 10px; border-bottom: 1px solid #ddd;'>" + msg.Subject + "</h5>";
            body += "<p>Dear Sir/Madam,</p>";
            body += "<p style='line-height: 30px; text-align: justify;'>" + msg.Content + "</p>";
            body += "<table style = 'width: 100%;'><tbody>";

            body += "<tr><td style='width: 200px;'><strong>App Ref No:</strong></td><td> " + msg.RefNo + " </td></tr>";
            body += "<tr><td><strong>Company:</strong></td><td> " + msg.CompanyDetails + " </td></tr>";
            body += "<tr><td><strong>Well Details:</strong></td><td> " + msg.FacilityDetails + " </td></tr>";
            body += "<tr><td><strong>Stage:</strong></td><td> " + msg.Stage + " </td></tr>";
            body += "<tr><td><strong>Status:</strong></td><td> " + msg.Status + " </td></tr>";
            body += "<tr><td><strong>Amount:</strong></td><td> ₦" + string.Format("{0:N}", msg.TotalAmount) + " </td></tr>";
            body += "<tr><td><strong>Date Applied:</strong></td><td> " + msg.DateApplied + " </td></tr>";
            body += "<tr><td><strong>Date Submitted:</strong></td><td> " + msg.DateSubmitted + " </td></tr>";
            body += "</tbody></table><br/>";

            body += "<p>Kindly Note that this is NOT an Approval Letter.</p>";
            body += "<p>Nigerian Upstream Petroleum Regulator Commission<br/> <small> (Well Test) </small></p> </div>";
            body += "<div style='padding:10px 0 10px; 10px; background-color:#888; color:#f9f9f9; width:800px;'> &copy; " + DateTime.Now.Year + " Nigerian Upstream Petroleum Regulator Commission &minus; NUPRC Nigeria</div></div></div>";

            return body;
        }



        public async System.Threading.Tasks.Task<string> SendEmailMessageAsync(string emailTo, string fullname, string subject, string content, string option, List<AppMessage> appMessages = null)
        {
            string email = "";
            string name = "";
            var st = _context.Staff.Where(x => x.StaffEmail == emailTo);

            string txt = "";

            if (st.Any())
            {
                int staff_id = 0;

                var checkRelieve = _context.OutOfOffice.Where(x => x.StaffId == st.FirstOrDefault().StaffId && x.Status == GeneralClass._STARTED && x.DeletedStatus == false);

                if (checkRelieve.Any() && checkRelieve != null)
                {
                    staff_id = checkRelieve.FirstOrDefault().ReliverId;
                    var staffz = _context.Staff.Where(x => x.StaffId == st.FirstOrDefault().StaffId).FirstOrDefault();

                    txt = "You have received this message because you are acting for " + staffz.LastName + " " + staffz.FirstName + ". ";
                }
                else
                {
                    staff_id = st.FirstOrDefault().StaffId;
                }

                var staff = _context.Staff.Where(x => x.StaffId == staff_id);

                name = staff.FirstOrDefault().LastName + " " + staff.FirstOrDefault().FirstName;
                email = staff.FirstOrDefault().StaffEmail;

            }
            else
            {
                //company
                var com = _context.Companies.Where(x => x.CompanyEmail == emailTo);

                if (com.Any())
                {

                    email = com.FirstOrDefault()?.CompanyEmail;
                    name = com.FirstOrDefault()?.CompanyName;
                }
                else
                {
                    email = emailTo;
                    name = fullname;
                }
            }

            var result = "";

            var password = "BNW5He3DoWQAJVMkeMlEzPTtbYIXNveS4t+GuGtXzxQJ";
            var username = "AKIAQCM2OPFBW35OSTFV";
            var emailFrom = "no-reply@dpr.gov.ng";
            var Host = "email-smtp.us-west-2.amazonaws.com";
            var Port = 587;

            string msgBody = "";

            if (option == GeneralClass.DEFAULT_MESSAGE)
            {
                msgBody = MessageTemplate(subject, content);
            }
            else if (option == GeneralClass.STAFF_NOTIFY)
            {
                msgBody = StaffMessageTemplate(subject, txt + "" + content);
            }
            else if (option == GeneralClass.COMPANY_NOTIFY)
            {
                msgBody = CompanyMessageTemplate(appMessages);
            }


            MailMessage _mail = new MailMessage();
            SmtpClient client = new SmtpClient(Host, Port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(username, password);
            _mail.From = new MailAddress(emailFrom);
            _mail.To.Add(new MailAddress(email, name));
            _mail.Subject = subject;
            _mail.IsBodyHtml = true;
            _mail.Body = msgBody;


            try
            {
                await client.SendMailAsync(_mail);
                //var response = await client.SendEmailAsync(msg);
                result = "OK";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }



        public string getActionHistory(int roleid, int userid)
        {
            var result = "";

            var getRole = _context.UserRoles.Where(x => x.RoleId == roleid);

            if (getRole.Any())
            {
                if (getRole.FirstOrDefault().RoleName == GeneralClass.COMPANY)
                {
                    var getCompany = _context.Companies.Where(x => x.CompanyId == userid);

                    if (getCompany.Any())
                    {
                        result = getCompany.FirstOrDefault().CompanyName + " (" + GeneralClass.COMPANY + ")";
                    }
                    else
                    {
                        result = "";
                    }
                }
                else
                {
                    var getStaff = _context.Staff.Where(x => x.StaffId == userid);

                    if (getStaff.Any())
                    {
                        result = getStaff.FirstOrDefault().LastName + " " + getStaff.FirstOrDefault().FirstName + " (" + getRole.FirstOrDefault().RoleName + ")";
                    }
                    else
                    {
                        result = "";
                    }
                }
            }
            else
            {
                result = "";
            }

            return result;
        }



        /*
         * Encrypting application ID and processing rule id
         * 
         */
        public JsonResult GetEncrypt(string app_id, string process_id)
        {
            var result = "";

            string AppID = generalClass.Encrypt(app_id.Trim());
            string ProcessID = generalClass.Encrypt(process_id);

            string dAppID = generalClass.Decrypt(AppID);
            string dProcessID = generalClass.Decrypt(ProcessID);

            result = AppID + "|" + ProcessID;

            return Json(result.Trim());
        }




        /*
         * Updating application status to ELPS
         */
        public bool UpdateElpsApplication(List<DST.Models.DB.Applications> apps)
        {
            bool result = false;

            var app = from a in _context.Applications
                      join c in _context.Companies on a.CompanyId equals c.CompanyId
                      where a.AppId == apps.FirstOrDefault().AppId && a.DeletedStatus == false
                      select new
                      {
                          a,
                          c
                      };

            if (app.Any())
            {
                var paramData = restSharpServices.parameterData("orderId", app.FirstOrDefault().a.AppRefNo);
                var elpsApp = restSharpServices.Response("/api/Application/ByOrderId/{orderId}/{email}/{apiHash}", paramData, "GET");

                if (elpsApp.IsSuccessful == true)
                {
                    var resp = JsonConvert.DeserializeObject<JObject>(elpsApp.Content);

                    if (resp != null)
                    {
                        var values = new JObject();
                        values.Add("orderId", app.FirstOrDefault().a.AppRefNo);
                        values.Add("company_Id", app.FirstOrDefault().c.CompanyElpsId);
                        values.Add("status", app.FirstOrDefault().a.Status);
                        values.Add("date", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        values.Add("categoryName", (string)resp.SelectToken("categoryName"));
                        values.Add("licenseName", (string)resp.SelectToken("licenseName"));
                        values.Add("licenseId", (string)resp.SelectToken("licenseId"));
                        values.Add("id", (string)resp.SelectToken("id"));

                        var updateApp = restSharpServices.Response("api/Application/{email}/{apiHash}", null, "PUT", values);

                        if (updateApp.IsSuccessful == true)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }



        /*
         * Posting approved permit to elps.
         */
        public bool PostPermitToElps(int permitID, string permitNO, string OrderID, int ElpsCompID, DateTime issuedDate, DateTime expiryDate, bool isRenew)
        {
            var values = new JObject();
            values.Add("permit_No", permitNO);
            values.Add("orderId", OrderID);
            values.Add("company_Id", ElpsCompID.ToString());
            values.Add("date_Issued", issuedDate.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            values.Add("date_Expire", expiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            values.Add("categoryName", "Drill Stem Test (DST)");
            values.Add("is_Renewed", (isRenew == true) ? "Yes" : "No");
            values.Add("licenseId", permitID.ToString());
            values.Add("id", 0);

            List<JObject> newDocs = new List<JObject>();

            var paramData = restSharpServices.parameterData("CompId", ElpsCompID.ToString());
            var savePermit = restSharpServices.Response("api/Permits/{CompId}/{email}/{apiHash}", paramData, "POST", values);

            if (savePermit.IsSuccessful)
            {
                JObject eplsPermit = JsonConvert.DeserializeObject<JObject>(savePermit.Content);

                var permit = _context.Permits.Where(x => x.PermitId == permitID);

                if (permit.Any())
                {
                    permit.FirstOrDefault().PermitElpsId = (int)eplsPermit.SelectToken("id");
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }





        /*
        * Getting all states with a particular country, state id, state name or country id
        */
        public JsonResult GetAllStatesFromCountry(int CountryID = 0, int StateID = 0, string StateName = null, string CountryName = null, bool deleteStatus = false)
        {
            var states = (from s in _context.States
                          join c in _context.Countries on s.CountryId equals c.CountryId
                          where s.DeleteStatus == deleteStatus && c.DeleteStatus == deleteStatus
                          orderby s.StateName
                          select new
                          {
                              countryName = c.CountryName,
                              countryID = c.CountryId,
                              stateID = s.StateId,
                              stateName = s.StateName,
                              createAt = s.CreatedAt,
                              updatedAt = s.UpdatedAt,
                              deleteStatus = s.DeleteStatus,
                              deletedAt = s.DeletedAt,
                              deletedBy = s.DeletedBy,
                          });

            states = CountryName != null ? states.Where(s => s.countryName == CountryName.ToUpper()) :
                     CountryID != 0 ? states.Where(s => s.countryID == CountryID) :
                     StateName != null ? states.Where(s => s.stateName == StateName.ToUpper()) :
                     StateID != 0 ? states.Where(s => s.stateID == StateID) :
                     states.Where(s => s.deleteStatus == deleteStatus);

            return Json(states.ToList());
        }



        /*
         * Getting all zones with ID or Name
         */
        public JsonResult GetAllZones(int zonalID = 0, string zonalName = null, bool deletedStatus = false)
        {
            var zones = (from z in _context.ZonalOffice where z.DeleteStatus == deletedStatus select z);

            zones = zonalName != null ? zones.Where(z => z.ZoneName == zonalName.ToUpper()) :
                    zonalID != 0 ? zones.Where(z => z.ZoneId == zonalID) :
                    zones.Where(z => z.DeleteStatus == deletedStatus);

            return Json(zones.ToList());
        }



        /*
         * Getting all field office with ID or Name
         */
        public JsonResult GetAllFieldOffice(int OfficeID = 0, string FieldOfficeName = null, bool deletedStatus = false)
        {
            var _fieldOffice = (from z in _context.FieldOffices where z.DeleteStatus == deletedStatus select z);

            _fieldOffice = FieldOfficeName != null ? _fieldOffice.Where(z => z.OfficeName == FieldOfficeName.ToUpper()) :
                    OfficeID != 0 ? _fieldOffice.Where(z => z.FieldOfficeId == OfficeID) :
                    _fieldOffice.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_fieldOffice.ToList());
        }



        /*
        * Getting all company document types from elps to save specific
        */
        public JsonResult GetElpsDocumentsTypes()
        {
            var result = generalClass.RestResult("Documents/Types", "GET", null, null, null); // GET
            return Json(result.Value);
        }



       
        public JsonResult GetStaffs()
        {
            int staff_id = Convert.ToInt32(generalClass.Decrypt(_httpContextAccessor.HttpContext.Session.GetString("_sessionUserID")).Trim());
            var getStaffFieldOfice = _context.Staff.Where(x => x.StaffId == staff_id);
            var staff = _context.Staff.Where(x => x.DeleteStatus == false && x.ActiveStatus == true && x.StaffId != staff_id && x.FieldOfficeId == getStaffFieldOfice.FirstOrDefault().FieldOfficeId);
            return Json(staff.ToList());
        }


        /*
        * Getting all facility document types from elps to save specific
        */
        public JsonResult GetElpsFacDocumentsTypes()
        {
            var paramData = restSharpServices.parameterData("Type", "facility");
            var result = generalClass.RestResult("Documents/Facility", "GET", paramData, null, null, "/{Type}"); // GET
            return Json(result.Value);
        }



        /*
        * Getting all Application Documents from local DataBase
        */
        public JsonResult GetAppDocs(int DocID = 0, bool deletedStatus = false)
        {
            var _doc = (from z in _context.ApplicationDocuments where z.DeleteStatus == deletedStatus select z);
            _doc = DocID != 0 ? _doc.Where(z => z.AppDocId == DocID) :
              _doc.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_doc.ToList());
        }



        /*
         * Getting all Application Stages
         */
        public JsonResult GetAppStages(int StageID = 0, bool deletedStatus = false)
        {
            var _stage = (from z in _context.ApplicationStage where z.DeleteStatus == deletedStatus select z);
            _stage = StageID != 0 ? _stage.Where(z => z.AppStageId == StageID) :
              _stage.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_stage.ToList());
        }


        /*
        * Getting all Application Types
        */
        public JsonResult GetAppTypes(int TypeID = 0, bool deletedStatus = false)
        {
            var _types = (from z in _context.ApplicationType where z.DeleteStatus == deletedStatus select z);
            _types = TypeID != 0 ? _types.Where(z => z.AppTypeId == TypeID) :
              _types.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_types.ToList());
        }


      

        /*
         * Get all application stage from application type
         */
        public JsonResult GetAppStageFromType(int TypeID)
        {
            var stage = from ts in _context.AppTypeStage
                        join t in _context.ApplicationType on ts.AppTypeId equals t.AppTypeId
                        join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                        where ts.AppTypeId == TypeID && ts.DeleteStatus == false && s.DeleteStatus == false && t.DeleteStatus == false
                        select new
                        {
                            StageID = ts.AppStageId,
                            StageName = s.StageName
                        };
            return Json(stage.ToList());
        }


        


        /*
        * Getting all Location for staff
        */
        public JsonResult GetLocation(int locationID = 0, bool deletedStatus = false)
        {
            var _location = (from z in _context.Location where z.DeleteStatus == deletedStatus select z);
            _location = locationID != 0 ? _location.Where(z => z.LocationId == locationID) :
              _location.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_location.ToList());
        }



        /*
         * Getting all roles for staff
         */
        public JsonResult GetStaffRoles(int roleID = 0, bool deletedStatus = false)
        {
            var _roles = (from z in _context.UserRoles where z.DeleteStatus == deletedStatus select z);
            _roles = roleID != 0 ? _roles.Where(z => z.RoleId == roleID) :
              _roles.Where(z => z.DeleteStatus == deletedStatus);

            return Json(_roles.ToList());
        }



        /*
         * Get the registered address of a company by using the company's registered id
         */
        public JsonResult GetCompanyAddress(string address_id)
        {
            if (!string.IsNullOrWhiteSpace(address_id))
            {
                var paramData = restSharpServices.parameterData("Id", address_id);
                var result = generalClass.RestResult("Address/ById/{Id}", "GET", paramData, null, null); // GET
                return Json(result.Value);
            }
            else
            {
                return Json("Empty");
            }
        }


        /*
        * Get All states by country id
        */
        public JsonResult GetStates(string CountryId)
        {
            var paramData = restSharpServices.parameterData("Id", CountryId);
            var result = generalClass.RestResult("Address/states/{Id}", "GET", paramData, null, null); // GET
            return Json(result.Value);
        }
        



        public JsonResult DeleteCompanyDocument(string CompElpsDocID, string DocType)
        {
            var deleteURL = "";

            if (DocType == "Company")
            {
                deleteURL = "CompanyDocument";
            }
            else
            {
                deleteURL = "FacilityDocument";
            }

            var paramData = restSharpServices.parameterData("Id", CompElpsDocID);
            var result = generalClass.RestResult(deleteURL + "/Delete/{Id}", "DELETE", paramData, null, "Document Deleted", null); // DELETE
            return Json(result.Value);
        }



        /*
         * Generating permit numbers with different parameters
         */
        public string GeneratePermitNumber(int appid)
        {
            string PermitNumber = "";

            int i = 1;
            int num = 0;
            var year = DateTime.Now.Year;

            var apps = from a in _context.Applications.AsEnumerable()
                       join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                       join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                       join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                       where a.AppId == appid
                       select new
                       {
                           Type = ty.TypeName
                       };

            var shortName = apps.FirstOrDefault().Type == GeneralClass.MER ? "MER" :
                            apps.FirstOrDefault().Type == GeneralClass.DSTs ? "DST" :
                            apps.FirstOrDefault().Type == GeneralClass.EWT ? "EWT" : "AUS";

            var seq = _context.Permits.OrderByDescending(x => x.PermitId);

            if (seq.Any())
            {
                if (year > seq.FirstOrDefault().CreatedAt.Year)
                {
                    num += i;
                    PermitNumber = "NUPRC/UMR/RM/"+shortName+"/EPERMIT/" + year + "/" + num.ToString("D4");
                }
                else
                {
                    if (seq.FirstOrDefault().PermitSequence >= 999)
                    {
                        num = seq.FirstOrDefault().PermitSequence + i;
                        PermitNumber = "NUPRC/UMR/RM/" + shortName + "/EPERMIT/" + year + "/" + num;
                    }
                    else
                    {
                        num = seq.FirstOrDefault().PermitSequence + i;
                        PermitNumber = "NUPRC/UMR/RM/" + shortName + "/EPERMIT/" + year + "/" + num.ToString("D4");
                    }
                }
            }
            else
            {
                PermitNumber = "NUPRC/UMR/RM/" + shortName + "/EPERMIT/" + i.ToString("D4");
            }

            return PermitNumber;

        }



        public List<AppDetails> GetAppDetails(int appid)
        {
            var apps = from a in _context.Applications.AsEnumerable()
                       join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                       join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                       join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                       where a.AppId == appid
                       select new AppDetails
                       {
                           Type = ty.TypeName,
                           Stage = s.StageName,
                           ShortName = s.ShortName
                       };

            return apps.ToList();
        }




        public DateTime PermitExpiry(int appid)
        {
            var today = DateTime.Now.Month;

            DateTime expiryDate;

            var apps = GetAppDetails(appid);

            if(apps.FirstOrDefault().Type == GeneralClass.MER)
            {
                if(today > 6) { 
                    expiryDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else
                {
                    expiryDate = new DateTime(DateTime.Now.Year, 6, 30);
                }
            }
            else if (apps.FirstOrDefault().Type == GeneralClass.DSTs)
            {
                expiryDate = DateTime.Now.AddYears(1);
            }
            else if(apps.FirstOrDefault().Type == GeneralClass.EWT)
            {
                expiryDate = DateTime.Now.AddMonths(6);
            }
            else
            {
                expiryDate = DateTime.Now.AddMonths(6);
            }

            return expiryDate;
        }




        /*
         * Get field office staff based on their state id
         * 
         * StateID => Not encrypted state id
         * 
         */
        public List<int> FieldOfficeStaff(int StateID)
        {
            var getZone = _context.ZoneStates.Where(x => x.StateId == StateID && x.DeleteStatus == false);
            List<int> staffFieldOffices = new List<int>();

            if (getZone.Any())
            {
                var getFieldOffice = _context.ZoneFieldOffice.Where(x => x.DeleteStatus == false);

                foreach (var f in getFieldOffice.ToList())
                {
                    foreach (var z in getZone.ToList())
                    {
                        if (z.ZoneId == f.ZoneId)
                        {
                            staffFieldOffices.Add(f.FieldOfficeId);
                        }
                    }
                }
            }
            else
            {
                staffFieldOffices.Add(0);
            }
            return staffFieldOffices;
        }



        /*
         * Get staff to drop application to.
         * 
         * sort => Not encrypted process sort.
         * 
         */
        public int ApplicationDropStaff(int stage_id, int sort, string process = null)
        {
            var result = 0;

            if (string.IsNullOrWhiteSpace(sort.ToString()))
            {
                result = 0;
            }
            else
            {
                var getProccess = _context.ApplicationProccess.Where(x => x.StageId == stage_id && (x.Sort == sort || x.Process == process) && x.DeleteStatus == false);

                if (getProccess.Any())
                {
                    var getSaff = from s in _context.Staff.AsEnumerable()
                                  where ((s.RoleId == getProccess.FirstOrDefault().RoleId && s.LocationId == getProccess.FirstOrDefault().LocationId && s.DeleteStatus == false && s.ActiveStatus == true))
                                  select new
                                  {
                                      FieldOfficeId = s.FieldOfficeId,
                                      StaffId = s.StaffId,
                                      DeskCount = _context.MyDesk.Where(x => x.StaffId == s.StaffId && x.HasWork == false).Count()
                                  };

                    if (getSaff.Any())
                    {
                        var minDeskCount = getSaff.Min(x => x.DeskCount);

                        List<States> state = new List<States>();

                        if ((getProccess.FirstOrDefault().Process == GeneralClass.DONE) || (getProccess.FirstOrDefault().Process == GeneralClass.START) || (getProccess.FirstOrDefault().Process == GeneralClass.NEXT) || (getProccess.FirstOrDefault().Process == GeneralClass.END) || (getProccess.FirstOrDefault().Process == GeneralClass.BEGIN))
                        {
                            state = _context.States.Where(x => x.StateName.Contains("LAGOS")).ToList();
                        }

                        List<int> staffFieldOffices = FieldOfficeStaff(state.FirstOrDefault().StateId);

                        foreach (var s in getSaff)
                        {
                            foreach (var fieldOffice in staffFieldOffices)
                            {
                                if (s.FieldOfficeId == fieldOffice && s.DeskCount == minDeskCount)
                                {
                                    result = s.StaffId;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }




        /*
         * Getting the list of application processes
         * 
         * AppStageID => un encrypted application stage ID
         * ProcessID => un encrypted process id already gotten
         * Sort => A particular sort for a particular stage
         * proces => Application process (START, NEXT, END, PASS) 
         */
        public List<ApplicationProccess> GetAppProcess(int AppStageID, int ProcessID = 0, int Sort = 0, string processes = null)
        {
            var process = (from c in _context.ApplicationProccess
                           where c.StageId == AppStageID && c.DeleteStatus == false
                           select c);

            if (ProcessID != 0)
            {
                process = process.Where(x => x.ProccessId == ProcessID);
            }
            else if (Sort != 0)
            {
                process = process.Where(x => x.Sort == Sort);
            }
            else if (ProcessID != 0 && Sort != 0)
            {
                process = process.Where(x => x.ProccessId == ProcessID && x.Sort == Sort);
            }
            else if (processes != null)
            {
                process = process.Where(x => x.Process == processes);
            }

            process = process.OrderBy(x => x.Sort);

            return process.ToList();
        }



       


        /*
         * Getting all field office from a particular zones
         * 
         * zonalID => integer of zonal office
         */
        public List<int> GetFieldOfficesFrromZones(int zonalID)
        {
            List<int> _fieldOffice = new List<int>();

            var fieldOffice = from zf in _context.ZoneFieldOffice
                              join z in _context.ZonalOffice on zf.ZoneId equals z.ZoneId
                              join f in _context.FieldOffices on zf.FieldOfficeId equals f.FieldOfficeId
                              where zf.ZoneId == zonalID && zf.DeleteStatus == false && z.DeleteStatus == false && f.DeleteStatus == false
                              select zf;

            if (fieldOffice.Any())
            {
                foreach (var f in fieldOffice)
                {
                    _fieldOffice.Add(f.FieldOfficeId);
                }
            }
            return _fieldOffice;
        }



        /*
         * Getting all zonal office from a field office ID
         * 
         * field_office => integer of field office
         */
        public int GetZonesFromFieldOffice(int field_office)
        {
            int zone = 0;
            var zones = from zf in _context.ZoneFieldOffice
                        join z in _context.ZonalOffice on zf.ZoneId equals z.ZoneId
                        join f in _context.FieldOffices on zf.FieldOfficeId equals f.FieldOfficeId
                        where zf.FieldOfficeId == field_office && zf.DeleteStatus == false && z.DeleteStatus == false && f.DeleteStatus == false
                        select zf;

            if (zones.Any())
            {
                zone = zones.FirstOrDefault().ZoneId;
            }
            return zone;
        }






    }

    
}