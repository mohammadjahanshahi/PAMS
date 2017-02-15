using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BSIActivityManagement.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSIActivityManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(new Claim(ClaimTypes.Thumbprint, this.AMUserId.ToString()));
            return userIdentity;
        }
        [ForeignKey("AMUser")]
        public int AMUserId { get; set; }

        public virtual AMUser AMUser { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(): base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<AMOrganization> AMOrganizationEnt { get; set; }
        public virtual DbSet<AMProcess> AMProcessEnt { get; set; }
        public virtual DbSet<AMProcessType> AMProcessTypeEnt { get; set; }
        public virtual DbSet<AMActivity> AMActivityEnt { get; set; }
        public virtual DbSet<AMActivityType> AMActivityTypeEnt { get; set; }
        public virtual DbSet<AMActProcOrgRel> AMActProcOrgRelEnt { get; set; }
        public virtual DbSet<AMActivityItem> AMActivityItemEnt { get; set; }
        public virtual DbSet<AMActivityItemType> AMActivityItemTypeEnt { get; set; }
        public virtual DbSet<AMDocument> AMDocumentEnt { get; set; }
        public virtual DbSet<AMFile> AMFileEnt { get; set; }
        public virtual DbSet<AMImage> AMImageEnt { get; set; }
        /// 
        /// User DBsets 
        /// 
        public virtual DbSet<AMUnitOfOrg> AMUnitEnt { get; set; }
        public virtual DbSet<AMUser> AMUserEnt { get; set; }
        public virtual DbSet<AMUserType> AMUserTypeEnt { get; set; }
        public virtual DbSet<AMUserTypeAccessList> AMUserTypeAccessListEnt { get; set; }
        public virtual DbSet<AMAccess> AMAccessEnt { get; set; }
        public virtual DbSet<AMUserPlacement> AMUserPlaceMentEnt { get; set; }
        public virtual DbSet<AMUserPlacementReq> AMUserPlacementReqEnt { get; set; }
        ///
        /// Revisioning and PDCA Cycle Tables
        /// 
        public virtual DbSet<AMRevision> AMRevisionEnt { get; set; }
        public virtual DbSet<AMRevisionStatus> AMRevisionStatusEnt { get; set; }

        ///
        /// Quality Data Registering
        ///
        public virtual DbSet<AMQualityIndex> AMQualityIndexEnt { get; set; }
        public virtual DbSet<AMQualityRule> AMQualityRuleEnt { get; set; }
        public virtual DbSet<AMQualityMileStone> AMQualityMileStoneEnt { get; set; }
        public virtual DbSet<AMRegisterActivity> AMRegisterActivityEnt { get; set; }
        public virtual DbSet<AMActivityData> AMActivityDataEnt { get; set; }

        ///
        /// Loan and Account Management
        ///

        public virtual DbSet<AMCustomer> AMCustomerEnt { get; set; }
        public virtual DbSet<AMAccount> AMAccountEnt { get; set; }
        public virtual DbSet<AMCustomerAccount> AMCustomerAccountEnt { get; set; }
        public virtual DbSet<AMCustomerAddress> AMCustomerAddressEnt { get; set; }
        public virtual DbSet<AMLoan> AMLoanEnt { get; set; }
        public virtual DbSet<AMReferee> AMRefereeEnt { get; set; }
        public virtual DbSet<AMInstallment> AMInstallmentEnt { get; set; }
        public virtual DbSet<AMCall> AMCallEnt { get; set; }
        public virtual DbSet<AMInstallmentNotification> AMInstallmentNotificationEnt { get; set; }
        public virtual DbSet<AMCallNotification> AMCallNotificationEnt { get; set; }
        public virtual DbSet<AMCustomerLoan> AMCustomerLoanEnt { get; set; }
        public virtual DbSet<AMInstallmentUserLog> AMInstallmentUserLogEnt { get; set; }
        public virtual DbSet<AMInstallmentNotificationUserLog> AMInstallmentNotificationUserLogEnt { get; set; }
        public virtual DbSet<AMUpdateLoanLog> AMUpdateLoanLogEnt { get; set; }


        ///Personal And Corporation
        ///
        public virtual DbSet<AMCustomerPersonal> AMCustomerPersonalEnt { get; set; }
        public virtual DbSet<AMCorporation> AMCorporationEnt { get; set; }
        public virtual DbSet<AMCorporationMember> AMCorporationMemberEnt { get; set; }

        ///Mizan Customer and Loan Tables
        public virtual DbSet<AMCustomerRecordMizan> AMCustomerRecordMizanEnt { get; set; }
        public virtual DbSet<AMLoanRecordMizan> AMLoanRecordMizanEnt { get; set; }
        public virtual DbSet<AMMizanCustomerConversion> AMMizanCustomerConversionEnt { get; set; }
        public virtual DbSet<AMMizanLoanConversion> AMMizanLoanConversionEnt { get; set; }
        public virtual DbSet<AMLoanExtraInfo> AMLoanExtraInfoEnt { get; set; }

        public virtual DbSet<AMForgotPasswordCode> AMForgotPasswordCodeEnt { get; set; }
        

    }
}