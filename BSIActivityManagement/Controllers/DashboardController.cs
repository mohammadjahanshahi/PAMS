using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    public class DashboardController : Controller
    {
        BSIActivityManagement.Logic.DML DmlObj = new Logic.DML();
        // GET: Dashborad
        public ActionResult Index(string UnitId)
        {
            if (!DmlObj.CheckPageParameters(UnitId))
                return View("Error");

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnit = UnitId;
            return View();
        }
    }
}