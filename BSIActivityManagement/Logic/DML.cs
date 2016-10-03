using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BSIActivityManagement.DAL;
using BSIActivityManagement.Models;
using BSIActivityManagement.ViewModel;
using AutoMapper;
using System.IO;

namespace BSIActivityManagement.Logic
{
    public class DML
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string _DEFAULTIMAGEFILEADDRESS = HttpContext.Current.Server.MapPath("~/Image/Default.jpg");
        public IEnumerable<AMOrganization> GetOrganizationList()
        {
            return db.AMOrganizationEnt;
        }
        public IEnumerable<AMProcess> GetProcessList()
        {
            return db.AMProcessEnt;

        }
        // This Method returns a 2d list of organizations which the first row is the first organizations in the chart and the second row is the selected organization parents and siblings while get to the childs of the currently selected organization
        public IEnumerable<OrgRow> GetOrgList(int? OrgId)
        {
            List<OrgRow> result = new List<OrgRow>();
            var currentOrg = db.AMOrganizationEnt.Find(OrgId);
            if (OrgId == null || currentOrg == null)
            {
                OrgRow resultrow = new OrgRow();
                OrgId = 0;
                resultrow.OrgList = db.AMOrganizationEnt.Where(m => m.ParentId == 0 || m.ParentId == null);
                result.Add(resultrow);
                return result.AsEnumerable();
            }

            OrgRow resultrowchild = new OrgRow();
            resultrowchild.OrgList = db.AMOrganizationEnt.Where(m => m.ParentId == OrgId);
            result.Add(resultrowchild);

            OrgRow resultrowsibling = new OrgRow();
            resultrowsibling.OrgList = db.AMOrganizationEnt.Where(m => m.ParentId == currentOrg.ParentId);
            resultrowsibling.SelectedOrgId = OrgId;
            result.Add(resultrowsibling);

            int? currentparent = currentOrg.ParentId;
            int currentparentint = 0;
            if (currentparent != null && currentparent != 0) { currentparentint = (int)currentparent; }

            var currentparentObj = db.AMOrganizationEnt.Find(currentparentint);

            while (currentparentObj != null)
            {
                OrgRow resultrow = new OrgRow();
                int? currentparId = currentparentObj.ParentId;
                int? currentId = currentparentObj.Id;
                resultrow.OrgList = db.AMOrganizationEnt.Where(m => m.ParentId == currentparId);
                resultrow.SelectedOrgId = currentId;
                result.Add(resultrow);
                if (currentparId == null || currentparId == 0)
                    break;
                currentparentObj = db.AMOrganizationEnt.Find(currentparentObj.ParentId);
            }
            result.Reverse();

            return result.AsEnumerable();
        }

        // This Method returns a 2d list of organizations which the first row is the first organizations in the chart and the second row is the selected organization parents and siblings while get to the childs of the currently selected organization
        public IEnumerable<ProcessRow> GetProcessList(int? ProcessId)
        {
            List<ProcessRow> result = new List<ProcessRow>();
            var currentProcess = db.AMProcessEnt.Find(ProcessId);
            if (ProcessId == null || currentProcess == null)
            {
                ProcessRow resultrow = new ProcessRow();
                ProcessId = 0;
                resultrow.ProcessList = db.AMProcessEnt.Where(m => m.ParentId == 0 || m.ParentId == null);
                result.Add(resultrow);
                return result.AsEnumerable();
            }

            ProcessRow resultrowchild = new ProcessRow();
            resultrowchild.ProcessList = db.AMProcessEnt.Where(m => m.ParentId == ProcessId);
            result.Add(resultrowchild);

            ProcessRow resultrowsibling = new ProcessRow();
            resultrowsibling.ProcessList = db.AMProcessEnt.Where(m => m.ParentId == currentProcess.ParentId);
            resultrowsibling.SelectedProcessId = ProcessId;
            result.Add(resultrowsibling);

            int? currentparent = currentProcess.ParentId;
            int currentparentint = 0;
            if (currentparent != null && currentparent != 0) { currentparentint = (int)currentparent; }

            var currentparentObj = db.AMProcessEnt.Find(currentparentint);

            while (currentparentObj != null)
            {
                ProcessRow resultrow = new ProcessRow();
                int? currentparId = currentparentObj.ParentId;
                int? currentId = currentparentObj.Id;
                resultrow.ProcessList = db.AMProcessEnt.Where(m => m.ParentId == currentparId);
                resultrow.SelectedProcessId = currentId;
                result.Add(resultrow);
                if (currentparId == null || currentparId == 0)
                    break;
                currentparentObj = db.AMProcessEnt.Find(currentparentObj.ParentId);
            }
            result.Reverse();
            return result.AsEnumerable();
        }



