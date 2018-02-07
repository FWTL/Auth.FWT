namespace Auth.FWT.Core.Entities.Identity
{
    public partial class UserClaim : BaseEntity<int>
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }
    }
}
