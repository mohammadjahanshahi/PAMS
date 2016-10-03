using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Logic;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.Models;
using BSIActivityManagement.DAL;

namespace BSIActivityManagement.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access

        DML DMLObj = new DML();
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Name,Description,AccessKey,Value")] AMAccess Access)
        {
            if (!ModelState.IsValid)
                return View(Access);
            bool resAdd = false;
            var addedUserT = DMLObj.AddNewAccess(Access, out resAdd);
            if (resAdd)
                return RedirectToAction("Index", "SysAdmin");
            return View(Access);
        }

        public ActionResult Edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            var AccessObj = DMLObj.GetAccessById(CurrentId);
            if (AccessObj == null) return View("Error");
            return View(AccessObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,AccessKey,Value")] AMAccess AccessT)
        {
            if (!ModelState.IsValid)
                return View(AccessT);
            if (DMLObj.AccessEditByObject(AccessT))
                return RedirectToAction("Index", "SysAdmin");
            return View(AccessT);
        }

        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (DMLObj.AccessHasUser(CurrentId))
            {
                ModelState.AddModelError("Name", "دستکم یک نوع از کاربری دارای این نوع مجوز است و برای حذف ابتدا باید آن مجوز برای نوع کاربری تغییر داده شود");
            }
            return View(DMLObj.GetAccessById(CurrentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,Name,Description,AccessKey,Value")] AMAccess AccessObj)
        {
            int CurrentId = 0;
            Int32.TryParse(AccessObj.Id.ToString(), out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (!DMLObj.AccessHasUser(CurrentId) && DMLObj.DeleteAccessById(CurrentId))
                return RedirectToAction("Index", "SysAdmin");
            return View("Error");
        }
    }
}