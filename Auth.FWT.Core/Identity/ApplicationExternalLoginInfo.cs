using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Core.Identity
{
    public class ApplicationExternalLoginInfo
    {
        public ApplicationExternalLoginInfo()
        {
        }

        public string DefaultUserName { get; set; }

        public string Email { get; set; }

        public ClaimsIdentity ExternalIdentity { get; set; }

        public UserLoginInfo Login { get; set; }
    }
}
