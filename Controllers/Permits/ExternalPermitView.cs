using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DST.Controllers.Configurations;
using DST.Helpers;
using DST.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QRCoder;
using Rotativa.AspNetCore;
using static DST.Models.GeneralModel;

namespace LPGDepot.Controllers.Permits
{
    public class ExternalPermitView : Controller
    {

        private readonly DST_DBContext _context;
        GeneralClass generalClass = new GeneralClass();
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;


        public ExternalPermitView(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }


        // GET: ExternalPermitView
        public ActionResult ViewPermit(int id)
        {
            var getPermit = from p in _context.Permits
                            join a in _context.Applications on p.AppId equals a.AppId
                            join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                            join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                            join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                            join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                            where p.PermitElpsId == id
                            select new
                            {
                                ShortName = s.ShortName,
                                PermitId = p.PermitId
                            };

            if(getPermit.Any())
            {
                if(getPermit.FirstOrDefault().ShortName == "DST")
                {
                    return RedirectToAction("ViewDSTPermit", "ExternalPermitView", new { id = generalClass.Encrypt(getPermit.FirstOrDefault().PermitId.ToString()) });
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not in correct format. Kindly contact support.") });
                }
            }
            else
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not in correct format. Kindly contact support.") });
            }
        }



        public IActionResult ViewDSTPermit(string id)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                ViewData["PermitDocumentName"] = "";

                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = from p in _context.Permits.AsEnumerable()
                                 join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                                 join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                                 join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                                 join tr in _context.Transactions.AsEnumerable() on a.AppId equals tr.AppId into trans
                                 from t in trans.DefaultIfEmpty()
                                 join sf in _context.Staff on p.ApprovedBy equals sf.StaffId into staff
                                 where p.PermitId == permitID
                                 select new PermitModel
                                 {
                                     PermitID = p.PermitId,
                                     AppId = a.AppId,
                                     RefNo = a.AppRefNo,
                                     PermitNO = p.PermitNo,
                                     IssuedDate = p.IssuedDate.ToString("dd MMMM, yyyy"),
                                     ExpiaryDate = p.ExpireDate.ToString("dddd, dd MMMM yyyy"),
                                     CompanyID = a.CompanyId,
                                     CompanyName = c.CompanyName,
                                     CompanyAddress = c.Address,
                                     CompanyCity = c.City,
                                     CompanyState = c.StateName,
                                     State = f.State,
                                     Signature = staff.FirstOrDefault()?.SignatureName == null ? "" : staff.FirstOrDefault()?.SignatureName,
                                     QrCode = QrCode,
                                     TotalAmount = t == null ? 0 : (int)t.TotalAmt,
                                     RRR = t == null ? "" : t?.Rrr,
                                     DatePaid = t == null ? "" : t?.TransactionDate.ToLongDateString(),
                                     DateSubmitted = a.DateSubmitted,
                                     ApprovedBy = staff.FirstOrDefault()?.LastName + " " + staff.FirstOrDefault()?.FirstName,
                                     WellDetails = f.FacilityName,
                                 };

                if (getPermits.Any())
                {
                    var getZoneStaff = from s in _context.Staff.AsEnumerable()
                                       join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                                       join f in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals f.FieldOfficeId
                                       join zfo in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zfo.FieldOfficeId
                                       join z in _context.ZonalOffice.AsEnumerable() on zfo.ZoneId equals z.ZoneId
                                       join zs in _context.ZoneStates.AsEnumerable() on z.ZoneId equals zs.ZoneId
                                       join st in _context.States.AsEnumerable() on zs.StateId equals st.StateId
                                       where ((r.RoleName == GeneralClass.ZOPSCON) && st.StateId == getPermits.FirstOrDefault().State)
                                       select new
                                       {
                                           ZoneName = z.ZoneName
                                       };

                    var getNominatedStaff = from n in _context.NominatedStaff.AsEnumerable()
                                            join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                            join fo in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals fo.FieldOfficeId
                                            where n.AppId == getPermits.FirstOrDefault().AppId
                                            select new StaffNomination
                                            {
                                                FullName = s.LastName + " " + s.FirstName,
                                                Division = n.Designation,
                                                FieldOffice = fo.OfficeName,
                                                Phone = n.PhoneNumber,
                                                StaffEmail = s.StaffEmail
                                            };

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        ZoneName = getZoneStaff.FirstOrDefault().ZoneName,
                        staffNominations = getNominatedStaff.ToList()
                    });

                    ViewData["PermitDocumentName"] = "View Permit for " + getPermits.FirstOrDefault().PermitNO;

                    return new ViewAsPdf("ViewDSTPermit", permitViewModels.ToList())
                    {
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    };
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }



        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }



        private static Byte[] GenerateQR(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            var imageResult = BitmapToBytes(qrCodeImage);
            return imageResult;
        }








    }
}
