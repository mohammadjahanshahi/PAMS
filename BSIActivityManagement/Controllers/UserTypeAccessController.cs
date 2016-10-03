using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.ViewModel;

namespace BSIActivityManagement.Controllers
{
    public class UserTypeAccessController : Controller
    {
        // GET: UserTypeAccess
        Logic.DML DmlObj = new Logic.DML();
        public ActionResult Index(string Id)
        {
            int CurrentId = 0;
            Int32.TryParse(Id, out CurrentId);
            if (CurrentId == 0 || DmlObj.GetUserTypeById(CurrentId) == null) return View("Error"); 
            return View(DmlObj.GetTypesAccessListByTypeId(CurrentId));
        }

        public ActionResult Add(string TypeId, string AccessId)
        {
            int UserTypeId = 0;
            Int32.TryParse(TypeId, out UserTypeId);
            int UserAccessId = 0;
            Int32.TryParse(AccessId, out UserAccessId);
            if(UserAccessId == 0 || DmlObj.GetAccessById(UserAccessId) == null)
            { 
            DAL.AMUserType CurrentType = DmlObj.GetUserTypeById(UserTypeId);
            if (UserTypeId == 0 || CurrentType == null) return View("Error");
            ViewBag.CurrentType = CurrentType;
            return View(DmlObj.GetRemainTypesAccessListByTypeId(UserTypeId));
            }else
            {
                DAL.AMUserTypeAccessList UserTypeAcObj = new DAL.AMUserTypeAccessList
                {
                    AccessId = UserAccessId,
                    UserTypeId = UserTypeId
                };
                if (DmlObj.AddUserTypeAccess(UserTypeAcObj))
                   return RedirectToAction("Index", "SysAdmin");
            }
            return View("Error");
        }
    }
}