using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    [Authorize]
    public class UserPlacementController : Controller
    {
        // GET: UserPlacement
        Logic.DML DmlObj = new Logic.DML();
        public ActionResult Index(string UnitId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            int AmUnitId = 0;
            Int32.TryParse(UnitId, out AmUnitId);
            if (AmUserId == 0 || AmUnitId == 0)
            {
                ViewBag.Message = "پارمترهای صفحه نادرست است.";
                return View("Error");
            }
                
            List<UserPlacementViewModel> CurrentList = new List<UserPlacementViewModel>();
            var UserUnits = DmlObj.GetUserUnitsByUserId(AmUserId);
            foreach(var u in UserUnits)
                CurrentList.Add(DmlObj.GetUserPlaceMentViewModel(u));

            List<NavViewModel> Nav = DmlObj.GetLogoutNaviagtion();
            if (DmlObj.CheckUserTypeAccess(DmlObj.GetAccessByAccessKey("MANAGE_UNIT_USERS"), DmlObj.GetAmUserById(AmUserId).UserType))
                Nav = DmlObj.AddManagerNaviagtion(Nav, AmUnitId);
            Nav = DmlObj.AddUnitNaviagtion(Nav);
            ViewBag.Nav = Nav;
            return View(CurrentList.AsEnumerable());
        }

        public ActionResult Accept(string UnitId, string UserId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            if (AmUserId == 0)
                return View("Error");

            int currentUserId = 0;
            int currentUnitId = 0;
            bool resultProcess = false;
            DAL.AMProcess DefaultProcess = DmlObj.GetDefaultProcess(out resultProcess);

            Int32.TryParse(UserId, out currentUserId);
            Int32.TryParse(UnitId, out currentUnitId);
            DAL.AMUserPlacementReq currentReq = DmlObj.GetUserPlacementReqByUserAndUnit(currentUserId, currentUnitId, 0);
            if (currentUserId == 0 || currentUnitId == 0 || !DmlObj.GetUserUnitsByUserId(AmUserId).Where(p => p.Id == currentUnitId).Any() || currentReq == null) return View("Error");
            bool resAccept = false;
            DmlObj.AcceptUserPlacement(out resAccept, currentReq, DmlObj.GetAmUserById(AmUserId));
            if (resAccept)
                return RedirectToAction("Index",new {UnitId = currentUnitId });
            return View("Error");
        }

        public ActionResult Reject(string UnitId, string UserId)
        {
            int AmUserId = 0;
            Int32.TryParse(User.GetAmUser(), out AmUserId);
            if (AmUserId == 0)
                return View("Error");

            int currentUserId = 0;
            int currentUnitId = 0;
            bool resultProcess = false;
            DAL.AMProcess DefaultProcess = DmlObj.GetDefaultProcess(out resultProcess);

            Int32.TryParse(UserId, out currentUserId);
            Int32.TryParse(UnitId, out currentUnitId);
            DAL.AMUserPlacementReq currentReq = DmlObj.GetUserPlacementReqByUserAndUnit(currentUserId, currentUnitId, 0);
            if (currentUserId == 0 || currentUnitId == 0 || !DmlObj.GetUserUnitsByUserId(AmUserId).Where(p => p.Id == currentUnitId).Any() || currentReq == null) return View("Error");
            if (DmlObj.RejectUserPlacement(currentReq,DmlObj.GetAmUserById(AmUserId)))
                return RedirectToAction("Index", new { UnitId = currentUnitId });
            return View("Error");
        }



    }
}