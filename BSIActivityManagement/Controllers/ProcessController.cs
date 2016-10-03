using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Logic;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.Models;
using BSIActivityManagement.DAL;
using System.IO;
using System.Drawing;

namespace BSIActivityManagement.Controllers
{
    public class ProcessController : Controller
    {
        DML DMLObj = new DML();
        // GET: Posts

        public ActionResult Add(int? ProcessParentId)
        {
            ViewBag.ParentProcess = DMLObj.GetProcessById(ProcessParentId);
            ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ParentId,Name,Description,ProcessTypeId")] AMProcess ProcessObj)
        {
            if (ModelState.ContainsKey("ProcessType"))
                ModelState["ProcessType"].Errors.Clear();
            if (!ModelState.IsValid)
            {
                ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
                ViewBag.ParentProcess = DMLObj.GetProcessById(ProcessObj.ParentId);
                return View(ProcessObj);
            }
            bool operationRes = false;
            ProcessObj.ProcessType = DMLObj.GetProcessTypeById(ProcessObj.ProcessTypeId);
            var addedProcess = DMLObj.AddNewProcess(ProcessObj, out operationRes);
            if (operationRes)
                return RedirectToAction("Index", "SysAdmin", new { Prcs = addedProcess.Id });
            ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
            ViewBag.ParentProcess = DMLObj.GetProcessById(ProcessObj.ParentId);
            return View(ProcessObj);
        }

        public ActionResult Edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            var ProcessObj = DMLObj.GetProcessById(CurrentId);
            if (ProcessObj == null) return View("Error");
            ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
            ViewBag.ParentProcess = DMLObj.GetProcessById(ProcessObj.ParentId);
            return View(ProcessObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentId,Name,Description,ProcessTypeId")] AMProcess ProcessObj)
        {
            if (ModelState.ContainsKey("ProcessType"))
                ModelState["ProcessType"].Errors.Clear();
            if (!ModelState.IsValid)
            {
                ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
                ViewBag.ParentProcess = DMLObj.GetProcessById(ProcessObj.ParentId);
                return View(ProcessObj);
            }
            ProcessObj.ProcessType = DMLObj.GetProcessTypeById(ProcessObj.ProcessTypeId);
            if (DMLObj.ProcessEditById(ProcessObj))
                return RedirectToAction("Index", "SysAdmin", new { Prcs = ProcessObj.Id });
            ViewBag.ProcessTypes = DMLObj.GetAllProcessTypes();
            return View(ProcessObj);
        }

        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (DMLObj.ProcessHasChild(CurrentId))
            {
                ModelState.AddModelError("Name", "این فرآیند دارای زیر مجموعه است و برای حذف ابتدا زیرمجموعه ها را حذف نمایید");
            }
            if (DMLObj.ProcessHasDocument(CurrentId))
            {
                ModelState.AddModelError("Name", "دستکم یک فعالیت برای این فرآیند ثبت شده است و برای حذف ابتدا باید آن فعالیت حذف شود");
            }
            return View(DMLObj.GetProcessById(CurrentId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,ParentId,Name,Description,ProcessTypeId")] AMProcess ProcessObj)
        {
            int CurrentId = 0;
            Int32.TryParse(ProcessObj.Id.ToString(), out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (!DMLObj.ProcessHasChild(CurrentId) && !DMLObj.ProcessHasDocument(CurrentId) && DMLObj.DeleteProcessById(CurrentId))
                return RedirectToAction("Index", "SysAdmin");
            return View("Error");
        }


        public ActionResult GetImage(int id)
        {
            string imagetype = "image/jpg";
            var imageData = DMLObj.GetImageDataById(id, out imagetype);
            return File(imageData, imagetype);
        }
    }
}