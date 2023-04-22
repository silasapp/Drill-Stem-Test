using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using DST.Models.DB;
using DST.Helpers;

namespace DST.Controllers.Configurations
{

    public class ApplicationStagesController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public ApplicationStagesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }


        // GET: ApplicationStages
       
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationStage.ToListAsync());
        }


        
        public JsonResult GetAppStages()
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

            var getAppStage = from r in _context.ApplicationStage
                              where r.DeleteStatus == false
                              select new
                              {
                                  AppStageID = r.AppStageId,
                                  StageName = r.StageName,
                                  ShortName = r.ShortName,
                                  StageAmount = r.Amount,
                                  ServiceCharge = r.ServiceCharge,
                                  UpdatedAt = r.UpdatedAt.ToString(),
                                  CreatedAt = r.CreatedAt.ToString(),
                              };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getAppStage = sortColumn == "stageName" ? getAppStage.OrderByDescending(c => c.StageName) :
                               sortColumn == "updatedAt" ? getAppStage.OrderByDescending(c => c.UpdatedAt) :
                               sortColumn == "shortName" ? getAppStage.OrderByDescending(c => c.ShortName) :
                               sortColumn == "stageAmount" ? getAppStage.OrderByDescending(c => c.StageAmount) :
                               sortColumn == "createdAt" ? getAppStage.OrderByDescending(c => c.CreatedAt) :
                               getAppStage.OrderByDescending(c => c.AppStageID + " " + sortColumnDir);
                }
                else
                {
                    getAppStage = sortColumn == "stageName" ? getAppStage.OrderBy(c => c.StageName) :
                               sortColumn == "updatedAt" ? getAppStage.OrderBy(c => c.UpdatedAt) :
                               sortColumn == "shortName" ? getAppStage.OrderBy(c => c.ShortName) :
                               sortColumn == "stageAmount" ? getAppStage.OrderBy(c => c.StageAmount) :
                               sortColumn == "createdAt" ? getAppStage.OrderBy(c => c.CreatedAt) :
                               getAppStage.OrderBy(c => c.AppStageID);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getAppStage = getAppStage.Where(c => c.StageName.Contains(txtSearch.ToUpper()) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch));
            }

            totalRecords = getAppStage.Count();
            var data = getAppStage.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying application stages.", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });
        }





        // POST: ApplicationStages/Create
       
        public async Task<IActionResult> CreateAppStage(string AppStageName, string AppShortName, int StageAmount, int ServiceCharge)
        {
            string response = "";

            var appstage = from a in _context.ApplicationStage
                           where a.StageName == AppStageName.ToUpper() && a.DeleteStatus == false
                           select a;

            if (appstage.Any())
            {
                response = "Application stage already exits, please enter another stage.";
            }
            else
            {
                ApplicationStage con = new ApplicationStage()
                {
                    StageName = AppStageName.ToUpper(),
                    ShortName = AppShortName.ToUpper(),
                    ServiceCharge = ServiceCharge,
                    Amount = StageAmount,
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.ApplicationStage.Add(con);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "AppStage Created";
                }
                else
                {
                    response = "Something went wrong trying to create this App Stage. Please try again.";
                }
            }

            _helpersController.LogMessages("Creating application stage. Status : " + response + " Application stage Name : " + AppStageName, _helpersController.getSessionEmail());

            return Json(response);
        }





        // POST: ApplicationStages/Edit/5
       
        public async Task<IActionResult> EditAppStage(int AppStageID, string AppStageName, string AppShortName, int StageAmount, int ServiceCharge)
        {
            string response = "";
            var getAppStage = from c in _context.ApplicationStage where c.AppStageId == AppStageID select c;

            getAppStage.FirstOrDefault().StageName = AppStageName.ToUpper();
            getAppStage.FirstOrDefault().ServiceCharge = ServiceCharge;
            getAppStage.FirstOrDefault().ShortName = AppShortName;
            getAppStage.FirstOrDefault().Amount = StageAmount;
            getAppStage.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppStage.FirstOrDefault().DeleteStatus = false;

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppStage Updated";
            }
            else
            {
                response = "Nothing was updated.";
            }

            _helpersController.LogMessages("Updating application stage. Status : " + response + " Application Stage ID : " + AppStageID, _helpersController.getSessionEmail());

            return Json(response);
        }



        // POST: ApplicationStages/Delete/5
       
        public async Task<IActionResult> DeleteAppStage(int AppStageID)
        {
            string response = "";

            var getAppStage = from c in _context.ApplicationStage where c.AppStageId == AppStageID select c;

            getAppStage.FirstOrDefault().DeletedAt = DateTime.Now;
            getAppStage.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppStage.FirstOrDefault().DeleteStatus = true;
            getAppStage.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppStage Deleted";
            }
            else
            {
                response = "Application stage not deleted. Something went wrong trying to delete this application stage.";
            }

            _helpersController.LogMessages("Deleting application stage. Status : " + response + " Application stage ID : " + AppStageID, _helpersController.getSessionEmail());

            return Json(response);
        }


    }
}
