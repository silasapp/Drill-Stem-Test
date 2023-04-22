using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DST.Models.DB;
using Microsoft.AspNetCore.Http;
using DST.Controllers.Configurations;
using DST.Helpers;
using Microsoft.Extensions.Configuration;
using static DST.Models.GeneralModel;
using Microsoft.AspNetCore.Authorization;

namespace DST.Controllers.Transactions
{
    [Authorize]

    public class TransactionsController : Controller
    {
        private readonly DST_DBContext _context;

        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();

        public TransactionsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }


        public IActionResult Payments(string id)
        {
            var transactions = (
                from tns in _context.Transactions.AsEnumerable()
                join a in _context.Applications.AsEnumerable() on tns.AppId equals a.AppId
                join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId into Company
                join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                select new TransactionDetails
                {
                    RefNo = a.AppRefNo,
                    RRR = tns.Rrr,
                    CompanyName = Company.FirstOrDefault().CompanyName,
                    WellDetails = f.FacilityName,
                    Amount = tns.AmtPaid,
                    TotalAmount = tns.TotalAmt,
                    ServiceCharge = tns.ServiceCharge,
                    TransDate = tns.TransactionDate,
                    TransStatus = tns.TransactionStatus,
                    TransType = tns.TransactionType,
                    TransRef = tns.TransRef,
                    Description = tns.Description,
                });

            ViewData["TransViewType"] = "All Payments";

            if (id == "_pending")
            {
                ViewData["TransViewType"] = "All Pending Payments";
                transactions = transactions.Where(x => x.TransStatus == GeneralClass.PaymentPending);
            }
            else if (id == "_completed")
            {
                ViewData["TransViewType"] = "All Completed Payments";
                transactions = transactions.Where(x => x.TransStatus == GeneralClass.PaymentCompleted);
            }
            else
            {
                transactions = transactions;
            }

            _helpersController.LogMessages("Displaying payment details", _helpersController.getSessionEmail());

            return View(transactions.ToList());
        }


    }

   
}
