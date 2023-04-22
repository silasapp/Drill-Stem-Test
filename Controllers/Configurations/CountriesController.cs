using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Helpers;
using DST.Models.DB;

namespace DST.Controllers.Configurations
{

    public class CountriesController : Controller
    {

        private readonly DST_DBContext _context;
        GeneralClass generalClass = new GeneralClass();
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;

        public CountriesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



       
        public JsonResult GetCountries()
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

            var getCountries = from c in _context.Countries
                               where c.DeleteStatus == false
                               select new
                               {
                                   CountryId = c.CountryId,
                                   CountryName = c.CountryName,
                                   UpdatedAt = c.UpdatedAt.ToString(),
                                   CreatedAt = c.CreatedAt.ToString()
                               };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getCountries = sortColumn == "countryName" ? getCountries.OrderByDescending(c => c.CountryName) :
                               sortColumn == "updatedAt" ? getCountries.OrderByDescending(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getCountries.OrderByDescending(c => c.CreatedAt) :
                               getCountries.OrderByDescending(c => c.CountryId + " " + sortColumnDir);
                }
                else
                {
                    getCountries = sortColumn == "countryName" ? getCountries.OrderBy(c => c.CountryName) :
                               sortColumn == "updatedAt" ? getCountries.OrderBy(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getCountries.OrderBy(c => c.CreatedAt) :
                               getCountries.OrderBy(c => c.CountryId);
                }

            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getCountries = getCountries.Where(c => c.CountryName.Contains(txtSearch.ToUpper()) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch));
            }

            totalRecords = getCountries.Count();
            var data = getCountries.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying all countries...", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }



        // GET: Countries
     
        public ActionResult Index()
        {
            return View();
        }



       
        public async Task<IActionResult> CreateCountry(string Country)
        {
            string response = "";

            var country = from c in _context.Countries
                          where c.CountryName == Country.ToUpper() && c.DeleteStatus == false
                          select c;

            if (country.Any())
            {
                response = "Country already exits, please enter another country.";
            }
            else
            {
                Countries con = new Countries()
                {
                    CountryName = Country.ToUpper(),
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.Countries.Add(con);
                int CountryCreated = await _context.SaveChangesAsync();

                if (CountryCreated > 0)
                {
                    response = "Country Created";
                }
                else
                {
                    response = "Something went wrong trying to create country. Please try again.";
                }
            }

            _helpersController.LogMessages("Creating new country. Status : " + response + " Country name : " + Country, _helpersController.getSessionEmail());

            return Json(response);
        }



   
        public async Task<IActionResult> EditCountry(int CountryId, string Country)
        {
            string response = "";
            var getCountry = from c in _context.Countries where c.CountryId == CountryId select c;

            getCountry.FirstOrDefault().CountryName = Country.ToUpper();
            getCountry.FirstOrDefault().UpdatedAt = DateTime.Now;
            getCountry.FirstOrDefault().DeleteStatus = false;

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "Country Updated";
            }
            else
            {
                response = "Nothing was updated.";
            }

            _helpersController.LogMessages("Updating country. Status : " + response + " New Country : " + Country, _helpersController.getSessionEmail());

            return Json(response);
        }



       
        public async Task<IActionResult> DeleteCountry(int CountryId)
        {
            string response = "";
            var getCountry = from c in _context.Countries where c.CountryId == CountryId select c;

            getCountry.FirstOrDefault().DeleteStatus = true;
            getCountry.FirstOrDefault().DeletedAt = DateTime.Now;
            getCountry.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "Country Deleted";
            }
            else
            {
                response = "Nothing was Deleted.";
            }

            _helpersController.LogMessages("Updating country. Status : " + response + " New Country : " + getCountry.FirstOrDefault().CountryName, _helpersController.getSessionEmail());

            return Json(response);
        }


    }
}