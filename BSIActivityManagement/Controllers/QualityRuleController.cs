using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Models;
using BSIActivityManagement.ViewModel;
using System.Net;

namespace BSIActivityManagement.Controllers
{
    public class QualityRuleController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: QualityRule
        public ActionResult Create(string ActivityId, string ProcessId)
        {
            int ActId = 0;
            Int32.TryParse(ActivityId, out ActId);
            int PrcsId = 0;
            Int32.TryParse(ProcessId, out PrcsId);
            if (ActId == 0 || PrcsId == 0 || DmlObj.GetActivityById(ActId) == null ||  DmlObj.GetProcessById(PrcsId) == null)
                return View("Error");

            CreateQualityRuleViewModel model = new CreateQualityRuleViewModel
            {
                Activity = DmlObj.GetActivityById(ActId),
                Process = DmlObj.GetProcessById(PrcsId),
                IndexId = DmlObj.GetQualityIndexList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ActivityId,IndexId,ProcessId")] CreateQualityRulePostBackViewModel RuleObj)
        {

            if (RuleObj.IndexId > 0 && ModelState.IsValid)
            {
                if (DmlObj.AddQualityRule(new AMQualityRule { ActivityId = RuleObj.ActivityId, IndexId = RuleObj.IndexId }))
                    return RedirectToAction("ShowActivity", "Sysadmin", new { ActivityId = RuleObj.ActivityId, ProcessId = RuleObj.ProcessId });
            }

            if (RuleObj.IndexId <= 0)
                ModelState.AddModelError("IndexId", "شاخص انتخابی نادرست است");
            AMActivity Act = DmlObj.GetActivityById(RuleObj.ActivityId);
            AMProcess Prcs = DmlObj.GetProcessById(RuleObj.ProcessId);
            IEnumerable<AMQualityIndex> Indx = DmlObj.GetQualityIndexList();

            if (Act == null || Prcs == null) return View("Error");
            return View(new CreateQualityRuleViewModel {Activity = Act, IndexId = Indx, Process = Prcs });
        }

        public ActionResult Index(string ActivityId, string ProcessId)
        {
            int ActId = 0;
            Int32.TryParse(ActivityId, out ActId);
            int PrcsId = 0;
            Int32.TryParse(ProcessId, out PrcsId);
            if (ActId == 0 || PrcsId == 0 || DmlObj.GetActivityById(ActId) == null || DmlObj.GetProcessById(PrcsId) == null)
                return View("Error");

            QualityRuleIndexViewModel model = new QualityRuleIndexViewModel
            {
                Activity = DmlObj.GetActivityById(ActId),
                RuleList = DmlObj.GetQualityRuleListByActivityId(ActId),
                ProcessId = PrcsId
            };
            return View(model);
        }


        public ActionResult Delete(string Id, string ActivityId, string ProcessId)
        {
            int ActId = 0;
            Int32.TryParse(ActivityId, out ActId);
            int PrcsId = 0;
            Int32.TryParse(ProcessId, out PrcsId);
            if (ActId == 0 || PrcsId == 0 || DmlObj.GetActivityById(ActId) == null || DmlObj.GetProcessById(PrcsId) == null)
                return View("Error");

            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AMQualityRule RuleObj = DmlObj.GetQualityRuleById(CurrentId);
            if (RuleObj == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityId = ActId;
            ViewBag.ProcessId = PrcsId;
            return View(RuleObj);
        }

        //// POST: QualityIndex/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Id, string ActivityId, string ProcessId)
        {
            int ActId = 0;
            Int32.TryParse(ActivityId, out ActId);
            int PrcsId = 0;
            Int32.TryParse(ProcessId, out PrcsId);
            if (ActId == 0 || PrcsId == 0 || DmlObj.GetActivityById(ActId) == null || DmlObj.GetProcessById(PrcsId) == null)
                return View("Error");

            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            AMQualityRule RuleObj = DmlObj.GetQualityRuleById(CurrentId);
            if (RuleObj != null && DmlObj.DeleteQualityRuleByObject(RuleObj))
                return RedirectToAction("Index", new {ActivityId = ActId, ProcessId = PrcsId });

            return View("Error");
        }



    }
}