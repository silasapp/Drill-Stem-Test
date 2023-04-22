using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Models.DB;
using DST.Helpers;
using DST.Controllers.Configurations;

namespace LPG_Depot.Controllers
{
    [Authorize]
    public class ApplicationTypesController : Controller
    {
        private readonly DST_DBContext _context;
        GeneralClass generalClass = new GeneralClass();
        HelpersController helpers;
        IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;

        public ApplicationTypesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            helpers = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: ApplicationTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationType.ToListAsync());
        }



        public JsonResult GetAppType()
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

            var getAppType = from r in _context.ApplicationType
                              where r.DeleteStatus == false
                              select new
                              {
                                  AppTypeID = r.AppTypeId,
                                  TypeName = r.TypeName,
                                  UpdatedAt = r.UpdatedAt.ToString(),
                                  CreatedAt = r.CreatedAt.ToString()
                              };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getAppType = sortColumn == "typeName" ? getAppType.OrderByDescending(c => c.TypeName) :
                               sortColumn == "updatedAt" ? getAppType.OrderByDescending(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getAppType.OrderByDescending(c => c.CreatedAt) :
                               getAppType.OrderByDescending(c => c.AppTypeID + " " + sortColumnDir);
                }
                else
                {
                    getAppType = sortColumn == "typeName" ? getAppType.OrderBy(c => c.TypeName) :
                               sortColumn == "updatedAt" ? getAppType.OrderBy(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getAppType.OrderBy(c => c.CreatedAt) :
                               getAppType.OrderBy(c => c.AppTypeID);
                }

            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getAppType = getAppType.Where(c => c.TypeName.Contains(txtSearch.ToUpper()) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch));
            }

            totalRecords = getAppType.Count();
            var data = getAppType.Skip(skip).Take(pageSize).ToList();

            helpers.LogMessages("Displaying application types.",helpers.getSessionEmail());


            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }





        // POST: ApplicationTypes/Create
        public async Task<IActionResult> CreateAppType(string AppTypeName)
        {
            string response = "";

            var getAppType = from a in _context.ApplicationType
                           where a.TypeName == AppTypeName.ToUpper() && a.DeleteStatus == false
                           select a;

            if (getAppType.Any())
            {
                response = "Application type already exits, please enter another type.";
            }
            else
            {
                ApplicationType _appType = new ApplicationType()
                {
                    TypeName = AppTypeName.ToUpper(),
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.ApplicationType.Add(_appType);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "AppType Created";
                }
                else
                {
                    response = "Something went wrong trying to create this App Type. Please try again.";
                }
            }

            helpers.LogMessages("Creating application types. Status : " + response + " Application type name : " + AppTypeName, helpers.getSessionEmail());

            return Json(response);
        }



        // POST: ApplicationTypes/Edit/5
        public async Task<IActionResult> EditAppType(int AppTypeID, string AppTypeName)
        {
            string response = "";
            var getAppType = from c in _context.ApplicationType where c.AppTypeId == AppTypeID select c;

            getAppType.FirstOrDefault().TypeName = AppTypeName.ToUpper();
            getAppType.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppType.FirstOrDefault().DeleteStatus = false;

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppType Updated";
            }
            else
            {
                response = "Nothing was updated.";
            }

            helpers.LogMessages("Updating application types. Status : " + response + " Application types ID : " + AppTypeID, helpers.getSessionEmail());

            return Json(response);
        }




        // POST: ApplicationTypes/Delete/5
        public async Task<IActionResult> DeleteAppType(int AppTypeID)
        {
            string response = "";

            var getAppType = from c in _context.ApplicationType where c.AppTypeId == AppTypeID select c;

            getAppType.FirstOrDefault().DeletedAt = DateTime.Now;
            getAppType.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppType.FirstOrDefault().DeleteStatus = true;
            getAppType.FirstOrDefault().DeletedBy = helpers.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppType Deleted";
            }
            else
            {
                response = "Application type not deleted. Something went wrong trying to delete this application type.";
            }

            helpers.LogMessages("Deleting application type. Status : " + response + " Application type ID : " + AppTypeID, helpers.getSessionEmail());

            return Json(response);
        }

        
    }
}
