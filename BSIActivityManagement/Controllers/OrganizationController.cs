using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Logic;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.Models;
using System.IO;
using System.Drawing;

namespace BSIActivityManagement.Controllers
{
    public class OrganizationController : Controller
    {
        DML DMLObj = new DML();
        // GET: Organization
        public ActionResult Index()
        {
            return View(DMLObj.GetOrganizationList());
        }

        public ActionResult Add(int? OrgParentId)
        {
            ViewBag.ParentOrg = DMLObj.GetOrgById(OrgParentId);
            ViewBag.ParentId = OrgParentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ParentId,Name,Description,ImageId")] OrganizationViewModel Org)
        {
            if (!ModelState.IsValid)
                return View(Org);
            var addedOrg = DMLObj.AddNewOrganization(Org);
            if (addedOrg.OperationResult)
                return RedirectToAction("Index", "SysAdmin", new { Org = addedOrg.Id });
            return View(Org);
        }
        public ActionResult Edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            var OrgObj = DMLObj.GetOrgById(CurrentId);
            if (OrgObj == null) return View("Error");
            return View(OrgObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentId,Name,Description,ImageId")] BSIActivityManagement.DAL.AMOrganization Org)
        {
            if (!ModelState.IsValid)
                return View(Org);
            if (DMLObj.OrganizationEditById(Org))
                return RedirectToAction("Index", "SysAdmin", new { Org = Org.Id });
            return View(Org);
        }
        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (DMLObj.OrgHasChild(CurrentId))
            {
                ModelState.AddModelError("Name", "این سازمان دارای زیر مجموعه است و برای حذف ابتدا زیرمجموعه ها را حذف نمایید");
            }
            if(DMLObj.OrgHasDocument(CurrentId))
            {
                ModelState.AddModelError("Name", "دستکم یک فعالیت برای این سازمان ثبت شده است و برای حذف ابتدا باید آن فعالیت حذف شود");
            }
            return View(DMLObj.GetOrgById(CurrentId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,ParentId,Name,Description,ImageId")] BSIActivityManagement.DAL.AMOrganization Org)
        {
            int CurrentId = 0;
            Int32.TryParse(Org.Id.ToString(), out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (!DMLObj.OrgHasChild(CurrentId) && !DMLObj.OrgHasDocument(CurrentId) && DMLObj.DeleteOrgById(CurrentId))
                return RedirectToAction("Index", "SysAdmin");
            return View("Error");
        }
        public ActionResult GetImage(int id)
        {
            string imagetype = "image/jpg";
            var imageData = DMLObj.GetImageDataById(id, out imagetype);

        return File(imageData, imagetype);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            var httpPostedFile = HttpContext.Request.Files["UploadedImage"];
            savedImageinf savedObj = new savedImageinf();
            savedObj.fileaddress = "No File Address";
            savedObj.filesize = 0;
            savedObj.filetype = "Type not detected";
            savedObj.mediaid = 0;
            savedObj.errormessage = "0";
            if (httpPostedFile == null)
            {
                savedObj.errormessage = "هیچ فایلی انتخاب نشده است";
                return Json(savedObj, JsonRequestBehavior.AllowGet);
            }
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                if (!httpPostedFile.IsImage())
                {
                    savedObj.errormessage = "فایل انتخابی از نوع تصویر نیست، لطفا فایل تصویری انتخاب نمایید.";
                    return Json(savedObj, JsonRequestBehavior.AllowGet);
                }
                if (httpPostedFile.ContentLength > 10000000)
                {
                    savedObj.errormessage = "اندازه فایل تصویر بیش از حد زیاد است حداکثر سایز مجاز برابر 10 مگابایت است";
                    return Json(savedObj, JsonRequestBehavior.AllowGet);
                }
                


                byte[] fileData = imageToByteArray(Image.FromStream(httpPostedFile.InputStream, true, true));
                savedObj.mediaid = DMLObj.AddImageGetId(fileData, "تصویر نوع سازمان", 0, 0, httpPostedFile.ContentType);
            }
            return Json(savedObj, JsonRequestBehavior.AllowGet);
        }
    }
}