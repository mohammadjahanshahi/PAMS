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
    public class ActivityRegistrationController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: Activity Registration
        public ActionResult Register(string UnitId, string ProcessId, string ActivityId)
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
            if(CurrentActivity == 228)
            {
                return RedirectToAction("Index", "InstallmentsFollowUp", new {UnitId = CurrentUnit, ProcessId = CurrrentProcess, ActivityId = CurrentActivity });
            }

            ViewBag.RuleId = new SelectList(DmlObj.GetQualityRuleListByActivityId(CurrentActivity), "Id", "Index.Title");
            RegisterActivityViewModel model = new RegisterActivityViewModel
            {
                UnitId = CurrentUnit,
                ActivityId = CurrentActivity,
                ProcessId = CurrrentProcess,
                UserId = CurrentUser
            };


            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Id,ActivityId,UnitId,UserId,ProcessId,RuleId,Description,ActivityData")] RegisterActivityViewModel model)
        {
            if(ModelState.IsValid)
            {
                if (DmlObj.RegisterActivityData(model))
                {
                   return RedirectToAction("Index", new { ActivityId = model.ActivityId, UnitId = model.UnitId, ProcessId = model.ProcessId });
                }
            }
            ViewBag.RuleId = new SelectList(DmlObj.GetQualityRuleListByActivityId(model.ActivityId), "Id", "Index.Title");
            return View(model);
        }

        public ActionResult Index(string ActivityId, string UnitId, string ProcessId)
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

            ActivityQualityIndexViewModel model = new ActivityQualityIndexViewModel
            {
                Activity = DmlObj.GetActivityById(CurrentActivity),
                Process = DmlObj.GetProcessById(CurrrentProcess),
                Unit = DmlObj.GetUnitById(CurrentUnit),
                MileStoneStatusList = DmlObj.GetActivityQualityStatusList(CurrentActivity)
            };

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.QualityNavigation = Nav;

            return View(model);
        }

    }
}