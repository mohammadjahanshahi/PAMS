namespace BSIActivityManagement.DAL
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;
    using BSIActivityManagement.Enum;

    //public class MainDBModel : DbContext
    //{
    // Your context has been configured to use a 'MainDBModel' connection string from your application's 
    // configuration file (App.config or Web.config). By default, this connection string targets the 
    // 'BSIActivityManagement.DAL.MainDBModel' database on your LocalDb instance. 
    // 
    // If you wish to target a different database and/or database provider, modify the 'MainDBModel' 
    // connection string in the application configuration file.
    //public MainDBModel()
    //    : base("name=MainDBModel")
    //{
    //}

    // Add a DbSet for each entity type that you want to include in your model. For more information 
    // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

    // public virtual DbSet<MyEntity> MyEntities { get; set; }
    //}

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
    [Table("OrganizationTable")]
    public class AMOrganization
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام سازمان وارد نشده است")]
        [Display(Name = "سازمان اصلی")]
        public int? ParentId { get; set; }
        [Display(Name = "نام سازمان")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [ForeignKey("Image")]
        public int ImageId { get; set; }

        public virtual AMImage Image { get; set; }
    }


    [Table("ProcessTable")]
    public class AMProcess
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "نام فرآیند وارد نشده است")]
        [Display(Name = "نام فرآیند")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [ForeignKey("ProcessType")]
        [Required(ErrorMessage = "نوع فرآیند انتخاب نشده است")]
        public int ProcessTypeId { get; set; }
        [Required]
        public virtual AMProcessType ProcessType { get; set; }
    }

    [Table("ProcessTypeTable")]
    public class AMProcessType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام نوع وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [ForeignKey("Image")]
        public int? ImageId { get; set; }

        public virtual AMImage Image { get; set; }
    }

    [Table("ActivityTable")]
    public class AMActivity
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام فعالیت وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [InverseProperty("Activity")]
        public virtual ICollection<AMActivityItem> ActivityItems { get; set; }
        [ForeignKey("Type")]
        [Required]
        public int TypeId { get; set; }

        [Required]
        public virtual AMActivityType Type { get; set; }

        public virtual ICollection<AMQualityRule> RuleList { get; set; }
    }

    [Table("ActivityTypeTable")]
    public class AMActivityType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام نوع وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [ForeignKey("Image")]
        public int? ImageId { get; set; }

        public virtual AMImage Image { get; set; }
    }
    [Table("OrgProcessActRelationTable")]
    public class AMActProcOrgRel
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Activity")]
        [Required]
        public int ActivityId { get; set; }
        [ForeignKey("Process")]
        [Required]
        public int ProcessId { get; set; }
        [ForeignKey("Organization")]
        [Required]
        public int OrganizationId { get; set; }
        [Required]
        public virtual AMActivity Activity { get; set; }
        [Required]
        public virtual AMProcess Process { get; set; }
        [Required]
        public virtual AMOrganization Organization { get; set; }
    }
    [Table("ActivityItemTable")]
    public class AMActivityItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        [Display(Name = "عنوان مستند")]
        public string TextTitle { get; set; }
        [Display(Name = "توضیحات")]
        public string TextBody { get; set; }
        public DateTime CreationTime { get; set; }
        [ForeignKey("ItemType")]
        [Required]
        public int ItemTypeId { get; set; }
        [ForeignKey("Document")]
        public int? DocumentId { get; set; }
        [ForeignKey("Activity")]
        [Required]
        public int ActivityId { get; set; }
        [ForeignKey("ChildActivity")]
        public int? ChildActivityId { get; set; }
        [Required]
        public int Status { get; set; }

        public virtual AMActivityItemType ItemType { get; set; }
        public virtual AMDocument Document { get; set; }
        public virtual AMActivity Activity { get; set; }
        public virtual AMActivity ChildActivity { get; set; }
    }
    [Table("ActItemTypeTable")]
    public class AMActivityItemType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام نوع وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [ForeignKey("Image")]
        public int? ImageId { get; set; }

        public virtual AMImage Image { get; set; }

    }
    [Table("DocumentTable")]
    public class AMDocument
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [ForeignKey("File")]
        public int FileId { get; set; }
        [Required]
        public virtual AMFile File { get; set; }
    }
    [Table("FileTable")]
    public class AMFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        public double FileSize { get; set; }
        public string Title { get; set; }
        [Required]
        public byte[] FileData { get; set; }
        [Required]
        public string FileType { get; set; }
        public DateTime CreationTime { get; set; }
    }
    [Table("ImageTable")]
    public class AMImage
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [Required]
        public byte[] ImageData { get; set; }
        [Required]
        public string FileType { get; set; }
        public DateTime CreationTime { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    ///////// User
    [Table("OrganizationUnit")]
    public class AMUnitOfOrg
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "گروه سازمانی انتخاب نشده است")]
        [ForeignKey("Organization")]
        [Display(Name = "گروه سازمانی")]
        public int OrganizationId { get; set; }

        [Display(Name = "شناسه سازمانی")]
        public string IdentityCode { get; set; }

        [Required(ErrorMessage = "نام واحد سازمانی وارد نشده است")]
        [Display(Name = "نام واحد")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public virtual AMOrganization Organization { get; set; }

        [InverseProperty("Unit")]
        public virtual ICollection<AMLoan> LoanList { get; set; }
    }
    [Table("UserTable")]
    public class AMUser
    {
        [Required, Key]
        public int Id { get; set; }

        public int ValidationStatus { get; set; } // Is current user validated by a power user? what is his validation status Now?
        [Required]
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required]
        public string Lastname { get; set; }
        [Display(Name = "شماره همراه")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "شماره همراه دارای قالب صحیح نیست")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "نوع کاربر انتخاب نشده است")]
        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }
        public virtual AMUserType UserType { get; set; }

        [ForeignKey("Image")]
        public int? ImageId { get; set; }
        public virtual AMImage Image { get; set; }

        public virtual ICollection<AMForgotPasswordCode> ForgotPasswordCodeList { get; set; }
    }
    [Table("UserTypeTable")]
    public class AMUserType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "نام نوع وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [ForeignKey("Image")]
        public int? ImageId { get; set; }
        public virtual AMImage Image { get; set; }

        [InverseProperty("UserType")]
        public virtual ICollection<AMUserTypeAccessList> AccessList { get; set; }
    }

    [Table("AMUserTypeAccessTable")]
    public class AMUserTypeAccessList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }

        [Required]
        [ForeignKey("Access")]
        public int AccessId { get; set; }

        public virtual AMUserType UserType { get; set; }
        public virtual AMAccess Access { get; set; }
    }
    [Table("AMAccessTable")]
    public class AMAccess
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        [Display(Name = "کلید دسترسی")]
        [Required(ErrorMessage = "ورود کلید دسترسی الزامیست")]
        public string AccessKey { get; set; }
        [Required(ErrorMessage = "ورود مقدار برای دسترسی الزامیست")]
        [Display(Name = "مقدار")]
        public int Value { get; set; }
    }
    [Table("UserPlacementTable")]
    public class AMUserPlacement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }

        [Required]
        [ForeignKey("Process")]
        public int ProcessId { get; set; }

        public virtual AMUnitOfOrg Unit { get; set; }
        public virtual AMProcess Process { get; set; }
        public virtual AMUser User { get; set; }
    }
    [Table("UserPlacementRequestTable")]
    public class AMUserPlacementReq
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("ConfirmingUser")]
        public int? ConfirmingUserId { get; set; }

        [Required]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }

        //Status = 0 Means user has already asked for joining to the unit
        //Status = 1 Means user has been accepted to this unit but not been viewed by himself and a notification should appear
        //Status = 2 Means user has been rejected for joining to this unit but not been viewed by himself and a notification should appear
        //Status = 3 Means user Accepted and Viewed
        //Status = 4 Means user Rejected and Viewed
        public int Status { get; set; }

        public DateTime AcceptationRejecionDateTime { get; set; }

        public virtual AMUnitOfOrg Unit { get; set; }
        public virtual AMUser User { get; set; }
        public virtual AMUser ConfirmingUser { get; set; }

    }

    [Table("RevisionsTable")]
    public class AMRevision
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "فعالیت انتخابی نادرست است")]
        [ForeignKey("Activity")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "خطا در شناسه کاربری")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "واحد انتخابی نادرست است")]
        [ForeignKey("Unit")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "فرآیند انتخابی نادرست است")]
        [ForeignKey("Process")]
        public int ProcessId { get; set; }

        public DateTime RegDateTime { get; set; }

        [Display(Name = "شرح عدم انطباق")]
        public string ConflictDescription { get; set; }
        [Display(Name = "ریشه یابی عدم انطباق")]
        public string ConflictSource { get; set; }
        [Display(Name = "اقدام اصلاحی پیشنهادی")]
        public string ConflictSolution { get; set; }

        public virtual AMUnitOfOrg Unit { get; set; }
        public virtual AMUser User { get; set; }
        public virtual AMActivity Activity { get; set; }
        public virtual AMProcess Process { get; set; }

        public virtual ICollection<AMRevisionStatus> StatusList { get; set; }
    }

    [Table("RevisionStatusTable")]
    public class AMRevisionStatus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اقدام اصلاحی انتخابی نادرست است")]
        [ForeignKey("Revision")]
        public int RevisionId { get; set; }

        [ForeignKey("User")]
        public int? SigningUserId { get; set; }

        [Required]
        public DateTime StatusDateTime { get; set; }

        [Required]
        public int Status { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public virtual AMUser User { get; set; }
        public virtual AMRevision Revision { get; set; }
    }

    /*
    Status == 1  >>> Just Comment
    Status == 2  >>> Confirm Level 1 Process Owner
    Status == 3  >>> Reject Level 1 
    Status == 4  >>> Confirm Level 2 Unit Manager
    Status == 5  >>> Reject Level 2
    Status == 6  >>> Confirm Level 3 State Manager
    Status == 7  >>> Reject Level 3
    Status == 8  >>> Confirm Level 4 Top Manager
    Status == 9  >>> Reject Level 4
    Status == 10 >>> Confirm Level 5 CEO
    Status == 11 >>> Reject Level 5
    Status == 10 >>> Closed
*/

    ///////// Quality Mile Stone ///////
    
    [Table("QualityIndexTable")]
    public class AMQualityIndex
    {
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "عنوان وارد نشده است")]
        public string Title { get; set; }

        [Required(ErrorMessage = "نوع شاخص نادرست است")]
        [Display(Name = "نوع شاخص")]
        public QualityIndexEnum EnumType { get; set; }
    }

    [Table("QualityRuleTable")]
    public class AMQualityRule
    {
        public int Id { get; set; }

        [ForeignKey("Activity")]
        public int? ActivityId { get; set; }

        [ForeignKey("Index")]
        public int IndexId { get; set; }

        public virtual AMActivity Activity { get; set; }
        public virtual AMQualityIndex Index { get; set; }
    }

    [Table("QualityMileStoneTable")]
    public class AMQualityMileStone
    {
        public int Id { get; set; }

        [ForeignKey("Rule")]
        [Display(Name = "قانون کیفیت")]
        public int RuleId { get; set; }

        [ForeignKey("Unit")]
        public int UnitId { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        [Display(Name = "مقدار اولیه")]
        public decimal Minimum { get; set; }
        [Display(Name = "مقدار هدف")]
        public decimal Maximum { get; set; }

        public int UserId { get; set; }

        public DateTime RegistrationDate { get; set; }
        [Display(Name = "تاریخ پایان دوره")]
        public DateTime ExpirationDate { get; set; }

        public virtual AMQualityRule Rule { get; set; }
        public virtual AMUser User { get; set; }
        public virtual AMUnitOfOrg Unit { get; set; }
    }

    [Table("RegisterActivityTable")]
    public class AMRegisterActivity
    {
        public int Id { get; set; }

        [ForeignKey("Activity")]
        public int ActivityId { get; set;}
        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
        public DateTime RegisteringDate { get; set; }

        public virtual AMActivity Activity { get; set; }
        public virtual AMUnitOfOrg Unit { get; set; }
        public virtual AMUser User { get; set; }

    }

    [Table("ActivityDataTable")]
    public class AMActivityData
    {
        public int Id { get; set; }
        [ForeignKey("RegisteringActivity")]
        public int RegisteringActivityId { get; set; }
        [ForeignKey("Rule")]
        public int RuleId { get; set; }
        
        public decimal ActivityData { get; set; }

        public virtual AMRegisterActivity RegisteringActivity { get; set; }
        public virtual AMQualityRule Rule { get; set; }
    }

    /// <summary>
    /// اطلاعات مربوط به مشتری و حسابهایش
    /// </summary>
    [Table("CustomerTable")]
    public class AMCustomer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ورود نام الزامی است")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "ورود نام خانوادگی الزامی است")]
        public string Lastname { get; set; }

        [Display(Name = "نوع مشتری")]
        [Required(ErrorMessage = "انتخاب نوع مشتری الزامی است")]
        public CustomerType CustomerType { get; set; }

        [Display(Name = "شماره مشتری")]
        [Required(ErrorMessage = "ورود شماره مشتری الزامی است")]
        [MaxLength(10,ErrorMessage = "حداکثر تعداد 10 عدد قابل قبول است")]
        [Index(IsUnique = true)]
        public string CustomerNumber { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMCustomerLoan> LoanList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMCustomerAccount> AccountList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMCustomerAddress> AddressList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMReferee> AsRefereeList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMCustomerPersonal> PersonalCodeList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMCorporationMember> AsCorporationMemberList { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<AMMizanCustomerConversion> MizanCustomerInfoList { get; set; }


    }


    [Table("PersonTable")]
    public class AMCustomerPersonal
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        [Index(IsUnique = true)]
        public int CustomerId { get; set; }
        public virtual AMCustomer Customer { get; set; }

        [Display(Name = "شماره ملی")]
        [Required(ErrorMessage = "ورود شماره ملی الزامی است")]
        [StringLength(10, ErrorMessage = "دقیقا تعداد ۱۰ عدد قابل قبول است")]
        public string NationalCode { get; set; }

        [Display(Name = "شماره شناسنامه")]
        [MaxLength(10, ErrorMessage = "تعداد کاراکترهای شماره شناسنامه نباید بیشتر از 10رقم باشد")]
        public string PersonalId { get; set; }

    }

    [Table("CorporationTable")]
    public class AMCorporation
    {
        public int Id { get; set; }

        [Display(Name = "شناسه ملی")]
        [Required(ErrorMessage = "ورود شماره ملی الزامی است")]
        [StringLength(11, ErrorMessage = "دقیقا تعداد ۱۱ عدد قابل قبول است")]
        public string NationalCode { get; set; }
    }

    [Table("CorporationMemberTable")]
    public class AMCorporationMember
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual AMCustomer Customer { get; set; }

        [ForeignKey("Corporation")]
        public int CorporationId { get; set; }
        public virtual AMCorporation Corporation { get; set; }

        [Display(Name = "نوع عضویت شماره مشتری")]
        [Required(ErrorMessage = "انتخاب نوع عضویت این شماره مشتری الزامی است")]
        public CorporationMemberType MembershipType { get; set; }
    }

    [Table("AccountTable")]
    public class AMAccount
    {
        public int Id { get; set; }

        [Display(Name = "شماره حساب")]
        [Required(ErrorMessage = "ورود شماره حساب الزامی است")]
        [MaxLength(13, ErrorMessage = "حداکثر تعداد 13 عدد قابل قبول است")]
        [Index(IsUnique = true)]
        public string AccountNumber { get; set; }

        [Display(Name = "نوع حساب")]
        [Required(ErrorMessage = "انتخاب نوع حساب الزامی است")]
        public AccountType AccountType { get; set; }

        [InverseProperty("Account")]
        public virtual ICollection<AMCustomerAccount> AccountOwners { get; set; }
    }

    [Table("CustomerAccountsTable")]
    public class AMCustomerAccount
    {
        public int Id { get; set; }
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public virtual AMAccount Account { get; set; }

        public virtual AMCustomer Customer { get; set; }
    }

    [Table("CustomerAddressTable")]
    public class AMCustomerAddress
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "نشانی")]
        public string Address { get; set; }
        [Display(Name = "شماره تماس")]
        [Required(ErrorMessage = "ورود شماره تماس الزامی است")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نوع تلفن")]
        [Required(ErrorMessage = "انتخاب نوع تلفن الزامی است")]
        public PhoneType PhoneType { get; set; }

        public virtual AMCustomer Customer { get; set; }
    }

    [Table("LoanTable")]
    public class AMLoan
    {
        public int Id { get; set; }

        [Display(Name = "شماره تسهیلات")]
        [Required(ErrorMessage = "ورود شماره تسهیلات الزامی است")]
        [MaxLength(13, ErrorMessage = "حداکثر تعداد 13 عدد قابل قبول است")]
        [Index(IsUnique = true)]
        public string LoanNumber { get; set; }

        [Display(Name = "نوع تسهیلات")]
        [Required(ErrorMessage = "انتخاب نوع تسهیلات الزامی است")]
        public LoanType LoanType { get; set; }

        [Display(Name = "مبلغ کل وام")]
        [Required(ErrorMessage = "ورود مبلغ کل وام الزامی است")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal LoanTotalAmount { get; set; }

        [Display(Name = "تاریخ پرداخت وام")]
        public DateTime LoanDate { get; set; }

        [InverseProperty("Loan")]
        public virtual ICollection<AMInstallment> InstallmentList { get; set; }

        [InverseProperty("Loan")]
        public virtual ICollection<AMReferee> RefereeLists { get; set; }

        [InverseProperty("Loan")]
        public virtual ICollection<AMUpdateLoanLog> UpdateList { get; set; }

        [Required]
        public int UnitId { get; set; } 

        public virtual AMUnitOfOrg Unit { get; set; }

        [InverseProperty("Loan")]
        public virtual ICollection<AMLoanExtraInfo> ExtraInfoList { get; set; }

        [InverseProperty("Loan")]
        public virtual ICollection<AMMizanLoanConversion> MizanLoanInfoList { get; set; }
        [InverseProperty("Loan")]
        public virtual ICollection<AMCustomerLoan> DebtorList { get; set; }

    }


    [Table("LoanExtraInfoTable")]
    public class AMLoanExtraInfo
    {
        public int Id { get; set; }

        [Display(Name = "نوع اطلاعات")]
        [Required(ErrorMessage = "انتخاب نوع اطلاعات الزامی است")]
        public LoanExtraInfoType ValueType { get; set; }

        [Display(Name = "اطلاعات")]
        [Required(ErrorMessage = "ورود اطلاعات الزامی است")]
        public string Value { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }
        public virtual AMLoan Loan { get; set; }
    }

    [Table("CustomerLoanTable")]
    public class AMCustomerLoan
    {
        public int Id { get; set; }
        [ForeignKey("Loan")]
        public int LoantId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public virtual AMLoan Loan { get; set; }

        public virtual AMCustomer Customer { get; set; }
    }

    [Table("LoanRefereesTable")]
    public class AMReferee
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public virtual AMCustomer Customer { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }

        public virtual AMLoan Loan { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }
    }

    [Table("LoanInstallmentsTable")]
    public class AMInstallment
    {
        public int Id { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }

        public virtual AMLoan Loan { get; set; }

        [Display(Name = "مبلغ قسط")]
        [Required(ErrorMessage = "ورود مبلغ قسط الزامی است")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal InstallmentAmount { get; set; }

        [Display(Name = "تاریخ سررسید قسط")]
        public DateTime DueDate { get; set; }

        [Display(Name = "وضعیت پرداخت قسط")]
        public InstallmentStatus Status { get; set; }

        [Display(Name = "تاریخ پرداخت قسط")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "شماره ردیف")]
        public int IndexNumber { get; set; }

        [InverseProperty("Installment")]
        public virtual ICollection<AMCall> CallList { get; set; }

        [InverseProperty("Installment")]
        public virtual ICollection<AMInstallmentNotification> NotificationList { get; set; }
    }

    [Table("CallsTable")]
    public class AMCall
    {
        public int Id { get; set; }

        [Display(Name = "شناسه ویژه کاربر")]
        public int UserId { get; set; }

        public virtual AMUser User { get; set; }

        [Display(Name = "قسط")]
        public int InstallmentId { get; set; }

        public virtual AMInstallment Installment { get; set; }

        [Display(Name = "زمان تماس")]
        public DateTime CallTime { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "ورود اطلاعات این تماس الزامی است")]
        public string Description { get; set; }

        [Display(Name = "نشانی و شماره تماس")]
        [ForeignKey("Address")]
        public int AddressId { get; set; }

        public virtual AMCustomerAddress Address { get; set; }

        [InverseProperty("Call")]
        public virtual ICollection<AMCallNotification> NotificationList { get; set; }
    }

    [Table("InstallmentNotificationTable")]
    public class AMInstallmentNotification
    {
        public int Id { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "اطلاعات نمی تواند خالی باشد")]
        public string Description { get; set; }

        [Display(Name = "زمان اعلام")]
        public DateTime DueDate { get; set; }

        [Display(Name = "وضعیت مشاهده")]
        public NotificationStatus Status { get; set; }

        [Required]
        [ForeignKey("Installment")]
        public int InstallmentId { get; set; }

        public virtual AMInstallment Installment { get; set; }
    }

    [Table("CallNotificationTable")]
    public class AMCallNotification
    {
        public int Id { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "اطلاعات نمی تواند خالی باشد")]
        public string Description { get; set; }

        [Display(Name = "زمان اعلام")]
        public DateTime DueDate { get; set; }

        [Display(Name = "وضعیت مشاهده")]
        public NotificationStatus Status { get; set; }

        [Required]
        [ForeignKey("Call")]
        public int CallId { get; set; }

        public virtual AMCall Call { get; set; }
    }

    [Table("InstallmentUserLogTable")]
    public class AMInstallmentUserLog
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Installment")]
        public int InstallmentId { get; set; }

        public virtual AMInstallment Installment { get; set; }

        [Display(Name = "شناسه ویژه کاربر")]
        public int UserId { get; set; }

        public virtual AMUser User { get; set; }

        [Display(Name = "زمان ثبت")]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "عملیات")]
        public InstallmentAction Action { get; set; }

    }


    [Table("InstallmentNotificationUserLogTable")]
    public class AMInstallmentNotificationUserLog
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("InstallmentNotification")]
        public int InstallmentNotificationId { get; set; }

        public virtual AMInstallmentNotification InstallmentNotification { get; set; }

        [Display(Name = "شناسه ویژه کاربر")]
        public int UserId { get; set; }

        public virtual AMUser User { get; set; }

        [Display(Name = "زمان ثبت")]
        public DateTime CreationDateTime { get; set; }


        [Display(Name = "عملیات")]
        public InstallmentNotificationAction Action { get; set; }

    }

    [Table("UpdateLoanLogTable")]
    public class AMUpdateLoanLog
    {
        public int Id { get; set; }

        [Display(Name = "زمان ثبت")]
        public DateTime UpdateTime { get; set; }

        [Display(Name = "شناسه ویژه کاربر")]
        public int UserId { get; set; }

        public virtual AMUser User { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }

        public virtual AMLoan Loan { get; set; }

        public UpdateLoanStatus Status { get; set; }

    }

    [Table("MizanCustomerTable")]
    public class AMCustomerRecordMizan
    {
        public int Id { get; set; }

        [Display(Name = "شماره مشتری میزان")]
        [Required(ErrorMessage = "ورود شماره مشتری میزان الزامی است")]
        [MaxLength(20, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string CustomerNumber { get; set; }

        [Display(Name = "نشانی")]
        public string Address { get; set; }

        [Display(Name = "شماره تماس ۱")]
        public string PhoneNumber1 { get; set; }
        [Display(Name = "شماره تماس ۲")]
        public string PhoneNumber2 { get; set; }
        [Display(Name = "شماره تماس ۳")]
        public string PhoneNumber3 { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public MizanLoanCustomerType CustomerType { get; set; }

        [MaxLength(20, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string LoanNumber { get; set; }

        public LoanExtraInfoType LoanNumberType { get; set; }

        [Required(ErrorMessage = "ورود شماره حساب الزامی است")]
        [StringLength(10, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string NationalCode { get; set; }

        [Display(Name = "وضعیت انتقال")]
        public ConversionResultStatus ConversionStatus { get; set; }
        [Display(Name = "پیغام انتقال")]
        public string ConversionMessage { get; set; }
        [Display(Name = ("کد شعبه میزان"))]
        public int MizanBranchCode { get; set; }

        public CustomerTypePersonalCorporation IsCorporation { get; set; }

        [Display(Name = "شماره شناسنامه")]
        [MaxLength(10, ErrorMessage = "تعداد کاراکترهای شماره شناسنامه نباید بیشتر از 10رقم باشد")]
        public string PersonalId { get; set; }
    }

    [Table("MizanRecordsTable")]
    public class AMLoanRecordMizan
    {
        public int Id { get; set; }
        [MaxLength(20, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string SabaNumber { get; set; }
        [MaxLength(20, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string SimiaNumber { get; set; }

        [Display(Name = "شماره صبا سیمیا")]
        [Required(ErrorMessage = "ورود صباسیمیا الزامی است")]
        [MaxLength(20, ErrorMessage = "حداکثر تعداد ۲۰ عدد قابل قبول است")]
        public string SabaSimiaNumber { get; set; }
        public int MizanBranchCode { get; set; }

        public int BSIBranchCode { get; set; }

        [Display(Name = "مبلغ کل وام")]
        [Required(ErrorMessage = "ورود مبلغ کل وام الزامی است")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal LoanTotalAmount { get; set; }

        [Display(Name = "تعداد اقساط")]
        [Required(ErrorMessage = "ورود تعداد کل اقساط الزامی است")]
        public int TotalInstallmentCount { get; set; }

        [Display(Name = "نوع تسهیلات")]
        public int LoanType { get; set; }
        [Display(Name = "تاریخ پرداخت تسهیلات")]
        [Required(ErrorMessage = "ورود تاریخ پرداخت تسهیلات الزامی است")]
        public string PaymentDate { get; set; }
        [Display(Name = "فاصله اقساط")]
        public int InstallmentPeriod { get; set; }

        [Display(Name = "وضعیت انتقال")]
        public ConversionResultStatus ConversionStatus { get; set; }
        [Display(Name = "پیغام انتقال")]
        public string ConversionMessage { get; set; }
    }

    [Table("MizanCustomerConversion")]
    public class AMMizanCustomerConversion
    {
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual AMCustomer Customer { get; set; }
        [ForeignKey("MizanCustomer")]
        public int MizanCustomerId { get; set; }
        public virtual AMCustomerRecordMizan MizanCustomer { get; set; }
    }

    [Table("MizanLoanConversion")]
    public class AMMizanLoanConversion
    {
        public int Id { get; set; }

        [ForeignKey("Loan")]
        public int LoanId { get; set; }
        public virtual AMLoan Loan { get; set; }
        [ForeignKey("MizanLoan")]
        public int MizanLoanId { get; set; }
        public virtual AMLoanRecordMizan MizanLoan { get; set; }
    }

    [Table("ForgotPasswordCode")]
    public class AMForgotPasswordCode
    {
        public int Id { get; set; }

        [ForeignKey("AMUser")]
        public int UserId { get; set; }
        public AMUser AMUser { get; set; }

        public string UserName { get; set; }

        public int Code { get; set; }

        public DateTime Expiration { get; set; }

        public ForgotPasswordCodeStatus UsedStatus { get; set; }
    }

}