using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Models;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    public class CustomerController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: Customer

        public ActionResult Index(string UnitId, string ProcessId, string ActivityId, int? CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            return DmlObj.GetCustomerById(CustomerId) != null ? View(DmlObj.GetCustomerById(CustomerId)) : View();
        }

        [HttpPost]
        public ActionResult Index(string UnitId, string ProcessId, string ActivityId, string CustomerNumber)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;
            var k = DmlObj.GetCustomerByCustomerNumber(CustomerNumber);
            if (k.Count() > 0)
            {
                return View(k.FirstOrDefault());
            }
            return View(new AMCustomer {Id = 0, CustomerNumber = CustomerNumber });
        }

        // GET: Customer/Create
        public ActionResult New(string UnitId, string ProcessId, string ActivityId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "Id,FirstName,Lastname,CustomerType,CustomerNumber")] AMCustomer aMCustomer, string UnitId, string ProcessId, string ActivityId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;
            var k = DmlObj.GetCustomerByCustomerNumber(aMCustomer.CustomerNumber);
            if (k.Count() > 0)
            {
                ModelState.AddModelError("CustomerNumber", "این شماره مشتری قبلا ثبت شده است");
            }
            if (aMCustomer.CustomerType == 0)
                ModelState.AddModelError("CustomerType", "نوع مشتری انتخاب نشده است");

            if (ModelState.IsValid)
            {
                if (DmlObj.AddNewCustomer(aMCustomer))
                {
                    ViewBag.CustomerNumber = aMCustomer.CustomerNumber;
                    return View("Success");
                }
            }
            return View(aMCustomer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(string UnitId, string ProcessId, string ActivityId, int? CustomerId)
        {
            if (CustomerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            AMCustomer aMCustomer = DmlObj.GetCustomerById(CustomerId);

            if (aMCustomer == null)
            {
                return HttpNotFound();
            }
            return View(aMCustomer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,Lastname,CustomerType,CustomerNumber")] AMCustomer aMCustomer, string UnitId, string ProcessId, string ActivityId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetOtherCustomersByCustomerNumber(aMCustomer.CustomerNumber, aMCustomer.Id);
            if (k.Count() > 0)
            {
                ModelState.AddModelError("CustomerNumber", "این شماره مشتری قبلا ثبت شده است");
            }
            if (aMCustomer.CustomerType == 0)
                ModelState.AddModelError("CustomerType", "نوع مشتری انتخاب نشده است");

            if (ModelState.IsValid)
            {
                if(DmlObj.EditCustomer(aMCustomer))
                {
                    ViewBag.CustomerNumber = aMCustomer.CustomerNumber;
                    return View("Success");
                }               
                    
            }
            return View(aMCustomer);
        }

        // GET: Customer/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AMCustomer aMCustomer = db.AMCustomerEnt.Find(id);
        //    if (aMCustomer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aMCustomer);
        //}

        //// POST: Customer/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    AMCustomer aMCustomer = db.AMCustomerEnt.Find(id);
        //    db.AMCustomerEnt.Remove(aMCustomer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
