using LpgLicense.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace DST.Helpers
{
    public class GeneralClass : Controller
    {
        RestSharpServices restSharpServices = new RestSharpServices();
       
        public static string Approved = "Approved";
        public static string Rejected = "Rejected";
        public static string PaymentPending = "Payment Pending";
        public static string PaymentCompleted = "Payment Completed";
        public static string Processing = "Processing";
        public static string ResultSubmitted = "Result Submitted";
        public static string DocumentsRequired = "Documents Required";
        public static string DocumentsUploaded = "Documents Uploaded";
        public static string DSTCode = "800";
        public static string DISAPPROVE = "Disapproved";
        public static string Withdrawn = "Withdrawn";
        public static int elpsStateID = 0;

        public static string _WAITING = "WAITING";
        public static string _STARTED = "STARTED";
        public static string _FINISHED = "FINISHED";

        public static string START = "START";
        public static string NEXT = "NEXT";
        public static string END = "END";
        public static string PASS = "PASS";
        public static string DONE = "DONE";
        public static string BEGIN = "BEGIN";


        public static string MER = "MAXIMUM EFFICIENT RATE (MER)";
        public static string EWT = "EXTENDED WELL TEST (EWT)";
        public static string DSTs = "DRILL STEM TEST (DST)";


        public static string NEWT = "N-EWT";
        public static string EEWT = "E-EWT";
        public static string RMER = "R-MER";
        public static string OCMER = "OC-MER";
        public static string DST = "DST";
        public static string RTAR = "RTAR";
        public static string OTAR = "OTAR";
        public static string TARR = "TAR-R";



        public static string AD_UMR = "AD RM";
        public static string HEAD_UMR = "HEAD UMR";
        public static string SECTION_HEAD = "MANAGER RS";
        public static string TEAM = "TEAM";
        public static string HOOD = "HOOD";
        public static string ZOPSCON = "ZOPSCON";
        public static string OPSCON = "OPSCON";
        public static string UMR_ZONE_HEAD = "UMR ZONE HEAD";
        public static string COMPANY = "COMPANY";
        public static string SUPER_ADMIN = "SUPER ADMIN";
        public static string ADMIN = "ADMIN";
        public static string ICT_ADMIN = "ICT ADMIN";
        public static string DIRECTOR = "DIRECTOR";
        public static string SUPPORT = "SUPPORT";
       
        
        
        // Sending eamil parameters
        public static string DEFAULT_MESSAGE = "DEFAULT MESSAGE";
        public static string STAFF_NOTIFY = "STAFF NOTIFY";
        public static string COMPANY_NOTIFY = "COMPANY NOTIFY";


        public static string staff_application_report_fac_doc = "ATTACHED APLLICATION REPORT FOR STAFF"; // facility doc
        public static string staff_exercise_report_fac_doc = "STAFF REPORT FOR PARTICIPATING IN EXERCISE (WELL TEST)"; // facility doc
        public static string end_of_welltest_fac_doc = "COMPANY END OF WELL TEST REPORT"; // facility doc


        private Object lockThis = new object();
        

        public string Encrypt(string clearText)
        {
            try
            {
                byte[] b = ASCIIEncoding.ASCII.GetBytes(clearText);
                string crypt = Convert.ToBase64String(b);
                byte[] c = ASCIIEncoding.ASCII.GetBytes(crypt);
                string encrypt = Convert.ToBase64String(c);

                return encrypt;
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }



        public string Decrypt(string cipherText)
        {
            try
            {
                byte[] b;
                byte[] c;
                string decrypt;
                b = Convert.FromBase64String(cipherText);
                string crypt = ASCIIEncoding.ASCII.GetString(b);
                c = Convert.FromBase64String(crypt);
                decrypt = ASCIIEncoding.ASCII.GetString(c);

                return decrypt;
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }


         /* Decrypting all ID
         *
         * ids => encrypted ids
         */ 
        public int DecryptIDs(string ids)
        {
            int id = 0;
            var ID = this.Decrypt(ids);

            if (ID == "Error")
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(ID);
            }

            return id;
        }




        public JsonResult RestResult(string url, string method, List<ParameterData> parameterData = null, object app_object = null, string output = null, string endUrl = null)
        {
            AppModels appModel = new AppModels();

            var response = restSharpServices.Response("/api/" + url + "/{email}/{apiHash}" + endUrl, parameterData, method, app_object);

            if (response.ErrorException != null)
            {
                return Json("Network Error");
            }
            else
            {
                if(method == "POST" || method == "PUT" || method == "DELETE")
                {
                    if (!string.IsNullOrWhiteSpace(response.Content))
                    {
                        return Json(output);
                    }
                    else
                    {
                        return Json("Opps... an error occured, please try again. " + response.ErrorMessage);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(response.Content))
                    {
                        return Json(JsonConvert.DeserializeObject(response.Content));
                    }
                    else
                    {
                        return Json("Opps... an error occured, please try again. " + response.ErrorMessage);
                    }
                }
            }
        }


        public List<FacilityDocument> getFacilityDocuments(string facilityID)
        {
            List<FacilityDocument> facilityDocuments = new List<FacilityDocument>();

            var paramData = restSharpServices.parameterData("id", facilityID);
            var response = restSharpServices.Response("/api/FacilityFiles/{id}/{email}/{apiHash}", paramData); // GET

            if (response.IsSuccessful == false)
            {
                facilityDocuments = null;
            }
            else
            {
                facilityDocuments = JsonConvert.DeserializeObject<List<FacilityDocument>>(response.Content);
            }

            return facilityDocuments;
        }



        public List<Document> getCompanyDocuments(string companyID)
        {
            List<Document> documents = new List<Document>();

            var paramData = restSharpServices.parameterData("id", companyID);
            var response = restSharpServices.Response("/api/CompanyDocuments/{id}/{email}/{apiHash}", paramData); // GET

            if (response.IsSuccessful == false)
            {
                documents = null;
            }
            else
            {
                documents = JsonConvert.DeserializeObject<List<Document>>(response.Content);
            }
            return documents;
        }




        public int GetStatesFromCountry(string State)
        {
            var paramData2 = restSharpServices.parameterData("Id", "156");
            var response2 = restSharpServices.Response("/api/Address/states/{Id}/{email}/{apiHash}", paramData2); // GET

            var res2 = JsonConvert.DeserializeObject<List<LpgLicense.Models.State>>(response2.Content);

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string state = textInfo.ToTitleCase(State.ToLower());

            foreach (var s in res2)
            {
                if (s.Name.Contains(state))
                {
                    elpsStateID = s.Id;
                    break;
                }
            }

            return elpsStateID;
        }



        //Generating Application number
        public string Generate_Application_Number()
        {
            lock (lockThis)
            {
                Thread.Sleep(1000);
                return DSTCode + DateTime.Now.ToString("MMddyyHHmmss");
            }
        }



        public string Generate_Receipt_Number()
        {
            lock (lockThis)
            {
                Thread.Sleep(1000);
                return DateTime.Now.ToString("HHmmss");
            }
        }


       

        public string GetStateShortName(string state, string code)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>()
            {
                {"Abia", "AB" }, 
                {"Adamawa", "AD" }, 
                {"Akwa Ibom", "AK" }, 
                {"Anambra", "AN" }, 
                {"Bauchi", "BA" }, 
                {"Bayelsa", "BY" }, 
                {"Benue", "BE" }, 
                {"Borno", "BO" }, 
                {"Cross River", "CR" }, 
                {"Delta", "DE" }, 
                {"Ebonyi", "EB" }, 
                {"Edo", "ED" }, 
                {"Enugu", "EN" }, 
                {"Federal Capital Territory", "FC" }, 
                {"Abuja", "FC" }, 
                {"Gombe", "GO" }, 
                {"Imo", "IM" }, 
                {"Jigawa", "JI" }, 
                {"Kaduna", "KD" }, 
                {"Kano", "KN" }, 
                {"Katsina", "KT" }, 
                {"Kebbi", "KE" }, 
                {"Kogi", "KO" }, 
                {"Kwara", "KW" }, 
                {"Lagos", "LA" }, 
                {"Nasarawa", "NA" }, 
                {"Niger", "NI" }, 
                {"Ogun", "OG" }, 
                {"Ondo", "ON" }, 
                {"Osun", "OS" }, 
                {"Oyo", "OY" }, 
                {"Plateau", "PL" }, 
                {"Rivers", "RI" }, 
                {"Sokoto", "SO" }, 
                {"Taraba", "TA" }, 
                {"Yobe", "YO" }, 
                {"Zamfara", "ZA" }, 
            };
            var shortState = pairs.Where(x => x.Key.ToUpper() == state.ToUpper().Trim()).FirstOrDefault();
            var CompanyCode = "WELL-" + code.Trim() + "-" + shortState.Value;
            return CompanyCode;
        }




       
    }
}

    

