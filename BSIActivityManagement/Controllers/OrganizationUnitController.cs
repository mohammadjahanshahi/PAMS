using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSIActivityManagement.DAL;

namespace BSIActivityManagement.Controllers
{
    public class OrganizationUnitController : Controller
    {
        private Logic.DML DmlObj = new Logic.DML();
        // GET: OrganizationUnit
        public ActionResult GetImage(int id)
        {
            string imagetype = "image/jpeg";
            var imageData = DmlObj.GetImageDataById(id, out imagetype);
            return File(imageData, imagetype);
        }

        public ActionResult Index(string OrganizationId)
        {
            int CurrentOrgId = 0;
            Int32.TryParse(OrganizationId, out CurrentOrgId);
            if (CurrentOrgId == 0) return View("Error");
            var Org = DmlObj.GetOrgById(CurrentOrgId);
            if (Org == null) return View("Error");

            return View(DmlObj.GetUnitOfOrgList(CurrentOrgId));
        }
        public ActionResult Add(string OrganizationId)
        {
            int CurrentOrgId = 0;
            Int32.TryParse(OrganizationId, out CurrentOrgId);
            if (CurrentOrgId == 0) return View("Error");
            var Org = DmlObj.GetOrgById(CurrentOrgId);
            if(Org == null) return View("Error");
            ViewBag.Organization =  Org;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "IdentityCode,Name,Description,OrganizationId")] AMUnitOfOrg OrgUnit)
        {
            int CurrentOrgId = 0;
            Int32.TryParse(OrgUnit.OrganizationId.ToString(), out CurrentOrgId);
            if (CurrentOrgId == 0) return View("Error");
            var Org = DmlObj.GetOrgById(CurrentOrgId);
            if (Org == null) return View("Error");
            ViewBag.Organization = Org;

            if (!ModelState.IsValid)
                return View(OrgUnit);

            bool resultAdd = false;
            var addedOrg = DmlObj.AddNewOrganizationUnit(OrgUnit,out resultAdd);
            if (resultAdd) return RedirectToAction("Index", "SysAdmin", new { Org = addedOrg.Id });

            return View(OrgUnit);
        }

    }
}