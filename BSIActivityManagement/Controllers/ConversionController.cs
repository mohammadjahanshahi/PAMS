using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Logic;
using BSIActivityManagement.ViewModel;
using BSIActivityManagement.Authorization;

namespace BSIActivityManagement.Controllers
{
    [AMAuthorization(AccessKey = "SYSTEM_CHANGE")]
    public class ConversionController : Controller
    {
        // GET: Conversion
        DML DmlObj = new DML();
        public ActionResult Index(int? CFPage, int? LFPage, int? CWPage, int? LWPage)
        {
            const int rows = 20;
            int CustomerFailedPageNumber = CFPage == null ? 1 : (int)CFPage;
            int LoanFailedPageNumber = LFPage == null ? 1 : (int)LFPage;

            int CustomerWarningPageNumber = CWPage == null ? 1 : (int)CWPage;
            int LoanWarningPageNumber = LWPage == null ? 1 : (int)LWPage;

            ConversionIndexViewModel model = new ConversionIndexViewModel();
            model.CustomerConversionFailed = DmlObj.GetCustomerConversionFailed(CustomerFailedPageNumber, rows);
            model.CustomerConvertedWithWarning = DmlObj.GetCustomerConversionWarning(CustomerWarningPageNumber, rows);
            model.LoanConversionFailed = DmlObj.GetLoanConversionFailed(LoanFailedPageNumber, rows);
            model.LoanConvertedWithWarning = DmlObj.GetLoanConversionWarning(LoanWarningPageNumber, rows);

            model.MizanCustomerRecordsConversionFailedCount = DmlObj.GetMizanLoanFailedConvertCount();
            model.MizanCustomerRecordsConvertedSuccessfullyCount = DmlObj.GetMizanCustomerSuccessConvertCount();
            model.MizanCustomerRecordsConvertedWithWarningCount = DmlObj.GetMizanCustomerWarningConvertCount();
            model.MizanCustomerRecordsCount = DmlObj.GetMizanCustomerTotalCount();
            model.MizanCustomerRecordsUnConvertedCount = DmlObj.GetMizanCustomerUnConvertedCount();
            model.MizanLoanRecordsConversionFailedCount = DmlObj.GetMizanLoanFailedConvertCount();
            model.MizanLoanRecordsConvertedSuccessfullyCount = DmlObj.GetMizanLaonSuccessConvertCount();
            model.MizanLoanRecordsConvertedWithWarningCount = DmlObj.GetMizanLoanWarningConvertCount();
            model.MizanLoanRecordsCount = DmlObj.GetLoanConvertTotalCount();
            model.MizanLoanRecordsUnConvertedCount = DmlObj.GetMizanLoanUnConvertedCount();
            model.Statistics = DmlObj.GetConversionStatistics();
            return View(model);
        }

        public ActionResult Convert()
        {
            string ConversionResult = "عملیات آغاز نشده است";
            DmlObj.DoMizanConvert(out ConversionResult);
            return RedirectToAction("Index");
        }

        //public ActionResult ModifyLoanType()
        //{
        //    if(DmlObj.UpdateMizanLoanType())
        //        return RedirectToAction("Index");
        //    return View("Error");
        //}
    }
}