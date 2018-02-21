using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Auth.FWT.Core.Entities;
using Auth.FWT.Core.Entities.Identity;
using FactoryGirlCore;

namespace Auth.FWT.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Auth.FWT.Data.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppContext context)
        {
            ////if (System.Diagnostics.Debugger.IsAttached == false)
            ////{
            ////    System.Diagnostics.Debugger.Launch();
            ////}

            if (!context.Set<UserRole, int>().Any(ur => ur.Name == UserRoles.USER))
            {
                var userRole = new UserRole()
                {
                    Name = UserRoles.USER
                };

                context.Set<UserRole, int>().Add(userRole);
                context.SaveChanges();
            }

            if (!context.Set<RoleClaim, int>().Any(rc => rc.ClaimType == AppClaims.USER_READ))
            {
                var swaggerRead = new RoleClaim()
                {
                    ClaimType = AppClaims.USER_READ,
                    ClaimValue = AppClaims.USER_WRITE,
                    RoleId = context.Set<UserRole, int>().FirstOrDefault(ur => ur.Name == UserRoles.USER).Id
                };

                context.Set<RoleClaim, int>().Add(swaggerRead);
                context.SaveChanges();
            }

            if (!context.Set<RoleClaim, int>().Any(rc => rc.ClaimType == AppClaims.USER_WRITE))
            {
                var swaggerRead = new RoleClaim()
                {
                    ClaimType = AppClaims.USER_WRITE,
                    ClaimValue = AppClaims.USER_WRITE,
                    RoleId = context.Set<UserRole, int>().FirstOrDefault(ur => ur.Name == UserRoles.USER).Id
                };

                context.Set<RoleClaim, int>().Add(swaggerRead);
                context.SaveChanges();
            }
        }

        private void InsertFakeData<TEntity, TKey, TFactory>(AppContext context, int count = 1, string name = "") where TEntity : BaseEntity<TKey> where TFactory : IDefinable
        {
            FactoryGirl.ClearFactoryDefinitions();
            FactoryGirl.Initialize(typeof(TFactory));
            ICollection<TEntity> entities = null;
            if (string.IsNullOrWhiteSpace(name))
            {
                entities = FactoryGirl.BuildList<TEntity>(count);
            }
            else
            {
                entities = FactoryGirl.BuildList<TEntity>(count, name);
            }

            foreach (var entity in entities)
            {
                context.Set<TEntity, TKey>().Add(entity);
            }

            context.SaveChanges();
        }
    }
}
