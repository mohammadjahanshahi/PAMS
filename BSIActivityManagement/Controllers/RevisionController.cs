using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Extensions;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Authorization;


namespace BSIActivityManagement.Controllers
{
    public class RevisionController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();

        public ActionResult New(string ActivityId, string UnitId, string ProcessId)
        {
            if (!DmlObj.CheckPageParametersForRevision(ActivityId, UnitId, ProcessId, User.GetAmUser()))
                return View("Error");

            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, Int32.Parse(ProcessId), DmlObj.GetProcessById(Int32.Parse(ProcessId)).Name, Int32.Parse(UnitId), DmlObj.GetUnitById(Int32.Parse(UnitId)).Name, Int32.Parse(ActivityId), DmlObj.GetActivityById(Int32.Parse(ActivityId)).Name);
            ViewBag.Navigation = Nav;
            return View(new NewRevisionViewModel {Activity = DmlObj.GetActivityById(Int32.Parse(ActivityId)), Unit = DmlObj.GetUnitById(Int32.Parse(UnitId)), User = DmlObj.GetAmUserById(Int32.Parse(User.GetAmUser())), Process = DmlObj.GetProcessById(Int32.Parse(ProcessId))});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ActivityId, UnitId, ProcessId, UserId, ConflictDescription, ConflictSource, ConflictSolution")] AMRevision RevisionObj)
        {
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(RevisionObj.UnitId);
            Nav = DmlObj.AddShowActivityNaviagtion(Nav, RevisionObj.ProcessId, DmlObj.GetProcessById(RevisionObj.ProcessId).Name, RevisionObj.UnitId, DmlObj.GetUnitById(RevisionObj.UnitId).Name, RevisionObj.ActivityId, DmlObj.GetActivityById(RevisionObj.ActivityId).Name);
            ViewBag.Navigation = Nav;
            if (!ModelState.IsValid)
            {
                return View(RevisionObj);
            }
                
            if (DmlObj.AddRevision(RevisionObj))
                return RedirectToAction("ShowActivity","Main", new { UnitId = RevisionObj.UnitId, ProcessId = RevisionObj.ProcessId, ActivityId = RevisionObj.ActivityId });
            return View("Error");

        }

        string GetStatusMessage(int k)
        {
            switch (k)
            {
                case 1:
                    return "نظرات کاربران";
                case 2:
                    return "تایید مسئول فرآیند";
                case 3:
                    return "رد شده توسط مسئول فرآیند";
                case 4:
                    return "تایید شده توسط مسئول واحد";
                case 5:
                    return "رد شده توسط مسئول واحد";
                case 6:
                    return "تایید شده توسط مدیر استان";
                case 7:
                    return "رد شده توسط مدیر استان";
                case 8:
                    return "تایید شده توسط اداره سازمان و روشها";
                case 9:
                    return "رد شده توسط اداره سازمان و روشها";
                case 10:
                    return "تایید شده توسط هیات عامل";
                case 11:
                    return "رد شده توسط هیات عامل";
                case 12:
                    return "نهایی و اجرایی شده است";
                default:
                    return "منتظر تایید مسئول فرآیند";
            }
        }

        public ActionResult Me(string UnitId)
        {
            
            if (!DmlObj.CheckPageParameters(UnitId))
                return View("Error");
            List<IndexRevisionViewModel> modelList = new List<IndexRevisionViewModel>();
            var revisions = DmlObj.GetRevisionListByRegistrarUserId(Int32.Parse(User.GetAmUser()));
            System.Globalization.PersianCalendar taghvim = new System.Globalization.PersianCalendar();
            foreach (var item in revisions)
            {
                IndexRevisionViewModel model = new IndexRevisionViewModel
                {
                    Id = item.Id,
                    ActivityTitle = item.Activity.Name,
                    ConflictDescription = item.ConflictDescription,
                    PersianDate = new PersianDateViewModel {
                        DateYr = taghvim.GetYear(item.RegDateTime),
                        DateMM = taghvim.GetMonth(item.RegDateTime),
                        DateDD = taghvim.GetDayOfMonth(item.RegDateTime),
                        DateHR = taghvim.GetHour(item.RegDateTime),
                        DateMI = taghvim.GetMinute(item.RegDateTime),
                        DateSE = taghvim.GetSecond(item.RegDateTime)
                    },
                    Days = (DateTime.Now - item.RegDateTime).Days,
                    Hours = (DateTime.Now - item.RegDateTime).Hours
                };
                IEnumerable<AMRevisionStatus> StatusList = item.StatusList.AsEnumerable();
                if (StatusList.Count() > 0)
                {
                    int k = StatusList.OrderBy(m => m.Status).Last().Status;
                    model.LevelStatus = k;
                    model.CurrentStatus = GetStatusMessage(k);
                }
                else 
                {
                    model.LevelStatus = 0;
                    model.CurrentStatus = "هیچ نظری ثبت نشده است";
                }

                modelList.Add(model);
            }
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;
            return View(modelList.AsEnumerable());
        }

