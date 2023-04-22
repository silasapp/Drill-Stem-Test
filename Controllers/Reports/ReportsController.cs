using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using DST.Models.DB;
using DST.Controllers.Configurations;
using DST.Helpers;
using static DST.Models.GeneralModel;
using DST.Controllers.Permits;

namespace DST.Controllers.Reports
{

    public class ReportsController : Controller
    {
        private readonly DST_DBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        private readonly HelpersController _helpersController;
        public GeneralClass generalClass = new GeneralClass();
        public PermitsController _permitsController;


        public ReportsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _permitsController = new PermitsController(_context, _httpContextAccessor, _configuration);
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reports.ToListAsync());
        }


        public IActionResult Applications()
        {
            List<FieldOffices> getFieldOffice = new List<FieldOffices>();
            List<ZonalOffice> getZonalOffice = new List<ZonalOffice>();

            var getType = _context.ApplicationType.Where(x => x.DeleteStatus == false);
            var getStage = _context.ApplicationStage.Where(x => x.DeleteStatus == false);


            List<SearchList> searchLists = new List<SearchList>
            {
                new SearchList
                {
                    types = getType.ToList(),
                    stages = getStage.ToList(),
                }
            };

            return View(searchLists.ToList());
        }




        public JsonResult ApplicationReport()
        {
            StringBuilder builder = new StringBuilder();

            string result = "";

            var oldTypeString = "";
            var oldStageString = "";
            var oldStatusString = "";
            var oldDateFromString = "";
            //var oldDateToString = "";
            var oldBothDateFromString = "";

            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var sortColumn = HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
            var txtSearch = HttpContext.Request.Form["search[value]"][0];

            var type = string.Join(",", (HttpContext.Request.Form["type[0][]"].ToList()));
            var stage = string.Join(",", (HttpContext.Request.Form["stage[0][]"].ToList()));
            var status = string.Join(",", (HttpContext.Request.Form["status[0][]"].ToList()));
           
            var dateFrom = HttpContext.Request.Form["dateFrom"].FirstOrDefault();
            var dateTo = HttpContext.Request.Form["dateTo"].FirstOrDefault();


            builder.Append("SELECT a.* " +
                "FROM Applications AS a " +
                "INNER JOIN Facilities As f ON a.facilityId = f.facilityId " +
                "INNER JOIN Companies As c ON a.companyid = c.companyid " +
                "INNER JOIN States AS st ON f.state = st.state_Id " +
                "INNER JOIN AppTypeStage AS ts ON a.AppTypeStageId = ts.TypeStageId " +
                "INNER JOIN ApplicationStage AS ag ON ts.AppStageID = ag.AppStageID " +
                "INNER JOIN ApplicationType AS at ON ts.AppTypeID = at.AppTypeID " +
                "WHERE (a.DeletedStatus = 0)");


            if (type != null && type.Any())
            {
                foreach (var p in type.Split(',').ToList())
                {
                    oldTypeString += "at.AppTypeID = '" + p + "' OR ";
                }

                string newTypeString = oldTypeString.Substring(0, oldTypeString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newTypeString + ")");
            }

            if (stage != null && stage.Any())
            {
                foreach (var p in stage.Split(',').ToList())
                {
                    oldStageString += "ag.AppStageID = '" + p + "' OR ";
                }

                string newStageString = oldStageString.Substring(0, oldStageString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newStageString + ")");
            }

            if (status != null && status.Any())
            {
                foreach (var p in status.Split(',').ToList())
                {
                    oldStatusString += "a.status = '" + p + "' OR ";
                }

                string newStatusString = oldStatusString.Substring(0, oldStatusString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newStatusString + ")");
            }

            if (dateFrom != "" && dateTo != "")
            {
                var toDate = Convert.ToDateTime(dateTo.Trim()).Date.ToString("yyyy-MM-dd");
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldBothDateFromString += "a.datesubmitted >= '" + fromDate + "' AND  a.datesubmitted <= '" + toDate.ToString() + "' OR ";

                string newBothDateFromString = oldBothDateFromString.Substring(0, oldBothDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newBothDateFromString + ")");
            }
            else if (dateFrom != "")
            {
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldDateFromString += "CAST(a.datesubmitted as date) = '" + fromDate + "' OR ";

                string newDateFromString = oldDateFromString.Substring(0, oldDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newDateFromString + ")");
            }


            var get = _context.Applications.FromSqlRaw(builder.ToString()).ToList();

            var query = (from g in get.AsEnumerable()
                         join f in _context.Facilities.AsEnumerable() on g.FacilityId equals f.FacilityId
                         join c in _context.Companies.AsEnumerable() on g.CompanyId equals c.CompanyId
                         join st in _context.States.AsEnumerable() on f.State equals st.StateId
                         join ts in _context.AppTypeStage.AsEnumerable() on g.AppTypeStageId equals ts.TypeStageId
                         join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                         join at in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals at.AppTypeId
                         where g.DeletedStatus == false
                         select new
                         {
                             g.AppId,
                             RefNo = g.AppRefNo,
                             Stage = s.StageName.ToUpper(),
                             Type = at.TypeName.ToUpper(),
                             State = st.StateName.ToUpper(),
                             f.Lga,
                             CompanyName = c.CompanyName.ToUpper(),
                             FacId = f.FacilityId,
                             Facilities = f.FacilityName.ToUpper(),
                             CurrentDesk = g.Status != GeneralClass.Processing ? "Company" : _context.Staff.Where(x => x.StaffId == g.CurrentDeskId).AsEnumerable().Select(x => new { fullname = x.LastName + " " + x.FirstName }).FirstOrDefault().fullname,
                             g.Status,
                             DateApplied = g.DateApplied == null ? "" : g.DateApplied.ToString(),
                             DateSubmitted = g.DateSubmitted == null ? "Not Submitted" : g.DateSubmitted.ToString(),
                             WellNames = _permitsController.GetWells(g.AppId),
                             ReserviorNames = _permitsController.GetReserviors(g.AppId),
                             FieldNames = _permitsController.GetFields(g.AppId),
                         });


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    query = sortColumn == "companyName" ? query.ToList().OrderByDescending(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderByDescending(c => c.Type) :
                               sortColumn == "stage" ? query.OrderByDescending(c => c.Stage) :
                               sortColumn == "status" ? query.OrderByDescending(c => c.Status) :
                               sortColumn == "dateApplied" ? query.OrderByDescending(c => c.DateApplied) :
                               sortColumn == "dateSubmitted" ? query.OrderByDescending(c => c.DateSubmitted) :
                               query.OrderByDescending(c => c.CompanyName);
                }
                else
                {
                    query = sortColumn == "companyName" ? query.OrderBy(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderBy(c => c.Type) :
                               sortColumn == "stage" ? query.OrderBy(c => c.Stage) :
                               sortColumn == "status" ? query.OrderBy(c => c.Status) :
                               sortColumn == "dateApplied" ? query.OrderBy(c => c.DateApplied) :
                               sortColumn == "dateSubmitted" ? query.OrderBy(c => c.DateSubmitted) :
                               query.OrderBy(c => c.CompanyName);
                }

            }

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                query = query.Where(c => c.RefNo.Contains(txtSearch.ToUpper()) || c.CompanyName.Contains(txtSearch.ToUpper()) || c.State.Contains(txtSearch.ToUpper()) || c.Facilities.Contains(txtSearch.ToUpper()) || c.Status.Contains(txtSearch.ToUpper()) || c.Type.Contains(txtSearch.ToUpper()) || c.Stage.Contains(txtSearch.ToUpper()));
            }

            totalRecords = query.ToList().Count();

            var data = query.Skip(skip).Take(pageSize).ToList().OrderBy(x => x.Type).ThenByDescending(x => x.AppId);
           
            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data, result = result });

        }




        public IActionResult TransactionReports()
        {
            var getType = _context.ApplicationType.Where(x => x.DeleteStatus == false);
            var getStage = _context.ApplicationStage.Where(x => x.DeleteStatus == false);

           
            List<SearchList> searchLists = new List<SearchList>
            {
                new SearchList
                {
                    types = getType.ToList(),
                    stages = getStage.ToList(),
                }
            };

            return View(searchLists.ToList());
        }




        public JsonResult TransactionReport()
        {
            StringBuilder builder = new StringBuilder();

            string result = "";

            var oldTypeString = "";
            var oldStageString = "";
            var oldStatusString = "";
            
            var oldDateFromString = "";
            var oldBothDateFromString = "";

            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var sortColumn = HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
            var txtSearch = HttpContext.Request.Form["search[value]"][0];

            var type = string.Join(",", (HttpContext.Request.Form["type[0][]"].ToList()));
            var stage = string.Join(",", (HttpContext.Request.Form["stage[0][]"].ToList()));
            var status = string.Join(",", (HttpContext.Request.Form["status[0][]"].ToList()));
           

            var dateFrom = HttpContext.Request.Form["dateFrom"].FirstOrDefault();
            var dateTo = HttpContext.Request.Form["dateTo"].FirstOrDefault();


            IFormatProvider culture = new CultureInfo("en-US", true);

            builder.Append("SELECT t.* " +
                "FROM Transactions AS t " +
                "INNER JOIN Applications As a ON t.appid = a.appid " +
                "INNER JOIN Facilities As f ON a.facilityId = f.facilityId " +
                "INNER JOIN Companies As c ON a.companyid = c.companyid " +
                "INNER JOIN States AS st ON f.state = st.state_Id " +
                "INNER JOIN AppTypeStage AS ts ON a.AppTypeStageId = ts.TypeStageId " +
                "INNER JOIN ApplicationStage AS ag ON ts.AppStageID = ag.AppStageID " +
                "INNER JOIN ApplicationType AS at ON ts.AppTypeID = at.AppTypeID " +
                "WHERE (a.DeletedStatus = 0)");


            if (type != null && type.Any())
            {
                foreach (var p in type.Split(',').ToList())
                {
                    oldTypeString += "at.AppTypeID = '" + p + "' OR ";
                }

                string newTypeString = oldTypeString.Substring(0, oldTypeString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newTypeString + ")");
            }

            if (stage != null && stage.Any())
            {
                foreach (var p in stage.Split(',').ToList())
                {
                    oldStageString += "ag.AppStageID = '" + p + "' OR ";
                }

                string newStageString = oldStageString.Substring(0, oldStageString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newStageString + ")");
            }

            if (status != null && status.Any())
            {
                foreach (var p in status.Split(',').ToList())
                {
                    oldStatusString += "t.TransactionStatus = '" + p + "' OR ";
                }

                string newStatusString = oldStatusString.Substring(0, oldStatusString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newStatusString + ")");
            }

            if (dateFrom != "" && dateTo != "")
            {
                var toDate = Convert.ToDateTime(dateTo.Trim()).Date.ToString("yyyy-MM-dd");
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldBothDateFromString += "t.TransactionDate >= '" + fromDate + "' AND  t.TransactionDate <= '" + toDate.ToString() + "' OR ";

                string newBothDateFromString = oldBothDateFromString.Substring(0, oldBothDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newBothDateFromString + ")");
            }
            else if (dateFrom != "")
            {
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldDateFromString += "CAST(t.TransactionDate as date) = '" + fromDate + "' OR ";

                string newDateFromString = oldDateFromString.Substring(0, oldDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newDateFromString + ")");
            }


            var get = _context.Transactions.FromSqlRaw(builder.ToString()).ToList();

            var query = from g in get.AsEnumerable()
                        join a in _context.Applications.AsEnumerable() on g.AppId equals a.AppId
                        join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                        join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                        join st in _context.States.AsEnumerable() on f.State equals st.StateId
                        join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                        join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                        join at in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals at.AppTypeId
                        where a.DeletedStatus == false
                        select new
                        {
                            TransId = g.TransactionId,
                            RefNo = a.AppRefNo,
                            RRR = g.Rrr,
                            Stage = s.StageName.ToUpper(),
                            Type = at.TypeName.ToUpper(),
                            State = st.StateName.ToUpper(),
                            Lga = f.Lga,
                            CompanyName = c.CompanyName.ToUpper(),
                            Facilities = f.FacilityName.ToUpper(),
                            Status = g.TransactionStatus,
                            TransDate = g.TransactionDate == null ? "" : g.TransactionDate.ToString(),
                            Amount = g.AmtPaid,
                            ServicCharge = g.ServiceCharge,
                            TotalAmount = g.TotalAmt,
                        };


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    query = sortColumn == "companyName" ? query.OrderByDescending(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderByDescending(c => c.Type) :
                               sortColumn == "stage" ? query.OrderByDescending(c => c.Stage) :
                               sortColumn == "status" ? query.OrderByDescending(c => c.Status) :
                               sortColumn == "transDate" ? query.OrderByDescending(c => c.TransDate) :
                               query.OrderByDescending(c => c.CompanyName);
                }
                else
                {
                    query = sortColumn == "companyName" ? query.OrderBy(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderBy(c => c.Type) :
                               sortColumn == "stage" ? query.OrderBy(c => c.Stage) :
                               sortColumn == "status" ? query.OrderBy(c => c.Status) :
                               sortColumn == "transDate" ? query.OrderBy(c => c.TransDate) :
                               query.OrderBy(c => c.CompanyName);
                }

            }

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                query = query.Where(c => c.RefNo.Contains(txtSearch.ToUpper()) || c.RRR.Contains(txtSearch.ToUpper()) || c.CompanyName.Contains(txtSearch.ToUpper()) || c.State.Contains(txtSearch.ToUpper()) || c.Facilities.Contains(txtSearch.ToUpper()) || c.Status.Contains(txtSearch.ToUpper()) || c.Type.Contains(txtSearch.ToUpper()) || c.Stage.Contains(txtSearch.ToUpper()));
            }

            totalRecords = query.Count();

            var data = query.OrderBy(x => x.Type).ThenByDescending(x => x.TransId).Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data, result = result });

        }




        public IActionResult PermitsReport()
        {
            var getType = _context.ApplicationType.Where(x => x.DeleteStatus == false);
            var getStage = _context.ApplicationStage.Where(x => x.DeleteStatus == false);
           

            List<SearchList> searchLists = new List<SearchList>
            {
                new SearchList
                {
                    types = getType.ToList(),
                    stages = getStage.ToList(),
                }
            };

            return View(searchLists.ToList());
        }



        public JsonResult PermitReports()
        {
            StringBuilder builder = new StringBuilder();

            string result = "";

            var oldTypeString = "";
            var oldStageString = "";

            var oldDateFromString = "";
            var oldBothDateFromString = "";

            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var sortColumn = HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
            var txtSearch = HttpContext.Request.Form["search[value]"][0];

            var type = string.Join(",", (HttpContext.Request.Form["type[0][]"].ToList()));
            var stage = string.Join(",", (HttpContext.Request.Form["stage[0][]"].ToList()));

            var dateFrom = HttpContext.Request.Form["dateFrom"].FirstOrDefault();
            var dateTo = HttpContext.Request.Form["dateTo"].FirstOrDefault();


            IFormatProvider culture = new CultureInfo("en-US", true);

            builder.Append("SELECT t.* " +
                "FROM Permits AS t " +
                "INNER JOIN Applications As a ON t.appid = a.appid " +
                "INNER JOIN Facilities As f ON a.facilityId = f.facilityId " +
                "INNER JOIN Companies As c ON a.companyid = c.companyid " +
                "INNER JOIN States AS st ON f.state = st.state_Id " +
                "INNER JOIN AppTypeStage AS ts ON a.AppTypeStageId = ts.TypeStageId " +
                "INNER JOIN ApplicationStage AS ag ON ts.AppStageID = ag.AppStageID " +
                "INNER JOIN ApplicationType AS at ON ts.AppTypeID = at.AppTypeID " +
                "WHERE (a.DeletedStatus = 0)");

           

            if (type != null && type.Any())
            {
                foreach (var p in type.Split(',').ToList())
                {
                    oldTypeString += "at.AppTypeID = '" + p + "' OR ";
                }

                string newTypeString = oldTypeString.Substring(0, oldTypeString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newTypeString + ")");
            }

            if (stage != null && stage.Any())
            {
                foreach (var p in stage.Split(',').ToList())
                {
                    oldStageString += "ag.AppStageID = '" + p + "' OR ";
                }

                string newStageString = oldStageString.Substring(0, oldStageString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newStageString + ")");
            }

          

            if (dateFrom != "" && dateTo != "")
            {
                var toDate = Convert.ToDateTime(dateTo.Trim()).Date.ToString("yyyy-MM-dd");
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldBothDateFromString += "t.issueddate >= '" + fromDate + "' AND  t.issueddate <= '" + toDate.ToString() + "' OR ";

                string newBothDateFromString = oldBothDateFromString.Substring(0, oldBothDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newBothDateFromString + ")");
            }
            else if (dateFrom != "")
            {
                var fromDate = Convert.ToDateTime(dateFrom.Trim()).Date.ToString("yyyy-MM-dd");

                oldDateFromString += "CAST(t.issueddate as date) = '" + fromDate + "' OR ";

                string newDateFromString = oldDateFromString.Substring(0, oldDateFromString.Trim().LastIndexOf("OR")).Trim();
                builder.Append(" AND (" + newDateFromString + ")");
            }


            var get = _context.Permits.FromSqlRaw(builder.ToString()).ToList();

            var query = from g in get.AsEnumerable()
                        join a in _context.Applications.AsEnumerable() on g.AppId equals a.AppId
                        join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                        join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                        join st in _context.States.AsEnumerable() on f.State equals st.StateId
                        join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                        join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                        join at in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals at.AppTypeId
                        join sf in _context.Staff.AsEnumerable() on g.ApprovedBy equals sf.StaffId
                        where a.DeletedStatus == false
                        select new
                        {
                            PermitId = g.PermitId,
                            PermitNo = g.PermitNo,
                            Stage = s.StageName.ToUpper(),
                            Type = at.TypeName.ToUpper(),
                            CompanyName = c.CompanyName.ToUpper(),
                            Facilities = f.FacilityName.ToUpper(),
                            IssuedDate = g.IssuedDate.ToShortDateString(),
                            ExpiryDate = g.ExpireDate.ToShortDateString(),
                            ApprovedBy = sf.LastName + " " + sf.FirstName
                        };


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    query = sortColumn == "companyName" ? query.OrderByDescending(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderByDescending(c => c.Type) :
                               sortColumn == "stage" ? query.OrderByDescending(c => c.Stage) :
                               sortColumn == "issuedDate" ? query.OrderByDescending(c => c.IssuedDate) :
                               sortColumn == "expiryDate" ? query.OrderByDescending(c => c.ExpiryDate) :
                               query.OrderByDescending(c => c.CompanyName);
                }
                else
                {
                    query = sortColumn == "companyName" ? query.OrderBy(c => c.CompanyName) :
                               sortColumn == "type" ? query.OrderBy(c => c.Type) :
                               sortColumn == "stage" ? query.OrderBy(c => c.Stage) :
                               sortColumn == "issuedDate" ? query.OrderBy(c => c.IssuedDate) :
                               sortColumn == "expiryDate" ? query.OrderBy(c => c.ExpiryDate) :
                               query.OrderBy(c => c.CompanyName);
                }

            }

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                query = query.Where(c => c.CompanyName.Contains(txtSearch.ToUpper()) || c.Facilities.Contains(txtSearch.ToUpper()) || c.Type.Contains(txtSearch.ToUpper()) || c.Stage.Contains(txtSearch.ToUpper()));
            }

            totalRecords = query.ToList().Count();

            var data = query.Skip(skip).Take(pageSize).ToList().OrderBy(x => x.Type).ThenByDescending(x => x.PermitId);
           
            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data, result = result });

        }




    }
}
