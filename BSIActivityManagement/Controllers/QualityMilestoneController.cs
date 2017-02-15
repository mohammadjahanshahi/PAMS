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
using System.Globalization;
using BSIActivityManagement.Authorization;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    public class QualityMilestoneController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();

        // GET: QualityMilestone
        public ActionResult Index(string UnitId, string ProcessId, string ActivityId)
        {
            int CurrentUnit = 0;
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            int CurrentUser = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            Int32.TryParse(ProcessId, out CurrrentProcess);
            Int32.TryParse(ActivityId, out CurrentActivity);
            Int32.TryParse(User.GetAmUser(), out CurrentUser);


            if (CurrentUnit == 0 || CurrrentProcess == 0 || CurrentActivity == 0 || CurrentUser == 0 || DmlObj.GetUnitById(CurrentUnit) == null || DmlObj.GetProcessById(CurrrentProcess) == null || DmlObj.GetActivityById(CurrentActivity) == null || DmlObj.GetAmUserById(CurrentUser) == null || !DmlObj.VerifyUserUnit(CurrentUnit, CurrentUser))
                return View("Error");


            var aMQualityMileStoneEnt = DmlObj.GetQualityMileStoneList(CurrentActivity, CurrentUnit);
            ViewBag.PageParams = new ViewModel.UnitProcessActViewModel
            {
                U = CurrentUnit, A = CurrentActivity, P = CurrrentProcess
            };

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View(aMQualityMileStoneEnt);
        }

        // GET: QualityMilestone/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AMQualityMileStone aMQualityMileStone = db.AMQualityMileStoneEnt.Find(id);
        //    if (aMQualityMileStone == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aMQualityMileStone);
        //}

        // GET: QualityMilestone/Create
        public ActionResult Create(string UnitId, string ProcessId, string ActivityId)
        {
            int CurrentUnit = 0;
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            int CurrentUser = 0;
            Int32.TryParse(UnitId , out CurrentUnit);
            Int32.TryParse(ProcessId , out CurrrentProcess);
            Int32.TryParse(ActivityId , out CurrentActivity);
            Int32.TryParse(User.GetAmUser(), out CurrentUser);


            if (CurrentUnit == 0 || CurrrentProcess == 0 || CurrentActivity == 0 || CurrentUser == 0 || DmlObj.GetUnitById(CurrentUnit) == null || DmlObj.GetProcessById(CurrrentProcess) == null || DmlObj.GetActivityById(CurrentActivity) == null || DmlObj.GetAmUserById(CurrentUser) == null || !DmlObj.VerifyUserUnit(CurrentUnit, CurrentUser))
                return View("Error");

            ViewBag.RuleId = new SelectList(DmlObj.GetQualityRuleListByActivityId(CurrentActivity), "Id", "Index.Title");
            ViewBag.UnitId = CurrentUnit;
            ViewBag.UserId = CurrentUser;
            ViewBag.ProcessId = CurrrentProcess;
            ViewBag.ActivityId = CurrentActivity;

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View();
        }

        // POST: QualityMilestone/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RuleId,UnitId,Title,Description,Minimum,Maximum,UserId,RegistrationDate,ExpirationDate")] AMQualityMileStone aMQualityMileStone, string ProcessId, string ActivityId)
        {
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            Int32.TryParse(ProcessId, out CurrrentProcess);
            Int32.TryParse(ActivityId, out CurrentActivity);

            if (ModelState.IsValid)
            {
                PersianCalendar Taghvim = new PersianCalendar();
                try
                {
                    DateTime GeorgianDate = Taghvim.ToDateTime(aMQualityMileStone.ExpirationDate.Year, aMQualityMileStone.ExpirationDate.Month, aMQualityMileStone.ExpirationDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                    if (GeorgianDate != null && GeorgianDate >= DateTime.Now.AddDays(1))
                    {
                        aMQualityMileStone.ExpirationDate = GeorgianDate;
                        aMQualityMileStone.RegistrationDate = DateTime.Now;
                        if(DmlObj.AddQualityMileStone(aMQualityMileStone))
                        return RedirectToAction("Index", "QualityMileStone",new {ActivityId = CurrentActivity, ProcessId = CurrrentProcess, UnitId = aMQualityMileStone.UnitId });
                    }
                    else
                    {
                        ModelState.AddModelError("ExpirationDate", "تاریخ پایان دوره نادرست است");
                    }
                }
                catch
                {

                }
            }

            ViewBag.RuleId = new SelectList(DmlObj.GetQualityRuleListByActivityId(CurrentActivity), "Id", "Index.Title");
            ViewBag.UnitId = aMQualityMileStone.UnitId;
            ViewBag.UserId = aMQualityMileStone.UserId;
            ViewBag.ProcessId = CurrrentProcess;
            ViewBag.ActivityId = CurrentActivity;
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(aMQualityMileStone.UnitId);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, aMQualityMileStone.UnitId, DmlObj.GetUnitById(aMQualityMileStone.UnitId).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View(aMQualityMileStone);
        }

        // GET: QualityMilestone/Edit/5
        [AMAuthorization(AccessKey = "CREATE_MILESTONE")]
        public ActionResult Edit(int? id, string UnitId, string ProcessId, string ActivityId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int CurrentUnit = 0;
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            int CurrentUser = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            Int32.TryParse(ProcessId, out CurrrentProcess);
            Int32.TryParse(ActivityId, out CurrentActivity);
            Int32.TryParse(User.GetAmUser(), out CurrentUser);

            AMQualityMileStone aMQualityMileStone = DmlObj.GetQualityMileStoneById(id, CurrentUnit);
            if (aMQualityMileStone == null)
            {
                return HttpNotFound();
            }
            if (CurrentUnit == 0 || CurrrentProcess == 0 || CurrentActivity == 0 || CurrentUser == 0 || DmlObj.GetUnitById(CurrentUnit) == null || DmlObj.GetProcessById(CurrrentProcess) == null || DmlObj.GetActivityById(CurrentActivity) == null || DmlObj.GetAmUserById(CurrentUser) == null || !DmlObj.VerifyUserUnit(CurrentUnit, CurrentUser))
                return View("Error");

            ViewBag.RuleId = new SelectList(DmlObj.GetQualityRuleListByActivityId(CurrentActivity), "Id", "Index.Title");

            ViewBag.PageParams = new ViewModel.UnitProcessActViewModel
            {
                U = CurrentUnit,
                A = CurrentActivity,
                P = CurrrentProcess
            };

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View(aMQualityMileStone);
        }

        // POST: QualityMilestone/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RuleId,UnitId,Title,Description,Minimum,Maximum,UserId,RegistrationDate,ExpirationDate")] AMQualityMileStone aMQualityMileStone, string ProcessId, string ActivityId)
        {
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            Int32.TryParse(ProcessId, out CurrrentProcess);
            Int32.TryParse(ActivityId, out CurrentActivity);

            if (ModelState.IsValid)
            {
                        if (DmlObj.EditQualityMileStone(aMQualityMileStone))
                            return RedirectToAction("Index", "QualityMileStone", new { ActivityId = CurrentActivity, ProcessId = CurrrentProcess, UnitId = aMQualityMileStone.UnitId });
            }
            ViewBag.PageParams = new ViewModel.UnitProcessActViewModel
            {
                U = aMQualityMileStone.UnitId,
                A = CurrentActivity,
                P = CurrrentProcess
            };

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(aMQualityMileStone.UnitId);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, aMQualityMileStone.UnitId, DmlObj.GetUnitById(aMQualityMileStone.UnitId).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;


            return View(aMQualityMileStone);
        }

        // GET: QualityMilestone/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AMQualityMileStone aMQualityMileStone = db.AMQualityMileStoneEnt.Find(id);
        //    if (aMQualityMileStone == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aMQualityMileStone);
        //}

        //// POST: QualityMilestone/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    AMQualityMileStone aMQualityMileStone = db.AMQualityMileStoneEnt.Find(id);
        //    db.AMQualityMileStoneEnt.Remove(aMQualityMileStone);
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
