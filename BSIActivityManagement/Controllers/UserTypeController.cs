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
    public class UserTypeController : Controller
    {
        // GET: ProcessType
        DML DMLObj = new DML();
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Name,Description,ImageId")] AMUserType UserT)
        {
            if (!ModelState.IsValid)
                return View(UserT);
            bool resAdd = false;
            var addedUserT = DMLObj.AddNewUserType(UserT, out resAdd);
            if (resAdd)
                return RedirectToAction("Index", "SysAdmin");
            return View(UserT);
        }

        public ActionResult Edit(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            var UserTypeObj = DMLObj.GetUserTypeById(CurrentId);
            if (UserTypeObj == null) return View("Error");
            return View(UserTypeObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ImageId")] AMUserType UserT)
        {
            if (!ModelState.IsValid)
                return View(UserT);
            if (DMLObj.UserTypeEditById(UserT))
                return RedirectToAction("Index", "SysAdmin");
            return View(UserT);
        }

        public ActionResult Delete(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (DMLObj.UserTypeHasUser(CurrentId))
            {
                ModelState.AddModelError("Name", "دستکم یک کاربر از این نوع ثبت شده است و برای حذف ابتدا باید آن کاربری تغییر داده شود");
            }
            return View(DMLObj.GetUserTypeById(CurrentId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,Name,Description,ImageId")] AMUserType UserTObj)
        {
            int CurrentId = 0;
            Int32.TryParse(UserTObj.Id.ToString(), out CurrentId);
            if (CurrentId == 0) return View("Error");
            if (!DMLObj.UserTypeHasUser(CurrentId) && DMLObj.DeleteUserTypeById(CurrentId))
                return RedirectToAction("Index", "SysAdmin");
            return View("Error");
        }


        public ActionResult GetImage(int id)
        {
            string imagetype = "image/jpeg";
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
                savedObj.mediaid = DMLObj.AddImageGetId(fileData, "تصویر نوع کاربری", 0, 0, httpPostedFile.ContentType);
            }
            return Json(savedObj, JsonRequestBehavior.AllowGet);
        }
    }
}