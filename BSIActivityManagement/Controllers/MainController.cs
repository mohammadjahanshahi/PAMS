using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.Authorization;
using System.IO;
using System.Drawing;
using BSIActivityManagement.Models;

namespace BSIActivityManagement.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Unit()
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            if (AmUserId == 0)
                return View("Error");
            var Units = DmlObj.GetUserUnitsByUserId(AmUserId);
            //if (Units.Count() == 1) return RedirectToAction("UnitProcesses", new { UnitId = Units.FirstOrDefault().Id });

            MainViewModelUnit model = new MainViewModelUnit();
            model.Units = Units;
            List<NavViewModel> Nav = DmlObj.GetLogoutNaviagtion();
            Nav = DmlObj.AddUnitNaviagtion(Nav);

            model.Navigation = Nav.AsEnumerable();
            return View(model);
        }

        public ActionResult Page(string UnitId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            int currentUnit = 0;
            Int32.TryParse(UnitId,out currentUnit);
            if (AmUserId == 0 || currentUnit == 0 || DmlObj.GetUnitById(currentUnit) == null || !DmlObj.VerifyUserUnit(currentUnit, AmUserId))
                return View("Error");
            ViewBag.UnitId = currentUnit;
            return View();
        }

        public ActionResult UnitProcesses(string UnitId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            int currentUnit = 0;
            Int32.TryParse(UnitId, out currentUnit);
            var selectedUnit = DmlObj.GetUnitById(currentUnit);
            if (AmUserId == 0 || currentUnit == 0 || selectedUnit == null || !DmlObj.VerifyUserUnit(currentUnit, AmUserId))
                return View("Error");

            MainViewModelProcesses model = new MainViewModelProcesses();
            model.Unit = selectedUnit;
            model.Processes = DmlObj.GetUnitProcesses(currentUnit);
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(currentUnit);
            Nav = DmlObj.AddUnitProcessesNaviagtion(Nav, currentUnit, selectedUnit.Name);
            if (DmlObj.CheckUserTypeAccess(DmlObj.GetAccessByAccessKey("MANAGE_UNIT_USERS"), DmlObj.GetAmUserById(AmUserId).UserType))
                model.Navigation = DmlObj.AddManagerNaviagtion(Nav, currentUnit).AsEnumerable();
            else model.Navigation = Nav.AsEnumerable();
            return View(model);
        }

        public ActionResult UserProcesses(string UnitId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            int currentUnit = 0;
            Int32.TryParse(UnitId, out currentUnit);
            var selectedUnit = DmlObj.GetUnitById(currentUnit);
            if (AmUserId == 0 || currentUnit == 0 || selectedUnit == null || !DmlObj.VerifyUserUnit(currentUnit, AmUserId))
                return View("Error");

            MainViewModelProcesses model = new MainViewModelProcesses();
            model.Unit = selectedUnit;
            model.Processes = DmlObj.GetUnitUserProcesses(currentUnit, AmUserId);
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(currentUnit);
            Nav = DmlObj.AddUnitProcessesNaviagtion(Nav, currentUnit, selectedUnit.Name);
            if (DmlObj.CheckUserTypeAccess(DmlObj.GetAccessByAccessKey("MANAGE_UNIT_USERS"), DmlObj.GetAmUserById(AmUserId).UserType))
                model.Navigation = DmlObj.AddManagerNaviagtion(Nav, currentUnit).AsEnumerable();
            else model.Navigation = Nav.AsEnumerable();
            return View(model);
        }

        public ActionResult UnitUsers(string UnitId)
        {
            int AmUserId = 0;
            int AmUnitId = 0;
            Int32.TryParse(UnitId, out AmUnitId);
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            if (AmUserId == 0 || AmUnitId == 0 || DmlObj.GetUnitById(AmUnitId) == null || !DmlObj.VerifyUserUnit(AmUnitId, AmUserId))
                return View("Error");

            MainViewModelUsers model = new MainViewModelUsers();
            model.UnitId = AmUnitId;
            model.UnitUsers = DmlObj.GetUnitUsers(AmUnitId);
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(AmUnitId);
            model.Navigation = DmlObj.AddManagerNaviagtion(Nav, AmUnitId).AsEnumerable();
            return View(model);
        }

        public ActionResult ModifyUserProcesses(string UnitId, string UserId)
        {
            int AmUserId = 0;
            int AmUnitId = 0;
            int ModifyingUserId = 0;
            Int32.TryParse(UnitId, out AmUnitId);
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            Int32.TryParse(UserId, out ModifyingUserId);
            if (AmUserId == 0 || AmUnitId == 0 || DmlObj.GetUnitById(AmUnitId) == null || !DmlObj.VerifyUserUnit(AmUnitId, ModifyingUserId) || !DmlObj.VerifyUserUnit(AmUnitId, AmUserId))
                return View("Error");

            MainViewModelUserProcesses model = new MainViewModelUserProcesses();
            model.UnitId = AmUnitId;
            model.RemainUnitProcesses = DmlObj.GetUnitProcesses(AmUnitId).Where(m => !DmlObj.GetUnitUserProcesses(AmUnitId, ModifyingUserId).Where(k => k.Id == m.Id).Any());
            model.UserProcesses = DmlObj.GetUnitUserProcesses(AmUnitId, ModifyingUserId);
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(AmUnitId);
            model.Navigation = DmlObj.AddManagerNaviagtion(Nav, AmUnitId).AsEnumerable();
            model.ModifyingUserId = ModifyingUserId;
            return View(model);

        }

        public ActionResult RemoveUserFromProcess(string UnitId, string UserId, string ProcessId)
        {
            int AmUserId = 0;
            int AmUnitId = 0;
            int ModifyingUserId = 0;
            int AmProcessId = 0;
            Int32.TryParse(ProcessId,out AmProcessId);
            Int32.TryParse(UnitId, out AmUnitId);
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            Int32.TryParse(UserId, out ModifyingUserId);
            if (AmUserId == 0 || AmUnitId == 0 || ModifyingUserId == 0 || AmProcessId == 0 || DmlObj.GetUnitById(AmUnitId) == null || !DmlObj.VerifyUserUnit(AmUnitId, ModifyingUserId) || !DmlObj.VerifyUserUnit(AmUnitId, AmUserId))
            {
                ViewBag.Message = "به نظر می رسد پارامترهای صفحه نادرست است. و یا شما مجاز به انجام این عملیات نیستید.";
                return View("Error");
            }
                
            string message = "";
            if (DmlObj.RemoveUserFromProcess(AmUnitId, ModifyingUserId, AmProcessId, out message))
            { 
                return RedirectToAction("ModifyUserProcesses", new { UnitId = AmUnitId, UserId = ModifyingUserId });
            }
            ViewBag.Message = message;
            return View("Error");
        }
        public ActionResult AddUserInProcess(string UnitId, string UserId, string ProcessId)
        {
            int AmUserId = 0;
            int AmUnitId = 0;
            int ModifyingUserId = 0;
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            Int32.TryParse(UnitId, out AmUnitId);
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            Int32.TryParse(UserId, out ModifyingUserId);
            if (AmUserId == 0 || AmUnitId == 0 || ModifyingUserId == 0 || AmProcessId == 0 || DmlObj.GetUnitById(AmUnitId) == null || !DmlObj.VerifyUserUnit(AmUnitId, ModifyingUserId) || !DmlObj.VerifyUserUnit(AmUnitId, AmUserId))
                return View("Error");
            string message = "";

            if (DmlObj.AddUserToProcess(AmUnitId, ModifyingUserId, AmProcessId, out message))
                return RedirectToAction("ModifyUserProcesses", new { UnitId = AmUnitId, UserId = ModifyingUserId });
            ViewBag.Message = message;
            return View("Error");
        }

        public ActionResult ProcessActivities(string UnitId, string ProcessId)
        {
            int AmUserId = 0;
            int AmUnitId = 0;
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            Int32.TryParse(UnitId, out AmUnitId);
            Int32.TryParse(User.GetAmUser(), out AmUserId);

            if (AmUserId == 0 || AmUnitId == 0 || AmProcessId == 0)
                return View("Error");
            var AmUnit = DmlObj.GetUnitById(AmUnitId);
            var AmProcess = DmlObj.GetProcessById(AmProcessId);
            if (AmUnit == null || AmProcess == null) return View("Error");
            MainViewModelProcessActivities model = new MainViewModelProcessActivities();
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(AmUnitId);
            Nav = DmlObj.AddProcessActivityNaviagtion(Nav, AmProcessId, DmlObj.GetProcessById(AmProcessId).Name, AmUnitId, DmlObj.GetUnitById(AmUnitId).Name);
            model.Navigation = Nav.AsEnumerable();
            model.ActivityGropus = DmlObj.GetActivityByUnitProcess(AmUnitId,AmProcessId);
            model.Unit = AmUnit;
            model.Process = AmProcess;
            return View(model);
        }

        public ActionResult ShowActivity(string UnitId, string ProcessId, string ActivityId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            string AmUserStr = User.GetAmUser();

            if (AmActivityId == 0 || !DmlObj.CheckPageParameters(UnitId, AmUserStr, ProcessId) || !DmlObj.CheckPageParameters(AmUserStr,AmActivityId))
                return View("Error");

            MainViewModelShowActivity model = new MainViewModelShowActivity();
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, Int32.Parse(ProcessId), DmlObj.GetProcessById(Int32.Parse(ProcessId)).Name, Int32.Parse(UnitId), DmlObj.GetUnitById(Int32.Parse(UnitId)).Name, AmActivityId, DmlObj.GetActivityById(AmActivityId).Name);
            model.ActivityItemGropus = DmlObj.GetActivityItemsByActivityId(AmActivityId);
            model.Activity = DmlObj.GetActivityById(AmActivityId);
            model.Unit = DmlObj.GetUnitById(Int32.Parse(UnitId));
            model.Process = DmlObj.GetProcessById(Int32.Parse(ProcessId));
            model.Navigation = Nav;
            return View(model);
        }

        public ActionResult ShowItem(string UnitId, string ProcessId, string ActivityId, string ItemId)
        {
            if (!DmlObj.CheckPageParameters(UnitId, User.GetAmUser(),  ProcessId, ActivityId,  ItemId)) return View("Error");
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            Nav = DmlObj.AddShowItemNaviagtion(Nav, Int32.Parse(ProcessId), DmlObj.GetProcessById(Int32.Parse(ProcessId)).Name, Int32.Parse(UnitId), DmlObj.GetUnitById(Int32.Parse(UnitId)).Name, Int32.Parse(ActivityId), DmlObj.GetActivityById(Int32.Parse(ActivityId)).Name, Int32.Parse(ItemId), DmlObj.GetActivityItemById(Int32.Parse(ItemId)).TextTitle);
            MainViewModelShowItem model = new MainViewModelShowItem { Navigation = Nav, Unit = DmlObj.GetUnitById(Int32.Parse(UnitId)), Activity = DmlObj.GetActivityById(Int32.Parse(ActivityId)), Process = DmlObj.GetProcessById(Int32.Parse(ProcessId)) };
            DAL.AMActivityItem item = DmlObj.GetActivityItemById(Int32.Parse(ItemId));
            model.DocumentPages = DmlObj.GetDocumentPageCount(item.DocumentId.ToString());
            model.Item = item;
            return View(model);
        }



        public ActionResult GetDocument(string DocId, string PageNumber)
        {
            int AmDocId = 0;
            int AmPn = 0;
            Int32.TryParse(DocId, out AmDocId);
            Int32.TryParse(PageNumber,out AmPn);
            string doctype = "image/jpg";
            var imageData = DmlObj.GetDocDataById(AmDocId, out doctype);
            if (imageData == null) return null;
            if (doctype.ToLower() == "application/pdf")
            {
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                document.LoadFromStream(new MemoryStream(imageData));
                Image ImgDoc = document.SaveAsImage(AmPn, Spire.Pdf.Graphics.PdfImageType.Bitmap, 400, 400);
                ImgDoc = ImgDoc.cropImage(new Rectangle(0, 57, ImgDoc.Width, ImgDoc.Height - 57));
                return File(imageToByteArray(ImgDoc), "image/jpeg");
            }
            return File(imageData, doctype);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }


    }
}