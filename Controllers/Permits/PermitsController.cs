using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using QRCoder;
using Microsoft.AspNetCore.Authorization;
using DST.Controllers.Configurations;
using DST.Models.DB;
using DST.Helpers;
using static DST.Models.GeneralModel;
using Rotativa.AspNetCore;
using System.Drawing;

namespace DST.Controllers.Permits
{
    [Authorize]
    public class PermitsController : Controller
    {
        private readonly DST_DBContext _context;

        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        GeneralClass generalClass = new GeneralClass();
        RestSharpServices _restService = new RestSharpServices();

        public PermitsController(DST_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        }

        // GET: Permits
        public IActionResult Index(string id, string option)
        {
            int ids = 0;

            var type = id;
            var general_id = generalClass.Decrypt(option);

            var myPermits = from p in _context.Permits
                            join a in _context.Applications on p.AppId equals a.AppId
                            join c in _context.Companies on a.CompanyId equals c.CompanyId
                            join ts in _context.AppTypeStage.AsEnumerable() on a.AppTypeStageId equals ts.TypeStageId
                            join s in _context.ApplicationStage.AsEnumerable() on ts.AppStageId equals s.AppStageId
                            join ty in _context.ApplicationType.AsEnumerable() on ts.AppTypeId equals ty.AppTypeId
                            join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                            select new MyPermit
                            {
                                PermitID = p.PermitId,
                                PermitNo = p.PermitNo,
                                RefNo = a.AppRefNo,
                                IssuedDate = p.IssuedDate,
                                ExpireDate = p.ExpireDate,
                                isPrinted = p.Printed,
                                CompanyName = c.CompanyName,
                                CompanyID = c.CompanyId,
                                CompanyEmail = c.CompanyEmail,
                                Category = ty.TypeName + " - " + s.StageName,
                                WellDetails = f.FacilityName,
                                ShortName = s.ShortName,
                                StageName = s.StageName,
                            };

            ViewData["ClassifyPermits"] = "All Permits";

            if (!string.IsNullOrWhiteSpace(id) || !string.IsNullOrWhiteSpace(option))
            {
                if (id == "_all")
                {
                    myPermits = myPermits.Where(x => x.PermitID > 0 /*&& x.isLegacy == ""*/);
                }
                else if (id == "_printed")
                {
                    myPermits = myPermits.Where(x => x.isPrinted == true /*&& x.isLegacy == ""*/);

                    ViewData["ClassifyPermits"] = "All Printed Permits";
                }
                else if (id == "_notprinted")
                {
                    myPermits = myPermits.Where(x => x.isPrinted == false /*&& x.isLegacy == ""*/);

                    ViewData["ClassifyPermits"] = "All Not Printed Permits";
                }
                else if (id == "_company")
                {
                    ids = Convert.ToInt32(general_id);
                    myPermits = myPermits.Where(x => x.CompanyID == ids);

                    ViewData["ClassifyPermits"] = "All Permits for " + myPermits.FirstOrDefault()?.CompanyName + " Company";
                }
            }

            _helpersController.LogMessages("Displaying " + ViewData["ClassifyPermits"], _helpersController.getSessionEmail());

            return View(myPermits.ToList());
        }

