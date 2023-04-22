using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DST.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Helpers;
using DST.Controllers.Authentications;
using Microsoft.AspNetCore.Authorization;

namespace DST.Controllers.Configurations
{

   
    public class ZoneFieldOfficesController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public ZoneFieldOfficesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: ZoneFieldOffices
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.ZoneFieldOffice.ToListAsync());
        }



        
        public JsonResult GetZoneFieldOffice()
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

            var getZoneState = (from zf in _context.ZoneFieldOffice
                                join f in _context.FieldOffices on zf.FieldOfficeId equals f.FieldOfficeId
                                join z in _context.ZonalOffice on zf.ZoneId equals z.ZoneId
                                join zs in _context.ZoneStates on z.ZoneId equals zs.ZoneId
                                join s in _context.States on zs.StateId equals s.StateId
                                join c in _context.Countries on s.CountryId equals c.CountryId
                                where (zf.DeleteStatus == false && f.DeleteStatus == false && z.DeleteStatus == false && s.DeleteStatus == false && c.DeleteStatus == false && zs.DeleteStatus == false)
                                select new
                                {
                                    ZoneFieldOfficeID = zf.ZoneFieldOfficeId,
                                    CountryID = c.CountryId,
                                    StateId = s.StateId,
                                    ZoneId = zf.ZoneId,
                                    FieldOfficeID = f.FieldOfficeId,
                                    CountryName = c.CountryName,
                                    StateName = s.StateName,
                                    ZoneName = z.ZoneName,
                                    OfficeName = f.OfficeName,
                                    UpdatedAt = zf.UpdatedAt.ToString(),
                                    CreatedAt = zf.CreatedAt.ToString()
                                });


            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getZoneState = sortColumn == "countryName" ? getZoneState.OrderByDescending(s => s.CountryName) :
                                    sortColumn == "stateName" ? getZoneState.OrderByDescending(s => s.StateName) :
                                    sortColumn == "zoneName" ? getZoneState.OrderByDescending(s => s.ZoneName) :
                                    sortColumn == "officeName" ? getZoneState.OrderByDescending(s => s.OfficeName) :
                                    sortColumn == "updatedAt" ? getZoneState.OrderByDescending(s => s.UpdatedAt) :
                                    sortColumn == "createdAt" ? getZoneState.OrderByDescending(s => s.CreatedAt) :
                                    getZoneState.OrderByDescending(s => s.ZoneFieldOfficeID + " " + sortColumnDir);
                }
                else
                {
                    getZoneState = sortColumn == "countryName" ? getZoneState.OrderBy(c => c.CountryName) :
                                   sortColumn == "stateName" ? getZoneState.OrderBy(s => s.StateName) :
                                   sortColumn == "officeName" ? getZoneState.OrderBy(s => s.OfficeName) :
                                   sortColumn == "zoneName" ? getZoneState.OrderBy(s => s.ZoneName) :
                                   sortColumn == "updatedAt" ? getZoneState.OrderBy(c => c.UpdatedAt) :
                                   sortColumn == "createdAt" ? getZoneState.OrderBy(c => c.CreatedAt) :
                                   getZoneState.OrderBy(c => c.ZoneFieldOfficeID);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getZoneState = getZoneState.Where(c => c.CountryName.Contains(txtSearch.ToUpper()) || c.StateName.Contains(txtSearch.ToUpper()) || c.ZoneName.Contains(txtSearch.ToUpper()) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch) || c.OfficeName.Contains(txtSearch.ToUpper()));
            }

            totalRecords = getZoneState.Count();
            var data = getZoneState.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying all zonal field office.", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }




        // POST: ZoneFieldOffices/Create
        
        public async Task<IActionResult> CreateZoneFieldOffice(int ZoneID, int FieldOfficeID)
        {
            string response = "";

            var check = from zs in _context.ZoneFieldOffice
                        join f in _context.FieldOffices on zs.FieldOfficeId equals f.FieldOfficeId
                        join z in _context.ZonalOffice on zs.ZoneId equals z.ZoneId
                        where zs.ZoneId == ZoneID && zs.FieldOfficeId == FieldOfficeID && zs.DeleteStatus == false
                        select new
                        {
                            f.OfficeName,
                            z.ZoneName
                        };

            if (check.Any())
            {
                response = check.FirstOrDefault().OfficeName + " and " + check.FirstOrDefault().ZoneName + " relationship already exits.";
            }
            else
            {
                ZoneFieldOffice _zoneFieldOffice = new ZoneFieldOffice()
                {
                    ZoneId = ZoneID,
                    FieldOfficeId = FieldOfficeID,
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.ZoneFieldOffice.Add(_zoneFieldOffice);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "ZoneFieldOffice Created";
                }
                else
                {
                    response = "Something went wrong trying to create Zone and Field Office relationship. Please try again.";
                }
            }
            _helpersController.LogMessages("Creating new zonal field office. Status : " + response + " zonal office ID : " + ZoneID + " field office ID : " + FieldOfficeID, _helpersController.getSessionEmail());

            return Json(response);
        }





        // POST: ZoneFieldOffices/Edit/5
        
        public async Task<IActionResult> EditZoneFieldOffice(int ZoneID, int FieldOfficeID, int ZoneFieldOfficeID)
        {
            string response = "";
            var getZoneFieldOffice = from x in _context.ZoneFieldOffice where x.ZoneFieldOfficeId == ZoneFieldOfficeID select x;

            if (getZoneFieldOffice.FirstOrDefault().FieldOfficeId == FieldOfficeID && getZoneFieldOffice.FirstOrDefault().ZoneId == ZoneID)
            {
                response = "This relationship already exits. Try a different one.";
            }
            else
            {
                getZoneFieldOffice.FirstOrDefault().FieldOfficeId = FieldOfficeID;
                getZoneFieldOffice.FirstOrDefault().ZoneId = ZoneID;
                getZoneFieldOffice.FirstOrDefault().UpdatedAt = DateTime.Now;
                getZoneFieldOffice.FirstOrDefault().DeleteStatus = false;

                int updated = await _context.SaveChangesAsync();

                if (updated > 0)
                {
                    response = "ZoneFieldOffice Updated";
                }
                else
                {
                    response = "Nothing was updated.";
                }
            }

            _helpersController.LogMessages("Updating zonal field office. Status : " + response + " Zonal Office ID : " + ZoneID + " field office ID : " + FieldOfficeID, _helpersController.getSessionEmail());

            return Json(response);
        }



        // POST: ZoneFieldOffices/Delete/5
        
        public async Task<IActionResult> DeleteZoneFieldOffice(int ZoneFieldOfficeID)
        {
            string response = "";

            var getZoneFieldOffice = from c in _context.ZoneFieldOffice where c.FieldOfficeId == ZoneFieldOfficeID select c;

            getZoneFieldOffice.FirstOrDefault().DeletedAt = DateTime.Now;
            getZoneFieldOffice.FirstOrDefault().UpdatedAt = DateTime.Now;
            getZoneFieldOffice.FirstOrDefault().DeleteStatus = true;
            getZoneFieldOffice.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "ZoneFieldOffice Deleted";
            }
            else
            {
                response = "Zone => Field Office not deleted. Something went wrong trying to delete this entry.";
            }

            _helpersController.LogMessages("Deleting zonal field office. Status : " + response + " Zonal field Office ID : " + ZoneFieldOfficeID, _helpersController.getSessionEmail());

            return Json(response);
        }

    }

   
}
