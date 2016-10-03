using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSIActivityManagement.Controllers
{
    public class UserTypeManagerController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: UnitManager
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult SearchUser(string UserQuery)
        {
            var keywordlist = UserQuery.Trim().Split(' ', ';', ',', '+').Where(str => str.Length >= 1);
            ViewBag.currentQuery = UserQuery;
            return View(DmlObj.SearchUserByKeyWords(keywordlist));
        }
        public ActionResult Select(string UserId)
        {
            int CurrentUser = 0;
            Int32.TryParse(UserId, out CurrentUser);
            if (CurrentUser == 0 || DmlObj.GetAmUserById(CurrentUser) == null) return View("Error");

            return View(new ViewModel.UserTypeUsersViewModel
            {
                Types = DmlObj.GetUserTypeList(),
                User = DmlObj.GetAmUserById(CurrentUser)
            }
            );
        }

        public ActionResult Add(string TypeId, string UserId)
        {
            int CurrentType = 0;
            Int32.TryParse(TypeId, out CurrentType);
            int CurrentUser = 0;
            Int32.TryParse(UserId, out CurrentUser);
            if (CurrentType == 0 || CurrentUser == 0 || DmlObj.GetUserTypeById(CurrentType) == null || DmlObj.GetAmUserById(CurrentUser) == null) return View("Error");

            ViewModel.AddUserTypeManagerViewModel model = new ViewModel.AddUserTypeManagerViewModel
            {
                Type = DmlObj.GetUserTypeById(CurrentType),
                User = DmlObj.GetAmUserById(CurrentUser)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInto(string TypeId, string UserId)
        {
            int CurrentType = 0;
            Int32.TryParse(TypeId, out CurrentType);
            int CurrentUser = 0;
            Int32.TryParse(UserId, out CurrentUser);
            if (CurrentType == 0 || CurrentUser == 0 || DmlObj.GetUserTypeById(CurrentType) == null || DmlObj.GetAmUserById(CurrentUser) == null) return View("Error");

            ViewModel.AddUserTypeManagerViewModel model = new ViewModel.AddUserTypeManagerViewModel
            {
                Type = DmlObj.GetUserTypeById(CurrentType),
                User = DmlObj.GetAmUserById(CurrentUser)
            };

            string message = "";
            if (ModelState.IsValid && DmlObj.SetUserType(model, out message))
                return View("Success");
            ModelState.AddModelError("TypeId", message);
            return View("Add",model);
        }


    }
}