        public ActionResult History(string RevisionId, string UnitId)
        {
            int RevId = 0;
            Int32.TryParse(RevisionId, out RevId);
            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            if (RevId == 0 || !DmlObj.CheckPageParameters(UnitId) || DmlObj.GetRevisionById(RevId) == null)
                return View("Error");
            //if (DmlObj.GetRevisionById(RevId).UserId != UserId) return View("Error"); //Users From another units wouldn't be able to view history

            var statusList = DmlObj.GetRevisionById(RevId).StatusList.AsEnumerable();

            System.Globalization.PersianCalendar taghvim = new System.Globalization.PersianCalendar();
            List<RevisionHistoryViewModel> modelList = new List<RevisionHistoryViewModel>();
            foreach (var s in statusList)
            {
                RevisionHistoryViewModel model = new RevisionHistoryViewModel
                {
                    PersianDate = new PersianDateViewModel
                    {
                        DateYr = taghvim.GetYear(s.StatusDateTime),
                        DateMM = taghvim.GetMonth(s.StatusDateTime),
                        DateDD = taghvim.GetDayOfMonth(s.StatusDateTime),
                        DateHR = taghvim.GetHour(s.StatusDateTime),
                        DateMI = taghvim.GetMinute(s.StatusDateTime),
                        DateSE = taghvim.GetSecond(s.StatusDateTime)
                    },
                    Days = (DateTime.Now - s.StatusDateTime).Days,
                    Hours = (DateTime.Now - s.StatusDateTime).Hours,
                    Status = s
                };
                modelList.Add(model);
            }
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;
            return View(modelList.AsEnumerable());

        }

        public ActionResult SelectUnitProcess(string UnitId)
        {
            if (!DmlObj.CheckPageParameters(UnitId))
                return View("Error");
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;

            return View(DmlObj.GetUnitProcesses(Int32.Parse(UnitId)));
        }

        public ActionResult UnitProcesses(string UnitId, string ProcessId)
        {
            if (!DmlObj.CheckPageParameters(UnitId, User.GetAmUser(), ProcessId) || !DmlObj.VerifyUserUnit(Int32.Parse(UnitId), Int32.Parse(User.GetAmUser())))
                return View("Error");
            
            List<IndexRevisionViewModel> modelList = new List<IndexRevisionViewModel>();
            var revisions = DmlObj.GetProcessRevisionListByRegistrarUserId(Int32.Parse(UnitId), Int32.Parse(ProcessId));
            System.Globalization.PersianCalendar taghvim = new System.Globalization.PersianCalendar();
            foreach (var item in revisions)
            {
                IndexRevisionViewModel model = new IndexRevisionViewModel
                {
                    Id = item.Id,
                    ActivityTitle = item.Activity.Name,
                    ConflictDescription = item.ConflictDescription,
                    PersianDate = new PersianDateViewModel
                    {
                        DateYr = taghvim.GetYear(item.RegDateTime),
                        DateMM = taghvim.GetMonth(item.RegDateTime),
                        DateDD = taghvim.GetDayOfMonth(item.RegDateTime),
                        DateHR = taghvim.GetHour(item.RegDateTime),
                        DateMI = taghvim.GetMinute(item.RegDateTime),
                        DateSE = taghvim.GetSecond(item.RegDateTime)
                    },
                    Days = (DateTime.Now - item.RegDateTime).Days,
                    Hours = (DateTime.Now - item.RegDateTime).Hours
                };
                IEnumerable<AMRevisionStatus> StatusList = item.StatusList.AsEnumerable();
                if (StatusList.Count() > 0)
                {
                    int k = StatusList.OrderBy(m => m.Status).Last().Status;
                    model.LevelStatus = k;
                    model.CurrentStatus = GetStatusMessage(k);
                }
                else
                {
                    model.LevelStatus = 0;
                    model.CurrentStatus = "هیچ نظری ثبت نشده است";
                }

                modelList.Add(model);
            }
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;
            return View(modelList.AsEnumerable());
        }

