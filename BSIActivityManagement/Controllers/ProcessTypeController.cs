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
    public class ProcessTypeController : Controller
    {
        // GET: ProcessType
        DML DMLObj = new DML();
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Name,Description,ImageId")] AMProcessType ProcessT)
        {
            if (!ModelState.IsValid)
                return View(ProcessT);
            bool resAdd = false;
            var addedProcessT = DMLObj.AddNewProcessType(ProcessT,out resAdd);
            if (resAdd)
                return RedirectToAction("Index", "SysAdmin");
            return View(ProcessT);
        }

        public ActionResult Edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            var ProcessTypeObj = DMLObj.GetProcessTypeById(CurrentId);
            if (ProcessTypeObj == null) return View("Error");
            return View(ProcessTypeObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ImageId")] AMProcessType ProcessT)
        {
            if (!ModelState.IsValid)
                return View(ProcessT);
            if (DMLObj.ProcessTypeEditById(ProcessT))
                return RedirectToAction("Index", "SysAdmin");
            return View(ProcessT);
        }

        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (DMLObj.ProcessTypeHasProcess(CurrentId))
            {
                ModelState.AddModelError("Name", "دستکم یک فرآیند از این نوع ثبت شده است و برای حذف ابتدا باید آن فرآیند حذف شود");
            }
            return View(DMLObj.GetProcessTypeById(CurrentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,Name,Description,ImageId")] AMProcessType ProcessTObj)
        {
            int CurrentId = 0;
            Int32.TryParse(ProcessTObj.Id.ToString(), out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (!DMLObj.ProcessTypeHasProcess(CurrentId) && DMLObj.DeleteProcessTypeById(CurrentId))
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