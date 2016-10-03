using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Logic;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Authorization;
using System.IO;
using System.Drawing;
using BSIActivityManagement.Models;

namespace BSIActivityManagement.Controllers
{
    //[AMAuthorization(AccessKey = "SYSTEM_CHANGE")]
    public class SysAdminController : Controller
    {
        DML DMLObj = new DML();
        // GET: SysAdmin
        public ActionResult Index(int? Org, int? Prcs, int? Act)
        {
            
            SysAdminViewModel SysadminObj = new SysAdminViewModel();

            SysadminObj.OrgArray = DMLObj.GetOrgList(Org);
            SysadminObj.ProcessArray = DMLObj.GetProcessList(Prcs);
            SysadminObj.ProcessTypeArray = DMLObj.GetAllProcessTypes();
            SysadminObj.ActivityTypeArray = DMLObj.GetAllActivityTypes();
            SysadminObj.ActivityItemTypeArray = DMLObj.GetAllActivityItemTypes();
            SysadminObj.UserTypeArray = DMLObj.GetUserTypeList();
            SysadminObj.AccessArray = DMLObj.GetAccessList();

            SysadminObj.CurrentOrgId = Org;
            SysadminObj.CurrentOrgParentId = DMLObj.GetParentOrgId(Org);
            SysadminObj.CurrentProcessId = Prcs;
            SysadminObj.CurrentActId = Act;
            return View(SysadminObj);
        }
        public ActionResult ProcessActivities(string ProcessId)
        {
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);

            if (AmProcessId == 0)
                return View("Error");

            SysAdminViewModelProcessActivities model = new SysAdminViewModelProcessActivities();

            model.Navigation = null;
            model.ActivityGropus = DMLObj.GetActivityByProcess( AmProcessId);
            model.UnitId = 0;
            model.ProcessId = AmProcessId;
            return View(model);
        }
        public ActionResult ShowActivity(string ActivityId, string ProcessId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            if (AmActivityId == 0)
                return View("Error");

            SysAdminModelShowActivity model = new SysAdminModelShowActivity();
            List<NavViewModel> Nav = null;

            model.ActivityItemGropus = DMLObj.GetActivityAllItemsByActivityId(AmActivityId);
            model.ActivityId = AmActivityId;
            model.UnitId = 0;
            model.Navigation = Nav;
            model.ProcessId = AmProcessId;
            return View(model);
        }

