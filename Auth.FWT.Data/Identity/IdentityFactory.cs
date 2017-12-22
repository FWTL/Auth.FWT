using System;
using Auth.FWT.Core.DomainModels.Identity;
using Microsoft.AspNet.Identity;

namespace Auth.FWT.Data.Identity
{
    public class IdentityFactory
    {
        public static RoleManager<UserRole, byte> CreateRoleManager(IEntitiesContext context)
        {
            var manager = new RoleManager<UserRole, byte>(new ApplicationRoleStore(context));
            return manager;
        }

        public static UserManager<User, int> CreateUserManager(IEntitiesContext context)
        {
            var manager = new UserManager<User, int>(new ApplicationUserStore(context));
            ////Configure validation logic for usernames
            manager.UserValidator = new ApplicationUserValidator(manager);

            //// Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            //// Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            manager.PasswordHasher = new PasswordHasher();
            //// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            //// You can write your own provider and plug in here.
            ////manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<User, int>
            ////{
            ////    MessageFormat = "Your security code is: {0}"
            ////});
            ////manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<User, int>
            ////{
            ////    Subject = "SecurityCode",
            ////    BodyFormat = "Your security code is {0}"
            ////});
            ////manager.EmailService = new EmailService();
            ////manager.SmsService = new SmsService();

            return manager;
        }
    }
}
