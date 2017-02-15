using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BSIActivityManagement.DAL;
using System.ComponentModel.DataAnnotations.Schema;

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
        public IEnumerable<AMQualityIndex> QualityIndexArray { get; set; }

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
        public AMUser CurrentUser { get; set; }
        public bool CreateMileStoneAccess { get; set; }
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
        public AMUnitOfOrg Unit { get; set; }
        public AMProcess Process { get; set; }
        public AMActivity Activity { get; set; }
    }

    public class SysAdminViewModelProcessActivities
    {
        public IEnumerable<NavViewModel> Navigation { get; set; }
        public IEnumerable<ActTypeGroup> ActivityGropus { get; set; }
        public int UnitId { get; set; }
        public int ProcessId { get; set; }
    }

    public class NewRevisionViewModel
    {
        public AMActivity Activity { get; set; }
        public AMUnitOfOrg Unit { get; set; }
        public AMUser User { get; set; }
        public AMProcess Process { get; set; }


        [Display(Name = "شرح عدم انطباق")]
        public string ConflictDescription { get; set; }
        [Display(Name = "ریشه یابی عدم انطباق")]
        public string ConflictSource { get; set; }
        [Display(Name = "اقدام اصلاحی پیشنهادی")]
        public string ConflictSolution { get; set; }
    }

    public class IndexRevisionViewModel
    {
        public int Id { get; set; }
        public PersianDateViewModel PersianDate { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public string ActivityTitle { get; set; }
        public string ConflictDescription { get; set; }
        public string CurrentStatus { get; set; }
        public int LevelStatus { get; set; }
    }

    public class IndexRevisionFullViewModel
    {
        public PersianDateViewModel PersianDate { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public string CurrentStatus { get; set; }
        public int LevelStatus { get; set; }
        public AMRevision Revision { get; set; }
    }

    public class PersianDateViewModel
    {
        public int DateYr { get; set; }
        public int DateMM { get; set; }
        public int DateDD { get; set; }
        public int DateHR { get; set; }
        public int DateMI { get; set; }
        public int DateSE { get; set; }
    }

    public class RevisionHistoryViewModel
    {
        public PersianDateViewModel PersianDate { get; set; }

        public int Days { get; set; }
        public int Hours { get; set; }

        public AMRevisionStatus Status { get; set; }
    }

    public class CreateQualityRuleViewModel
    {
        public AMActivity Activity { get; set; }
        public AMProcess Process { get; set; }
        public IEnumerable<AMQualityIndex> IndexId { get; set; }
    }

    public class CreateQualityRulePostBackViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "فعالیت انتخابی نادرست است")]
        public int ActivityId { get; set; }
        [Required(ErrorMessage = "شاخص انتخابی نادرست است")]
        public int IndexId { get; set; }
        public int ProcessId { get; set; }
    }

    public class QualityRuleIndexViewModel
    {
        public IEnumerable<AMQualityRule> RuleList { get; set; }
        public AMActivity Activity { get; set; }
        public int ProcessId { get; set; }
    }

    public class QualityMileStoneCreate
    {
        public IEnumerable<AMQualityRule> RuleList { get; set; }
        public AMQualityMileStone MileStone { get; set; }
    }

    public class UnitProcessActViewModel
    {
        public int U { get; set; }
        public int P { get; set; }
        public int A { get; set; }
    }

    public class UnitProcessActObjectViewModel
    {
            public AMUnitOfOrg Unit { get; set; }
            public AMProcess Process { get; set; }
            public AMActivity Activity { get; set; }
    }

    public class RegisterActivityViewModel
    {
        public int UnitId { get; set; }
        public int ProcessId { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }

        [Display(Name = "قانون کیفیت")]
        [Required(ErrorMessage = "قانون کیفیت انتخاب نشده است")]
        public int RuleId { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "ورود توضیحات الزامیست")]
        public string Description { get; set; }

        [Display(Name = "داده مرتبط")]
        [Required(ErrorMessage = "ورود داده مرتبط الزامیست")]
        public decimal ActivityData { get; set; }
    }

    public class ActivityQualityIndexViewModel
    {
        public AMActivity Activity { get; set; }
        public AMUnitOfOrg Unit { get; set; }
        public AMProcess Process { get; set; }
        
        public IEnumerable<MileStoneStatusViewModel> MileStoneStatusList { get; set; }

    }

    public class MileStoneStatusViewModel
    {
        public AMQualityMileStone MileStone { get; set; }
        public TimeSpan RemainingTimeStatus { get; set; }
        public int RemainingTimePercentage { get; set; }
        public Decimal GoalStatus { get; set; }
        public int GoalPercentage { get; set; }
        public Decimal GoalRequiredChange { get; set; }

        public bool IsAlive { get; set; }
    }

    public class LoanRegisterationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "شماره تسهیلات")]
        [Required(ErrorMessage = "ورود شماره تسهیلات الزامی است")]
        [MaxLength(13, ErrorMessage = "حداکثر تعداد 13 عدد قابل قبول است")]
        [Index(IsUnique = true)]
        public string LoanNumber { get; set; }

        [Display(Name = "نوع تسهیلات")]
        [Required(ErrorMessage = "انتخاب نوع تسهیلات الزامی است")]
        public Enum.LoanType LoanType { get; set; }

        [Display(Name = "مبلغ کل وام")]
        [Required(ErrorMessage = "ورود مبلغ کل وام الزامی است")]
        public decimal LoanTotalAmount { get; set; }

        [Display(Name = "تاریخ پرداخت وام")]
        [Required(ErrorMessage = "ورود تاریخ پرداخت وام الزامی است")]
        public DateTime LoanDate { get; set; }

        [Display(Name = "تعداد اقساط وام")]
        [Required(ErrorMessage = "ورود تعداد اقساط وام الزامی است")]
        public int TotalInstallments { get; set; }

        [Display(Name = "فاصله زمانی اقساط وام")]
        [Required(ErrorMessage = "ورود فاصله طمانی اقساط وام الزامی است")]
        public Enum.MonthNumbers InstallmentDuration { get; set; }

        public int UnitId { get; set; }

    }

    public class FollowUpIndexViewModel
    {
        public IEnumerable<AMInstallment> TodayOverDueLoans { get; set; }
        public IEnumerable<AMInstallment> WeekOverDueLoans { get; set; }
        public IEnumerable<AMInstallment> OneMonthOverdueLoans { get; set; }
        public IEnumerable<AMInstallment> TwoMonthsOverDueLoans { get; set; }
        public IEnumerable<AMInstallment> MoreThanTwoMonthsOverDueLoans { get; set; }
        public IEnumerable<AMInstallmentNotification> LoansWithInstallmentNotification { get; set; }
        public IEnumerable<AMCallNotification> LoansWithCallNotification { get; set; }
    }

    public class UserLogViewModel
    {
        public AMUser ThisUser { get; set; }
        public IEnumerable<AMCall> CallList { get; set; }
        public IEnumerable<AMInstallmentUserLog> SetInstallmentStatusList { get; set; }
        public IEnumerable<AMInstallmentNotificationUserLog> SetInstallmentNotificationList { get; set; }
        public IEnumerable<AMInstallmentNotificationUserLog> DoneInstallmentNotificationList { get; set; }
        public IEnumerable<AMUpdateLoanLog> UpdateLogList { get; set; }
        public int FollowUpScore { get; set; }
    }

    public class ConversionIndexViewModel
    {
        public int MizanCustomerRecordsCount { get; set; }
        public int MizanLoanRecordsCount { get; set; }
        public int MizanCustomerRecordsUnConvertedCount { get; set; }
        public int MizanLoanRecordsUnConvertedCount { get; set; }
        public int MizanLoanRecordsConvertedSuccessfullyCount { get; set; }
        public int MizanCustomerRecordsConvertedSuccessfullyCount { get; set; }
        public int MizanCustomerRecordsConvertedWithWarningCount { get; set; }
        public int MizanLoanRecordsConvertedWithWarningCount { get; set; }
        public int MizanCustomerRecordsConversionFailedCount { get; set; }
        public int MizanLoanRecordsConversionFailedCount { get; set; }

        public LoanConvertFailureInfoViewModel Statistics { get; set; }

        public IEnumerable<AMCustomerRecordMizan> CustomerConversionFailed { get; set; }
        public IEnumerable<AMLoanRecordMizan> LoanConversionFailed { get; set; }

        public IEnumerable<AMCustomerRecordMizan> CustomerConvertedWithWarning { get; set; }
        public IEnumerable<AMLoanRecordMizan> LoanConvertedWithWarning { get; set; }
    }

    public class LoanConvertFailureInfoViewModel
    {
        [Display(Name = "خطای عمومی")]
        public int Failure { get; set; }
        [Display(Name = "از قبل موجود")]
        public int AlreadyExist { get; set; }
        [Display(Name = "خطا در اقساط")]
        public int InstallmentError { get; set; }
        [Display(Name = "خطا در شناسایی شعبه بانک صادرات")]
        public int BSIBranchError { get; set; }
        [Display(Name = "خطا در شناسایی ضامن و متعهد ")]
        public int DebtorRefereeError { get; set; }
        [Display(Name = " خطا در شناسایی متعهد")]
        public int DebtorError { get; set; }
        [Display(Name = "خطای تعداد اقساط")]
        public int InstallmentCountError { get; set; }
        [Display(Name = "خطای فاصله اقساط")]
        public int InstallmentPeriodError { get; set; }
        [Display(Name = "خطای نوع تسهیلات میزان")]
        public int MizanLoanTypeError { get; set; }
        [Display(Name = "خطای تاریخ اعطای تسهیلات")]
        public int LoanDateError { get; set; }
        [Display(Name = "تعداد کل تسهیلات ناموفق")]
        public int TotalFailed { get; set; }
    }

}