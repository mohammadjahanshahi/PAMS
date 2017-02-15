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
    public class BankAccountController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();

        public ActionResult New(string UnitId, string ProcessId, string ActivityId, int CustomerId)
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
        public ActionResult New([Bind(Include = "Id,AccountType,AccountNumber")] AMAccount aMAccount, string UnitId, string ProcessId, string ActivityId, int CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);
            if (k == null)
            {
                ModelState.AddModelError("AccountNumber", "مشتری شناسایی نشد");
            }
            if (aMAccount.AccountType == 0)
                ModelState.AddModelError("AccountType", "نوع حساب انتخاب نشده است");

            if(aMAccount.AccountNumber == null || aMAccount.AccountNumber.Length < 6 || aMAccount.AccountNumber.Length > 13)
                ModelState.AddModelError("AccountNumber", "شماره حساب نادرست است");

            if (ModelState.IsValid)
            {
                if (DmlObj.AddNewAccount(aMAccount, k))
                {
                    ViewBag.Customer = k;
                    return View("Success");
                }
            }

            return View(aMAccount);
        }

        public ActionResult Edit(string UnitId, string ProcessId, string ActivityId, int? CustomerId,int? AccountId)
        {
            if (AccountId == null)
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

            AMAccount AccountObj = DmlObj.GetAccountById(AccountId);

            if (AccountObj == null)
            {
                return HttpNotFound();
            }

            return View(AccountObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountType,AccountNumber")] AMAccount aMAccount, string UnitId, string ProcessId, string ActivityId, int CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);
            if (k == null)
            {
                ModelState.AddModelError("AccountNumber", "مشتری شناسایی نشد");
            }
            if (aMAccount.AccountType == 0)
                ModelState.AddModelError("AccountType", "نوع حساب انتخاب نشده است");

            if (aMAccount.AccountNumber.Length < 6 && aMAccount.AccountNumber.Length > 13)
                ModelState.AddModelError("AccountNumber", "شماره حساب نادرست است");

            if (ModelState.IsValid)
            {
                if (DmlObj.EditAccount(aMAccount))
                {
                    ViewBag.Customer = k;
                    return View("Success");
                }
            }
            ViewBag.Customer = k == null ? new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" } : k;
            return View(aMAccount);
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

            AMAccount AccountObj = DmlObj.GetAccountById(id);
            if (AccountObj == null)
            {
                return HttpNotFound();
            }
            return View(AccountObj);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UnitId, string ProcessId, string ActivityId, int id)
        {
            AMAccount AccountObj = DmlObj.GetAccountById(id);
            if (DmlObj.DeleteAccount(AccountObj))
                return RedirectToAction("Index", "Customer", new {UnitId = UnitId, ProcessId = ProcessId, ActivityId = ActivityId });
            else return View("Error");
        }


    }
}