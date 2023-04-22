using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Models.DB;
using DST.Helpers;
using DST.Controllers.Configurations;

namespace LPG_Depot.Controllers.Configurations
{
    [Authorize]
    public class AppTypeWithStageController : Controller
    {
        private readonly DST_DBContext _context;
        GeneralClass generalClass = new GeneralClass();
        HelpersController helpers;
        IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;

        public AppTypeWithStageController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            helpers = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: AppTypeWithStage
        public IActionResult Index()
        {
            return View();
        }


        public JsonResult GetTypeStage()
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

            var get = from ts in _context.AppTypeStage
                      join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                      join t in _context.ApplicationType on ts.AppTypeId equals t.AppTypeId
                      where ts.DeleteStatus == false && s.DeleteStatus == false && t.DeleteStatus == false
                      select new
                      {
                          TypeStageID = ts.TypeStageId,
                          TypeID = t.AppTypeId,
                          TypeName = t.TypeName,
                          StageID = s.AppStageId,
                          Counter = ts.Counter,
                          StageName = s.StageName,
                          ShortName = s.ShortName,
                          ServiceCharge = s.ServiceCharge,
                          StageAmount = s.Amount,
                          UpdatedAt = ts.UpdatedAt.ToString(),
                          CreatedAt = ts.CreatedAt.ToString()
                      };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    get = sortColumn == "stageName" ? get.OrderByDescending(s => s.StageName) :
                                    sortColumn == "typeName" ? get.OrderByDescending(t => t.TypeName) :
                                    sortColumn == "updatedAt" ? get.OrderByDescending(s => s.UpdatedAt) :
                                    sortColumn == "counter" ? get.OrderByDescending(s => s.Counter) :
                                    sortColumn == "shortName" ? get.OrderByDescending(s => s.ShortName) :
                                    sortColumn == "stageAmount" ? get.OrderByDescending(s => s.StageAmount) :

                                    sortColumn == "createdAt" ? get.OrderByDescending(s => s.CreatedAt) :
                                    get.OrderByDescending(ts => ts.TypeStageID + " " + sortColumnDir);
                }
                else
                {
                    get = sortColumn == "stageName" ? get.OrderBy(s => s.StageName) :
                                   sortColumn == "typeName" ? get.OrderBy(t => t.TypeName) :
                                   sortColumn == "counter" ? get.OrderBy(t => t.Counter) :
                                   sortColumn == "shortName" ? get.OrderBy(t => t.ShortName) :
                                   sortColumn == "stageAmount" ? get.OrderBy(t => t.StageAmount) :
                                   sortColumn == "updatedAt" ? get.OrderBy(c => c.UpdatedAt) :
                                   sortColumn == "createdAt" ? get.OrderBy(c => c.CreatedAt) :
                                   get.OrderBy(ts => ts.TypeStageID);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                get = get.Where(c => c.StageName.Contains(txtSearch.ToUpper()) || c.TypeName.Contains(txtSearch.ToUpper()));
            }

            totalRecords = get.Count();
            var data = get.Skip(skip).Take(pageSize).ToList();

            helpers.LogMessages("Displaying all application stage and type", helpers.getSessionEmail());


            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }


        // Creating Type and Stage 
        public async Task<IActionResult> CreateTypeStage(int TypeID, int StageID, int Counter)
        {
            string response = "";

            var check = from ts in _context.AppTypeStage
                        join t in _context.ApplicationType on ts.AppTypeId equals t.AppTypeId
                        join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                        where ts.AppTypeId == TypeID && ts.AppStageId == StageID && ts.DeleteStatus == false
                        select new
                        {
                            s.StageName,
                            t.TypeName
                        };

            if (check.Any())
            {
                response = check.FirstOrDefault().StageName + " and " + check.FirstOrDefault().TypeName + " relationship already exits.";
            }
            else
            {
                AppTypeStage _typeStage = new AppTypeStage()
                {
                    AppTypeId = TypeID,
                    AppStageId = StageID,
                    Counter = Counter,
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.AppTypeStage.Add(_typeStage);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "TypeState Created";
                }
                else
                {
                    response = "Something went wrong trying to create Type and Stage relationship. Please try again.";
                }
            }

            helpers.LogMessages("Creating new application stage and type. Status : " + response + " Application type ID : " + TypeID + "Application stage ID : " + StageID, helpers.getSessionEmail());

            return Json(response);
        }


        // Eidt Type Stage 
        public async Task<IActionResult> EditTypeStage(int TypeStageID, int TypeID, int StageID, int Counter)
        {
            string response = "";
            var check = from x in _context.AppTypeStage where x.TypeStageId == TypeStageID select x;

            if (check.FirstOrDefault().AppTypeId == TypeID && check.FirstOrDefault().AppStageId == StageID && check.FirstOrDefault().Counter == Counter)
            {
                response = "This relationship already exits. Try a different one.";
            }
            else
            {
                check.FirstOrDefault().AppTypeId = TypeID;
                check.FirstOrDefault().AppStageId = StageID;
                check.FirstOrDefault().Counter = Counter;
                check.FirstOrDefault().UpdatedAt = DateTime.Now;
                check.FirstOrDefault().DeleteStatus = false;

                int updated = await _context.SaveChangesAsync();

                if (updated > 0)
                {
                    response = "TypeStage Updated";
                }
                else
                {
                    response = "Nothing was updated.";
                }
            }

            helpers.LogMessages("Updating application stage and type. Status : " + response + " Application type ID : " + TypeID + "Application stage ID : " + StageID, helpers.getSessionEmail());

            return Json(response);
        }


        // Delete Type Stage
        public async Task<IActionResult> DeleteTypeStage(int TypeStageID)
        {
            string response = "";

            var get = from c in _context.AppTypeStage where c.TypeStageId == TypeStageID select c;

            get.FirstOrDefault().DeletedAt = DateTime.Now;
            get.FirstOrDefault().UpdatedAt = DateTime.Now;
            get.FirstOrDefault().DeleteStatus = true;
            get.FirstOrDefault().DeletedBy = helpers.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "TypeStage Deleted";
            }
            else
            {
                response = "Type => Stage not deleted. Something went wrong trying to delete this entry.";
            }

            helpers.LogMessages("Deleting application stage and type. Status : " + response + " Application typeStage ID : " + TypeStageID, helpers.getSessionEmail());

            return Json(response);
        }

        
    }
}
