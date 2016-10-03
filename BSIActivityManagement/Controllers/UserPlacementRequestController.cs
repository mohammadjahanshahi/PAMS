using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    public class UserPlacementRequestController : Controller
    {
        // GET: UserPlacementRequest
        Logic.DML DmlObj = new Logic.DML();
        public ActionResult Index()
        {
            List<NavViewModel> Nav = DmlObj.GetLogoutNaviagtion();
            Nav = DmlObj.AddUnitNaviagtion(Nav);
            ViewBag.Nav = Nav.AsEnumerable();
            return View();
        }
        public ActionResult Search(string AMquery)
        {
            var keywordlist = AMquery.Trim().Split(' ', ';', ',', '+').Where(str => str.Length >= 1);
            ViewBag.currentQuery = AMquery;
            List<NavViewModel> Nav = DmlObj.GetLogoutNaviagtion();
            Nav = DmlObj.AddUnitNaviagtion(Nav);
            ViewBag.Nav = Nav.AsEnumerable();
            return View(DmlObj.SearchUnitByKeyWords(keywordlist));
        }
        public ActionResult Add(string UnitId)
        {
            int CurrentUnitId = 0;
            Int32.TryParse(UnitId, out CurrentUnitId);
            if (CurrentUnitId == 0 || DmlObj.GetUnitById(CurrentUnitId) == null) return View("Error");
            int CurrentUserId = 0;
            Int32.TryParse(User.GetAmUser(), out CurrentUserId);
            if (CurrentUserId == 0 || DmlObj.GetAmUserById(CurrentUserId) == null) return View("Error");
            bool resAdd = false;
            DmlObj.AddUserPlacementRequest(CurrentUserId, CurrentUnitId, out resAdd);
            if (resAdd)
            {
                List<NavViewModel> Nav = DmlObj.GetLogoutNaviagtion();
                Nav = DmlObj.AddUnitNaviagtion(Nav);
                ViewBag.Nav = Nav.AsEnumerable();
                return View("Success");
            }
                
            return View("Error");
        }
    }
}