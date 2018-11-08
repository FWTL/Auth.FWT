using Auth.FWT.Infrastructure.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FWT.Core.CQRS;
using FWT.Core.Extensions;
using FWT.Core.Services.Redis;
using FWT.Core.Services.Telegram;
using FWT.Database;
using FWT.Infrastructure.CQRS;
using FWT.Infrastructure.Dapper;
using FWT.Infrastructure.Telegram;
using FWT.Infrastructure.Unique;
using FWT.Infrastructure.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using StackExchange.Redis;
using System;

namespace FWT.AuthServer
{
    public class IocConfig
    {
        public static void RegisterCredentials(ContainerBuilder builder)
        {
            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                var credentails = new RedisCredentials();
                credentails.BuildConnectionString(
                    configuration["Redis:Name"],
                    configuration["Redis:Password"],
                    configuration["Redis:Port"].To<int>(),
                    isSsl: true,
                    allowAdmin: true);

                return ConnectionMultiplexer.Connect(credentails.ConnectionString);
            }).SingleInstance();

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

            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                var settings = new TelegramSettings()
                {
                    AppHash = configuration["Telegram:Settings:AppHash"],
                    AppId = configuration["Telegram:Settings:AppId"].To<int>(),
                    ServerAddress = configuration["Telegram:Settings:ServerAddress"],
                    ServerPort = configuration["Telegram:Settings:ServerPort"].To<int>(),
                    ServerPublicKey = configuration["Telegram:Settings:ServerPublicKey"]
                };

                return settings;
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

            builder.Register(b =>
            {
                var redis = b.Resolve<ConnectionMultiplexer>();
                return redis.GetDatabase();
            }).InstancePerLifetimeScope();

            builder.Register(b =>
            {
                var configuration = b.Resolve<IConfiguration>();
                var redis = b.Resolve<ConnectionMultiplexer>();
                return redis.GetServer(configuration["Redis:Url"]);
            }).InstancePerLifetimeScope();

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

            builder.Register<IMemoryCache>(b =>
            {
                return new MemoryCache(new MemoryCacheOptions());
            }).SingleInstance();

            builder.RegisterType<DapperConnector>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<GuidService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<TelegramService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}