        public AMImage GetImageById(int ImageId)
        {
            return db.AMImageEnt.Find(ImageId);
        }
        public bool OrganizationEditById(AMOrganization OrgObj)
        {
            try
            {
                db.Entry(OrgObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }


        public int? GetParentOrgId(int? OrgId)
        {
            if (OrgId == null || db.AMOrganizationEnt.Find(OrgId) == null) return null;
            return db.AMOrganizationEnt.Find(OrgId).ParentId;
        }
        public AMOrganization GetOrgById(int? OrgId)
        {
            if (OrgId == null || db.AMOrganizationEnt.Find(OrgId) == null) return null;
            return db.AMOrganizationEnt.Find(OrgId);
        }
        public AMProcess GetProcessById(int? ProcessId)
        {
            if (ProcessId == null || db.AMProcessEnt.Find(ProcessId) == null) return null;
            return db.AMProcessEnt.Find(ProcessId);
        }


        public bool OrgHasChild(int OrgId)
        {
            return (db.AMOrganizationEnt.Count(m => m.ParentId == OrgId) > 0) ? true : false;
        }
        public bool OrgHasDocument(int OrgId)
        {
            return (db.AMActProcOrgRelEnt.Count(m => m.OrganizationId == OrgId) > 0) ? true : false;
        }

        public bool DeleteOrgById(int OrgId)
        {
            var CurrentOrg = db.AMOrganizationEnt.Find(OrgId);
            if (CurrentOrg != null)
            {
                try
                {
                    db.AMOrganizationEnt.Remove(CurrentOrg);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public int AddImageGetId(byte[] filedata, string title, int width, int height, string imagetype)
        {
            AMImage ImageObj = new AMImage()
            {
                Title = title,
                width = width,
                height = height,
                FileType = imagetype,
                ImageData = filedata,
                CreationTime = DateTime.Now
            };
            db.AMImageEnt.Add(ImageObj);
            db.SaveChanges();
            return ImageObj.Id;
        }

        public int AddDocGetId(byte[] filedata, string title, string fileName, int fileSize, string fileType)
        {
            AMFile FileObj = new AMFile()
            {
                FileName = fileName,
                FileSize = fileSize,
                Title = title,
                FileData = filedata,
                FileType = fileType,
                CreationTime = DateTime.Now
            };
            db.AMFileEnt.Add(FileObj);
            db.SaveChanges();
            AMDocument DocObj = new AMDocument
            {
                Title = title,
                FileId = FileObj.Id,
                File = FileObj
            };
            db.AMDocumentEnt.Add(DocObj);
            db.SaveChanges();
            return DocObj.Id;
        }

        public byte[] GetImageDataById(int ImageId, out string type)
        {
            var k = db.AMImageEnt.Find(ImageId);
            type = "image/jpg";
            if (k == null) return null;
            type = k.FileType;
            return k.ImageData;
        }

        public byte[] GetDocDataById(int DocId, out string type)
        {
            var k = db.AMDocumentEnt.Find(DocId);
            type = "image/jpg";
            if (k == null) return null;
            type = k.File.FileType;
            return k.File.FileData;
        }

        public OrganizationViewModel AddNewOrganization(OrganizationViewModel OrgObj)
        {
            int CurrentParentId = 0;
            if (OrgObj.ParentId != null) CurrentParentId = (int)OrgObj.ParentId;
            try
            {
                AMOrganization CurrentOrganization = new AMOrganization
                {
                    Name = OrgObj.Name,
                    Description = OrgObj.Description,
                    ParentId = CurrentParentId
                };
                if (OrgObj.ImageId != null)
                    CurrentOrganization.Image = db.AMImageEnt.Find(OrgObj.ImageId);
                db.AMOrganizationEnt.Add(CurrentOrganization);
                db.SaveChanges();
                OrgObj.Id = CurrentOrganization.Id;
                OrgObj.OperationMessage = "#lang-enlish:Operation Completed Successfully. #lang-persian:عملیات با موفقیت انجام شد.";
                OrgObj.OperationResult = true;
                return OrgObj;
            }
            catch (Exception e)
            {
                OrgObj.OperationMessage = "#lang-english: Operation has failed!: Error Details:" + e.Message + "#lang-persian: انجام عملیات با خطا مواجه شد! جزییات خطا: " + e.Message;
                OrgObj.OperationResult = false;
                return OrgObj;
            }
        }
        // Process Methods
        public AMProcess AddNewProcess(AMProcess ProcessObj, out bool res)
        {
            try
            {
                db.AMProcessEnt.Add(ProcessObj);
                db.SaveChanges();
                res = true;
                return ProcessObj;
            }
            catch (Exception e1)
            {
                res = false;
                return ProcessObj;
            }
        }

        public AMProcess GetDefaultProcess(out bool res)
        {
            res = false;
            var d = db.AMProcessEnt.FirstOrDefault();
            if (d != null) { res = true; return d; }
            bool resDefaultProcessType = false;
            AMProcessType DefaultProcessType = GetDefaultProcessType(out resDefaultProcessType);
            if (!resDefaultProcessType) return null;
            AMProcess AddingProcess = new AMProcess { Name = "فرآیند پیش فرض", Description = "این فرآیند به دلایل فنی ایجاد شده است و هیچکدام از فعالیتها متعلق به این فرآیند نیستند", ParentId = 0, ProcessTypeId = DefaultProcessType.Id, ProcessType = DefaultProcessType };
            try
            {
                db.AMProcessEnt.Add(AddingProcess);
                db.SaveChanges();
                res = true;
                return AddingProcess;
            }
            catch
            {
                return AddingProcess;
            }
        }

        public AMProcessType GetDefaultProcessType(out bool res)
        {
            var resAddImage = false;
            res = false;
            var d = db.AMProcessTypeEnt.FirstOrDefault();
            if (d != null) return d;
            AMImage DefaultImage = GetDefaultImage(out resAddImage);
            if (!resAddImage) return null;
            AMProcessType AddingProcessType = new AMProcessType { Name = "فرآیند پیش فرض", Description = "این فرآیند به دلایل فنی ایجاد شده است و هیچکدام از فعالیتها متعلق به این فرآیند نیستند", Image = DefaultImage, ImageId = DefaultImage.Id };
            try
            {
                db.AMProcessTypeEnt.Add(AddingProcessType);
                db.SaveChanges();
                res = true;
                return AddingProcessType;
            }
            catch
            {
                return null;
            }
        }

        public bool ProcessEditById(AMProcess ProcessObj)
        {
            try
            {
                db.Entry(ProcessObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ProcessHasChild(int ProcessId)
        {
            return (db.AMProcessEnt.Count(m => m.ParentId == ProcessId) > 0) ? true : false;
        }
        public bool ProcessHasDocument(int ProcessId)
        {
            return (db.AMActProcOrgRelEnt.Count(m => m.ProcessId == ProcessId) > 0) ? true : false;
        }

        public bool DeleteProcessById(int ProcessId)
        {
            var CurrentProcess = db.AMProcessEnt.Find(ProcessId);
            if (CurrentProcess != null)
            {
                try
                {
                    db.AMProcessEnt.Remove(CurrentProcess);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// All Types are Manipulated and added here
        /// </summary>
        /// <param name="ProcessTObj"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public AMProcessType AddNewProcessType(AMProcessType ProcessTObj, out bool result)
        {
            try
            {
                db.AMProcessTypeEnt.Add(ProcessTObj);
                db.SaveChanges();
                result = true;
                return ProcessTObj;
            }
            catch
            {
                result = false;
                return ProcessTObj;
            }
        }
        public IEnumerable<AMProcessType> GetAllProcessTypes()
        {
            return db.AMProcessTypeEnt;
        }

        public AMProcessType GetProcessTypeById(int Id)
        {
            return db.AMProcessTypeEnt.Find(Id);
        }
        public bool ProcessTypeEditById(AMProcessType ProcessTObj)
        {
            try
            {
                db.Entry(ProcessTObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }
        public bool ProcessTypeHasProcess(int ProcessTId)
        {
            return (db.AMProcessEnt.Count(m => m.ProcessTypeId == ProcessTId) > 0) ? true : false;
        }
        public bool DeleteProcessTypeById(int ProcessTId)
        {
            var CurrentProcessT = db.AMProcessTypeEnt.Find(ProcessTId);
            if (CurrentProcessT != null)
            {
                try
                {
                    db.AMProcessTypeEnt.Remove(CurrentProcessT);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        ///// Activity Type Methods
        public AMActivityType AddNewActivityType(AMActivityType ActivityTObj, out bool result)
        {
            try
            {
                db.AMActivityTypeEnt.Add(ActivityTObj);
                db.SaveChanges();
                result = true;
                return ActivityTObj;
            }
            catch
            {
                result = false;
                return ActivityTObj;
            }
        }
        public IEnumerable<AMActivityType> GetAllActivityTypes()
        {
            return db.AMActivityTypeEnt;
        }

        public AMActivityType GetActivityTypeById(int Id)
        {
            return db.AMActivityTypeEnt.Find(Id);
        }
        public bool ActivityTypeEditById(AMActivityType ActivityTObj)
        {
            try
            {
                db.Entry(ActivityTObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }
        public bool ActivityTypeHasActivity(int ActivityTId)
        {
            return (db.AMActivityEnt.Count(m => m.TypeId == ActivityTId) > 0) ? true : false;
        }

        public bool DeleteActivityTypeById(int ActivityTId)
        {
            var CurrentActivityT = db.AMActivityTypeEnt.Find(ActivityTId);
            if (CurrentActivityT != null)
            {
                try
                {
                    db.AMActivityTypeEnt.Remove(CurrentActivityT);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        ///// Activity Item Type Methods
        public AMActivityItemType AddNewActivityItemType(AMActivityItemType ActivityItemTObj, out bool result)
        {
            try
            {
                db.AMActivityItemTypeEnt.Add(ActivityItemTObj);
                db.SaveChanges();
                result = true;
                return ActivityItemTObj;
            }
            catch
            {
                result = false;
                return ActivityItemTObj;
            }
        }
        public IEnumerable<AMActivityItemType> GetAllActivityItemTypes()
        {
            return db.AMActivityItemTypeEnt;
        }

        public AMActivityItemType GetActivityItemTypeById(int Id)
        {
            return db.AMActivityItemTypeEnt.Find(Id);
        }
        public bool ActivityItemTypeEditById(AMActivityItemType ActivityItemTObj)
        {
            try
            {
                db.Entry(ActivityItemTObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }
        public bool ActivityItemTypeHasActivityItem(int ActivityItemTId)
        {
            return (db.AMActivityItemEnt.Count(m => m.ItemTypeId == ActivityItemTId) > 0) ? true : false;
        }
        public bool DeleteActivityItemTypeById(int ActivityItemTId)
        {
            var CurrentActivityItemT = db.AMActivityItemTypeEnt.Find(ActivityItemTId);
            if (CurrentActivityItemT != null)
            {
                try
                {
                    db.AMActivityItemTypeEnt.Remove(CurrentActivityItemT);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        ///////Add Activity////
        public IEnumerable<AMOrganization> FindOrgsForAddActivityByStringIds(string IdsStr)
        {
            var k = IdsStr.Split(';').ToList();
            return db.AMOrganizationEnt.Where(x => k.Contains(x.Id.ToString()));
        }
        public IEnumerable<AMProcess> FindProcessesForAddActivityByStringIds(string IdsStr)
        {
            var k = IdsStr.Split(';').ToList();
            return db.AMProcessEnt.Where(x => k.Contains(x.Id.ToString()));
        }
        public AMActivity AddNewActivity(AMActivity ActivityObj, out bool result)
        {
            try
            {
                db.AMActivityEnt.Add(ActivityObj);
                db.SaveChanges();
                result = true;
                return ActivityObj;
            }
            catch
            {
                result = false;
                return ActivityObj;
            }
        }

        public AMActivity EditActivity(AMActivity ActivityObj, out bool result)
        {
            AMActivity k = db.AMActivityEnt.Find(ActivityObj.Id);

            if (k == null)
            {
                result = false;
                return ActivityObj;
            }
                
            try
            {
                k.Name = ActivityObj.Name;
                k.Description = ActivityObj.Description;
                k.TypeId = ActivityObj.TypeId;
                db.Entry(k).State = EntityState.Modified;
                db.SaveChanges();
                result = true;
                return ActivityObj;
            }
            catch(Exception e1)
            {
                result = false;
                return ActivityObj;
            }
        }

        public int AddNewActivityRelation(IEnumerable<AMOrganization> OrgsList, IEnumerable<AMProcess> ProcessesList, AMActivity Act)
        {
            int k = 0;
            foreach (var o in OrgsList)
            {
                foreach (var p in ProcessesList)
                {
                    k++;
                    db.AMActProcOrgRelEnt.Add(new AMActProcOrgRel { Activity = Act, ActivityId = Act.Id, Organization = o, OrganizationId = o.Id, Process = p, ProcessId = p.Id });
                }
            }
            try
            {
                db.SaveChanges();
                return k;
            }
            catch
            {
                return k;
            }
        }

        public int EditActivityRelation(IEnumerable<AMOrganization> OrgsList, IEnumerable<AMProcess> ProcessesList, AMActivity Act)
        {
            var currentrelations = db.AMActProcOrgRelEnt.Where(m => m.ActivityId == Act.Id);
            try
            {
                db.AMActProcOrgRelEnt.RemoveRange(currentrelations);
                db.SaveChanges();
            }
            catch
            {
                return 0;
            }

            int k = 0;
            foreach (var o in OrgsList)
            {
                foreach (var p in ProcessesList)
                {
                    k++;
                    db.AMActProcOrgRelEnt.Add(new AMActProcOrgRel { Activity = Act, ActivityId = Act.Id, Organization = o, OrganizationId = o.Id, Process = p, ProcessId = p.Id });
                }
            }
            try
            {
                db.SaveChanges();
                return k;
            }
            catch
            {
                return k;
            }
        }


        ///////// User Methods
        public AMUser AddAmUser(AMUser UserObj, out bool result)
        {
            try
            {
                db.AMUserEnt.Add(UserObj);
                db.SaveChanges();
                result = true;
                return UserObj;
            }
            catch
            {
                result = false;
                return UserObj;
            }
        }

        public AMUserType GetUserTypeById(int TypeId)
        {
            return db.AMUserTypeEnt.Find(TypeId);
        }

        public AMImage GetDefaultImage(out bool result)
        {
            var countK = db.AMImageEnt.Count();
            if (countK == 0)
            {
                AMImage ImageObj = new AMImage
                {
                    Title = "تصویر پیش فرض",
                    CreationTime = DateTime.Now,
                    FileType = "image/jpeg",
                    ImageData = File.ReadAllBytes(_DEFAULTIMAGEFILEADDRESS),
                    height = 250,
                    width = 250
                };
                try
                {
                    db.AMImageEnt.Add(ImageObj);
                    db.SaveChanges();
                    result = true;
                    return ImageObj;
                }
                catch
                {
                    result = false;
                    return ImageObj;
                }
            }
            else
            {
                result = true;
                return db.AMImageEnt.First();
            }
        }

        public AMUserType GetDefaultUserType(out bool result)
        {
            var countk = db.AMUserTypeEnt.Count();
            if (countk == 0)
            {
                AMUserType AmUserTypeObj = new AMUserType();
                AmUserTypeObj.Name = "کاربری پیش فرض";
                AmUserTypeObj.Description = "این نوع کاربری برای تمام کاربران جدید در نظر گرفته می شود";
                bool ImageDefaultResult = false;
                AmUserTypeObj.Image = GetDefaultImage(out ImageDefaultResult);
                if (ImageDefaultResult) { AmUserTypeObj.ImageId = AmUserTypeObj.Image.Id; }
                else { result = false; return AmUserTypeObj; }
                try
                {
                    db.AMUserTypeEnt.Add(AmUserTypeObj);
                    db.SaveChanges();
                    result = true;
                    return AmUserTypeObj;
                }
                catch
                {
                    result = false;
                    return AmUserTypeObj;
                }
            }
            else
            {
                result = true;
                return db.AMUserTypeEnt.First();
            }
        }


        public AMUnitOfOrg AddNewOrganizationUnit(AMUnitOfOrg UnitObj, out bool result)
        {
            try
            {
                db.AMUnitEnt.Add(UnitObj);
                db.SaveChanges();
                result = true;
                return UnitObj;
            }
            catch
            {
                result = false;
                return UnitObj;
            }
        }

        public IEnumerable<AMUnitOfOrg> GetUnitOfOrgList(int OrgId)
        {
            return db.AMUnitEnt.Where(m => m.OrganizationId == OrgId);
        }

        public IEnumerable<AMUserType> GetUserTypeList()
        {
            return db.AMUserTypeEnt;
        }

        public AMUserType AddNewUserType(AMUserType UserTObj, out bool result)
        {
            try
            {
                db.AMUserTypeEnt.Add(UserTObj);
                db.SaveChanges();
                result = true;
                return UserTObj;
            }
            catch
            {
                result = false;
                return UserTObj;
            }
        }

        public bool UserTypeEditById(AMUserType UserTObj)
        {
            try
            {
                db.Entry(UserTObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public bool UserTypeHasUser(int UserTId)
        {
            return (db.AMUserEnt.Count(m => m.UserTypeId == UserTId) > 0) ? true : false;
        }

        public bool DeleteUserTypeById(int UserTId)
        {
            var CurrentUserT = db.AMUserTypeEnt.Find(UserTId);
            if (CurrentUserT != null)
            {
                try
                {
                    db.AMUserTypeEnt.Remove(CurrentUserT);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<AMAccess> GetAccessList()
        {
            return db.AMAccessEnt;
        }

        public AMAccess AddNewAccess(AMAccess AccessObj, out bool result)
        {
            try
            {
                db.AMAccessEnt.Add(AccessObj);
                db.SaveChanges();
                result = true;
                return AccessObj;
            }
            catch
            {
                result = false;
                return AccessObj;
            }
        }

        public AMAccess GetAccessById(int AccessId)
        {
            return db.AMAccessEnt.Find(AccessId);
        }

        public bool AccessEditByObject(AMAccess AccessObj)
        {
            try
            {
                db.Entry(AccessObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public bool AccessHasUser(int AccessId)
        {
            return (db.AMUserTypeAccessListEnt.Count(m => m.AccessId == AccessId) > 0) ? true : false;
        }

        public bool DeleteAccessById(int AccessId)
        {
            var CurrentAccess = db.AMAccessEnt.Find(AccessId);
            if (CurrentAccess != null)
            {
                try
                {
                    db.AMAccessEnt.Remove(CurrentAccess);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public AMUser GetAmUserById(int id)
        {
            return db.AMUserEnt.Find(id);
        }

        /////////////// User PlaceMent

        public IQueryable<IGrouping<AMUnitOfOrg, AMUserPlacement>> GetUserPlacement(int AmUserId)
        {
            return db.AMUserPlaceMentEnt.Where(m => m.UserId == AmUserId).GroupBy(k => k.Unit);
        }

        public IEnumerable<AMUnitOfOrg> SearchUnitByKeyWords(IEnumerable<string> keywords)
        {
            var matches = from m in db.AMUnitEnt
                          where (keywords.Where(sub => m.Name.Contains(sub)).Any() || keywords.Where(sub => m.IdentityCode.Contains(sub)).Any())
                          select m;
            return matches.AsEnumerable();
        }

        public AMUnitOfOrg GetUnitById(int unitId)
        {
            return db.AMUnitEnt.Find(unitId);
        }

        public AMUserPlacementReq AddUserPlacementRequest(int UserId, int UnitId, out bool result)
        {
            AMUserPlacementReq ReqObj = new AMUserPlacementReq
            {
                Status = 0,
                UnitId = UnitId,
                UserId = UserId,
                AcceptationRejecionDateTime = DateTime.Now
            };
            if (db.AMUserPlacementReqEnt.Count(k => k.Status == 0 && k.UnitId == UnitId && k.UserId == UserId) > 0)
            {
                result = true;
                return ReqObj;
            }
            try
            {
                db.AMUserPlacementReqEnt.Add(ReqObj);
                db.SaveChanges();
                result = true;
                return ReqObj;
            }
            catch
            {
                result = false;
                return ReqObj;
            }
        }

        public UserPlacementViewModel GetUserPlaceMentViewModel(AMUnitOfOrg CurrentUnit)
        {
            //var UnitProcesses = db.AMUserPlaceMentEnt.Where(p => p.Unit.Id == CurrentUnit.Id);
            //List<ProcessUsersViewModel> ProcessesList = new List<ProcessUsersViewModel>();
            //foreach(var prcs in UnitProcesses)
            //{
            //    List<AMUser> UserList = new List<AMUser>();
            //    UserList.AddRange(db.AMUserPlaceMentEnt.Where(z => z.Process.Id == prcs.Process.Id && z.Unit.Id == CurrentUnit.Id).Select(k => k.User));
            //    ProcessesList.Add(new ProcessUsersViewModel { Process = prcs.Process, Users = UserList.AsEnumerable()});
            //}
            UserPlacementViewModel CurrentObj = new UserPlacementViewModel();
            CurrentObj.AmUnit = CurrentUnit;
            CurrentObj.UsersWaitingForJoin = db.AMUserPlacementReqEnt.Where(m => m.Unit.Id == CurrentUnit.Id && m.Status == 0).Select(k => k.User).AsEnumerable();
            //CurrentObj.ProcessWithCurrentUsers = ProcessesList.AsEnumerable();
            return CurrentObj;
        }
        public IEnumerable<AMUnitOfOrg> GetUserUnitsByUserId(int UserId)
        {
            return db.AMUserPlaceMentEnt.Where(u => u.UserId == UserId).Select(k => k.Unit).Distinct();
        }

        public IEnumerable<AMUser> SearchUserByKeyWords(IEnumerable<string> keywords)
        {
            var matches = db.AMUserEnt.Where(user => keywords.Contains(user.FirstName) && keywords.Contains(user.Lastname));
            //var matches = from m in db.AMUserEnt
            //              where (keywords.Where(sub => m.FirstName.Contains(sub)).Any() || keywords.Where(sub => m.Lastname.Contains(sub)).Any() || keywords.Where(sub=> db.Users.Where(k=>k.UserName.Contains(sub)).Any()).Any())
            //              select m;
            return matches.AsEnumerable();
        }

        public AMAccess GetAccessByAccessKey(string AcKey)
        {
            return db.AMAccessEnt.Where(m => m.AccessKey == AcKey.Trim().ToUpper()).FirstOrDefault();
        }

        public AMAccess GetDefaultAccessByAccessKey(string AcKey, out bool res, string Name, string Description, int Val)
        {
            if (AcKey.Trim().Length < 4)
            {
                res = false;
                return null;
            }
            var ResObj = GetAccessByAccessKey(AcKey);
            if (ResObj != null)
            {
                res = true;
                return ResObj;
            }

            AMAccess AddingObj = new AMAccess
            {
                Name = Name,
                Description = Description,
                Value = Val,
                AccessKey = AcKey
            };
            try
            {
                db.AMAccessEnt.Add(AddingObj);
                db.SaveChanges();
                res = true;
                return AddingObj;
            }
            catch
            {
                res = false;
                return AddingObj;
            }

        }

        public bool CheckUserTypeAccess(AMAccess Access, AMUserType UserType)
        {
            if (db.AMUserTypeAccessListEnt.Where(m => m.UserType.Id == UserType.Id && m.Access.Id == Access.Id).Any()) return true;
            return false;
        }

        public bool AddUnitManager(AddUnitManagerViewModel UnitManagerObj, out string message)
        {
            bool resDefaultAccess = false;
            message = "در عملیات ایجاد مجوز دسترسی به کاربر خطایی رخ داده است. متاسفیم که عملیات نا موفق بود!";
            AMAccess AccessObj = GetDefaultAccessByAccessKey("MANAGE_UNIT_USERS", out resDefaultAccess, "پذیرش یا حذف کاربر در سازمان", "با داشتن این مجوز این کاربر می تواند کاربران دیگر را به واحدهایی که در آنها مشغول کار است اضافه نموده یا حذف نماید", 0);
            if (!resDefaultAccess) return false;
            // UserType is not accepted for this operation
            message = "این نوع کاربری قادر به انجام چنین فعالیتی نخواهد بود لطفا ابتدا نوع کاربری را تغییر دهید";
            if (!CheckUserTypeAccess(AccessObj, UnitManagerObj.SelectedUser.UserType)) return false;
            message = "در مرحله ایجاد فرآیند پیش فرض برای کاربر فعلی خطایی رخ داده است. متاسفیم که عملیات ناموفق بوده است";
            AMUserPlacement UserPlcamentObj = new AMUserPlacement();
            bool resDefaultProcess = false;
            UserPlcamentObj.Process = GetDefaultProcess(out resDefaultProcess);
            if (!resDefaultProcess) return false;
            UserPlcamentObj.ProcessId = UserPlcamentObj.Process.Id;
            UserPlcamentObj.Unit = UnitManagerObj.SelectedUnit;
            UserPlcamentObj.UnitId = UnitManagerObj.SelectedUnit.Id;
            UserPlcamentObj.User = UnitManagerObj.SelectedUser;
            UserPlcamentObj.UserId = UnitManagerObj.SelectedUser.Id;
            try
            {
                db.AMUserPlaceMentEnt.Add(UserPlcamentObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                message = "در مرحله ذخیره اطلاعات در پایگاه داده ها خطایی رخ داده است. متاسفیم که اجرای عملیات با خطا مواجه شده است";
                return false;
            }
        }

        public IEnumerable<AMUserTypeAccessList> GetAllTypesAccessList()
        {
            return db.AMUserTypeAccessListEnt.OrderBy(k => k.UserType);
        }
        public IEnumerable<AMUserTypeAccessList> GetTypesAccessListByTypeId(int Id)
        {
            return db.AMUserTypeAccessListEnt.Where(m => m.UserTypeId == Id);
        }

        public IEnumerable<AMAccess> GetRemainTypesAccessListByTypeId(int Id)
        {
            return db.AMAccessEnt.Where(m => db.AMUserTypeAccessListEnt.Where(k => k.UserTypeId == Id).FirstOrDefault().Access != m);
        }

        public bool AddUserTypeAccess(AMUserTypeAccessList UserAccessObj)
        {
            if (db.AMUserTypeAccessListEnt.Count(u => u.AccessId == UserAccessObj.AccessId && u.UserTypeId == UserAccessObj.UserTypeId) > 0)
                return true;
            try
            {
                db.AMUserTypeAccessListEnt.Add(UserAccessObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckUserAccessKeyByUserId(string AcKey, string UserId)
        {
            int AmUserId = 0;
            Int32.TryParse(UserId, out AmUserId);
            var k = db.AMAccessEnt.Where(m => m.AccessKey == AcKey);
            var u = db.AMUserEnt.Find(AmUserId);
            if (k == null || u == null) return false;
            if (CheckUserTypeAccess(k.FirstOrDefault(), u.UserType)) return true;
            return false;
        }

        public bool SetUserType(AddUserTypeManagerViewModel model, out string message)
        {
            AMUser CurrentUser = db.AMUserEnt.Find(model.User.Id);
            AMUserType CurrentType = db.AMUserTypeEnt.Find(model.Type.Id);
            message = "کاربر و یا نوع کاربری ناشناخته است";
            if (CurrentUser == null || CurrentType == null) return false;
            try
            {
                CurrentUser.UserTypeId = CurrentType.Id;
                CurrentUser.UserType = CurrentType;
                db.Entry(CurrentUser).State = EntityState.Modified;
                db.SaveChanges();
                message = "عملیات با موفقیت انجام شد";
                return true;
            }
            catch
            {
                message = "انجام عملیات در زمان ذخیره سازی در پایگاه داده ها با خطا مواجه شد";
                return false;
            }
        }

        public bool CheckUserPlacementRequest(int UserId, int UnitId, int Status)
        {
            if (db.AMUserPlacementReqEnt.Where(k => k.Status == Status && k.UserId == UserId && k.UnitId == UnitId).Any()) return true;
            return false;
        }

        public AMUserPlacement AcceptUserPlacement(out bool res, AMUserPlacementReq Req, AMUser ConfirmingUser)
        {
            res = false;
            Req.Status = 1;
            Req.AcceptationRejecionDateTime = DateTime.Now;
            Req.ConfirmingUserId = ConfirmingUser.Id;
            Req.ConfirmingUser = ConfirmingUser;
            bool resDefaultProcess = false;
            AMUserPlacement AddingObj = new AMUserPlacement();
            AMProcess defProcess = GetDefaultProcess(out resDefaultProcess);
            if (!resDefaultProcess) return null;
            AddingObj.Process = defProcess;
            AddingObj.ProcessId = defProcess.Id;
            AddingObj.UnitId = Req.UnitId;
            AddingObj.UserId = Req.UserId;
            try
            {
                db.Entry(Req).State = EntityState.Modified;
                db.AMUserPlaceMentEnt.Add(AddingObj);
                db.SaveChanges();
                res = true;
                return AddingObj;
            }
            catch
            {
                return null;
            }
        }

        public AMUserPlacementReq GetUserPlacementReqByUserAndUnit(int userId, int unitId, int status)
        {
            return db.AMUserPlacementReqEnt.Where(m => m.UserId == userId && m.UnitId == unitId && m.Status == status).FirstOrDefault();
        }

        public bool RejectUserPlacement(AMUserPlacementReq Req, AMUser ConfirmingUser)
        {
            Req.Status = 2;
            Req.AcceptationRejecionDateTime = DateTime.Now;
            Req.ConfirmingUserId = ConfirmingUser.Id;
            Req.ConfirmingUser = ConfirmingUser;
            try
            {
                db.Entry(Req).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMUser> GetUnitUsers(int UnitId)
        {
            return db.AMUserPlaceMentEnt.Where(m => m.UnitId == UnitId).Select(k => k.User).AsEnumerable();
        }

        public List<NavViewModel> GetMainNaviagtion(int UnitId)
        {
            //< ul >
            //    < li class="cd-label">منوی اصلی</li>
            //    <li> <a href = "@Url.Action("Unit","Main")">واحد سازمانی</a> </li>
            //    <li class="has-children bookmarks">
            //        <a href = "#0" > فرآیندهای سازمان</a>
            //    </li>
            //    <li class="has-children bookmarks">
            //        <a href = "#0" > فرآیندهای کاربر</a>
            //    </li>
            //    <li class="has-children bookmarks">
            //        <a href = "#0" > فعالیتهای کاربر</a>
            //    </li>
            //</ul>

            List<NavViewModel> NavList = new List<NavViewModel>();
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();

            LinkList.Add(new NavLinkViewModel
            {
                action = "UnitProcesses",
                controller = "Main",
                tag = "",
                text = "فرآیندهای سازمان"
            });
            LinkList.Add(new NavLinkViewModel
            {
                action = "UserProcesses",
                controller = "Main",
                tag = "",
                text = "فرآیندهای کاربر"
            });
            LinkList.Add(new NavLinkViewModel
            {
                action = "Unit",
                controller = "Main",
                tag = "",
                text = "انتخاب واحد سازمانی"
            });
            NavList.Add(new NavViewModel
            {
                Title = "منوی اصلی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }


        public List<NavViewModel> GetLogoutNaviagtion()
        {
            //< ul >
            //    < li class="cd-label">منوی راهبری</li>
            //    <li> <a href = "#Org0" > معرفی نرم افزار</a> </li>
            //    <li class="has-children overview">
            //        <a href = "#0" > تاریخچه </ a >
            //    </ li >
            //    < li class="has-children overview">
            //        <a href = "#0" > کاربرد </ a >
            //    </ li >
            //    < li class="has-children overview">
            //        <a href = "#0" > امکانات </ a >
            //    </ li >
            //    < li class="has-children notifications">
            //        <a href = "#0" > کاربران </ a >
            //    </ li >
            //    < li class="has-children notifications">
            //        <a href = "#0" > طراحی و توسعه</a>
            //    </li>
            //    <li class="has-children notifications">
            //        <a href = "#0" > چشم انداز آینده</a>
            //    </li>
            //</ul>

            List<NavViewModel> NavList = new List<NavViewModel>();
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "Intro",
                controller = "Home",
                tag = "",
                text = "معرفی نرم افزار"
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "History",
                controller = "Home",
                tag = "",
                text = "تاریخچه"
            });

            //LinkList.Add(new NavLinkViewModel
            //{
            //    action = "Index",
            //    controller = "Home",
            //    tag = "",
            //    text = "کاربرد"
            //});

            //LinkList.Add(new NavLinkViewModel
            //{
            //    action = "Index",
            //    controller = "Home",
            //    tag = "",
            //    text = "امکانات"
            //});

            //LinkList.Add(new NavLinkViewModel
            //{
            //    action = "Index",
            //    controller = "Home",
            //    tag = "",
            //    text = "کاربران"
            //});

            //LinkList.Add(new NavLinkViewModel
            //{
            //    action = "Index",
            //    controller = "Home",
            //    tag = "",
            //    text = "طراحی و توسعه"
            //});

            //LinkList.Add(new NavLinkViewModel
            //{
            //    action = "Index",
            //    controller = "Home",
            //    tag = "",
            //    text = "چشم انداز آینده"
            //});

            NavList.Add(new NavViewModel
            {
                Title = "منوی اصلی",
                UnitId = 0,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public List<NavViewModel> AddUnitNaviagtion(List<NavViewModel> NavList)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "Index",
                controller = "UserPlacementRequest",
                tag = "",
                text = "درخواست ورود به واحد"
            });

            NavList.Add(new NavViewModel
            {
                Title = "منوی کاربر واحد سازمانی",
                UnitId = 0,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public List<NavViewModel> AddProcessActivityNaviagtion(List<NavViewModel> NavList, int ProcessId, string ProcessTxt, int UnitId, string UnitTxt)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "UnitProcesses",
                controller = "Main",
                tag = "",
                text = UnitTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ProcessActivities",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString(),
                text = ProcessTxt
            });

            NavList.Add(new NavViewModel
            {
                Title = "مسیر فعلی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public List<NavViewModel> AddShowActivityNaviagtion(List<NavViewModel> NavList, int ProcessId, string ProcessTxt, int UnitId, string UnitTxt, int ActivityId, string ActivityTxt)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "UnitProcesses",
                controller = "Main",
                tag = "",
                text = UnitTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ProcessActivities",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString(),
                text = ProcessTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ShowActivity",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString() + "&ActivityId=" + ActivityId.ToString(),
                text = ActivityTxt
            });

            NavList.Add(new NavViewModel
            {
                Title = "مسیر فعلی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public List<NavViewModel> AddShowItemNaviagtion(List<NavViewModel> NavList, int ProcessId, string ProcessTxt, int UnitId, string UnitTxt, int ActivityId, string ActivityTxt, int ItemId, string ItemTxt)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "UnitProcesses",
                controller = "Main",
                tag = "",
                text = UnitTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ProcessActivities",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString(),
                text = ProcessTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ShowActivity",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString() + "&ActivityId=" + ActivityId.ToString(),
                text = ActivityTxt
            });

            LinkList.Add(new NavLinkViewModel
            {
                action = "ShowItem",
                controller = "Main",
                tag = "&ProcessId=" + ProcessId.ToString() + "&ActivityId=" + ActivityId.ToString() + "&ItemId=" + ItemId,
                text = ItemTxt
            });

            NavList.Add(new NavViewModel
            {
                Title = "مسیر فعلی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }


        public List<NavViewModel> AddUnitProcessesNaviagtion(List<NavViewModel> NavList, int UnitId, string UnitTxt)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "Unit",
                controller = "Main",
                tag = "",
                text = UnitTxt
            });

            NavList.Add(new NavViewModel
            {
                Title = "مسیر فعلی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public List<NavViewModel> AddManagerNaviagtion(List<NavViewModel> NavList, int UnitId)
        {
            List<NavLinkViewModel> LinkList = new List<NavLinkViewModel>();
            LinkList.Add(new NavLinkViewModel
            {
                action = "UnitUsers",
                controller = "Main",
                tag = "",
                text = "مشاهده کاربران واحد سازمانی"
            });
            LinkList.Add(new NavLinkViewModel
            {
                action = "Index",
                controller = "UserPlacement",
                tag = "",
                text = "درخواستهای ورود به واحد"
            });
            NavList.Add(new NavViewModel
            {
                Title = "منوی مدیر واحد سازمانی",
                UnitId = UnitId,
                Links = LinkList.AsEnumerable()
            });
            return NavList;
        }

        public IEnumerable<AMProcess> GetUnitUserProcesses(int UnitId, int UserId)
        {
            return db.AMUserPlaceMentEnt.Where(m => m.UnitId == UnitId && m.UserId == UserId).Select(k => k.Process);
        }

        public IEnumerable<AMProcess> GetUnitProcesses(int UnitId)
        {
            int s = db.AMUnitEnt.Find(UnitId).OrganizationId;
            return db.AMActProcOrgRelEnt.Where(m => m.OrganizationId == s).Select(k => k.Process).Distinct();
        }

        public bool VerifyUserUnit(int UnitId, int UserId)
        {
            if (db.AMUserPlaceMentEnt.Count(m => m.UnitId == UnitId && m.UserId == UserId) > 0) return true;
            return false;
        }

        public bool RemoveUserFromProcess(int UnitId, int UserId, int ProcessId, out string message)
        {
            var r = db.AMUserPlaceMentEnt.Where(m => m.UnitId == UnitId && m.UserId == UserId && m.ProcessId == ProcessId);

            if (db.AMUserPlaceMentEnt.Count(m => m.UnitId == UnitId && m.UserId == UserId) <= 1)
            {
                message = "حداقل وجود یک فرآیند برای هر کاربری الزامیست و این فرآیند برای این کاربر قابل حذف شدن نیست.";
                return false;
            }
            try
            {
                db.AMUserPlaceMentEnt.RemoveRange(r);
                db.SaveChanges();
                message = "عملیات موفقیت آمیز بود";
                return true;
            }
            catch
            {
                message = "انجام عملیات در مرحله پایگاه داده ها با خطا مواجه شد.";
                return false;
            }
        }

        public bool AddUserToProcess(int UnitId, int UserId, int ProcessId, out string message)
        {
            if (db.AMUserPlaceMentEnt.Count(e => e.UnitId == UnitId && e.UserId == UserId && e.ProcessId == ProcessId) > 0)
            {
                message = "این اطلاعات قبلا ثبت شده است.";
                return true;
            }
            AMUserPlacement k = new AMUserPlacement
            {
                UnitId = UnitId,
                ProcessId = ProcessId,
                UserId = UserId
            };
            try
            {
                db.AMUserPlaceMentEnt.Add(k);
                db.SaveChanges();
                message = "عملیات با موفقیت انجام شد";
                return true;
            }
            catch (Exception e1)
            {
                message = "خطا در مرحله ذخیره سازی در پایگاه داده ها اتفاق افتاده است. جزئیات خطا: " + e1.Message;
                return false;
            }
        }
        public IEnumerable<ActTypeGroup> GetActivityByUnitProcess(int UnitId, int ProcessId)
        {
            List<ActTypeGroup> resList = new List<ActTypeGroup>();
            var org = db.AMUnitEnt.Find(UnitId);
            if (org == null) return null;
            var acts = db.AMActProcOrgRelEnt.Where(a => a.OrganizationId == org.Organization.Id && a.ProcessId == ProcessId).Select(s => s.Activity);
            var types = acts.Select(m => m.Type).Distinct();
            foreach (var t in types)
            {
                ActTypeGroup item = new ActTypeGroup
                {
                    Type = t,
                    Activities = acts.Where(a => a.TypeId == t.Id)
                };
                resList.Add(item);
            }
            return resList.AsEnumerable();
        }

        public IEnumerable<ActTypeGroup> GetActivityByProcess(int ProcessId)
        {
            List<ActTypeGroup> resList = new List<ActTypeGroup>();
            var acts = db.AMActProcOrgRelEnt.Where(a => a.ProcessId == ProcessId).Select(s => s.Activity).Distinct();
            var types = acts.Select(m => m.Type).Distinct();
            foreach (var t in types)
            {
                ActTypeGroup item = new ActTypeGroup
                {
                    Type = t,
                    Activities = acts.Where(a => a.TypeId == t.Id)
                };
                resList.Add(item);
            }
            return resList.AsEnumerable();
        }

        public bool UserHasAccessToActivity(AMActivity Act, AMUser User)
        {
            var orgs = db.AMUserPlaceMentEnt.Where(p => p.UserId == User.Id).Select(t => t.Unit).Select(o => o.OrganizationId);
            return db.AMActProcOrgRelEnt.Where(r => orgs.Where(w => w == r.OrganizationId).Any() && r.ActivityId == Act.Id).Any();
        }

        public bool UserHasAccessToProcess(AMProcess Prcs, AMUser User)
        {
            var orgs = db.AMUserPlaceMentEnt.Where(p => p.UserId == User.Id).Select(t => t.Unit).Select(o => o.OrganizationId);
            return db.AMActProcOrgRelEnt.Where(r => orgs.Where(w => w == r.OrganizationId).Any() && r.ProcessId == Prcs.Id).Any();
        }

        public bool CheckPageParameters(string UnitId)
        {
            int AmUnitId = 0;
            Int32.TryParse(UnitId, out AmUnitId);
            if (AmUnitId == 0) return false;
            var u = db.AMUnitEnt.Find(AmUnitId);
            if (u == null) return false;
            return true;
        }

        public bool CheckPageParameters(string UnitId, string UserId)
        {
            int AmUserId = 0;
            Int32.TryParse(UserId, out AmUserId);
            if (AmUserId == 0 || !CheckPageParameters(UnitId)) return false;
            var u = db.AMUserEnt.Find(AmUserId);
            if (u == null) return false;
            return true;
        }

        public bool CheckPageParameters(string UnitId, string UserId, string ProcessId)
        {
            int AmProcessId = 0;
            Int32.TryParse(ProcessId, out AmProcessId);
            if (!CheckPageParameters(UnitId, UserId) || AmProcessId == 0) return false;
            var p = db.AMProcessEnt.Find(AmProcessId);
            if (p == null) return false;
            return true;
        }

        public bool CheckPageParameters(string UnitId, string UserId, string ProcessId, string ActivityId, string ItemId)
        {
            int AmItemId = 0;
            Int32.TryParse(ItemId, out AmItemId);
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            if (!CheckPageParameters(UnitId, UserId, ProcessId) || AmActivityId == 0 || db.AMActivityEnt.Find(AmActivityId) == null || AmItemId == 0) return false;
            var p = db.AMActivityItemEnt.Find(AmItemId);
            if (p == null) return false;
            return true;
        }

        public bool CheckPageParameters(string UserId, int ActivityId)
        {
            int AmUserId = 0;
            Int32.TryParse(UserId, out AmUserId);
            if (AmUserId == 0 || db.AMUserEnt.Find(AmUserId) == null) return false;
            var Orgs = GetUserUnitsByUserId(AmUserId).Select(k => k.Organization);
            var z = db.AMActProcOrgRelEnt.Where(m => m.ActivityId == ActivityId);
            if (Orgs == null || z == null) return false;
            foreach (var o in Orgs)
                if (z.Any(p => p.OrganizationId == o.Id)) return true;
            return false;
        }

        public bool CheckPageParameters(string UnitId, string ItemId, int p = 1)
        {

            int AmItemId = 0;
            Int32.TryParse(ItemId, out AmItemId);
            if (AmItemId == 0 || !CheckPageParameters(UnitId) || GetActivityItemById(AmItemId) == null)
                return false;
            return true;
        }

        public IEnumerable<ActItemTypeGroup> GetActivityItemsByActivityId(int ActivityId)
        {
            List<ActItemTypeGroup> resList = new List<ActItemTypeGroup>();
            var Act = db.AMActivityEnt.Find(ActivityId);
            if (Act == null) return null;
            var items = db.AMActivityItemEnt.Where(i => i.ActivityId == ActivityId);
            var types = items.Select(m => m.ItemType).Distinct();
            foreach (var t in types)
            {
                ActItemTypeGroup item = new ActItemTypeGroup
                {
                    Type = t,
                    Items = items.Where(a => a.ItemTypeId == t.Id)
                };
                resList.Add(item);
            }
            return resList.AsEnumerable();
        }

        public IEnumerable<ActItemTypeGroup> GetActivityAllItemsByActivityId(int ActivityId)
        {
            List<ActItemTypeGroup> resList = new List<ActItemTypeGroup>();
            var Act = db.AMActivityEnt.Find(ActivityId);
            if (Act == null) return null;
            var items = db.AMActivityItemEnt.Where(i => i.ActivityId == ActivityId);
            foreach (var t in db.AMActivityItemTypeEnt)
            {
                ActItemTypeGroup item = new ActItemTypeGroup
                {
                    Type = t,
                    Items = items.Where(a => a.ItemTypeId == t.Id)
                };
                resList.Add(item);
            }
            return resList.AsEnumerable();
        }

        public AMActivity GetActivityById(int ActId)
        {
            return db.AMActivityEnt.Find(ActId);
        }

        public AMActivityItem AddActivityItem(AMActivityItem Obj, out bool result)
        {
            try
            {
                db.AMActivityItemEnt.Add(Obj);
                db.SaveChanges();
                result = true;
                return Obj;
            }
            catch
            {
                result = false;
                return Obj;
            }
        }

        public AMActivityItem EditActivityItem(AMActivityItem Obj, out bool result)
        {
            try
            {
                db.Entry(Obj).State = EntityState.Modified;
                db.SaveChanges();
                result = true;
                return Obj;
            }
            catch
            {
                result = false;
                return Obj;
            }
        }

        public AMActivityItem GetActivityItemById(int Id)
        {
            return db.AMActivityItemEnt.Find(Id);
        }

        public AMDocument GetDocumentById(int? Id)
        {
            if (Id == null) return null;
            return db.AMDocumentEnt.Find(Id);
        }

        public bool RemoveActivityItem(AMActivityItem model)
        {
            try
            {
                db.AMActivityItemEnt.Remove(model);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetDocumentPageCount(string DocId)
        {
            int AmDocId = 0;
            Int32.TryParse(DocId, out AmDocId);
            var k = GetDocumentById(AmDocId);
            if (AmDocId == 0 || k == null) return 0;
            string doctype = "image/jpg";
            var imageData = GetDocDataById(AmDocId, out doctype);
            if (doctype.ToLower() == "application/pdf")
            {
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                document.LoadFromStream(new MemoryStream(imageData));
                return document.Pages.Count;
            }
            return 1;
        }

        public bool ActivityHasItem(int ActId)
        {
            return (db.AMActivityItemEnt.Any(i => i.ActivityId == ActId));
        }

        public bool DeleteActivityById(int ActId)
        {
            var A = db.AMActivityEnt.Find(ActId);
            if (A == null) return false;
            var rels = db.AMActProcOrgRelEnt.Where(r => r.ActivityId == ActId);
            try
            {
                db.AMActProcOrgRelEnt.RemoveRange(rels);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            try
            {
                db.AMActivityEnt.Remove(A);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetActivityOrgsByActIdAsString(int ActId)
        {
            var k = db.AMActProcOrgRelEnt.Where(r => r.ActivityId == ActId).Select(e => e.OrganizationId);
            string result = "";
            foreach (var s in k)
                result += s.ToString() + ";";
            return result;
        }

        public string GetActivityProcessesByActIdAsString(int ActId)
        {
            var k = db.AMActProcOrgRelEnt.Where(r => r.ActivityId == ActId).Select(p => p.ProcessId);
            string result = "";
            foreach (var s in k)
                result += s.ToString() + ";";
            return result;
        }



        //////// End of DML class ////////
    }

    public static class DMLExt
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetImageIdByAmUserId(string UserIdStr)
        {
            int UserId = 0;
            Int32.TryParse(UserIdStr, out UserId);
            if (UserId == 0) return null;
            var res = db.AMUserEnt.Find(UserId);
            if (res == null || res.ImageId == null) return null;
            return res.ImageId.ToString();
        }
    }
}
/*
// List of Access Keys -->
// A user has this privilige to add another user to this unit or remove it => AccessKey == "MANAGE_UNIT_USERS"
// 
//
//
//
//


*/
