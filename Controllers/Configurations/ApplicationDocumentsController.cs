﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Models.DB;
using DST.Helpers;

namespace DST.Controllers.Configurations
{

    public class ApplicationDocumentsController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public ApplicationDocumentsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: ApplicationDocuments
      
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationDocuments.ToListAsync());
        }


        
        public JsonResult GetAppDoc()
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

            var getAppDoc = from r in _context.ApplicationDocuments
                            where r.DeleteStatus == false
                            select new
                            {
                                AppDocID = r.AppDocId,
                                AppDocElpsID = r.ElpsDocTypeId,
                                DocName = r.DocName,
                                DocType = r.DocType,
                                UpdatedAt = r.UpdatedAt.ToString(),
                                CreatedAt = r.CreatedAt.ToString()
                            };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    getAppDoc = sortColumn == "docName" ? getAppDoc.OrderByDescending(c => c.DocName) :
                               sortColumn == "updatedAt" ? getAppDoc.OrderByDescending(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getAppDoc.OrderByDescending(c => c.CreatedAt) :
                               sortColumn == "docType" ? getAppDoc.OrderByDescending(c => c.DocType) :
                               getAppDoc.OrderByDescending(c => c.AppDocID + " " + sortColumnDir);
                }
                else
                {
                    getAppDoc = sortColumn == "docName" ? getAppDoc.OrderBy(c => c.DocName) :
                               sortColumn == "updatedAt" ? getAppDoc.OrderBy(c => c.UpdatedAt) :
                               sortColumn == "createdAt" ? getAppDoc.OrderBy(c => c.CreatedAt) :
                               sortColumn == "docType" ? getAppDoc.OrderBy(c => c.DocType) :
                               getAppDoc.OrderBy(c => c.AppDocID);
                }

            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                getAppDoc = getAppDoc.Where(c => c.DocName.Contains(txtSearch.ToUpper()) || c.DocType.Contains(txtSearch) || c.CreatedAt.Contains(txtSearch) || c.UpdatedAt.Contains(txtSearch));
            }

            totalRecords = getAppDoc.Count();
            var data = getAppDoc.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying application documents", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }



        //[Authorize(Roles = "SUPPORT, IT ADMIN, SUPER ADMIN, HEAD OFFICE ADMIN")]
        // POST: ApplicationDocuments/Create
      
        public async Task<IActionResult> CreateAppDoc(int AppDocElpsID, string AppDocName, string AppDocType)
        {
            string response = "";

            var appstage = from a in _context.ApplicationDocuments
                           where (a.DocName == AppDocName.ToUpper() && a.ElpsDocTypeId == AppDocElpsID && a.DocType == AppDocType && a.DeleteStatus == false)
                           select a;

            if (appstage.Any())
            {
                response = "Application document name already exits, please enter another name.";
            }
            else
            {
                ApplicationDocuments con = new ApplicationDocuments()
                {
                    DocName = AppDocName.ToUpper(),
                    DocType = AppDocType,
                    ElpsDocTypeId = AppDocElpsID,
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.ApplicationDocuments.Add(con);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "AppDoc Created";
                }
                else
                {
                    response = "Something went wrong trying to create this App Doc. Please try again.";
                }
            }

            _helpersController.LogMessages("Creating application documents. Status : " + response, _helpersController.getSessionEmail());

            return Json(response);
        }




        //[Authorize(Roles = "SUPPORT, IT ADMIN, SUPER ADMIN, HEAD OFFICE ADMIN")]
        // POST: ApplicationDocuments/Edit/5
        
        public async Task<IActionResult> EditAppDoc(int AppDocID, string AppDocName, string AppDocType)
        {
            string response = "";
            var getAppDoc = from c in _context.ApplicationDocuments where c.AppDocId == AppDocID select c;

            getAppDoc.FirstOrDefault().DocName = AppDocName.ToUpper();
            getAppDoc.FirstOrDefault().DocType = AppDocType;
            getAppDoc.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppDoc.FirstOrDefault().DeleteStatus = false;

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppDoc Updated";
            }
            else
            {
                response = "Nothing was updated.";
            }

            _helpersController.LogMessages("Updating application documents. Status : " + response + " Application Document ID : " + AppDocID, _helpersController.getSessionEmail());
            return Json(response);
        }





        //Authorize(Roles = "SUPPORT, IT ADMIN, SUPER ADMIN, HEAD OFFICE ADMIN")]
        // POST: ApplicationDocuments/Delete/5
       
        public async Task<IActionResult> DeleteAppDoc(int AppDocID)
        {
            string response = "";

            var getAppDoc = from c in _context.ApplicationDocuments where c.AppDocId == AppDocID select c;

            getAppDoc.FirstOrDefault().DeletedAt = DateTime.Now;
            getAppDoc.FirstOrDefault().UpdatedAt = DateTime.Now;
            getAppDoc.FirstOrDefault().DeleteStatus = true;
            getAppDoc.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "AppDoc Deleted";
            }
            else
            {
                response = "Application document not deleted. Something went wrong trying to delete this application document.";
            }

            _helpersController.LogMessages("Deleting application documents. Status : " + response + " Application Document ID : " + AppDocID, _helpersController.getSessionEmail());

            return Json(response);
        }

    }
}