        public ActionResult Unit(string UnitId)
        {
            if (!DmlObj.CheckPageParameters(UnitId) || !DmlObj.VerifyUserUnit(Int32.Parse(UnitId), Int32.Parse(User.GetAmUser())))
                return View("Error");

            List<IndexRevisionViewModel> modelList = new List<IndexRevisionViewModel>();
            var revisions = DmlObj.GetProcessRevisionListByUnitId(Int32.Parse(UnitId));
            System.Globalization.PersianCalendar taghvim = new System.Globalization.PersianCalendar();
            foreach (var item in revisions)
            {
                IndexRevisionViewModel model = new IndexRevisionViewModel
                {
                    Id = item.Id,
                    ActivityTitle = item.Activity.Name,
                    ConflictDescription = item.ConflictDescription,
                    PersianDate = new PersianDateViewModel
                    {
                        DateYr = taghvim.GetYear(item.RegDateTime),
                        DateMM = taghvim.GetMonth(item.RegDateTime),
                        DateDD = taghvim.GetDayOfMonth(item.RegDateTime),
                        DateHR = taghvim.GetHour(item.RegDateTime),
                        DateMI = taghvim.GetMinute(item.RegDateTime),
                        DateSE = taghvim.GetSecond(item.RegDateTime)
                    },
                    Days = (DateTime.Now - item.RegDateTime).Days,
                    Hours = (DateTime.Now - item.RegDateTime).Hours
                };
                IEnumerable<AMRevisionStatus> StatusList = item.StatusList.AsEnumerable();
                if (StatusList.Count() > 0)
                {
                    int k = StatusList.OrderBy(m => m.Status).Last().Status;
                    model.LevelStatus = k;
                    model.CurrentStatus = GetStatusMessage(k);
                }
                else
                {
                    model.LevelStatus = 0;
                    model.CurrentStatus = "هیچ نظری ثبت نشده است";
                }

                modelList.Add(model);
            }
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;
            return View(modelList.AsEnumerable());
        }

        public ActionResult AppliedToMe(string UnitId)
        {
            if (!DmlObj.CheckPageParameters(UnitId) || !DmlObj.VerifyUserUnit(Int32.Parse(UnitId), Int32.Parse(User.GetAmUser())))
                return View("Error");

            List<IndexRevisionFullViewModel> modelList = new List<IndexRevisionFullViewModel>();
            var revisions = DmlObj.GetRevisionListAppliedToMe(Int32.Parse(UnitId), Int32.Parse(User.GetAmUser()));
            System.Globalization.PersianCalendar taghvim = new System.Globalization.PersianCalendar();
            foreach (var item in revisions)
            {
                IndexRevisionFullViewModel model = new IndexRevisionFullViewModel
                {
                    PersianDate = new PersianDateViewModel
                    {
                        DateYr = taghvim.GetYear(item.RegDateTime),
                        DateMM = taghvim.GetMonth(item.RegDateTime),
                        DateDD = taghvim.GetDayOfMonth(item.RegDateTime),
                        DateHR = taghvim.GetHour(item.RegDateTime),
                        DateMI = taghvim.GetMinute(item.RegDateTime),
                        DateSE = taghvim.GetSecond(item.RegDateTime)
                    },
                    Days = (DateTime.Now - item.RegDateTime).Days,
                    Hours = (DateTime.Now - item.RegDateTime).Hours,
                    Revision = item
                };
                IEnumerable<AMRevisionStatus> StatusList = item.StatusList.AsEnumerable();
                if (StatusList.Count() > 0)
                {
                    int k = StatusList.OrderBy(m => m.Status).Last().Status;
                    model.LevelStatus = k;
                    model.CurrentStatus = GetStatusMessage(k);
                }
                else
                {
                    model.LevelStatus = 0;
                    model.CurrentStatus = "هیچ نظری ثبت نشده است";
                }

                modelList.Add(model);
            }
            List<NavViewModel> Nav = DmlObj.GetMainNaviagtion(Int32.Parse(UnitId));
            ViewBag.Navigation = Nav;
            ViewBag.CurrentUnitId = UnitId;
            return View(modelList.AsEnumerable());
        }

        public ActionResult Confirm(string UnitId, string RevisionId)
        {
            
            int RevId = 0;
            Int32.TryParse(RevisionId, out RevId);
            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            if (RevId == 0 || !DmlObj.CheckPageParameters(UnitId) || DmlObj.GetRevisionById(RevId) == null)
                return View("Error");

            if (!DmlObj.CheckUserAccessKeyByUserId("REVISION_CONFIRM_UNIT", UserId.ToString()) && !DmlObj.CheckUserAccessKeyByUserId("REVISION_CONFIRM_ALL", UserId.ToString()))
                return View("Error");

            if (DmlObj.ConfirmRevision(RevId, UserId, Int32.Parse(UnitId)))
                return RedirectToAction("AppliedToMe", new { UnitId = UnitId });

            return View("Error");
        }

        public ActionResult Reject(string UnitId, string RevisionId)
        {

            int RevId = 0;
            Int32.TryParse(RevisionId, out RevId);
            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            if (RevId == 0 || !DmlObj.CheckPageParameters(UnitId) || DmlObj.GetRevisionById(RevId) == null)
                return View("Error");

            if (!DmlObj.CheckUserAccessKeyByUserId("REVISION_CONFIRM_UNIT", UserId.ToString()) && !DmlObj.CheckUserAccessKeyByUserId("REVISION_CONFIRM_ALL", UserId.ToString()))
                return View("Error");

            if (DmlObj.RejectRevision(RevId, UserId, Int32.Parse(UnitId)))
                return RedirectToAction("AppliedToMe", new { UnitId = UnitId });

            return View("Error");
        }

    }
}