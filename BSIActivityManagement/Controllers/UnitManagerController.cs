using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSIActivityManagement.Controllers
{
    public class UnitManagerController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: UnitManager
        public ActionResult Index(string UnitId)
        {
            int CurrentUnit = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            if(CurrentUnit == 0) return View();
            if (DmlObj.GetUnitById(CurrentUnit) != null)
                return View("IndexUser", new { UnitId = CurrentUnit });
            return View("Error");
        }
        
        public ActionResult IndexUser(string UnitId)
        {
            int CurrentUnit = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            if (CurrentUnit == 0) return View("Index");
            if (DmlObj.GetUnitById(CurrentUnit) != null)
            {
                ViewBag.UnitId = UnitId;
                return View();
            }
            return View("Error");
        }

        public ActionResult Search(string AMquery)
        {
            //var keywordlist = AMquery.Trim().Split(' ', ';', ',', '+').Where(str => str.Length >= 1);
            //ViewBag.currentQuery = AMquery;
            return View(DmlObj.GetUnitByIdentity(AMquery));
        }

        public ActionResult SearchUser(string AMquery, string UnitId)
        {
            int CurrentUnit = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            if (CurrentUnit == 0 || DmlObj.GetUnitById(CurrentUnit) == null) return View("Index");
            //var keywordlist = AMquery.Trim().Split(' ', ';', ',', '+').Where(str => str.Length >= 1);
            ViewBag.currentQuery = AMquery;
            return View(new ViewModel.SearchUserViewModel
                {
                Unit = DmlObj.GetUnitById(CurrentUnit),
                Users = DmlObj.SearchUserByUserName(AMquery)
                });
        }
        public ActionResult Add(string UnitId, string UserId)
        {
            int CurrentUnit = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            int CurrentUser = 0;
            Int32.TryParse(UserId, out CurrentUser);
            if (CurrentUnit == 0 || CurrentUser == 0 || DmlObj.GetUnitById(CurrentUnit) == null || DmlObj.GetAmUserById(CurrentUser) == null) return View("Error");

            return View(new ViewModel.AddUnitManagerViewModel
            {
                SelectedUnit = DmlObj.GetUnitById(CurrentUnit),
                SelectedUser = DmlObj.GetAmUserById(CurrentUser)
            }
            );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInto(string SelectedUnit_Id, string SelectedUser_Id)
        {
            int CurrentUnit = 0;
            Int32.TryParse(SelectedUnit_Id, out CurrentUnit);
            int CurrentUser = 0;
            Int32.TryParse(SelectedUser_Id, out CurrentUser);
            if (CurrentUnit == 0 || CurrentUser == 0 || DmlObj.GetUnitById(CurrentUnit) == null || DmlObj.GetAmUserById(CurrentUser) == null) return View("Error");

            ViewModel.AddUnitManagerViewModel model = new ViewModel.AddUnitManagerViewModel
            {
                SelectedUnit = DmlObj.GetUnitById(CurrentUnit),
                SelectedUser = DmlObj.GetAmUserById(CurrentUser)
            };

            string message = "";
            if (ModelState.IsValid && DmlObj.AddUnitManager(model, out message))
                    return View("Success");
            ModelState.AddModelError("SelectedUnit", message);
            return View("Add",model);
        }
    }
}