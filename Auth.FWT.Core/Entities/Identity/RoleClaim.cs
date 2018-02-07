namespace Auth.FWT.Core.Entities.Identity
{
    public partial class RoleClaim : BaseEntity<int>
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual UserRole Role { get; set; }

        public int RoleId { get; set; }
    }
}
