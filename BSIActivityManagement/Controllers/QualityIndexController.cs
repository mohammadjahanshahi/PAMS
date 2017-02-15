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

namespace BSIActivityManagement.Controllers
{
    public class QualityIndexController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        // GET: QualityIndex
        //public ActionResult Index()
        //{
        //    return View(DmlObj.GetQualityIndexList());
        //}

        // GET: QualityIndex/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AMQualityIndex aMQualityIndex = db.AMQualityIndexEnt.Find(id);
        //    if (aMQualityIndex == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aMQualityIndex);
        //}

        // GET: QualityIndex/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QualityIndex/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,EnumType")] AMQualityIndex aMQualityIndex)
        {
            if (ModelState.IsValid && DmlObj.AddQualityIndex(aMQualityIndex))
                return View("SuccessfulCreate");
            return View(aMQualityIndex);
        }

        // GET: QualityIndex/Edit/5
        public ActionResult edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AMQualityIndex model = DmlObj.GetQualityIndexById(CurrentId);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: QualityIndex/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,EnumType")] AMQualityIndex aMQualityIndex)
        {
            if (ModelState.IsValid && DmlObj.EditQualityIndexByObject(aMQualityIndex))
                return View("SuccessfulCreate");

                return View(aMQualityIndex);
        }

        //// GET: QualityIndex/Delete/5
        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AMQualityIndex aMQualityIndex = DmlObj.GetQualityIndexById(CurrentId);
            if (aMQualityIndex == null)
            {
                return HttpNotFound();
            }
            return View(aMQualityIndex);
        }

        //// POST: QualityIndex/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            AMQualityIndex aMQualityIndex = DmlObj.GetQualityIndexById(CurrentId);
            if(DmlObj.DeleteQualityIndexByObject(aMQualityIndex))
            return View("SuccessfulCreate");
            return View("Error");
        }

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
