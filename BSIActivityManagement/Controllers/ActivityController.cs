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
    public class ActivityController : Controller
    {
        // GET: Activity
        DML DMLObj = new DML();

        public ActionResult Add()
        {
            var OrgsObj = DMLObj.GetOrganizationList();
            ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
            ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();

            var ProcessObj = DMLObj.GetProcessList();
            ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Id,Name,Description,TypeId,SelectedOrganizationStr,SelectedProcessesStr")] AddActivityViewModel AddObj)
        {
            if (!ModelState.IsValid)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);
                return View(AddObj);
            }
            var OrgsList = DMLObj.FindOrgsForAddActivityByStringIds(AddObj.SelectedOrganizationStr);
            if(OrgsList.Count() == 0)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("SelectedOrganizationStr", "هیچ سازمان معتبری انتخاب نشده است");
                return View(AddObj);
            }
            var ProcessesList = DMLObj.FindProcessesForAddActivityByStringIds(AddObj.SelectedProcessesStr);
            if(ProcessesList.Count() == 0)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("SelectedProcessesStr", "هیچ فرآیند معتبری انتخاب نشده است");
                return View(AddObj);
            }
            var selectedActType = DMLObj.GetActivityTypeById(AddObj.TypeId);
            if(selectedActType == null)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("TypeId", "هیچ نوع فعالیت معتبری انتخاب نشده است");
                return View(AddObj);
            } 
            bool resAdd = false;
            var addedActivity = DMLObj.AddNewActivity(new AMActivity {Name = AddObj.Name, Description = AddObj.Description, ActivityItems=null, TypeId = AddObj.TypeId, Type = selectedActType },out resAdd);
            if (resAdd && DMLObj.AddNewActivityRelation(OrgsList, ProcessesList, addedActivity) > 0)
                    return View("Success");

            var OrgsObjp = DMLObj.GetOrganizationList();
            ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObjp);
            ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
            var ProcessObjp = DMLObj.GetProcessList();
            ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObjp);

            return View("Error");
        }

        public ActionResult Delete(string ActivityId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            if (AmActivityId == 0) return View("Error");
            if (DMLObj.ActivityHasItem(AmActivityId))
            {
                ModelState.AddModelError("Name", "این فعالیت دارای چندین جزء در درون خودش است، برای حذف ابتدا آنها را حذف نمایید.");
            }
            return View(DMLObj.GetActivityById(AmActivityId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Id,Name,Description")] AMActivity ActivityObj)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityObj.Id.ToString(), out AmActivityId);
            if (AmActivityId == 0) return View("Error");
            if (!DMLObj.ActivityHasItem(AmActivityId)  && DMLObj.DeleteActivityById(AmActivityId))
                return RedirectToAction("Index", "SysAdmin");
            return View("Error");
        }

        public ActionResult Edit(string ActivityId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            if (AmActivityId == 0 || DMLObj.GetActivityById(AmActivityId) == null)
                return View("Error");
            AMActivity EditAct = DMLObj.GetActivityById(AmActivityId);

            var OrgsObj = DMLObj.GetOrganizationList();
            ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
            ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
            var ProcessObj = DMLObj.GetProcessList();
            ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);
            AddActivityViewModel EditObj = new AddActivityViewModel();
            EditObj.Name = EditAct.Name;
            EditObj.Description = EditAct.Description;
            EditObj.Id = EditAct.Id;
            EditObj.SelectedOrganizationStr = DMLObj.GetActivityOrgsByActIdAsString(AmActivityId);
            EditObj.SelectedProcessesStr = DMLObj.GetActivityProcessesByActIdAsString(AmActivityId);
            EditObj.TypeId = EditAct.TypeId;
            return View(EditObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,TypeId,SelectedOrganizationStr,SelectedProcessesStr")] AddActivityViewModel AddObj)
        {
            if (!ModelState.IsValid)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);
                return View(AddObj);
            }
            var OrgsList = DMLObj.FindOrgsForAddActivityByStringIds(AddObj.SelectedOrganizationStr);
            if (OrgsList.Count() == 0)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("SelectedOrganizationStr", "هیچ سازمان معتبری انتخاب نشده است");
                return View(AddObj);
            }
            var ProcessesList = DMLObj.FindProcessesForAddActivityByStringIds(AddObj.SelectedProcessesStr);
            if (ProcessesList.Count() == 0)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("SelectedProcessesStr", "هیچ فرآیند معتبری انتخاب نشده است");
                return View(AddObj);
            }
            var selectedActType = DMLObj.GetActivityTypeById(AddObj.TypeId);
            if (selectedActType == null)
            {
                var OrgsObj = DMLObj.GetOrganizationList();
                ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObj);
                ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
                var ProcessObj = DMLObj.GetProcessList();
                ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObj);

                ModelState.AddModelError("TypeId", "هیچ نوع فعالیت معتبری انتخاب نشده است");
                return View(AddObj);
            }
            bool resAdd = false;
            if (AddObj.Id == null)
                return View("Error");
            var addedActivity = DMLObj.EditActivity(new AMActivity { Id = AddObj.Id.GetValueOrDefault(), Name = AddObj.Name, Description = AddObj.Description, TypeId = AddObj.TypeId, Type = selectedActType }, out resAdd);
            if (resAdd && DMLObj.EditActivityRelation(OrgsList, ProcessesList, addedActivity) > 0)
                return View("Success");

            var OrgsObjp = DMLObj.GetOrganizationList();
            ViewBag.AllOrganizations = AutoMapper.Mapper.Map<IEnumerable<AMOrganization>, IEnumerable<JsonOrganizationViewModel>>(OrgsObjp);
            ViewBag.ActivityTypes = DMLObj.GetAllActivityTypes();
            var ProcessObjp = DMLObj.GetProcessList();
            ViewBag.AllProcesses = AutoMapper.Mapper.Map<IEnumerable<AMProcess>, IEnumerable<JsonProcessViewModel>>(ProcessObjp);

            return View("Error");
        }


    }
}