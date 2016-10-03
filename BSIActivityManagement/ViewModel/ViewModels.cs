using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BSIActivityManagement.DAL;

namespace BSIActivityManagement.ViewModel
{
    public class OrganizationViewModel
    {
        public int? Id { get; set; }
        [Display(Name = "سازمان اصلی")]
        public int? ParentId { get; set; }
        [Required(ErrorMessage ="نام سازمان وارد نشده است")]
        [Display(Name = "نام سازمان")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        public string OperationMessage { get; set; }
        public bool OperationResult { get; set; }
        public int? ImageId { get; set; }
    }
    public class SysAdminViewModel
    {
        public IEnumerable<OrgRow> OrgArray { get; set; }
        public IEnumerable<ProcessRow> ProcessArray { get; set; }
        public IEnumerable<ActivityRow> ActivityArray { get; set; }
        public IEnumerable<AMProcessType> ProcessTypeArray { get; set; }
        public IEnumerable<AMActivityType> ActivityTypeArray { get; set; }
        public IEnumerable<AMActivityItemType> ActivityItemTypeArray { get; set; }
        public IEnumerable<AMUserType> UserTypeArray { get; set; }
        public IEnumerable<AMAccess> AccessArray { get; set; }

        public int? CurrentOrgId { get; set; }
        public int? CurrentOrgParentId { get; set; }
        public int? CurrentProcessId { get; set; }
        public int? CurrentProcessParentId { get; set; }
        public int? CurrentActId { get; set; }
    }
    public class OrgRow
    {
        public IEnumerable<AMOrganization> OrgList { get; set; }
        public int? SelectedOrgId { get; set; }
    }
    public class ProcessRow
    {
        public IEnumerable<AMProcess> ProcessList { get; set; }
        public int? SelectedProcessId { get; set; }
    }
    public class ActivityRow
    {
        public IEnumerable<AMActivity> ActivityList { get; set; }
    }
    public class savedImageinf
    {
        public string fileaddress { get; set; }
        public int filesize { get; set; }
        public string filetype { get; set; }
        public int mediaid { get; set; }
        public string errormessage { get; set; }
    }
    public class JsonOrganizationViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ImageId { get; set; }
    }
    public class JsonProcessViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ImageId { get; set; }
    }
    public class AddActivityViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "نام فعالیت وارد نشده است")]
        [Display(Name ="نام فعالیت")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Required(ErrorMessage = "نوع فعالیت انتخاب نشده است")]
        public int TypeId { get; set; }
        [Required(ErrorMessage = "حداقل یک سازمان باید انتخاب شود")]
        public string SelectedOrganizationStr { get; set; }
        [Required(ErrorMessage = "حداقل یک فرآیند باید انتخاب شود")]
        public string SelectedProcessesStr { get; set; }
    }
    public class UserPlacementViewModel
    {
        public AMUnitOfOrg AmUnit { get; set; }
        //public IEnumerable<ProcessUsersViewModel> ProcessWithCurrentUsers { get; set; }
        public IEnumerable<AMUser> UsersWaitingForJoin { get; set; }
    }
    public class ProcessUsersViewModel
    {
        public AMProcess Process { get; set; }
        public IEnumerable<AMUser> Users { get; set; }
    }

    public class SearchUserViewModel
    {
        public AMUnitOfOrg Unit { get; set; }
        public IEnumerable<AMUser> Users { get; set; }
    }

    public class AddUnitManagerViewModel
    {
        [Required(ErrorMessage = "کاربر انتخابی نامعتبر است")]
        public AMUser SelectedUser { get; set; }
        [Required(ErrorMessage ="واحد سازمانی انتخابی نامعتبر است")]
        public AMUnitOfOrg SelectedUnit { get; set; }
    }

    public class UserTypeUsersViewModel
    {
        public AMUser User { get; set; }
        public IEnumerable<AMUserType> Types { get; set; }
    }

    public class AddUserTypeManagerViewModel
    {
        public AMUser User { get; set; }
        public AMUserType Type { get; set; }
    }

    public class NavLinkViewModel
    {
        public string controller { get; set; }
        public string action { get; set; }
        public string tag { get; set; }
        public string text { get; set; }
    }

    public class NavViewModel
    {
        public string Title { get; set; }
        public int UnitId { get; set; }
        public IEnumerable<NavLinkViewModel> Links { get; set; }
    }

    public class MainViewModelUsers
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<AMUser> UnitUsers { get; set; }
        public int UnitId { get; set; }
    }

    public class MainViewModelProcesses
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<AMProcess> Processes { get; set; }
        public AMUnitOfOrg Unit { get; set; }
    }

    public class MainViewModelUserProcesses
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<AMProcess> RemainUnitProcesses { get; set; }
        public IEnumerable<AMProcess> UserProcesses { get; set; }
        public int UnitId { get; set; }
        public int ModifyingUserId { get; set; }
    }

    public class MainViewModelUnit
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<AMUnitOfOrg> Units { get; set; }
    }

    public class MainViewModelProcessActivities
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<ActTypeGroup> ActivityGropus { get; set; }
        public AMUnitOfOrg Unit { get; set; }
        public AMProcess Process { get; set; }
    }
    public class ActTypeGroup
    {
        public IEnumerable<AMActivity> Activities { get; set; }
        public AMActivityType Type { get; set; }
    } 

    public class MainViewModelShowActivity
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<ActItemTypeGroup> ActivityItemGropus { get; set; }
        public AMUnitOfOrg Unit { get; set; }
        public AMProcess Process { get; set; }
        public AMActivity Activity { get; set; }
    }

    public class ActItemTypeGroup
    {
        public IEnumerable<AMActivityItem> Items { get; set; }
        public AMActivityItemType Type { get; set; }
    }

    public class MainViewModelShowItem
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public AMActivityItem Item { get; set; }
        public int DocumentPages { get; set; }
        public AMUnitOfOrg Unit { get; set; }
        public AMProcess Process { get; set; }
        public AMActivity Activity { get; set; }
    }

    public class SysAdminViewModelShowItem
    {
        public AMActivityItem Item { get; set; }
        public int DocumentPages { get; set; }
        public int ActivityId { get; set; }
        public int ProcessId { get; set; }
    }

    
   public class SysAdminModelShowActivity
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<ActItemTypeGroup> ActivityItemGropus { get; set; }
        public int UnitId { get; set; }
        public int ProcessId { get; set; }
        public int ActivityId { get; set; }
    }

    public class SysAdminViewModelProcessActivities
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<ActTypeGroup> ActivityGropus { get; set; }
        public int UnitId { get; set; }
        public int ProcessId { get; set; }
    }
}