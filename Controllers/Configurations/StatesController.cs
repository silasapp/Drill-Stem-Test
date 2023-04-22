using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DST.Models.DB;
using Microsoft.AspNetCore.Http;
using DST.Helpers;
using Microsoft.Extensions.Configuration;
using DST.Controllers.Authentications;
using Microsoft.AspNetCore.Authorization;

namespace DST.Controllers.Configurations
{

   
    public class StatesController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public StatesController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



        // GET: States
        
        public IActionResult Index()
        {
            return View();
        }



       
        public JsonResult GetStates()
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

            var getStates = from s in _context.States
                            join c in _context.Countries on s.CountryId equals c.CountryId
                            where s.DeleteStatus == false && c.DeleteStatus == false
                            select new
                            {
                                CountryId = c.CountryId,
                                StateId = s.StateId,
                                CountryName = c.CountryName,
                                StateName = s.StateName,
                                UpdatedAt = s.UpdatedAt.ToString(),
                                CreatedAt = s.CreatedAt.ToString()
                            };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getStates = sortColumn == "countryName" ? getStates.OrderByDescending(s => s.CountryName) :
                               sortColumn == "stateName" ? getStates.OrderByDescending(s => s.StateName) :
                               sortColumn == "updatedAt" ? getStates.OrderByDescending(s => s.UpdatedAt) :
                               sortColumn == "createdAt" ? getStates.OrderByDescending(s => s.CreatedAt) :
                               getStates.OrderByDescending(s => s.StateId + " " + sortColumnDir);
                }
                else
                {
                    getStates = sortColumn == "countryName" ? getStates.OrderBy(c => c.CountryName) :
                                sortColumn == "stateName" ? getStates.OrderBy(s => s.StateName) :
                               sortColumn == "updatedAt" ? getStates.OrderBy(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getStates.OrderBy(c => c.CreatedAt) :
                               getStates.OrderBy(c => c.StateId);
                }

            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getStates = getStates.Where(c => c.CountryName.Contains(txtSearch.ToUpper()) || c.StateName.Contains(txtSearch.ToUpper()) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch));
            }

            totalRecords = getStates.Count();
            var data = getStates.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying all States", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }


        // POST: States/Create
       
        public async Task<IActionResult> CreateState(int CountryID, string StateName)
        {
            string response = "";

            var country = from s in _context.States
                          where s.CountryId == CountryID && s.StateName == StateName.ToUpper() && s.DeleteStatus == false
                          select s;

            if (country.Any())
            {
                response = "State already exits, please enter another state.";
            }
            else
            {
                States states = new States()
                {
                    CountryId = CountryID,
                    StateName = StateName.ToUpper(),
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.States.Add(states);
                int StateCreated = await _context.SaveChangesAsync();

                if (StateCreated > 0)
                {
                    response = "State Created";
                }
                else
                {
                    response = "Something went wrong trying to create state. Please try again.";
                }
            }
            _helpersController.LogMessages("Creating new State. Status : " + response + " Country ID : " + CountryID + "State Name : " + StateName, _helpersController.getSessionEmail());

            return Json(response);
        }


        /*
        * edit state and country
        */
        
        public async Task<IActionResult> EditState(string StateName, int StateID, int CountryID)
        {
            string response = "";
            var getState = from x in _context.States where x.StateId == StateID select x;

            getState.FirstOrDefault().StateName = StateName.ToUpper();
            getState.FirstOrDefault().CountryId = CountryID;
            getState.FirstOrDefault().UpdatedAt = DateTime.Now;
            getState.FirstOrDefault().DeleteStatus = false;

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "State Updated";
            }
            else
            {
                response = "Nothing was updated.";
            }

            _helpersController.LogMessages("Updating State. Status : " + response + " State ID : " + StateID + " Country ID : " + CountryID, _helpersController.getSessionEmail());

            return Json(response);
        }


        // Removing a state
        
        public async Task<IActionResult> DeleteState(int StateID)
        {
            string response = "";

            var getState = from c in _context.States where c.StateId == StateID select c;

            getState.FirstOrDefault().DeletedAt = DateTime.Now;
            getState.FirstOrDefault().UpdatedAt = DateTime.Now;
            getState.FirstOrDefault().DeleteStatus = true;
            getState.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "State Deleted";
            }
            else
            {
                response = "State not deleted. Something went wrong trying to delete this state.";
            }

            _helpersController.LogMessages("Deleting State. Status : " + response + " State ID : " + StateID, _helpersController.getSessionEmail());

            return Json(response);
        }



    }
}
