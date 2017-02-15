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
using BSIActivityManagement.Enum;

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
                return k;
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

        public IEnumerable<AMUnitOfOrg> GetUnitByIdentity(string UnitIdentity)
        {
            return db.AMUnitEnt.Where(m => m.IdentityCode == UnitIdentity);
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

        public IEnumerable<AMUser> SearchUserByUserName(string UserName)
        {
            return db.Users.Where(m => m.UserName == UserName).Select(m => m.AMUser);
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
            if (Access == null || UserType == null) return false;
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
            catch(Exception e1)
            {
                message = "در مرحله ذخیره اطلاعات در پایگاه داده ها خطایی رخ داده است. متاسفیم که اجرای عملیات با خطا مواجه شده است" + e1;
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

            var countExist = db.AMUserPlaceMentEnt.Count(m => m.UserId == AddingObj.UserId && m.ProcessId == AddingObj.ProcessId && m.UnitId == AddingObj.UnitId);

            try
            {
                db.Entry(Req).State = EntityState.Modified;

                if(countExist == 0)
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
            LinkList.Add(new NavLinkViewModel
            {
                action = "Index",
                controller = "Dashboard",
                tag = "",
                text = "داشبورد کاری"
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

        public bool CheckPageParametersForRevision(string ActivityId, string UnitId, string ProcessId, string UserId)
        {
            int AmActivityId = 0;
            Int32.TryParse(ActivityId, out AmActivityId);
            if (!CheckPageParameters(UnitId, UserId, ProcessId) || AmActivityId == 0 || db.AMActivityEnt.Find(AmActivityId) == null ) return false;
            return true;
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
            var k = db.AMActProcOrgRelEnt.Where(r => r.ActivityId == ActId).Select(e => e.OrganizationId).Distinct();
            string result = "";
            foreach (var s in k)
                result += s.ToString() + ";";
            return result;
        }

        public string GetActivityProcessesByActIdAsString(int ActId)
        {
            var k = db.AMActProcOrgRelEnt.Where(r => r.ActivityId == ActId).Select(p => p.ProcessId).Distinct();
            string result = "";
            foreach (var s in k)
                result += s.ToString() + ";";
            return result;
        }

        /////////// Revision //////////021-66748675

        public bool AddRevision(AMRevision Revision)
        {
            try
            {
                Revision.RegDateTime = DateTime.Now;
                db.AMRevisionEnt.Add(Revision);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMRevision> GetRevisionListByRegistrarUserId(int UserId)
        {
            return db.AMRevisionEnt.Where(r => r.UserId == UserId);
        }

        public IEnumerable<AMRevision> GetProcessRevisionListByRegistrarUserId(int UnitId, int ProcessId)
        {
            return db.AMRevisionEnt.Where(r => r.UnitId == UnitId && r.ProcessId == ProcessId);
        }

        public IEnumerable<AMRevision> GetProcessRevisionListByUnitId(int UnitId)
        {
            return db.AMRevisionEnt.Where(r => r.UnitId == UnitId);
        }

        public IEnumerable<AMRevision> GetRevisionListAppliedToMe(int UnitId, int UserId)
        {
            AMUser CurrentUser = GetAmUserById(UserId);
            if (CurrentUser == null) return null;
            if(CurrentUser.UserTypeId == 4) //  4 = Process Owner, 3 = Unit Manager
            {
                var UserProcesses = GetUnitUserProcesses(UnitId, UserId);
                if(UserProcesses.Count() == 0 || (UserProcesses.Count() == 1 && UserProcesses.FirstOrDefault().Id == 1))
                {
                    return db.AMRevisionEnt.Where(r => r.UnitId == UnitId).Where(k=> k.StatusList.Count == 0 || k.StatusList.Where(p=>p.Status < 4 && p.SigningUserId != UserId).Any()).AsEnumerable();
                }
                else
                {
                    List<AMRevision> MyList = new List<AMRevision>();
                    var StatList = db.AMRevisionEnt.Where(r => r.UnitId == UnitId).Where(k => k.StatusList.Count == 0 || k.StatusList.Where(p => p.Status < 4 && p.SigningUserId != UserId).Any()).AsEnumerable();
                    foreach(var s in StatList)
                    {
                        if(UserProcesses.Where(p=>p.Id == s.ProcessId).Any())
                        {
                            MyList.Add(s);
                        }
                    }
                    return MyList.AsEnumerable();
                }
            }
            else if (CurrentUser.UserTypeId == 3) // Unit Manager
            {
                var p1 = db.AMRevisionStatusEnt.Where(p => p.Revision.UnitId == UnitId && p.Status == 2 && p.SigningUserId != UserId).Select(w => w.Revision);
                var p2Except = p1.Where(k => k.StatusList.Where(p => p.Status > 3).Any()).AsEnumerable();
                return p1.Except(p2Except);
            }
            else if (CurrentUser.UserTypeId == 5) // Manager
            {
                var p1 = db.AMRevisionStatusEnt.Where(p => p.Status == 4 && p.SigningUserId != UserId).Select(w => w.Revision);
                var p2Except = p1.Where(k => k.StatusList.Where(p => p.Status > 5).Any()).AsEnumerable();
                return p1.Except(p2Except);
            }
            else if (CurrentUser.UserTypeId == 6) // Top Manager
            {
                var p1 = db.AMRevisionStatusEnt.Where(p => p.Status == 6 && p.SigningUserId != UserId).Select(w => w.Revision);
                var p2Except = p1.Where(k => k.StatusList.Where(p => p.Status > 7).Any()).AsEnumerable();
                return p1.Except(p2Except);
            }
            else if (CurrentUser.UserTypeId == 8) // Top Manager
            {
                var p1 = db.AMRevisionStatusEnt.Where(p => p.Status == 8 && p.SigningUserId != UserId).Select(w => w.Revision);
                var p2Except = p1.Where(k => k.StatusList.Where(p => p.Status > 9).Any()).AsEnumerable();
                return p1.Except(p2Except);
            }
            return db.AMRevisionEnt.Where(r => r.UnitId == UnitId).Where(k => k.StatusList.Where(p => p.Status < 2 && p.SigningUserId != UserId).Any()).AsEnumerable();
        }
        
       bool AddRevisionStatus(int RevId, int Status, string Description, int UserId)
        {
            AMUser CurrentUser = GetAmUserById(UserId);
            AMRevision CurrentRevision = GetRevisionById(RevId);
            if (CurrentUser == null || CurrentRevision == null) return false;
            try
            {
                db.AMRevisionStatusEnt.Add(new AMRevisionStatus { Description = Description, Revision = CurrentRevision, RevisionId = RevId, SigningUserId = UserId, User = CurrentUser, StatusDateTime = DateTime.Now, Status = Status });
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ConfirmRevision(int RevId, int UserId, int UnitId)
        {
            AMUser CurrentUser = GetAmUserById(UserId);
            AMRevision CurrentRevision = GetRevisionById(RevId);
            if (CurrentUser == null || CurrentRevision == null) return false;
            bool UnitAccess = CheckUserAccessKeyByUserId("REVISION_CONFIRM_UNIT", UserId.ToString());
            bool OrgAccess = CheckUserAccessKeyByUserId("REVISION_CONFIRM_ALL", UserId.ToString());
            bool IsInUnit = VerifyUserUnit(UnitId, UserId);
            if (!UnitAccess && !OrgAccess)
                return false;
            if (UnitAccess && IsInUnit && CurrentUser.UserTypeId == 4 && AddRevisionStatus(RevId, 2, "", UserId)) // Process Owner
                return true;
            if (UnitAccess && IsInUnit && CurrentUser.UserTypeId == 3 && AddRevisionStatus(RevId, 4, "", UserId)) // Unit Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 5 && AddRevisionStatus(RevId, 6, "", UserId)) // Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 6 && AddRevisionStatus(RevId, 8, "", UserId)) // Top Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 8 && AddRevisionStatus(RevId, 10, "", UserId)) // CEO
                return true;
            return false;
        }

        public bool RejectRevision(int RevId, int UserId, int UnitId)
        {
            AMUser CurrentUser = GetAmUserById(UserId);
            AMRevision CurrentRevision = GetRevisionById(RevId);
            if (CurrentUser == null || CurrentRevision == null) return false;
            bool UnitAccess = CheckUserAccessKeyByUserId("REVISION_CONFIRM_UNIT", UserId.ToString());
            bool OrgAccess = CheckUserAccessKeyByUserId("REVISION_CONFIRM_ALL", UserId.ToString());
            bool IsInUnit = VerifyUserUnit(UnitId, UserId);
            if (!UnitAccess && !OrgAccess)
                return false;
            if (UnitAccess && IsInUnit && CurrentUser.UserTypeId == 4 && AddRevisionStatus(RevId, 3, "", UserId)) // Process Owner
                return true;
            if (UnitAccess && IsInUnit && CurrentUser.UserTypeId == 3 && AddRevisionStatus(RevId, 5, "", UserId)) // Unit Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 5 && AddRevisionStatus(RevId, 7, "", UserId)) // Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 6 && AddRevisionStatus(RevId, 9, "", UserId)) // Top Manager
                return true;
            if (OrgAccess && CurrentUser.UserTypeId == 8 && AddRevisionStatus(RevId, 11, "", UserId)) // CEO
                return true;
            return false;
        }



        public IEnumerable<AMRevisionStatus> GetRevisionStatusListById(int RevisionId)
        {
            return db.AMRevisionStatusEnt.Where(m => m.RevisionId == RevisionId);
        }

        public AMRevision GetRevisionById (int RevId)
        {
            return db.AMRevisionEnt.Find(RevId);
        }

        /// <summary>
        /// Get All required parameters for a page having Unit, Process and Activity
        /// </summary>
        /// <returns></returns>
        public bool GetUPAwithNav(string UnitId, string ProcessId, string ActivityId, string UserId, out List<NavViewModel> Nav, out UnitProcessActObjectViewModel UPA)
        {
            int CurrentUnit = 0;
            int CurrrentProcess = 0;
            int CurrentActivity = 0;
            int CurrentUser = 0;
            Int32.TryParse(UnitId, out CurrentUnit);
            Int32.TryParse(ProcessId, out CurrrentProcess);
            Int32.TryParse(ActivityId, out CurrentActivity);
            Int32.TryParse(UserId, out CurrentUser);

            AMUnitOfOrg UnitObj = GetUnitById(CurrentUnit);
            AMProcess ProcessObj = GetProcessById(CurrrentProcess);
            AMActivity ActivityObj = GetActivityById(CurrentActivity);
            Nav = null;
            UPA = null;
            if (CurrentUnit == 0 || CurrrentProcess == 0 || CurrentActivity == 0 || CurrentUser == 0 || UnitObj == null || ProcessObj == null || ActivityObj == null || GetAmUserById(CurrentUser) == null || !VerifyUserUnit(CurrentUnit, CurrentUser))
                return false;

            Nav = GetMainNaviagtion(CurrentUnit);
            Nav = AddShowActivityNaviagtion(Nav, CurrrentProcess, ProcessObj.Name, CurrentUnit, UnitObj.Name, CurrentActivity, ActivityObj.Name);

            UPA = new UnitProcessActObjectViewModel { Unit = UnitObj, Process = ProcessObj, Activity = ActivityObj };

            return true;
        }


        /////////// Quality Tables Data Manipulation Layer

        public IEnumerable<AMQualityIndex> GetQualityIndexList()
        {
            return db.AMQualityIndexEnt;
        }

        public bool AddQualityIndex(AMQualityIndex IndexObj)
        {
            try
            {
                db.AMQualityIndexEnt.Add(IndexObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddQualityRule(AMQualityRule RuleObj)
        {
            try
            {
                db.AMQualityRuleEnt.Add(RuleObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AMQualityIndex GetQualityIndexById(int Id)
        {
            return db.AMQualityIndexEnt.Find(Id);
        }

        public bool EditQualityIndexByObject(AMQualityIndex Obj)
        {
            try
            {
                db.Entry(Obj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteQualityIndexByObject(AMQualityIndex Obj)
        {
            try
            {
                db.AMQualityIndexEnt.Remove(Obj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMQualityRule> GetQualityRuleListByActivityId(int ActivityId)
        {
            return db.AMQualityRuleEnt.Where(m => m.ActivityId == ActivityId);
        }

        public AMQualityRule GetQualityRuleById(int RuleId)
        {
            return db.AMQualityRuleEnt.Find(RuleId);
        }


        public bool DeleteQualityRuleByObject(AMQualityRule Obj)
        {
            try
            {
                db.AMQualityRuleEnt.Remove(Obj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddQualityMileStone(AMQualityMileStone MileStoneObj)
        {
            try
            {
                db.AMQualityMileStoneEnt.Add(MileStoneObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditQualityMileStone(AMQualityMileStone MileStoneObj)
        {
            try
            {
                db.Entry(MileStoneObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMQualityMileStone> GetQualityMileStoneList(int ActivityId, int UnitId)
        {
            return db.AMQualityMileStoneEnt.Where(m => m.Rule.ActivityId == ActivityId && m.UnitId == UnitId);
        }

        public AMQualityMileStone GetQualityMileStoneById(int? MileStoneId)
        {
            return db.AMQualityMileStoneEnt.Find(MileStoneId);
        }

        public AMQualityMileStone GetQualityMileStoneById(int? MileStoneId, int UnitId)
        {
            AMQualityMileStone MileStone = db.AMQualityMileStoneEnt.Find(MileStoneId);
            if (MileStone.UnitId == UnitId) return MileStone;
            return null;

        }

        public bool RegisterActivityData(RegisterActivityViewModel model)
        {
            AMRegisterActivity RegActObj = new AMRegisterActivity
            {
                ActivityId = model.ActivityId,
                Description = model.Description,
                RegisteringDate = DateTime.Now,
                UnitId = model.UnitId,
                UserId = model.UserId
            };

            try
            {
                db.AMRegisterActivityEnt.Add(RegActObj);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            AMActivityData ActDataObj = new AMActivityData
            {
                RuleId = model.RuleId,
                RegisteringActivityId = RegActObj.Id,
                ActivityData = model.ActivityData
            };
            try
            {
                db.AMActivityDataEnt.Add(ActDataObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MileStoneStatusViewModel> GetActivityQualityStatusList(int ActivityId)
        {
            List<MileStoneStatusViewModel> MileStoneList = new List<MileStoneStatusViewModel>();
            var ActivityMileStones = db.AMQualityMileStoneEnt.Where(m => m.Rule.ActivityId == ActivityId);
            foreach(var item in ActivityMileStones)
            {
                MileStoneStatusViewModel model = new MileStoneStatusViewModel{MileStone = item, IsAlive = true};

                if (item.ExpirationDate <= DateTime.Now)
                {
                    model.IsAlive = false;
                    model.RemainingTimePercentage = 0;
                    model.RemainingTimeStatus = item.ExpirationDate - item.RegistrationDate;
                }
                else
                {
                    model.RemainingTimeStatus = item.ExpirationDate - DateTime.Now;
                    model.RemainingTimePercentage = Convert.ToInt32((model.RemainingTimeStatus.TotalDays / (item.ExpirationDate - item.RegistrationDate).TotalDays)*100);
                }

                var RegisteredDataInDuration = db.AMActivityDataEnt.Where(d => d.RuleId == item.RuleId && d.RegisteringActivity.RegisteringDate > item.RegistrationDate && d.RegisteringActivity.RegisteringDate < item.ExpirationDate).Select(s => s.ActivityData);


                if (item.Rule.Index.EnumType == QualityIndexEnum.IncreasingPrice)
                {
                    model.GoalStatus = RegisteredDataInDuration.Sum();
                    model.GoalRequiredChange = item.Maximum - item.Minimum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = model.GoalStatus + item.Minimum;
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.DecreasingPrice)
                {
                    model.GoalStatus = RegisteredDataInDuration.Sum();
                    model.GoalRequiredChange = item.Minimum - item.Maximum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = item.Minimum - model.GoalStatus;
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.IncreasingAmount)
                {
                    model.GoalStatus = RegisteredDataInDuration.Count();
                    model.GoalRequiredChange = item.Maximum - item.Minimum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = model.GoalStatus + item.Minimum;
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.DecreasingAmount)
                {
                    model.GoalStatus = RegisteredDataInDuration.Count();
                    model.GoalRequiredChange = item.Minimum - item.Maximum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = item.Minimum - model.GoalStatus;
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.Score && item.Maximum >= item.Minimum)
                {
                    model.GoalStatus = RegisteredDataInDuration.Average();
                    model.GoalRequiredChange = item.Maximum - item.Minimum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = new decimal[] { model.GoalStatus, item.Minimum }.Average();
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.Score && item.Maximum < item.Minimum)
                {
                    model.GoalStatus = RegisteredDataInDuration.Average();
                    model.GoalRequiredChange = item.Minimum - item.Maximum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = new decimal[] { model.GoalStatus, item.Minimum }.Average();
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.TimeDuration && item.Maximum >= item.Minimum)
                {
                    model.GoalStatus = RegisteredDataInDuration.Average();
                    model.GoalRequiredChange = item.Maximum - item.Minimum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = new decimal[] { model.GoalStatus, item.Minimum }.Average();
                }
                else if (item.Rule.Index.EnumType == QualityIndexEnum.TimeDuration && item.Maximum < item.Minimum)
                {
                    model.GoalStatus = RegisteredDataInDuration.Average();
                    model.GoalRequiredChange = item.Minimum - item.Maximum;
                    model.GoalPercentage = Decimal.ToInt32((model.GoalStatus / (model.GoalRequiredChange)) * 100);
                    model.GoalStatus = new decimal[] { model.GoalStatus, item.Minimum }.Average();
                }
                if (model.GoalPercentage > 100) model.GoalPercentage = 100;
                MileStoneList.Add(model);
            }
            
            return MileStoneList.AsEnumerable();
            } 
        ///// Customer ////
        public bool AddNewCustomer(AMCustomer CustomerObj)
        {
            try
            {
                CustomerObj.CustomerNumber = AddZero(CustomerObj.CustomerNumber, 10);
                db.AMCustomerEnt.Add(CustomerObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddNewAccount(AMAccount AccountObj, AMCustomer CustomerObj)
        {
            try
            {
                string str1 = AccountObj.AccountNumber.Substring(0, 2);
                string str2 = AddZero(AccountObj.AccountNumber.Substring(2), 11);
                AccountObj.AccountNumber = str1 + str2;
                db.AMAccountEnt.Add(AccountObj);
                db.SaveChanges();

                db.AMCustomerAccountEnt.Add(new AMCustomerAccount { AccountId = AccountObj.Id, CustomerId = CustomerObj.Id });
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddNewLoan(LoanRegisterationViewModel LoanRegObj, AMCustomer CustomerObj)
        {
            try
            {
                string str1 = LoanRegObj.LoanNumber.Substring(0, 2);
                string str2 = AddZero(LoanRegObj.LoanNumber.Substring(2), 11);
                AMLoan LoanObj = new AMLoan { LoanNumber = str1 + str2, LoanType = LoanRegObj.LoanType, LoanDate = LoanRegObj.LoanDate, LoanTotalAmount = LoanRegObj.LoanTotalAmount, UnitId = LoanRegObj.UnitId };
                db.AMLoanEnt.Add(LoanObj);
                db.SaveChanges();

                db.AMCustomerLoanEnt.Add(new AMCustomerLoan { LoantId = LoanObj.Id, CustomerId = CustomerObj.Id });
                db.SaveChanges();

                if(InsertInstallments(LoanObj, LoanRegObj.InstallmentDuration, LoanRegObj.TotalInstallments))
                return true;
            }
            catch(Exception e1)
            {
                string message = e1.Message;
                return false;
            }
            return false;
        }

        public AMLoan GetLoanById(int? LoanId)
        {
            return db.AMLoanEnt.Find(LoanId);
        }

        public bool InsertInstallments(AMLoan LoanObj, MonthNumbers Duration, int TotalInstallments)
        {
            try
            {
                decimal InstallmentPrice = LoanObj.LoanTotalAmount / TotalInstallments;
                int d = 1;
                for (int k = 1; k <= TotalInstallments; k++)
                {
                    d = (int)Duration * k;
                    DateTime dt = LoanObj.LoanDate.AddMonths(d);
                    AMInstallment InstallmentObj = new AMInstallment { LoanId = LoanObj.Id, IndexNumber = k, Status = InstallmentStatus.Unpaid, InstallmentAmount = InstallmentPrice, DueDate = dt , PaymentDate = new DateTime(1800, 1, 1, 1 , 1, 1)};
                    db.AMInstallmentEnt.Add(InstallmentObj);
                }
                db.SaveChanges();
                return true;
            }
            catch(Exception e1)
            {
                string message = e1.Message;
                return false;
            }

        }


        public IEnumerable<AMCustomer> GetCustomerByCustomerNumber(string CustomerNumber)
        {
            CustomerNumber = AddZero(CustomerNumber, 10);
            return db.AMCustomerEnt.Where(m => m.CustomerNumber == CustomerNumber);
        }

        public IEnumerable<AMLoan> GetLoanByLoanNumber(string CustomerNumber)
        {
            string str1 = CustomerNumber.Substring(0, 2);
            string str2 = AddZero(CustomerNumber.Substring(2), 11);
            return db.AMLoanEnt.Where(m => m.LoanNumber == str1 + str2);
        }

        public IEnumerable<AMCustomer> GetOtherCustomersByCustomerNumber(string CustomerNumber, int ThisCustomerId)
        {
            CustomerNumber = AddZero(CustomerNumber, 10);
            return db.AMCustomerEnt.Where(m => m.CustomerNumber == CustomerNumber && m.Id != ThisCustomerId);
        }



        public AMCustomer GetCustomerById(int? CustomerId)
        {
            return db.AMCustomerEnt.Find(CustomerId);
        }

        public bool EditCustomer(AMCustomer CustomerObj)
        {
            try
            {
                CustomerObj.CustomerNumber = AddZero(CustomerObj.CustomerNumber, 10);
                db.Entry(CustomerObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AMAccount GetAccountById(int? Id)
        {
            return db.AMAccountEnt.Find(Id);
        }

        public bool EditAccount(AMAccount AccountObj)
        {
            try
            {
                string str1 = AccountObj.AccountNumber.Substring(0, 2);
                string str2 = AddZero(AccountObj.AccountNumber.Substring(2), 11);
                AccountObj.AccountNumber = str1 + str2;
                db.Entry(AccountObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditLoan(AMLoan LoanObj)
        {
            try
            {
                string str1 = LoanObj.LoanNumber.Substring(0, 2);
                string str2 = AddZero(LoanObj.LoanNumber.Substring(2), 11);
                LoanObj.LoanNumber = str1 + str2;
                db.Entry(LoanObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAccount(AMAccount AccountObj)
        {
            try
            {
                db.AMAccountEnt.Remove(AccountObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteLoan(AMLoan LoanObj)
        {
            try
            {
                db.AMLoanEnt.Remove(LoanObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string AddZero(string Number, int RequiredLength)
        {
            if (Number == null)
                return null;

            if (Number.Length < RequiredLength)
            {
                for (int k = Number.Length; k < RequiredLength; k++)
                    Number = "0" + Number;
                return Number;
            }
            else if (Number.Length > RequiredLength)
            {
                return Number.Substring(0, RequiredLength);
            }
            else return Number;
        }

        public AMInstallment GetInstallmentById(int? Id)
        {
            return db.AMInstallmentEnt.Find(Id);
        }

        public IEnumerable<AMInstallment> SetInstallmentsPaid(AMInstallment InstObj, AMUser CurrentUser)
        {
            var items = db.AMInstallmentEnt.Where(m => (m.IndexNumber < InstObj.IndexNumber && m.LoanId == InstObj.LoanId) || m.Id == InstObj.Id);
            foreach (var k in items)
            {
                k.PaymentDate = DateTime.Now;
                k.Status = InstallmentStatus.Paid;
                db.Entry(k).State = EntityState.Modified;
                db.AMInstallmentUserLogEnt.Add(new AMInstallmentUserLog { InstallmentId = k.Id, UserId = CurrentUser.Id, CreationDateTime = k.PaymentDate, Action = InstallmentAction.SetAsPaid });
            }
            db.SaveChanges();
            return items;
        }

        public int[] SetInstallmentsUnPaid(AMInstallment InstObj, AMUser CurrentUser)
        {
            var items = db.AMInstallmentEnt.Where(m => (m.IndexNumber > InstObj.IndexNumber && m.LoanId == InstObj.LoanId && m.Status == InstallmentStatus.Paid) || m.Id == InstObj.Id);
            int[] list = items.Select(m => m.Id).ToArray();
            foreach (var k in items)
            {
                k.PaymentDate = DateTime.Now;
                k.Status = InstallmentStatus.Unpaid;
                db.Entry(k).State = EntityState.Modified;
                db.AMInstallmentUserLogEnt.Add(new AMInstallmentUserLog { InstallmentId = k.Id, UserId = CurrentUser.Id, CreationDateTime = k.PaymentDate, Action = InstallmentAction.SetAsUnpaid });
            }
            db.SaveChanges();
            return list;
        }



        public bool SetInstallmentNotification(AMInstallmentNotification NotificationObj, AMUser CurrentUser)
        {
            try
            {
                db.AMInstallmentNotificationEnt.Add(NotificationObj);
                db.SaveChanges();
                db.AMInstallmentNotificationUserLogEnt.Add(new AMInstallmentNotificationUserLog { InstallmentNotificationId = NotificationObj.Id, UserId = CurrentUser.Id, CreationDateTime = DateTime.Now, Action = InstallmentNotificationAction.Create });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetCallNotification(AMCallNotification NotificationObj)
        {
            try
            {
                db.AMCallNotificationEnt.Add(NotificationObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AMCustomerAddress GetAddressById(int? id)
        {
            return db.AMCustomerAddressEnt.Find(id);
        }

        public AMCall GetCallById(int? id)
        {
            return db.AMCallEnt.Find(id);
        }

        public AMCall SetCallInformation(AMCall CallObj, out bool res)
        {
            try
            {
                db.AMCallEnt.Add(CallObj);
                db.SaveChanges();
                res = true;
                return CallObj;
            }
            catch
            {
                res = false;
                return CallObj;
            }
        }

        public IEnumerable<AMInstallmentNotification> GetLoansWithInstallmentNotification(int UnitId)
        {
            return db.AMInstallmentNotificationEnt.Where(m => m.Installment.Loan.UnitId == UnitId && DbFunctions.TruncateTime(m.DueDate) <= DbFunctions.TruncateTime(DateTime.Now) && m.Status == NotificationStatus.Unseen);
        }

        public IEnumerable<AMCallNotification> GetLoansWithCallNotification(int UnitId)
        {
            return db.AMCallNotificationEnt.Where(m => m.Call.Installment.Loan.UnitId == UnitId && DbFunctions.TruncateTime(m.DueDate) <= DbFunctions.TruncateTime(DateTime.Now) && m.Status == NotificationStatus.Unseen);
        }

        public IEnumerable<AMInstallment> GetLoansWithTodayOverDueDate(int UnitId)
        {
            return db.AMInstallmentEnt.Where(m => m.Loan.UnitId == UnitId && m.Status == InstallmentStatus.Unpaid && DbFunctions.TruncateTime(m.DueDate) == DbFunctions.TruncateTime(DateTime.Now)).GroupBy(k => k.LoanId).Select(m => m.FirstOrDefault());
        }

        public IEnumerable<AMInstallment> GetLoansWithWeekOverDueDate(int UnitId)
        {
            DateTime Date1 = DateTime.Now.AddDays(-1);
            DateTime Date2 = DateTime.Now.AddDays(-8);
            return db.AMInstallmentEnt.Where(k => k.Loan.UnitId == UnitId && k.Status == InstallmentStatus.Unpaid && DbFunctions.TruncateTime(k.DueDate) < DbFunctions.TruncateTime(Date1) && DbFunctions.TruncateTime(k.DueDate) > DbFunctions.TruncateTime(Date2)).GroupBy(k => k.LoanId).Select(m => m.FirstOrDefault()).Take(100);
        }

        public IEnumerable<AMInstallment> GetLoansWithMonthOverDueDate(int UnitId)
        {
            DateTime Date1 = DateTime.Now.AddDays(-7);
            DateTime Date2 = DateTime.Now.AddDays(-30);
            return db.AMInstallmentEnt.Where(k => k.Loan.UnitId == UnitId &&  k.Status == InstallmentStatus.Unpaid && DbFunctions.TruncateTime(k.DueDate) < DbFunctions.TruncateTime(Date1) && DbFunctions.TruncateTime(k.DueDate) > DbFunctions.TruncateTime(Date2)).GroupBy(k => k.LoanId).Select(m => m.FirstOrDefault()).Take(100);
        }

        public IEnumerable<AMInstallment> GetLoansWithTwoMonthsOverDueDate(int UnitId)
        {
            DateTime Date1 = DateTime.Now.AddDays(-29);
            DateTime Date2 = DateTime.Now.AddDays(-60);
            return db.AMInstallmentEnt.Where(k => k.Loan.UnitId == UnitId && k.Status == InstallmentStatus.Unpaid && DbFunctions.TruncateTime(k.DueDate) < DbFunctions.TruncateTime(Date1) && DbFunctions.TruncateTime(k.DueDate) > DbFunctions.TruncateTime(Date2)).GroupBy(k => k.LoanId).Select(m => m.FirstOrDefault()).Take(100);
        }

        public IEnumerable<AMInstallment> GetLoansWithMoreThanTwoMonthsOverDueDate(int UnitId)
        {
            DateTime Date1 = DateTime.Now.AddDays(-1);
            DateTime Date2 = DateTime.Now.AddDays(-59);
            return db.AMInstallmentEnt.Where(k => k.Loan.UnitId == UnitId && k.Status == InstallmentStatus.Unpaid && DbFunctions.TruncateTime(k.DueDate) < DbFunctions.TruncateTime(Date2)).GroupBy(k => k.LoanId).Select(m => m.FirstOrDefault()).Take(100);
        }

        public IEnumerable<AMCustomer> GetCustomersByLoanId(int? LoanId)
        {
            return db.AMCustomerEnt.Where(m => m.LoanList.Any(k => k.LoantId == LoanId));
        }

        public AMInstallmentNotification GetInstallmentNotificationById(int? id)
        {
            return db.AMInstallmentNotificationEnt.Find(id);
        }

        public bool SetInstallmentNotificationSeen(AMInstallmentNotification alert, AMUser CurrentUser)
        {
            try
            {
                alert.Status = NotificationStatus.Seen;
                db.Entry(alert).State = EntityState.Modified;
                db.AMInstallmentNotificationUserLogEnt.Add(new AMInstallmentNotificationUserLog { InstallmentNotificationId = alert.Id, UserId = CurrentUser.Id, CreationDateTime = DateTime.Now, Action = InstallmentNotificationAction.MakeAsRead });
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMCall> GetUserCallLog(AMUser CurrentUser)
        {
            return db.AMCallEnt.Where(m => m.UserId == CurrentUser.Id);
        }

        public IEnumerable<AMInstallmentNotificationUserLog> GetUserInstallmentNotification(AMUser CurrentUser)
        {
            return db.AMInstallmentNotificationUserLogEnt.Where(m => m.UserId == CurrentUser.Id && m.Action == InstallmentNotificationAction.Create);
        }

        public IEnumerable<AMInstallmentNotificationUserLog> GetUserInstallmentNotificationDone(AMUser CurrentUser)
        {
            return db.AMInstallmentNotificationUserLogEnt.Where(m => m.UserId == CurrentUser.Id && m.Action == InstallmentNotificationAction.MakeAsRead);
        }

        public IEnumerable<AMInstallmentUserLog> GetUserInstallmentStatusLog(AMUser CurrentUser)
        {
            return db.AMInstallmentUserLogEnt.Where(m => m.UserId == CurrentUser.Id);
        }

        public IEnumerable<AMUpdateLoanLog> GetUserLoanUpdateLog(AMUser CurrentUser)
        {
            return db.AMUpdateLoanLogEnt.Where(m => m.UserId == CurrentUser.Id);
        }

        public int GetUserFollowUpScore(AMUser CurrentUser, AMUnitOfOrg CurrentUnit)
        {
            return 0;
        }

        public bool AddNewAddress(AMCustomerAddress AddressObj)
        {
            try
            {
                
                db.AMCustomerAddressEnt.Add(AddressObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditAddress(AMCustomerAddress AddressObj)
        {
            try
            {
                db.Entry(AddressObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAddress(AMCustomerAddress AddressObj)
        {
            try
            {
                db.AMCustomerAddressEnt.Remove(AddressObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AMReferee AddNewReferee(AMReferee RefereeObj, out bool res)
        {
            try
            {
                db.AMRefereeEnt.Add(RefereeObj);
                db.SaveChanges();
                res = true;
                return RefereeObj;
            }
            catch
            {
                res = false;
                return RefereeObj;
            }
        }

        public AMUpdateLoanLog AddUpdateLoanLog(AMUpdateLoanLog LogObj, out bool res)
        {
            try
            {
                db.AMUpdateLoanLogEnt.Add(LogObj);
                db.SaveChanges();
                res = true;
                return LogObj;
            }
            catch
            {
                res = false;
                return LogObj;
            }
        }

        public AMUpdateLoanLog AddStartFollowUpLoanLog(AMUpdateLoanLog LogObj, out bool res)
        {
            try
            {
                db.AMUpdateLoanLogEnt.Add(LogObj);
                db.SaveChanges();
                res = true;
                return LogObj;
            }
            catch
            {
                res = false;
                return LogObj;
            }
        }

        public AMReferee GetRefereeById(int? id)
        {
            return db.AMRefereeEnt.Find(id);
        }

        public bool RemoveReferee(AMReferee RefereeObj)
        {
            try
            {
                db.AMRefereeEnt.Remove(RefereeObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /////// Conversion From Mizan To BSI

        public IEnumerable<AMCustomerRecordMizan> GetCustomerConversionFailed(int page, int rows)
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Failed).Skip((page - 1) * rows).Take(rows);
        }

        public IEnumerable<AMLoanRecordMizan> GetLoanConversionFailed(int page, int rows)
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus != ConversionResultStatus.Success || m.ConversionStatus != ConversionResultStatus.NotConverted || m.ConversionStatus != ConversionResultStatus.Warning).Skip((page - 1) * rows).Take(rows);
        }



        public IEnumerable<AMCustomerRecordMizan> GetCustomerConversionWarning(int page, int rows)
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Warning).Skip((page - 1) * rows).Take(rows);
        }

        public IEnumerable<AMLoanRecordMizan> GetLoanConversionWarning(int page, int rows)
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Warning).Skip((page - 1) * rows).Take(rows);
        }

        public int GetMizanCustomerTotalCount()
        {
            return db.AMCustomerRecordMizanEnt.Count();
        }
        public int GetMizanCustomerSuccessConvertCount()
        {
            return db.AMCustomerRecordMizanEnt.Where(m=>m.ConversionStatus == ConversionResultStatus.Success) .Count();
        }

        public int GetMizanCustomerFailedConvertCount()
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Failed).Count();
        }

        public int GetMizanCustomerWarningConvertCount()
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Warning).Count();
        }

        public int GetMizanCustomerUnConvertedCount()
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.NotConverted).Count();
        }

        public int GetLoanConvertTotalCount()
        {
            return db.AMLoanRecordMizanEnt.Count();
        }
        public int GetMizanLaonSuccessConvertCount()
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Success).Count();
        }

        public int GetMizanLoanWarningConvertCount()
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.Warning).Count();
        }

        public int GetMizanLoanUnConvertedCount()
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.NotConverted).Count();
        }

        public int GetMizanLoanFailedConvertCount()
        {
            return db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus != ConversionResultStatus.Success).Where(m=> m.ConversionStatus != ConversionResultStatus.NotConverted).Where(m => m.ConversionStatus != ConversionResultStatus.Warning).Count();
        }

        public LoanConvertFailureInfoViewModel GetConversionStatistics()
        {
            LoanConvertFailureInfoViewModel model = new LoanConvertFailureInfoViewModel();
            // Just for Branch Code 15
            var k = db.AMLoanRecordMizanEnt.Where(m=>m.BSIBranchCode == 15).Where(m => m.ConversionStatus != ConversionResultStatus.Success).Where(m => m.ConversionStatus != ConversionResultStatus.NotConverted).Where(m => m.ConversionStatus != ConversionResultStatus.Warning);

            // Uncomment for all branches
            //var k = db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus != ConversionResultStatus.Success).Where(m => m.ConversionStatus != ConversionResultStatus.NotConverted).Where(m => m.ConversionStatus != ConversionResultStatus.Warning);

            model.AlreadyExist = k.Count(m => m.ConversionStatus == ConversionResultStatus.AlreadyExist);
            model.BSIBranchError = k.Count(m => m.ConversionStatus == ConversionResultStatus.BSIBranchError);
            model.DebtorError = k.Count(m => m.ConversionStatus == ConversionResultStatus.DebtorError);
            model.DebtorRefereeError = k.Count(m => m.ConversionStatus == ConversionResultStatus.DebtorRefereeError);
            model.Failure = k.Count(m => m.ConversionStatus == ConversionResultStatus.Failed);
            model.InstallmentCountError = k.Count(m => m.ConversionStatus == ConversionResultStatus.InstallmentCountError);
            model.InstallmentError = k.Count(m => m.ConversionStatus == ConversionResultStatus.InstallmentError);
            model.InstallmentPeriodError = k.Count(m => m.ConversionStatus == ConversionResultStatus.InstallmentPeriodError);
            model.LoanDateError = k.Count(m => m.ConversionStatus == ConversionResultStatus.LoanDateError);
            model.MizanLoanTypeError = k.Count(m => m.ConversionStatus == ConversionResultStatus.MizanLoanTypeError);

            model.TotalFailed = k.Count();

            return model;
        }
        private bool UpdateMizanLoanRecord(AMLoanRecordMizan loanObj)
        {
            try
            {
                db.Entry(loanObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch(Exception e1)
            {
                string message = e1.Message;
                string message2 = e1.InnerException.Message;
                return false;
            }
        }
        //Saba = قدیمی
        private IEnumerable<AMCustomerRecordMizan> GetMizanCustomerBySabaNumber(string SabaNumber, int BranchCode)
        {
            var k = db.AMCustomerRecordMizanEnt.Where(m => m.LoanNumber.Trim() + BranchCode.ToString() == SabaNumber.Trim());
            return k;
        }

        //Simia = جدید
        private IEnumerable<AMCustomerRecordMizan> GetMizanCustomerBySimiaNumber(string SimiaNumber)
        {
            return db.AMCustomerRecordMizanEnt.Where(m => m.LoanNumber.Trim().Substring(0, m.LoanNumber.Length - 1) == SimiaNumber.Trim());
        }

        private int ComputeStringDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        private AMCustomer GetPersonalCustomerByNationalCode(string NationalCode, string PersonalId)
        {
            var CustomerPersonalList = db.AMCustomerPersonalEnt.Where(m => m.NationalCode == NationalCode.Trim() && m.PersonalId == PersonalId);
            return CustomerPersonalList.Count() > 0 ? CustomerPersonalList.FirstOrDefault().Customer:null;    
        }

        private AMCustomer GetCorporationCustomerByNationalCode(string NationalCode)
        {
            var CorporationList = db.AMCorporationMemberEnt.Where(m => m.Corporation.NationalCode == NationalCode && m.MembershipType == CorporationMemberType.AsMain);
            return CorporationList.Count() > 0 ? CorporationList.FirstOrDefault().Customer : null;
        }
        
        private AMCustomerPersonal AddNewCustomerPersonalInfo(AMCustomerPersonal PersonalObj)
        {
            try
            {
                db.AMCustomerPersonalEnt.Add(PersonalObj);
                db.SaveChanges();
                return PersonalObj;
            }
            catch
            {
                return null;
            }
        }

        private AMCorporation AddNewCorporationInfo(AMCorporation CorporationObj)
        {
            try
            {
                db.AMCorporationEnt.Add(CorporationObj);
                db.SaveChanges();
                return CorporationObj;
            }
            catch
            {
                return null;
            }
        }

        private AMMizanCustomerConversion AddNewMizanCustomerConversionInfo(AMMizanCustomerConversion ConversionObj)
        {
            try
            {
                db.AMMizanCustomerConversionEnt.Add(ConversionObj);
                db.SaveChanges();
                return ConversionObj;
            }
            catch
            {
                return null;
            }
        }

        private AMCustomer AddNewCustomerAndReturn(AMCustomer CustomerObj)
        {
            try
            {
                CustomerObj.CustomerNumber = AddZero(CustomerObj.CustomerNumber, 10);
                db.AMCustomerEnt.Add(CustomerObj);
                db.SaveChanges();
                return CustomerObj;
            }
            catch(Exception e1)
            {
                string message = e1.Message;
                return null;
            }
        }
        private PhoneType GetPhoneTypeFromPhoneNumber(string PhoneNumber)
        {
            if (PhoneNumber.Substring(0, 2) == "09")
                return PhoneType.Mobile;
            return PhoneType.LandLine;
        }

        private bool UpdateMizanCustomerRecord(AMCustomerRecordMizan MizanCustomerObj)
        {
            try
            {
                db.Entry(MizanCustomerObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private AMCustomer AddMizanCustomerToDB(AMCustomerRecordMizan MizanCustomer)
        {
            try
            {
                AMCustomer CustomerPersonalByNationalCodeAndId;

                if (MizanCustomer.IsCorporation == CustomerTypePersonalCorporation.Corporation) // 
                {
                    CustomerPersonalByNationalCodeAndId = GetCorporationCustomerByNationalCode(MizanCustomer.NationalCode);
                }
                else // Customer Type is Personal
                {
                    CustomerPersonalByNationalCodeAndId = GetPersonalCustomerByNationalCode(MizanCustomer.NationalCode, MizanCustomer.PersonalId);
                }
                if (CustomerPersonalByNationalCodeAndId == null) //National Code and PersonalId doesn't exist
                {
                    AMCustomer Customer = new AMCustomer();
                    Customer.CustomerNumber = "88" + AddZero(MizanCustomer.Id.ToString(), 8);
                    Customer.CustomerType = MizanCustomer.IsCorporation == CustomerTypePersonalCorporation.Corporation ? CustomerType.Corporation : CustomerType.Person;
                    Customer.FirstName = MizanCustomer.FirstName;
                    Customer.Lastname = MizanCustomer.LastName;


                    db.AMCustomerEnt.Add(Customer);
                    db.SaveChanges();

                    if (Customer.Id > 0)
                    {
                        if (MizanCustomer.IsCorporation == CustomerTypePersonalCorporation.Person)
                        {
                            AddNewCustomerPersonalInfo(new AMCustomerPersonal { CustomerId = Customer.Id, NationalCode = MizanCustomer.NationalCode, PersonalId = MizanCustomer.PersonalId });
                        }
                        else
                        {
                            AddNewCorporationInfo(new AMCorporation { NationalCode = MizanCustomer.NationalCode });
                        }
                        AddNewMizanCustomerConversionInfo(new AMMizanCustomerConversion { CustomerId = Customer.Id, MizanCustomerId = MizanCustomer.Id });
                        MizanCustomer.ConversionStatus = ConversionResultStatus.Success;
                        MizanCustomer.ConversionMessage = "به عنوان مشتری جدید با شماره  " + Customer.CustomerNumber + " ثبت شد";
                        if (MizanCustomer.PhoneNumber1 != null &&  MizanCustomer.PhoneNumber1.Length > 5) AddNewAddress(new AMCustomerAddress { CustomerId = Customer.Id, PhoneNumber = MizanCustomer.PhoneNumber1, Address = MizanCustomer.Address, PhoneType = GetPhoneTypeFromPhoneNumber(MizanCustomer.PhoneNumber1) });
                        if (MizanCustomer.PhoneNumber2 != null && MizanCustomer.PhoneNumber2.Length > 5) AddNewAddress(new AMCustomerAddress { CustomerId = Customer.Id, PhoneNumber = MizanCustomer.PhoneNumber2, Address = MizanCustomer.Address, PhoneType = GetPhoneTypeFromPhoneNumber(MizanCustomer.PhoneNumber2) });
                        if (MizanCustomer.PhoneNumber3 != null && MizanCustomer.PhoneNumber3.Length > 5) AddNewAddress(new AMCustomerAddress { CustomerId = Customer.Id, PhoneNumber = MizanCustomer.PhoneNumber3, Address = MizanCustomer.Address, PhoneType = GetPhoneTypeFromPhoneNumber(MizanCustomer.PhoneNumber3) });
                        UpdateMizanCustomerRecord(MizanCustomer);
                        return Customer;
                    }
                    MizanCustomer.ConversionStatus = ConversionResultStatus.Failed;
                    MizanCustomer.ConversionMessage = "در قسمت ایجاد مشتری جدید با خطا مواجه شد";
                    UpdateMizanCustomerRecord(MizanCustomer);
                    return null;
                }
                MizanCustomer.ConversionStatus = ConversionResultStatus.AlreadyExist;
                MizanCustomer.ConversionMessage = "اطلاعات مشتری به شماره   " + CustomerPersonalByNationalCodeAndId.CustomerNumber + "موجود بود";
                UpdateMizanCustomerRecord(MizanCustomer);
                return CustomerPersonalByNationalCodeAndId;
            }
            catch(Exception e1)
            {
                MizanCustomer.ConversionStatus = ConversionResultStatus.Failed;
                MizanCustomer.ConversionMessage = "در قسمت ایجاد مشتری خطای ناشناخته رخ داد: " + e1.Message;
                UpdateMizanCustomerRecord(MizanCustomer);
                return null;
            }
            }

        private DateTime ConvertStringToDatetime(string StringPersianDate)
        {
            var k = StringPersianDate.Split('/');
            int yr = 0;
            int mnth = 0;
            int day = 0;

            Int32.TryParse(k[0], out yr);
            if (yr == 0) return DateTime.MinValue;

            Int32.TryParse(k[1], out mnth);
            if (mnth == 0) return DateTime.MinValue;

            Int32.TryParse(k[2], out day);
            if (day == 0) return DateTime.MinValue;

            if(yr == 1370 && mnth == 1 && day == 1) return DateTime.MinValue;

            System.Globalization.PersianCalendar Taghvim = new System.Globalization.PersianCalendar();
            return Taghvim.ToDateTime(yr, mnth, day, 1, 1, 1, 1);
        }

        private LoanType GetLoanTypeFromTypeNumberMizan(int TypeNumber)
        {
            if (TypeNumber == 1)
                return LoanType.Mizan;
            LoanType lt = LoanType.Mizan;
            try
            {
                lt = (LoanType)TypeNumber;
                return lt;
            }
            catch
            {
                return lt;
            }
        } 

        private AMLoan AddNewLoanFromMizan(AMLoan LoanObj, AMCustomer Debtor)
        {
            try
            {
                db.AMLoanEnt.Add(LoanObj);
                db.SaveChanges();

                db.AMCustomerLoanEnt.Add(new AMCustomerLoan { CustomerId = Debtor.Id, LoantId = LoanObj.Id });
                db.SaveChanges();
                return LoanObj;
            }
            catch
            {
                return null;
            }
        }

        private bool CheckExtraInfoExistMizan(LoanExtraInfoType Type, string Value)
        {
            var k = db.AMLoanExtraInfoEnt.Where(m => m.ValueType == Type && m.Value == Value);
            if (k.Count() == 0)
                return false;
            return true;
        }

        private bool AddLoanExtraInfoFromMizan(AMLoanExtraInfo ExtraInfObj)
        {
            try
            {
                db.AMLoanExtraInfoEnt.Add(ExtraInfObj);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool AddMizanLoanConversionRecord(AMLoan NewLoan, AMLoanRecordMizan MizanLoan)
        {
            try
            {
                db.AMMizanLoanConversionEnt.Add(new AMMizanLoanConversion { LoanId = NewLoan.Id, MizanLoanId = MizanLoan.Id });
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool InsertInstallmentsMizan(AMLoan LoanObj, int DailyPeriod, int TotalInstallments)
        {
            try
            {
                decimal InstallmentPrice = LoanObj.LoanTotalAmount / TotalInstallments;
                int d = 1;
                for (int k = 1; k <= TotalInstallments; k++)
                {
                    d = DailyPeriod * k;
                    DateTime dt = LoanObj.LoanDate.AddDays(d);
                    AMInstallment InstallmentObj = new AMInstallment { LoanId = LoanObj.Id, IndexNumber = k, Status = InstallmentStatus.Unpaid, InstallmentAmount = InstallmentPrice, DueDate = dt, PaymentDate = new DateTime(1800, 1, 1, 1, 1, 1) };
                    db.AMInstallmentEnt.Add(InstallmentObj);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception e1)
            {
                string message = e1.Message;
                return false;
            }
        }

        public bool DoMizanConvert(out string Message)
        {
            int counter = 0;
            //Just For Branch Code 15
            var MizanLoanList = db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.NotConverted && m.BSIBranchCode == 15).Take(100);
            // Uncomment for All branches 
            //var MizanLoanList = db.AMLoanRecordMizanEnt.Where(m => m.ConversionStatus == ConversionResultStatus.NotConverted);

            IList<AMLoanRecordMizan> BigList = MizanLoanList.ToList();
            foreach (var MizanRow in BigList)
            {
                try
                {
                    counter++;
                    //if((counter % 50000) == 0)
                    //{
                    //    step++;
                    //}
                    string WarningMessage = "";
                    var BSIUnitList = db.AMUnitEnt.Where(m => m.IdentityCode.Trim() == MizanRow.BSIBranchCode.ToString());
                    if (BSIUnitList.Count() == 0)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.BSIBranchError;
                        EditableLoan.ConversionMessage = "شعبه بانک صادرات موجود نبود";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    AMUnitOfOrg BsiUnit = BSIUnitList.FirstOrDefault();
                    LoanExtraInfoType LoanType = LoanExtraInfoType.ContractNumber; // Just For Checking
                    string ExtraInfoValue = "";
                    if (MizanRow.SimiaNumber.Trim().Length == 16) // Simia
                    {
                        LoanType = LoanExtraInfoType.SimiaMizan;
                    }
                    else if (MizanRow.SimiaNumber.Trim().Length == 0 && MizanRow.SabaNumber.Trim().Length > 2) // Saba
                    {
                        LoanType = LoanExtraInfoType.SabaMizan;
                    }
                    if (LoanType == LoanExtraInfoType.ContractNumber)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.MizanLoanTypeError;
                        EditableLoan.ConversionMessage = "نوع تسهیلات شناسایی نشد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }

                    if (MizanRow.TotalInstallmentCount < 1)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.InstallmentCountError;
                        EditableLoan.ConversionMessage = "تعداد اقساط کمتر از حد انتظار";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }

                    if (MizanRow.TotalInstallmentCount > 60)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.InstallmentCountError;
                        EditableLoan.ConversionMessage = "تعداد اقساط بیشتر از حد انتظار";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }


                    if (MizanRow.InstallmentPeriod < 10)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.InstallmentPeriodError;
                        EditableLoan.ConversionMessage = "فاصله اقساط کمتر از حد انتظار";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }


                    bool LoanNumberExist = false;
                    if (LoanType == LoanExtraInfoType.SabaMizan)
                    {
                        ExtraInfoValue = MizanRow.MizanBranchCode + MizanRow.SabaNumber;
                        LoanNumberExist = CheckExtraInfoExistMizan(LoanType, MizanRow.MizanBranchCode + MizanRow.SabaNumber);
                    }
                    else
                    {
                        ExtraInfoValue = MizanRow.SimiaNumber.Substring(0, MizanRow.SimiaNumber.Length - 1);
                        LoanNumberExist = CheckExtraInfoExistMizan(LoanType, MizanRow.SimiaNumber);
                    }

                    if (LoanNumberExist)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.Failed;
                        EditableLoan.ConversionMessage = "شماره قرارداد تکراری است.";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    IEnumerable<AMCustomerRecordMizan> LoanCustomerList = null;

                    if (LoanType == LoanExtraInfoType.SabaMizan)
                    {
                        LoanCustomerList = GetMizanCustomerBySabaNumber(MizanRow.SabaNumber, MizanRow.MizanBranchCode);
                    }
                    else
                    {
                        LoanCustomerList = GetMizanCustomerBySimiaNumber(MizanRow.SimiaNumber.Substring(0, MizanRow.SimiaNumber.Length - 1));
                    }

                    if (LoanCustomerList.Count() == 0)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.DebtorRefereeError;
                        EditableLoan.ConversionMessage = "هیچ متعهد یا ضامنی با این شماره تسهیلات پیدا نشد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    var DebtorList = LoanCustomerList.Where(m => m.CustomerType == MizanLoanCustomerType.Debtor);

                    if (DebtorList.Count() > 1)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.DebtorError;
                        EditableLoan.ConversionMessage = "برای این تسهیلات بیشتر از یک نفر به عنوان متعهد پیدا شد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    if (DebtorList.Count() == 0)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.DebtorError;
                        EditableLoan.ConversionMessage = "برای این تسهیلات هیچ متعهدی پیدا نشد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    AMCustomerRecordMizan Debtor = DebtorList.FirstOrDefault();

                    AMCustomer CustomerDebtor = AddMizanCustomerToDB(Debtor);
                    if (CustomerDebtor == null)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.Failed;
                        EditableLoan.ConversionMessage = "افزودن مشتری میزان به پایگاه داده ها انجام نشد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }


                    ////////// Here
                    DateTime NewLoanDate = ConvertStringToDatetime(MizanRow.PaymentDate);
                    if (NewLoanDate == DateTime.MinValue)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.LoanDateError;
                        EditableLoan.ConversionMessage = "تاریخ اعطای تسهیلات نادرست است";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }
                    string createdLoanNumber = "98" + AddZero(MizanRow.Id.ToString(), 11);
                    var BsiLoanListFromNewLoanNumber = db.AMLoanEnt.Where(m => m.LoanNumber == createdLoanNumber);
                    if (BsiLoanListFromNewLoanNumber.Count() > 0)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.Failed;
                        EditableLoan.ConversionMessage = "این سطر جدول قبلا تبدیل شده است.";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }

                    AMLoan NewLoan = new AMLoan();
                    NewLoan.UnitId = BsiUnit.Id;
                    NewLoan.LoanNumber = "98" + AddZero(MizanRow.Id.ToString(), 11);
                    NewLoan.LoanTotalAmount = MizanRow.LoanTotalAmount;
                    NewLoan.LoanType = GetLoanTypeFromTypeNumberMizan((MizanRow.LoanType)+800);
                    NewLoan.LoanDate = NewLoanDate;
                    var AddedLoan = AddNewLoanFromMizan(NewLoan, CustomerDebtor);
                    if (AddedLoan == null)
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.Failed;
                        EditableLoan.ConversionMessage = "افزودن تسهیلات به پایگاه داده ها با خطا مواجه شد";
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }



                    //Loan Extra info -> simia itself and saba BranchCodeMizan+SabaNumber
                    if (!AddLoanExtraInfoFromMizan(new AMLoanExtraInfo { LoanId = AddedLoan.Id, ValueType = LoanType, Value = ExtraInfoValue }))
                        WarningMessage = WarningMessage + "افزودن شماره قرارداد تسهیلات میزان به عنوان سابقه در جدول سوابق با خطا مواجه شد";

                    if (!AddMizanLoanConversionRecord(AddedLoan, MizanRow))
                        WarningMessage = WarningMessage + "ارتباط تسهیلات قبلی میزان و جدید در سیستم با خطا مواجه شد. شماره قرارداد: " + AddedLoan.LoanNumber;

                    var RefereeList = LoanCustomerList.Where(m => m.CustomerType == MizanLoanCustomerType.Referee);
                    IList<AMCustomerRecordMizan> RefereeListIlist = RefereeList.ToList();
                    foreach (var referee in RefereeListIlist)
                    {
                        AMCustomer RefereeCustomer = AddMizanCustomerToDB(referee);
                        bool resRf = false;
                        AMReferee CurrentReferee = new AMReferee();
                        CurrentReferee.CustomerId = RefereeCustomer.Id;
                        CurrentReferee.LoanId = AddedLoan.Id;
                        CurrentReferee.Description = "";
                        AddNewReferee(CurrentReferee, out resRf);
                    }

                    if (!InsertInstallmentsMizan(AddedLoan, MizanRow.InstallmentPeriod, MizanRow.TotalInstallmentCount))
                    {
                        AMLoanRecordMizan EditableLoan = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                        EditableLoan.ConversionStatus = ConversionResultStatus.InstallmentError;
                        EditableLoan.ConversionMessage = WarningMessage + "قسط بندی با خطا مواجه شد. قرارداد: " + AddedLoan.LoanNumber;
                        UpdateMizanLoanRecord(EditableLoan);
                        continue;
                    }

                    AMLoanRecordMizan EditableLoanFinal = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                    EditableLoanFinal.ConversionStatus = ConversionResultStatus.Success;
                    EditableLoanFinal.ConversionMessage = "با موفقیت انجام شد. شماره جدید: " + AddedLoan.LoanNumber;
                    UpdateMizanLoanRecord(EditableLoanFinal);
                }
                catch (Exception e1)
                {
                    AMLoanRecordMizan EditableLoanFinal2 = db.AMLoanRecordMizanEnt.Find(MizanRow.Id);
                    EditableLoanFinal2.ConversionStatus = ConversionResultStatus.Failed;
                    EditableLoanFinal2.ConversionMessage = "خطای پیش بینی نشده اتفاق افتاد: " + e1.Message;
                    UpdateMizanLoanRecord(EditableLoanFinal2);
                }
            }
            Message = "با موفقیت انجام شد";
            return true;
        }

        public bool AddForgotPasswordCode(AMForgotPasswordCode model)
        {
            try
            {
                db.AMForgotPasswordCodeEnt.Add(model);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AMUser GetAmUserFromAspUser(string AspUserId)
        {
            return db.Users.Find(AspUserId).AMUser;
        }

        public ApplicationUser GetAspUserById(string AspUserId)
        {
            return db.Users.Find(AspUserId);
        }

        public IEnumerable<AMForgotPasswordCode> GetForgotCodeByAspUserId(string AspUserId)
        {
            ApplicationUser ThisUser = GetAspUserById(AspUserId);
            if (ThisUser == null) return null;
            return db.AMForgotPasswordCodeEnt.Where(m => m.UserName == ThisUser.UserName);
        }

        public bool SetForgotPasswordAsUsed(AMForgotPasswordCode model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<AMLoanExtraInfo> GetLoanExtraInfoByQuery(string query)
        {
            return db.AMLoanExtraInfoEnt.Where(m => m.Value == query);
        }

        public IEnumerable<AMLoanRecordMizan> SearchFailedMizanLoan(string query)
        {
            return db.AMLoanRecordMizanEnt.Where(m => (int)m.ConversionStatus > 1 && m.SabaNumber == query || m.SimiaNumber == query);
        }

        //public bool UpdateMizanLoanType()
        //{
        //    var k = db.AMLoanEnt.Where(m => (int)m.LoanType > 1);
        //    IList<AMLoan> IlistK = k.ToList();
        //    foreach(var r in IlistK)
        //    {
        //        AMLoan edited = new AMLoan();
        //        edited = r;
        //        try
        //        {
        //            edited.LoanType = (LoanType)(r.LoanType + 800);
        //            db.Entry(edited).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //        catch(Exception e1)
        //        {
        //            string message = e1.Message;
        //            return false;
        //        }

        //    }
        //    return true;
        //}



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
