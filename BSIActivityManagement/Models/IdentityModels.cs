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
    }
}