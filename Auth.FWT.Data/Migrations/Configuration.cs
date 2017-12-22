using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

            ////if (context.Set<UserRole, byte>().Any(x => x.Name == "Admin") == false)
            ////{
            ////    var adminRole = new UserRole()
            ////    {
            ////        Id = 1,
            ////        Name = "Admin",
            ////    };
            ////    context.Set<UserRole, byte>().Add(adminRole);
            ////    context.SaveChanges();
            ////}

            ////if (context.Set<UserRole, byte>().Any(x => x.Name == "User") == false)
            ////{
            ////    var userRole = new UserRole()
            ////    {
            ////        Id = 2,
            ////        Name = "User",
            ////    };
            ////    context.Set<UserRole, byte>().Add(userRole);
            ////    context.SaveChanges();
            ////}

            ////if (context.Set<User, int>().Any(x => x.Email == "a@g.pl") == false)
            ////{
            ////    var admin = new User()
            ////    {
            ////        Email = "a@g.pl",
            ////        EmailConfirmed = true,
            ////        PasswordHash = PasswordHelper.CreateHash("123"),
            ////        SecurityStamp = "123",
            ////        UserName = "Andrzej Go�aszewski",
            ////    };

            ////    var adminRole = context.Set<UserRole, byte>().Where(x => x.Name == "Admin").FirstOrDefault();
            ////    admin.Roles.Add(adminRole);
            ////    context.Set<User, int>().Add(admin);
            ////    context.SaveChanges();
            ////}
        }

        private void InsertFakeData<TEntity, TKey, TFactory>(AppContext context, int count = 1, string name = "") where TEntity : Core.DomainModels.BaseEntity<TKey> where TFactory : IDefinable
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