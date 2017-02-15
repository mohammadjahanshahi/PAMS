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
    public class InstallmentsFollowUpController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: InstallmentsFollowUp
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

            AMUnitOfOrg UnitObj = DmlObj.GetUnitById(CurrentUnit);
            AMProcess ProcessObj = DmlObj.GetProcessById(CurrrentProcess);
            AMActivity ActivityObj = DmlObj.GetActivityById(CurrentActivity);

            if (CurrentUnit == 0 || CurrrentProcess == 0 || CurrentActivity == 0 || CurrentUser == 0 || UnitObj == null || ProcessObj == null || ActivityObj == null || DmlObj.GetAmUserById(CurrentUser) == null || !DmlObj.VerifyUserUnit(CurrentUnit, CurrentUser))
                return View("Error");

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(CurrentUnit);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, CurrrentProcess, DmlObj.GetProcessById(CurrrentProcess).Name, CurrentUnit, DmlObj.GetUnitById(CurrentUnit).Name, CurrentActivity, DmlObj.GetActivityById(CurrentActivity).Name);
            ViewBag.Nav = Nav;

            ViewBag.UPA = new UnitProcessActObjectViewModel { Unit = UnitObj, Process = ProcessObj, Activity = ActivityObj };

            FollowUpIndexViewModel model = new FollowUpIndexViewModel
            {
                LoansWithInstallmentNotification = DmlObj.GetLoansWithInstallmentNotification(CurrentUnit),
                LoansWithCallNotification = DmlObj.GetLoansWithCallNotification(CurrentUnit),
                TodayOverDueLoans = DmlObj.GetLoansWithTodayOverDueDate(CurrentUnit),
                WeekOverDueLoans = DmlObj.GetLoansWithWeekOverDueDate(CurrentUnit),
                OneMonthOverdueLoans = DmlObj.GetLoansWithMonthOverDueDate(CurrentUnit),
                //TwoMonthsOverDueLoans = DmlObj.GetLoansWithTwoMonthsOverDueDate(CurrentUnit),
                //MoreThanTwoMonthsOverDueLoans = DmlObj.GetLoansWithMoreThanTwoMonthsOverDueDate(CurrentUnit)
            };

            return View(model);
        }

        public ActionResult MyLog(string UnitId, string ProcessId, string ActivityId)
        {
            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            int UnitIdInt = 0;
            Int32.TryParse(UnitId, out UnitIdInt);
            if (UnitIdInt == 0 || UserId == 0) return View("Error");
            var Unit = DmlObj.GetUnitById(UnitIdInt);
            var CurrentUser = DmlObj.GetAmUserById(UserId);
            if (Unit == null || CurrentUser == null || !DmlObj.VerifyUserUnit(Unit.Id, CurrentUser.Id)) return View("Error");

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            return View(new UserLogViewModel
            {
                ThisUser = CurrentUser,
                CallList = DmlObj.GetUserCallLog(CurrentUser),
                SetInstallmentNotificationList = DmlObj.GetUserInstallmentNotification(CurrentUser),
                DoneInstallmentNotificationList = DmlObj.GetUserInstallmentNotificationDone(CurrentUser),
                SetInstallmentStatusList = DmlObj.GetUserInstallmentStatusLog(CurrentUser),
                UpdateLogList = DmlObj.GetUserLoanUpdateLog(CurrentUser),
                FollowUpScore = DmlObj.GetUserFollowUpScore(CurrentUser, Unit)
            });
        }


        [HttpPost]
        public JsonResult SearchMizanLoan(string MizanLoanQuery)
        {
            if (MizanLoanQuery == null || MizanLoanQuery.Length < 4)
                return Json(null, JsonRequestBehavior.AllowGet);

            var k = DmlObj.GetLoanExtraInfoByQuery(MizanLoanQuery);
            var list = k.Select(m => new { Type = Extensions.HtmlExtensions.LoanExtraInfoEnumDisplayNameFor(m.ValueType).ToHtmlString(), value = m.Value, loanNumber = m.Loan.LoanNumber });
            
            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchFailedLoanMizan(string MizanLoanQuery)
        {
            if (MizanLoanQuery == null || MizanLoanQuery.Length < 4)
                return Json(null, JsonRequestBehavior.AllowGet);

            var k = DmlObj.SearchFailedMizanLoan(MizanLoanQuery);
            var list = k.Select(m => new { Type = m.BSIBranchCode , value = m.ConversionMessage, loanNumber = m.Id });

            return Json(list.ToArray(), JsonRequestBehavior.AllowGet);
        }

    }
}