        public ActionResult AddItem(string ActivityId, string ItemTypeId, string ProcessId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            int AmItemTypeId = 0;
            Int32.TryParse(ItemTypeId, out AmItemTypeId);
            AMActivity ActObj = DMLObj.GetActivityById(AmActivityId);
            AMActivityItemType ItemTypeObj = DMLObj.GetActivityItemTypeById(AmItemTypeId);
            if (AmActivityId == 0 || AmItemTypeId == 0 || ActObj == null || ItemTypeObj == null)
                return View("Error");

            AMActivityItem model = new AMActivityItem
            {
                ActivityId = AmActivityId,
                Activity = ActObj,
                ItemTypeId = AmItemTypeId,
                ItemType = ItemTypeObj
            };
            ViewBag.ProcessId = AmProcessId.ToString();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "TextTitle,TextBody,ActivityId,ItemTypeId,DocumentId")] AMActivityItem model, string ProcessId)
        {
            model.Status = 1;
            model.OrderNumber = 1;
            model.CreationTime = DateTime.Now;
            bool resAdd = false;
            DMLObj.AddActivityItem(model,out resAdd);
            if (resAdd) return RedirectToAction("ShowActivity", new { ActivityId = model.ActivityId, ProcessId = ProcessId });
            ViewBag.ProcessId = ProcessId;
            return View(model);
        }

        public ActionResult EditItem(string ItemId,  string ProcessId)
        {
            int AmItemId = 0;
            Int32.TryParse(ItemId, out AmItemId);

            AMActivityItem model = DMLObj.GetActivityItemById(AmItemId);
            
            if (model == null)
                return View("Error");

            ViewBag.ProcessId = ProcessId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItem([Bind(Include = "Id,TextTitle,TextBody,ActivityId,ItemTypeId,DocumentId,Status,OrderNumber")] AMActivityItem model, string ProcessId)
        {
            bool resEdit = false;
            AMActivityItem Obj = DMLObj.GetActivityItemById(model.Id);
            Obj.TextTitle = model.TextTitle;
            Obj.TextBody = model.TextBody;
            Obj.DocumentId = model.DocumentId;
            Obj.Document = DMLObj.GetDocumentById(model.DocumentId);
            DMLObj.EditActivityItem(Obj,out resEdit);
            if (resEdit) return RedirectToAction("ShowActivity", new { ActivityId = model.ActivityId, ProcessId = ProcessId });
            ViewBag.ProcessId = ProcessId;
            return View(model);
        }

        public ActionResult RemoveItem(string ItemId, string ProcessId)
        {
            int AmItemId = 0;
            Int32.TryParse(ItemId, out AmItemId);

            AMActivityItem model = DMLObj.GetActivityItemById(AmItemId);

            if (model == null)
                return View("Error");
            ViewBag.ProcessId = ProcessId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem([Bind(Include = "Id,TextTitle,TextBody")] AMActivityItem model, string ProcessId)
        {
            AMActivityItem Obj = DMLObj.GetActivityItemById(model.Id);
            if (Obj == null) return View("Error");
            int act = Obj.ActivityId;
            if(DMLObj.RemoveActivityItem(Obj)) 
                return RedirectToAction("ShowActivity", new { ActivityId = act , ProcessId = ProcessId });
            ViewBag.ProcessId = ProcessId;
            return View(model);
        }


        [HttpPost]
        public ActionResult UploadFile()
        {
            var httpPostedFile = HttpContext.Request.Files["UploadedDocument"];
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
                if (!httpPostedFile.IsImage() && !httpPostedFile.IsDoc())
                {
                    savedObj.errormessage = "فایل انتخابی معتبر نیست. فایلهای قابل پشتیبانی تمامی تصاویر و فایلهای PDF می باشند.";
                    return Json(savedObj, JsonRequestBehavior.AllowGet);
                }
                if (httpPostedFile.ContentLength > 50000000)
                {
                    savedObj.errormessage = "اندازه فایل بیش از حد زیاد است حداکثر سایز مجاز برابر 50 مگابایت است";
                    return Json(savedObj, JsonRequestBehavior.AllowGet);
                }
                Byte[] fileData;
                if (httpPostedFile.IsImage())
                {
                    
                }

                Stream fs = httpPostedFile.InputStream;
                fs.Position = 0;
                BinaryReader br = new BinaryReader(fs);
                fileData = br.ReadBytes((Int32)fs.Length);

                savedObj.mediaid = DMLObj.AddDocGetId(fileData,"", httpPostedFile.FileName,httpPostedFile.ContentLength, httpPostedFile.ContentType);
            }
            return Json(savedObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentThumbnail(int id)
        {
            string doctype = "image/jpg";
            var imageData = DMLObj.GetDocDataById(id, out doctype);
            if(doctype.ToLower() == "application/pdf")
            {
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                document.LoadFromStream(new MemoryStream(imageData));
                Image ImgDoc = document.SaveAsImage(0,Spire.Pdf.Graphics.PdfImageType.Bitmap,120,120);
                return File(imageToByteArray(ImgDoc.ScaleImage(150, 150)), "image/jpeg");
            }

            //if (doctype.ToLower() == "application/msword")
            //{
            //    Spire.Doc.Document document = new Spire.Doc.Document();
            //    document.LoadFromStream(new MemoryStream(imageData),Spire.Doc.FileFormat.Doc);
            //    Image ImgDoc = document.SaveToImages(0, Spire.Doc.Documents.ImageType.Metafile);
            //    return File(imageToByteArray(ImgDoc.ScaleImage(150, 150)), "image/jpeg");
            //}

            //if (doctype.ToLower() == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            //{
            //    Spire.Doc.Document document = new Spire.Doc.Document();
            //    document.LoadFromStream(new MemoryStream(imageData), Spire.Doc.FileFormat.Docx2010);
            //    Image ImgDoc = document.SaveToImages(0, Spire.Doc.Documents.ImageType.Metafile);
            //    return File(imageToByteArray(ImgDoc.ScaleImage(150, 150)), "image/jpeg");
            //}
            return File(imageData, doctype);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public ActionResult ShowItem(string ItemId, string ProcessId)
        {
            int AmItemId = 0;
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            if (!Int32.TryParse(ItemId, out AmItemId))
                return View("Error");
            SysAdminViewModelShowItem model = new SysAdminViewModelShowItem();
            DAL.AMActivityItem item = DMLObj.GetActivityItemById(Int32.Parse(ItemId));
            model.DocumentPages = DMLObj.GetDocumentPageCount(item.DocumentId.ToString());
            model.Item = item;
            model.ProcessId = AmProcessId;
            model.ActivityId = item.ActivityId;
            return View(model);
        }

        public ActionResult GetDocument(string DocId, string PageNumber)
        {
            int AmDocId = 0;
            int AmPn = 0;
            Int32.TryParse(DocId, out AmDocId);
            Int32.TryParse(PageNumber, out AmPn);
            string doctype = "image/jpg";
            var imageData = DMLObj.GetDocDataById(AmDocId, out doctype);
            if (imageData == null) return null;
            if (doctype.ToLower() == "application/pdf")
            {
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                document.LoadFromStream(new MemoryStream(imageData));
                Image ImgDoc = document.SaveAsImage(AmPn, Spire.Pdf.Graphics.PdfImageType.Bitmap, 400, 400);
                ImgDoc = ImgDoc.cropImage(new Rectangle(0, 57, ImgDoc.Width, ImgDoc.Height - 57));
                return File(imageToByteArray(ImgDoc), "image/jpeg");
            }
            return File(imageData, doctype);
        }



    }
}