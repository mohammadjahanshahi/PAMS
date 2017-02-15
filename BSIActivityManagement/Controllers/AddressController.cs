using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Models;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();

        public ActionResult New(string UnitId, string ProcessId, string ActivityId, int? CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);


            if (k != null)
            {
                ViewBag.Customer = k;
            }
            else
            {
                ViewBag.Customer = new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" };
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(AMCustomerAddress AddressObj, string UnitId, string ProcessId, string ActivityId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            

            var k = DmlObj.GetCustomerById(AddressObj.CustomerId);
            if (k == null)
            {
                return View("Error");
            }
            ViewBag.Customer = k;
            if (AddressObj.PhoneType == 0)
                ModelState.AddModelError("PhoneType", "نوع شماره تلفن انتخاب نشده است");

            if (AddressObj.PhoneNumber == null || AddressObj.PhoneNumber.Length < 6 || AddressObj.PhoneNumber.Length > 14)
                ModelState.AddModelError("PhoneNumber", "شماره تماس نادرست است");

            if (ModelState.IsValid && DmlObj.AddNewAddress(AddressObj))
                return View("Success");

            return View(AddressObj);
        }



        public ActionResult Edit(string UnitId, string ProcessId, string ActivityId, int? CustomerId, int? AddressId)
        {
            if (AddressId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);

            if (k != null)
            {
                ViewBag.Customer = k;
            }
            else
            {
                ViewBag.Customer = new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" };
            }

            AMCustomerAddress AddressObj = DmlObj.GetAddressById(AddressId);

            if (AddressObj == null)
            {
                return HttpNotFound();
            }

            return View(AddressObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMCustomerAddress AddressObj, string UnitId, string ProcessId, string ActivityId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;


            var k = DmlObj.GetCustomerById(AddressObj.CustomerId);
            if (k == null)
            {
                ModelState.AddModelError("PhoneNumber", "مشتری شناسایی نشد");
            }
            if (AddressObj.PhoneType == 0)
                ModelState.AddModelError("PhoneType", "نوع شماره تلفن انتخاب نشده است");

            if (AddressObj.PhoneNumber == null || AddressObj.PhoneNumber.Length < 8 || AddressObj.PhoneNumber.Length > 14)
                ModelState.AddModelError("PhoneNumber", "شماره تماس نادرست است");

            if (ModelState.IsValid)
            {
                if (DmlObj.EditAddress(AddressObj))
                {
                    ViewBag.Customer = k;
                    return View("Success");
                }
            }

            return View(AddressObj);
        }

        public ActionResult Delete(string UnitId, string ProcessId, string ActivityId, int? id, int? CustomerId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            AMCustomerAddress AddressObj = DmlObj.GetAddressById(id);
            if (AddressObj == null)
            {
                return HttpNotFound();
            }
            return View(AddressObj);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UnitId, string ProcessId, string ActivityId, int id)
        {
            AMCustomerAddress AddressObj = DmlObj.GetAddressById(id);
            if (DmlObj.DeleteAddress(AddressObj))
                return RedirectToAction("Index", "Customer", new { UnitId = UnitId, ProcessId = ProcessId, ActivityId = ActivityId });
            else return View("Error");
        }

    }
}