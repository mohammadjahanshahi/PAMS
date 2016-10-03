using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.Extensions;

namespace BSIActivityManagement.Controllers
{
    public class HomeController : Controller
    {
        Logic.DML DmlObj = new Logic.DML();
        public ActionResult GetImage(int id)
        {
            string imagetype = "image/jpeg";
            var imageData = DmlObj.GetImageDataById(id, out imagetype);
            return File(imageData, imagetype);
        }
        [Authorize]
        public ActionResult Index()
        {
            int AmUserId = 0;
            if (User.GetAmUser() != null && Int32.TryParse(User.GetAmUser(), out AmUserId) && AmUserId != 0 && DmlObj.GetAmUserById(AmUserId) != null)
            {

                if (DmlObj.CheckUserAccessKeyByUserId("SYSTEM_CHANGE", AmUserId.ToString()))
                    return RedirectToAction("Index", "SysAdmin");
                return RedirectToAction("Unit", "Main");
            }
            return View();
        }

        public ActionResult Intro()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }
    }
}