        public IActionResult ViewLetter(string id, string option)
        {
            int appid = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (appid == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. witness letter reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var getLetter = from n in _context.NominatedStaff.AsEnumerable()
                                join p in _context.Permits.AsEnumerable() on n.AppId equals p.AppId
                                 join a in _context.Applications.AsEnumerable() on n.AppId equals a.AppId
                                 join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                                 join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                                 join tr in _context.Transactions.AsEnumerable() on a.AppId equals tr.AppId into trans
                                 from t in trans.DefaultIfEmpty()
                                 join sf in _context.Staff on p.ApprovedBy equals sf.StaffId into staff
                                 where a.AppId == appid
                                 select new PermitModel
                                 {
                                     AppId = a.AppId,
                                     RefNo = a.AppRefNo,
                                     PermitNO = p.PermitNo,
                                     IssuedDate = p.IssuedDate.ToString("dd MMMM, yyyy"),
                                     ExpiaryDate = p.ExpireDate.ToString("dddd, dd MMMM yyyy"),
                                     CreatedAt =n.CreatedAt.ToString("dddd, dd MMMM yyyy"),
                                     CompanyID = a.CompanyId,
                                     CompanyName = c.CompanyName,
                                     CompanyAddress = c.Address,
                                     CompanyCity = c.City,
                                     CompanyState = c.StateName,
                                     State = f.State,
                                     Signature = staff.FirstOrDefault()?.SignatureName == null ? "" : staff.FirstOrDefault()?.SignatureName,
                                     TotalAmount = t == null ? 0 : (int)t.TotalAmt,
                                     RRR = t == null ? "" : t?.Rrr,
                                     DatePaid = t == null ? "" : t?.TransactionDate.ToLongDateString(),
                                     DateSubmitted = a.DateSubmitted,
                                     ApprovedBy = staff.FirstOrDefault()?.LastName + " " + staff.FirstOrDefault()?.FirstName,
                                     WellDetails = f.FacilityName,
                                 };

                if(getLetter.Any())
                {
                    var getNominatedStaff = from n in _context.NominatedStaff.AsEnumerable()
                                            join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                            join fo in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals fo.FieldOfficeId
                                            where n.AppId == getLetter.FirstOrDefault().AppId
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
                        permitModels = getLetter.ToList(),
                        staffNominations = getNominatedStaff.ToList()
                    });

                    ViewData["PermitDocumentName"] = "Nomination Letter for " + getLetter.FirstOrDefault().CompanyName + " (" + getLetter.FirstOrDefault().RefNo + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("ViewLetter", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("ViewLetter", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Witness Letter reference not found or not in correct format. Kindly contact support.") });
                }
            }
        }

