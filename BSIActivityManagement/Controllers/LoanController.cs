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
using BSIActivityManagement.Authorization;

namespace BSIActivityManagement.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();

        public ActionResult Index(string UnitId, string ProcessId, string ActivityId, int? CustomerId, int? LoanId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;
            

            IEnumerable<AMCustomer> Custs = new List<AMCustomer>().AsEnumerable();

            if (LoanId == null || DmlObj.GetLoanById(LoanId) == null)
            {
                ViewBag.Customer = Custs;
                return View();
            }

            var loan = DmlObj.GetLoanById(LoanId);

            Custs = DmlObj.GetCustomersByLoanId(loan.Id);

            ViewBag.Customer = Custs;
            return View(loan);
        }

        public ActionResult RedirectToLoan(string UnitId, string ProcessId, string ActivityId, string LoanNumber)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var q = DmlObj.GetLoanByLoanNumber(LoanNumber);
            if (q.Count() == 0)
            {
                ViewBag.Customer = new List<AMCustomer>().AsEnumerable();
                return View("Index", new AMLoan { Id = 0, LoanNumber = LoanNumber });
            }

            IEnumerable<AMCustomer> Custs = DmlObj.GetCustomersByLoanId(q.FirstOrDefault().Id);
            ViewBag.Customer = Custs;

            return View("Index", q.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult Index(string UnitId, string ProcessId, string ActivityId, string LoanNumber )
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var q = DmlObj.GetLoanByLoanNumber(LoanNumber);
            if (q.Count() == 0)
            {
                ViewBag.Customer = new List<AMCustomer>().AsEnumerable();
                return View(new AMLoan { Id = 0, LoanNumber = LoanNumber });
            }
                
            IEnumerable<AMCustomer> Custs = DmlObj.GetCustomersByLoanId(q.FirstOrDefault().Id);
            ViewBag.Customer = Custs;

            return View(q.FirstOrDefault());

        }

        public ActionResult New(string UnitId, string ProcessId, string ActivityId, int CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);

            if (k != null)
            {
                ViewBag.Customer = k;
            }
            else
            {
                ViewBag.Customer = new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" };
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "Id,LoanType,LoanNumber,LoanTotalAmount,LoanDate,TotalInstallments,InstallmentDuration")] LoanRegisterationViewModel AMLoanReg, string UnitId, string ProcessId, string ActivityId, int CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;
            AMCustomer k = new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" };
            k = DmlObj.GetCustomerById(CustomerId);
            if (k.Id == 0)
            {
                ModelState.AddModelError("LoanNumber", "مشتری شناسایی نشد");
            }
            if (AMLoanReg.LoanType == 0)
                ModelState.AddModelError("LoanType", "نوع تسهیلات انتخاب نشده است");

            if (AMLoanReg.InstallmentDuration == 0)
                ModelState.AddModelError("InstallmentDuration", "فاصله زمانی اقساط انتخاب نشده است");

            if (AMLoanReg.LoanNumber == null || AMLoanReg.LoanNumber.Length < 6 || AMLoanReg.LoanNumber.Length > 13)
                ModelState.AddModelError("LoanNumber", "شماره تسهیلات نادرست است");

            AMLoanReg.LoanDate = AMLoanReg.LoanDate.PersianDateToDateTime();
            if(AMLoanReg.LoanDate.Year == 1800)
                ModelState.AddModelError("LoanDate", "تاریخ تسهیلات نادرست است");

            if (ModelState.IsValid)
            {
                AMLoanReg.UnitId = UPA.Unit.Id;
                if (DmlObj.AddNewLoan(AMLoanReg, k))
                {
                    ViewBag.Customer = k;
                    return View("Success");
                }
            }
            ViewBag.Customer = k;
            return View(AMLoanReg);
        }

        public ActionResult Edit(string UnitId, string ProcessId, string ActivityId, int? CustomerId, int? LoanId)
        {
            if (LoanId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);

            if (k != null)
            {
                ViewBag.Customer = k;
            }
            else
            {
                ViewBag.Customer = new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" };
            }

            AMLoan LoanObj = DmlObj.GetLoanById(LoanId);

            if (LoanObj == null)
            {
                return HttpNotFound();
            }
            return View(LoanObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LoanType,LoanNumber")] AMLoan LoanObj, string UnitId, string ProcessId, string ActivityId, int CustomerId)
        {
            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            var k = DmlObj.GetCustomerById(CustomerId);
            if (k == null)
            {
                ModelState.AddModelError("AccountNumber", "مشتری شناسایی نشد");
            }
            if (LoanObj.LoanType == 0)
                ModelState.AddModelError("AccountType", "نوع حساب انتخاب نشده است");

            if (LoanObj.LoanNumber.Length < 6 && LoanObj.LoanNumber.Length > 13)
                ModelState.AddModelError("AccountNumber", "شماره حساب نادرست است");

            if (ModelState.IsValid)
            {
                AMLoan EditObj = DmlObj.GetLoanById(LoanObj.Id);
                EditObj.LoanType = LoanObj.LoanType;
                EditObj.LoanNumber = LoanObj.LoanNumber;
                if (DmlObj.EditLoan(EditObj))
                {
                    ViewBag.Customer = k;
                    return View("Success");
                }
            }
            ViewBag.Customer = k == null ? new AMCustomer { Id = 0, FirstName = "مشتری شناسایی نشد" } : k;
            return View(LoanObj);
        }
        [AMAuthorization(AccessKey = "REMOVE_LOAN")]
        public ActionResult Delete(string UnitId, string ProcessId, string ActivityId, int? id, int? CustomerId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<NavViewModel> Nav = new List<NavViewModel>();
            UnitProcessActObjectViewModel UPA = new UnitProcessActObjectViewModel();

            if (!DmlObj.GetUPAwithNav(UnitId, ProcessId, ActivityId, User.GetAmUser(), out Nav, out UPA))
                return View("Error");

            ViewBag.Nav = Nav;
            ViewBag.UPA = UPA;

            AMLoan LoanObj = DmlObj.GetLoanById(id);
            if (LoanObj == null)
            {
                return HttpNotFound();
            }
            return View(LoanObj);
        }

        // POST: Customer/Delete/5
        [AMAuthorization(AccessKey = "REMOVE_LOAN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UnitId, string ProcessId, string ActivityId, int id)
        {
            AMLoan LoanObj = DmlObj.GetLoanById(id);
            if (DmlObj.DeleteLoan(LoanObj))
                return RedirectToAction("Index", "Customer", new { UnitId = UnitId, ProcessId = ProcessId, ActivityId = ActivityId });
            else return View("Error");
        }


        [HttpPost]
        public JsonResult ConfirmInst(int? InstallmentId)
        {
            if(InstallmentId == null)
                return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);
            
            var k = DmlObj.GetInstallmentById(InstallmentId);
            if(k == null) return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);

            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);

            return Json(DmlObj.SetInstallmentsPaid(k, CurrentUser).Select(m => m.Id), JsonRequestBehavior.AllowGet);
        }

        [AMAuthorization(AccessKey = "REMOVE_LOAN")]
        [HttpPost]
        public JsonResult UnConfirmInst(int? InstallmentId)
        {
            if (InstallmentId == null)
                return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);

            var k = DmlObj.GetInstallmentById(InstallmentId);
            if (k == null) return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);

            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json(new { Id = 0 }, JsonRequestBehavior.AllowGet);

            return Json(DmlObj.SetInstallmentsUnPaid(k, CurrentUser), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegisterInstallmentNotification(int? InstallmentId, int? NotificationDays, string NotificationDescription)
        {
            if (InstallmentId == null)
            return Json("شناسایی قسط برای ثبت یادآوری ناموفق بود", JsonRequestBehavior.AllowGet);
            var k = DmlObj.GetInstallmentById(InstallmentId);

            if (k == null)
            return Json("شناسایی قسط برای ثبت یادآوری ناموفق بود", JsonRequestBehavior.AllowGet);

            if(NotificationDays == null || NotificationDays < 1)
            return Json("تعداد روز برای یادآوری را دوباره بررسی نمایید", JsonRequestBehavior.AllowGet);

            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json("نام کاربری شناسایی نشد. لطفا دوباره به سیستم وارد شوید.", JsonRequestBehavior.AllowGet);


            if (NotificationDescription == null || NotificationDescription.Length < 2)
                NotificationDescription = "بدون توضیحات";

            if (DmlObj.SetInstallmentNotification(new AMInstallmentNotification
            {
                Description = NotificationDescription,
                InstallmentId = k.Id,
                DueDate = DateTime.Now.AddDays((int)NotificationDays),
                Status = Enum.NotificationStatus.Unseen
            }, CurrentUser))
            return Json("یادآوری با موفقیت ثبت شد.", JsonRequestBehavior.AllowGet);
            else return Json("ثبت یادآوری نا موفق بود لطفا دوباره تلاش نمایید.", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult RegisterCallNotification(int? CallId, int? NotificationDays, string NotificationDescription)
        {
            if (CallId == null)
                return Json("شناسایی تماس برای ثبت یادآوری ناموفق بود", JsonRequestBehavior.AllowGet);
            var k = DmlObj.GetCallById(CallId);

            if (k == null)
                return Json("شناسایی تماس برای ثبت یادآوری ناموفق بود", JsonRequestBehavior.AllowGet);

            if (NotificationDays == null || NotificationDays < 1)
                return Json("تعداد روز برای یادآوری را دوباره بررسی نمایید", JsonRequestBehavior.AllowGet);

            if (NotificationDescription == null || NotificationDescription.Length < 2)
                NotificationDescription = "بدون توضیحات";

            if (DmlObj.SetCallNotification(new AMCallNotification
            {
                Description = NotificationDescription,
                CallId = k.Id,
                DueDate = DateTime.Now.AddDays((int)NotificationDays),
                Status = Enum.NotificationStatus.Unseen
            }))
                return Json("یادآوری با موفقیت ثبت شد.", JsonRequestBehavior.AllowGet);
            else return Json("ثبت یادآوری نا موفق بود لطفا دوباره تلاش نمایید.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInfoOfRegisteredCall(int? CallId)
        {
            if(CallId == null || DmlObj.GetCallById(CallId) == null)
            return Json(new { result = 0, message = "اطلاعات تماس شناسایی نشد" }, JsonRequestBehavior.AllowGet);
            AMCall k = DmlObj.GetCallById(CallId);
            return Json(new { result = 1, Name = k.User.FirstName+" "+ k.User.Lastname, CallTime = DisplayExtension.DateToPersian(k.CallTime), Description = k.Description, PhoneNumber = k.Address.PhoneNumber, Message = "عملیات موفقیت آمیز بود", Id = k.Id }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RegisterCall(int? InstallmentId, int? AddressId, string CallDescription)
        {
            if (InstallmentId == null)
                return Json(new { Id = 0, message = "شناسایی قسط برای ثبت یادآوری ناموفق بود" }, JsonRequestBehavior.AllowGet);
            var k = DmlObj.GetInstallmentById(InstallmentId);

            if (k == null)
                return Json(new { Id = 0, message = "شناسایی قسط برای ثبت یادآوری ناموفق بود" }, JsonRequestBehavior.AllowGet);

            if (AddressId == null || AddressId < 1 || DmlObj.GetAddressById(AddressId) == null)
                return Json(new { Id = 0, message = "شماره تلفن یا آدرس انتخاب نشده است" }, JsonRequestBehavior.AllowGet);

            if (CallDescription == null || CallDescription.Length < 2)
                CallDescription = "بدون توضیحات";

            AMUser CurrentUser = DmlObj.GetAmUserById(Int32.Parse(User.GetAmUser()));
            if(CurrentUser == null)
                return Json(new { Id = 0, message = "کاربر مجاز به انجام این عملیات نیست" }, JsonRequestBehavior.AllowGet);

            AMCall CallObj = new AMCall
            {
                Description = CallDescription,
                InstallmentId = k.Id,
                CallTime = DateTime.Now,
                AddressId = DmlObj.GetAddressById(AddressId).Id,
                UserId = CurrentUser.Id
            };
            bool res = false;
            CallObj = DmlObj.SetCallInformation(CallObj, out res);
            if (res)
                return Json(new { Id = CallObj.Id, message = " با موفقیت ثبت شد." }, JsonRequestBehavior.AllowGet);
            else return Json(new { Id = 0, message = "ثبت یادآوری نا موفق بود لطفا دوباره تلاش نمایید." }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SetNotificationSeen(int? NotificationId)
        {
            if(NotificationId == null || DmlObj.GetInstallmentNotificationById(NotificationId) == null)
            {
                return Json("یادآوری شناسایی نشد", JsonRequestBehavior.AllowGet);
            }


            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json("نام کاربری شناسایی نشد. لطفا دوباره به سیستم وارد شوید.", JsonRequestBehavior.AllowGet);


            if (DmlObj.SetInstallmentNotificationSeen(DmlObj.GetInstallmentNotificationById(NotificationId), CurrentUser))
            {
                return Json("با موفقیت ثبت شد", JsonRequestBehavior.AllowGet);
            }

            return Json("عملیات ناموفق بود٬ لطفا دوباره تلاش نمایید", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegisterReferee(int? LoanId, string CustomerNumber, string Description)
        {
            if (LoanId == null || DmlObj.GetLoanById(LoanId) == null)
                return Json(new { Id = 0, Message = "عملیات به دلیل خطا در دریافت اطلاعات تسهیلات انجام نشد" }, JsonRequestBehavior.AllowGet);

            var Loan = DmlObj.GetLoanById(LoanId);


            var Customer = DmlObj.GetCustomerByCustomerNumber(CustomerNumber);
            if(Customer.Count() == 0)
                return Json(new { Id = 0, Message = "شماره مشتری در این سیستم ثبت نشده است. لطفا به عنوان مشتری جدید ثبت نمایید" }, JsonRequestBehavior.AllowGet);

            bool op = false;
            AMReferee NewReferee = DmlObj.AddNewReferee(new AMReferee { CustomerId = Customer.FirstOrDefault().Id, LoanId = Loan.Id, Description = Description }, out op);
            if (op)
            return Json(new {Id = NewReferee.Id, CustomerId = Customer.FirstOrDefault().Id, CustomerNumber = Customer.FirstOrDefault().CustomerNumber, FirstName = Customer.FirstOrDefault().FirstName, LastName = Customer.FirstOrDefault().Lastname, Description = Description, Message = "ضامن با موفقیت ثبت شد" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Id = 0, Message = "خطا در ذخیره سازی در پایگاه داده ها اتفاق افتاده است٬ لطفا مجددا تلاش نمایید." }, JsonRequestBehavior.AllowGet);
        }


        
        [HttpPost]
        public JsonResult UpdateLoan(int? LoanId)
        {
            if (LoanId == null || DmlObj.GetLoanById(LoanId) == null)
                return Json(new { Id = 0, Message = "عملیات به دلیل خطا در دریافت اطلاعات تسهیلات انجام نشد" }, JsonRequestBehavior.AllowGet);

            var Loan = DmlObj.GetLoanById(LoanId);

            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json(new { Id = 0, Message = "اطلاعات کاربری شما یافت نشد. لطفا دوباره به سیستم وارد شوید." }, JsonRequestBehavior.AllowGet);


            
            bool op = false;
            AMUpdateLoanLog UpdateLoanLogObj = DmlObj.AddUpdateLoanLog(new AMUpdateLoanLog {  LoanId = Loan.Id, UpdateTime = DateTime.Now, UserId = CurrentUser.Id, Status = Enum.UpdateLoanStatus.ProcessCompleted }, out op);
            if (op)
                return Json(new { Id = UpdateLoanLogObj.Id, FirstName = UpdateLoanLogObj.User.FirstName, LastName = UpdateLoanLogObj.User.Lastname, ElapsedTime = DisplayExtension.ElapsedTime(UpdateLoanLogObj.UpdateTime) , Message = "موفقیت ثبت شد" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Id = 0, Message = "خطا در ذخیره سازی در پایگاه داده ها اتفاق افتاده است٬ لطفا مجددا تلاش نمایید." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StartFollowUp(int? LoanId)
        {
            if (LoanId == null || DmlObj.GetLoanById(LoanId) == null)
                return Json(new { Id = 0, Message = "عملیات به دلیل خطا در دریافت اطلاعات تسهیلات انجام نشد" }, JsonRequestBehavior.AllowGet);

            var Loan = DmlObj.GetLoanById(LoanId);

            int UserId = 0;
            Int32.TryParse(User.GetAmUser(), out UserId);
            AMUser CurrentUser = DmlObj.GetAmUserById(UserId);
            if (CurrentUser == null) return Json(new { Id = 0, Message = "اطلاعات کاربری شما یافت نشد. لطفا دوباره به سیستم وارد شوید." }, JsonRequestBehavior.AllowGet);



            bool op = false;
            AMUpdateLoanLog UpdateLoanLogObj = DmlObj.AddStartFollowUpLoanLog(new AMUpdateLoanLog { LoanId = Loan.Id, UpdateTime = DateTime.Now, UserId = CurrentUser.Id, Status = Enum.UpdateLoanStatus.UnderProcess }, out op);
            if (op)
                return Json(new { Id = UpdateLoanLogObj.Id, FirstName = UpdateLoanLogObj.User.FirstName, LastName = UpdateLoanLogObj.User.Lastname, ElapsedTime = DisplayExtension.ElapsedTime(UpdateLoanLogObj.UpdateTime), Message = "در حال بررسی" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Id = 0, Message = "خطا در ذخیره سازی در پایگاه داده ها اتفاق افتاده است٬ لطفا مجددا تلاش نمایید." }, JsonRequestBehavior.AllowGet);
        }


        [AMAuthorization(AccessKey = "REMOVE_LOAN")]
        [HttpPost]
        public JsonResult RemoveReferee(int? RefereeId)
        {
            if(RefereeId == null)
                return Json(new { Id = 0, Message = "شماره اختصاصی ضامن شناسایی نشد٬ لطفا دوباره تلاش نمایید." }, JsonRequestBehavior.AllowGet);

            AMReferee Referee = DmlObj.GetRefereeById(RefereeId);

            if(Referee == null)
                return Json(new { Id = 0, Message = "شماره اختصاصی ضامن شناسایی نشد٬ لطفا دوباره تلاش نمایید." }, JsonRequestBehavior.AllowGet);

            if(DmlObj.RemoveReferee(Referee))
                return Json(new { Id = 1, Message = "ضامن با موفقیت حذف شد." }, JsonRequestBehavior.AllowGet);

            return Json(new { Id = 0, Message = "عملیات در زمان حذف از پایگاه داده ناموفق بود!" }, JsonRequestBehavior.AllowGet);
        }
    }
}