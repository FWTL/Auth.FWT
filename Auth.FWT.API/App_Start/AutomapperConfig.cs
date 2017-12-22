using System.Collections.Generic;
using System.Reflection;
using Autofac;
using AutoMapper;

namespace Auth.FWT.API.App_Start
{
    public static class MapperConfig
    {
        internal static void Configure()
        {
            var myAssembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(myAssembly).Where(t => t.IsSubclassOf(typeof(Profile))).As<Profile>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var profiles = container.Resolve<IEnumerable<Profile>>();

                Mapper.Initialize(cfg =>
                {
                    foreach (var profile in profiles)
                    {
                        cfg.AddProfile(profile);
                    }
                });
            }
        }
    }
}
