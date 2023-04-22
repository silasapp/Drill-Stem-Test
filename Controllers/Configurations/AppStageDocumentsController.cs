using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DST.Helpers;
using DST.Models.DB;

namespace DST.Controllers.Configurations
{

    public class AppStageDocumentsController : Controller
    {
        private readonly DST_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public AppStageDocumentsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }



        // GET: AppStageDocuments
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppStageDocuments.ToListAsync());
        }



      
        public JsonResult GetStageDoc()
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

            var get = from ts in _context.AppStageDocuments
                      join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                      join d in _context.ApplicationDocuments on ts.AppDocId equals d.AppDocId
                      where ts.DeleteStatus == false && s.DeleteStatus == false && d.DeleteStatus == false
                      select new
                      {
                          StageDocID = ts.StageDocId,
                          DocID = d.AppDocId,
                          DocName = d.DocName,
                          StageID = s.AppStageId,
                          StageName = s.StageName,
                          DocType = d.DocType,
                          UpdatedAt = ts.UpdatedAt.ToString(),
                          CreatedAt = ts.CreatedAt.ToString()
                      };

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                if (sortColumnDir == "desc")
                {
                    get = sortColumn == "stageName" ? get.OrderByDescending(s => s.StageName) :
                                    sortColumn == "docName" ? get.OrderByDescending(t => t.DocName) :
                                    sortColumn == "docType" ? get.OrderByDescending(t => t.DocType) :
                                    sortColumn == "updatedAt" ? get.OrderByDescending(s => s.UpdatedAt) :
                                    sortColumn == "createdAt" ? get.OrderByDescending(s => s.CreatedAt) :
                                    get.OrderByDescending(ts => ts.StageDocID + " " + sortColumnDir);
                }
                else
                {
                    get = sortColumn == "stageName" ? get.OrderBy(s => s.StageName) :
                                   sortColumn == "docName" ? get.OrderBy(t => t.DocName) :
                                   sortColumn == "docType" ? get.OrderBy(t => t.DocType) :
                                   sortColumn == "updatedAt" ? get.OrderBy(c => c.UpdatedAt) :
                                   sortColumn == "createdAt" ? get.OrderBy(c => c.CreatedAt) :
                                   get.OrderBy(ts => ts.StageDocID);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtSearch))
            {
                get = get.Where(c => c.StageName.Contains(txtSearch.ToUpper()) || c.DocName.Contains(txtSearch.ToUpper()));
            }

            totalRecords = get.Count();
            var data = get.Skip(skip).Take(pageSize).ToList();

            _helpersController.LogMessages("Displaying all application stage documents.", _helpersController.getSessionEmail());

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data });

        }



        // Creating Type and Stage 
       
        public async Task<IActionResult> CreateStageDocuments(int DocID, int StageID)
        {
            string response = "";

            var check = from ts in _context.AppStageDocuments
                        join t in _context.ApplicationDocuments on ts.AppDocId equals t.AppDocId
                        join s in _context.ApplicationStage on ts.AppStageId equals s.AppStageId
                        where ts.AppDocId == DocID && ts.AppStageId == StageID && ts.DeleteStatus == false
                        select new
                        {
                            s.StageName,
                            t.DocName
                        };

            if (check.Any())
            {
                response = check.FirstOrDefault().StageName + " and " + check.FirstOrDefault().DocName + " relationship already exits.";
            }
            else
            {
                AppStageDocuments _stageDoc = new AppStageDocuments()
                {
                    AppDocId = DocID,
                    AppStageId = StageID,
                    CreatedAt = DateTime.Now,
                    DeleteStatus = false
                };

                _context.AppStageDocuments.Add(_stageDoc);
                int Created = await _context.SaveChangesAsync();

                if (Created > 0)
                {
                    response = "StageDoc Created";
                }
                else
                {
                    response = "Something went wrong trying to create Document and Stage relationship. Please try again.";
                }
            }

            _helpersController.LogMessages("Creating new application stage documents. Status : " + response + " Application Document ID : " + DocID + "Application stage ID : " + StageID, _helpersController.getSessionEmail());

            return Json(response);
        }


        // Eidt Type Stage 
        
        public async Task<IActionResult> EditStageDocuments(int StageDocID, int DocID, int StageID)
        {
            string response = "";
            var check = from x in _context.AppStageDocuments where x.StageDocId == StageDocID select x;

            if (check.FirstOrDefault().AppDocId == DocID && check.FirstOrDefault().AppStageId == StageID)
            {
                response = "This relationship already exits. Try a different one.";
            }
            else
            {
                check.FirstOrDefault().AppDocId = DocID;
                check.FirstOrDefault().AppStageId = StageID;
                check.FirstOrDefault().UpdatedAt = DateTime.Now;
                check.FirstOrDefault().DeleteStatus = false;

                int updated = await _context.SaveChangesAsync();

                if (updated > 0)
                {
                    response = "StageDoc Updated";
                }
                else
                {
                    response = "Nothing was updated.";
                }
            }
            _helpersController.LogMessages("Updating application stage documents. Status : " + response + " Application Document ID : " + DocID + "Application stage ID : " + StageID, _helpersController.getSessionEmail());

            return Json(response);
        }


        // Delete Type Stage
       
        public async Task<IActionResult> DeleteStageDocument(int StageDocID)
        {
            string response = "";

            var get = from c in _context.AppStageDocuments where c.StageDocId == StageDocID select c;

            get.FirstOrDefault().DeletedAt = DateTime.Now;
            get.FirstOrDefault().UpdatedAt = DateTime.Now;
            get.FirstOrDefault().DeleteStatus = true;
            get.FirstOrDefault().DeletedBy =  _helpersController.getSessionUserID();

            int updated = await _context.SaveChangesAsync();

            if (updated > 0)
            {
                response = "StageDoc Deleted";
            }
            else
            {
                response = "Doc => Stage not deleted. Something went wrong trying to delete this entry.";
            }

            _helpersController.LogMessages("Deleting application stage documents. Status : " + response + " Application Stage Document ID : " + StageDocID, _helpersController.getSessionEmail());

            return Json(response);
        }


    }
}
