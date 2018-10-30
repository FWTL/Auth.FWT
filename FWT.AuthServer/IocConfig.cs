using Auth.FWT.Infrastructure.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWT.Core.CQRS;
using FWT.Infrastructure.CQRS;
using FWT.Infrastructure.Dapper;
using FWT.Infrastructure.Unique;
using FWT.Infrastructure.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;

namespace FWT.AuthServer
{
    public class IocConfig
    {
        public static void RegisterCredentials(ContainerBuilder builder)
        {
        }

        public static void OverrideWithLocalCredentials(ContainerBuilder builder)
        {
        }

        public static IContainer RegisterDependencies(IServiceCollection services, IHostingEnvironment env, IConfiguration rootConfiguration)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            RegisterCredentials(builder);

            if (env.EnvironmentName == "Local")
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