using Auth.FWT.Infrastructure.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWT.Core.CQRS;
using FWT.Database;
using FWT.Infrastructure.CQRS;
using FWT.Infrastructure.Dapper;
using FWT.Infrastructure.Unique;
using FWT.Infrastructure.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;
using FWT.Core.Extensions;

namespace FWT.AuthServer
{
    public class IocConfig
    {
        public static void RegisterCredentials(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                var credentials = new TelegramDatabaseCredentials();
                credentials.BuildConnectionString(
                    configuration["Auth:Sql:Url"],
                    configuration["Auth:Sql:Port"].To<int>(),
                    configuration["Auth:Sql:Catalog"],
                    configuration["Auth:Sql:User"],
                    configuration["Auth:Sql:Password"]);

                return credentials;
            }).SingleInstance();
        }

        public static void OverrideWithLocalCredentials(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                var credentials = new TelegramDatabaseCredentials();
                credentials.BuildLocalConnectionString(
                        configuration["Auth:Sql:Url"],
                        configuration["Auth:Sql:Catalog"]);

                return credentials;
            }).SingleInstance();
        }

        public static IContainer RegisterDependencies(IServiceCollection services, IHostingEnvironment env, IConfiguration rootConfiguration)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            RegisterCredentials(builder);

            if (env.IsDevelopment())
            {
                OverrideWithLocalCredentials(builder);
            }

            builder.Register(b =>
            {
                return rootConfiguration;
            }).SingleInstance();

            builder.RegisterType<IEventDispatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IEventHandler<>)).InstancePerDependency();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ICommandHandler<>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ICommandHandler<,>)).InstancePerLifetimeScope();

            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerRequest().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IQueryHandler<,>)).InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(AppAbstractValidation<>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IReadCacheHandler<,>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IWriteCacheHandler<,>)).InstancePerLifetimeScope();

            builder.Register(b => NLogLogger.Instance).SingleInstance();

            builder.Register<IClock>(b =>
            {
                return SystemClock.Instance;
            }).SingleInstance();

            builder.RegisterType<DapperConnector>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<GuidService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}