namespace BSIActivityManagement.DAL
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity.EntityFramework;

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
        [Display(Name ="سازمان اصلی")]
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
}