        // For DST
        public IActionResult ViewPermit(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                if (getPermits.Any())
                {
                    var getZoneStaff = GetZoneName(getPermits.FirstOrDefault().AppId);

                    var getNominatedStaff = GetStaffNomination(getPermits.FirstOrDefault().AppId);

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        ZoneName = getZoneStaff,
                        staffNominations = getNominatedStaff.ToList()
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("ViewPermit", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("ViewPermit", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        // Routine MER
        public IActionResult RoutineMer(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                if (getPermits.Any())
                {
                    var getNominatedStaff = GetStaffNomination(getPermits.FirstOrDefault().AppId);

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        staffNominations = getNominatedStaff.ToList()
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("RoutineMer", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("RoutineMer", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        // New and RoutienTar
        public IActionResult RoutineTAR(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                if (getPermits.Any())
                {
                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("RoutineTAR", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("RoutineTAR", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        // Off Cycle MER
        public IActionResult OffCycleMer(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                if (getPermits.Any())
                {
                    var getNominatedStaff = GetStaffNomination(getPermits.FirstOrDefault().AppId);

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        staffNominations = getNominatedStaff.ToList()
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("OffCycleMer", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("OffCycleMer", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        // Off Cycle TAR
        public IActionResult OffCycleTar(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();
            List<PermitModel> getPerviousPermit = new List<PermitModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                var getPreviousPremitID = from p in _context.Permits.AsEnumerable()
                                  join a in _context.Applications.AsEnumerable() on p.AppId equals a.PreviousAppId
                                  where p.AppId == getPermits.FirstOrDefault().PerviousAppId
                                  select new
                                  {
                                      p.PermitId
                                  };

                if (getPreviousPremitID.Any())
                {
                    getPerviousPermit = this.GetPermitModel(getPreviousPremitID.FirstOrDefault().PermitId, QrCode).ToList();
                }

                if (getPermits.Any())
                {
                    var getNominatedStaff = GetStaffNomination(getPermits.FirstOrDefault().AppId);

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        staffNominations = getNominatedStaff.ToList(),
                        PreviousPermitModels = getPerviousPermit.ToList()
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("OffCycleTar", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("OffCycleTar", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        // New and Extended EWT
        public IActionResult EWT(string id, string option)
        {
            int permitID = generalClass.DecryptIDs(id);

            List<PermitViewModel> permitViewModels = new List<PermitViewModel>();

            if (permitID == 0)
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit reference not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                var Host = Request.Host;
                var absolutUrl = Host + "/Permits/VerifyPermitQrCode/" + id;
                var QrCode = GenerateQR(absolutUrl);

                var getPermits = this.GetPermitModel(permitID, QrCode);

                if (getPermits.Any())
                {
                    var getNominatedStaff = GetStaffNomination(getPermits.FirstOrDefault().AppId);
                    var zoneName = GetZoneName(getPermits.FirstOrDefault().AppId);

                    permitViewModels.Add(new PermitViewModel
                    {
                        permitModels = getPermits.ToList(),
                        staffNominations = getNominatedStaff.ToList(),
                        ZoneName = zoneName
                    });

                    ViewData["PermitDocumentName"] = "Permit for " + getPermits.FirstOrDefault().CompanyName + " (" + getPermits.FirstOrDefault().PermitNO + ").pdf";

                    if (option == "_view")
                    {
                        _helpersController.SavePermitHistory(permitID, "Preview");

                        _helpersController.LogMessages("Displaying " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("EWT", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        };
                    }
                    else if (option == "_download")
                    {
                        var fetchPermits = _context.Permits.Where(x => x.PermitId == permitID);

                        fetchPermits.FirstOrDefault().Printed = true;
                        _context.SaveChanges();

                        _helpersController.SavePermitHistory(permitID, "Download");

                        _helpersController.LogMessages("Downloading " + ViewData["PermitDocumentName"], _helpersController.getSessionEmail());

                        return new ViewAsPdf("EWT", permitViewModels.ToList())
                        {
                            PageSize = Rotativa.AspNetCore.Options.Size.A4,
                            FileName = ViewData["PermitDocumentName"].ToString()
                        };
                    }
                    else
                    {
                        return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps! What did you just do now??? Please click the back button.") });
                    }
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Could not find permit. Kindly contact support.") });
                }
            }
        }

        public IActionResult ViewHistory(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Reference is empty or not in correct format. Kindly contact support.") });
            }

            int permitID = 0;

            var permit = generalClass.Decrypt(id);

            if (permit == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Reference error or not in correct format. Kindly contact support.") });
            }
            else
            {
                permitID = Convert.ToInt32(permit);

                var getPermitHistory = from ph in _context.PermitHistory
                                       join p in _context.Permits on ph.PermitId equals p.PermitId
                                       where ph.PermitId == permitID
                                       select new PermitView
                                       {
                                           PermitNO = p.PermitNo,
                                           ViewType = ph.ViewType,
                                           PreviewedAt = ph.PreviewedAt,
                                           DownloadedAt = ph.DownloadedAt,
                                           UserDetails = ph.UserDetails
                                       };

                ViewData["PermitHistoryTitle"] = "Permit History Details";

                if (getPermitHistory.Any())
                {
                    ViewData["PermitHistoryTitle"] = "Permit History Details for : " + getPermitHistory.FirstOrDefault()?.PermitNO;
                }

                return View(getPermitHistory.ToList());
            }
        }

        [AllowAnonymous]
        public IActionResult VerifyPermitQrCode(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit not found or not in correct format. Kindly contact support.") });
            }

            int permitID = 0;

            var permit = generalClass.Decrypt(id);

            if (permit == "Error")
            {
                return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Something went wrong. Permit not found or not in correct format. Kindly contact support.") });
            }
            else
            {
                permitID = Convert.ToInt32(permit);

                var prm = from p in _context.Permits.AsEnumerable()
                          join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                          join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                          join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                          join t in _context.Transactions.AsEnumerable() on a.AppId equals t.AppId
                          where p.PermitId == permitID
                          select new PermitModel
                          {
                              RefNo = a.AppRefNo,
                              PermitNO = p.PermitNo,
                              IssuedDate = p.IssuedDate.ToString("dd MMMM, yyyy"),
                              ExpiaryDate = p.ExpireDate.ToString("dddd, dd MMMM yyyy"),
                              MyCompanyDetails = c.CompanyName + " (" + c.Address + ", " + c.City + ", " + c.StateName + ")",
                              WellDetails = f.FacilityName,
                              TotalAmount = (int)t.TotalAmt,
                              PayDescriiption = t.Description,
                              Status = a.Status,
                              DateSubmitted = a.DateSubmitted,
                              RRR = t.Rrr,
                              CompanyCode = c.IdentificationCode
                          };

                if (prm.Any())
                {
                    return View(prm.ToList());
                }
                else
                {
                    return RedirectToAction("Errorr", "Home", new { message = generalClass.Encrypt("Opps!!! Something went wrong tyring to verify this Permit or Permit was not found, Kindly contact support.") });
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

        public List<PermitModel> GetPermitModel(int permitId, byte[] QrCode)
        {
            var getPermits = from p in _context.Permits.AsEnumerable()
                             join a in _context.Applications.AsEnumerable() on p.AppId equals a.AppId
                             join f in _context.Facilities.AsEnumerable() on a.FacilityId equals f.FacilityId
                             join c in _context.Companies.AsEnumerable() on a.CompanyId equals c.CompanyId
                             join tr in _context.Transactions.AsEnumerable() on a.AppId equals tr.AppId into trans
                             from t in trans.DefaultIfEmpty()
                             join sf in _context.Staff on p.ApprovedBy equals sf.StaffId into staff
                             where p.PermitId == permitId
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
                                 Signature = (staff.FirstOrDefault()?.SignatureName) ?? "",
                                 QrCode = QrCode,
                                 Volume = a?.Volume,
                                 TotalAmount = t == null ? 0 : (int)t.TotalAmt,
                                 RRR = t == null ? "" : t?.Rrr,
                                 DatePaid = t == null ? "" : t?.TransactionDate.ToLongDateString(),
                                 DateSubmitted = a.DateSubmitted,
                                 ApprovedBy = staff.FirstOrDefault()?.LastName + " " + staff.FirstOrDefault()?.FirstName,
                                 WellDetails = f.FacilityName,
                                 Year = p.IssuedDate.Year,
                                 Month = p.ExpireDate.Month,
                                 MonthAdded = p.ExpireDate.AddMonths(1).ToString("MMMM"),
                                 WellNames = this.GetWells(a.AppId),
                                 ReserviorNames = this.GetReserviors(a.AppId),
                                 FieldNames = this.GetFields(a.AppId),
                                 PerviousAppId = a.PreviousAppId
                             };

            return getPermits.ToList();
        }

        public string GetZoneName(int appid)
        {
            var getZoneStaff = from nr in _context.NominationRequest.AsEnumerable()
                               join s in _context.Staff.AsEnumerable() on nr.StaffId equals s.StaffId
                               join r in _context.UserRoles.AsEnumerable() on s.RoleId equals r.RoleId
                               join f in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals f.FieldOfficeId
                               join zfo in _context.ZoneFieldOffice.AsEnumerable() on f.FieldOfficeId equals zfo.FieldOfficeId
                               join z in _context.ZonalOffice.AsEnumerable() on zfo.ZoneId equals z.ZoneId
                               join zs in _context.ZoneStates.AsEnumerable() on z.ZoneId equals zs.ZoneId
                               join st in _context.States.AsEnumerable() on zs.StateId equals st.StateId
                               where (nr.AppId == appid)
                               select new
                               {
                                   ZoneName = z.ZoneName
                               };
            return getZoneStaff?.FirstOrDefault()?.ZoneName;
        }

        public List<StaffNomination> GetStaffNomination(int appid)
        {
            var getNominatedStaff = from n in _context.NominatedStaff.AsEnumerable()
                                    join s in _context.Staff.AsEnumerable() on n.StaffId equals s.StaffId
                                    join fo in _context.FieldOffices.AsEnumerable() on s.FieldOfficeId equals fo.FieldOfficeId
                                    where n.AppId == appid
                                    select new StaffNomination
                                    {
                                        FullName = s.LastName + " " + s.FirstName,
                                        Division = n.Designation,
                                        FieldOffice = fo.OfficeName,
                                        Phone = n.PhoneNumber,
                                        StaffEmail = s.StaffEmail
                                    };

            return getNominatedStaff.ToList();
        }

        public string GetWells(int AppId)
        {

            var get = _context.TemplateTable.AsEnumerable().Where(x => x.AppId == AppId).GroupBy(x => x.WellName).Select(c => c.FirstOrDefault());

            List<string> Names = new List<string>();
           
            foreach (var p in get.ToList())
            {
                Names.Add(p.WellName);
            }

            return string.Join(", ", Names.ToList());
        }

        public string GetReserviors(int AppId)
        {

            var get = _context.TemplateTable.AsEnumerable().Where(x => x.AppId == AppId).GroupBy(x => x.Reservior).Select(c => c.FirstOrDefault());

            List<string> Names = new List<string>();

            foreach (var p in get.ToList())
            {
                Names.Add(p.Reservior);
            }

            return string.Join(", ", Names.ToList());
        }

        public string GetFields(int AppId)
        {

            var get = _context.TemplateTable.AsEnumerable().Where(x => x.AppId == AppId).GroupBy(x => x.FieldName).Select(c => c.FirstOrDefault());

            List<string> Names = new List<string>();

            foreach (var p in get.ToList())
            {
                Names.Add(p.FieldName);
            }

            return string.Join(", ", Names.ToList());
        }

    }


}
