using Auth.FWT.API.CQRS;
using Auth.FWT.Core;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Providers;
using Auth.FWT.Core.Services.Dapper;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Data;
using Auth.FWT.Data.Dapper;
using Auth.FWT.Infrastructure.Logging;
using Auth.FWT.Infrastructure.ServiceBus;
using Auth.FWT.Infrastructure.Telegram;
using Auth.FWT.Infrastructure.Telegram.Parsers;
using Autofac;
using NodaTime;
using Rws.Web.Core.CQRS;
using StackExchange.Redis;
using TLSharp.Core;

namespace Auth.FWT.API.App_Start
{
    public class HangfireIocConfig
    {
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork)).InstancePerDependency();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerDependency();
            builder.Register(b =>
            {
                return new HangfireCommandDispatcher(b.Resolve<ICommandDispatcher>());
            }).InstancePerDependency();

            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICommandHandler<>)).InstancePerDependency();

            builder.Register<IDapperConnector>(b =>
            {
                var connectionString = ConfigKeys.ConnectionString;
                return new DapperConnector(connectionString);
            }).InstancePerDependency();

            builder.Register<IEntitiesContext>(b =>
            {
                var context = new Data.AppContext("name=AppContext");
                return context;
            }).InstancePerDependency();

            builder.Register(b => NLogLogger.Instance).SingleInstance();

            builder.RegisterType<SQLSessionStore>().As<ISessionStore>().InstancePerDependency();

            builder.Register<ITelegramClient>(b =>
            {
                return new NewTelegramClient(b.Resolve<IUserSessionManager>(), b.Resolve<ISessionStore>(), ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash);
            }).InstancePerDependency();

            builder.Register<IUserSessionManager>(b =>
            {
                return AppUserSessionManager.Instance.UserSessionManager;
            }).SingleInstance();

            builder.Register(b =>
            {
                var manager = b.Resolve<IUserSessionManager>();
                var currentUserId = b.Resolve<IUserProvider>()?.CurrentUserId;
                return manager.Get(currentUserId.ToString(), b.Resolve<ISessionStore>());
            }).InstancePerDependency();

            builder.Register<IClock>(b =>
            {
                return SystemClock.Instance;
            }).SingleInstance();

            builder.Register(b =>
            {
                return ConnectionMultiplexer.Connect(ConfigKeys.RedisConnectionString);
            }).SingleInstance();

            builder.Register(b =>
            {
                var redis = b.Resolve<ConnectionMultiplexer>();
                return redis.GetDatabase();
            }).InstancePerDependency();

            builder.Register<IServiceBus>(b =>
            {
                return new AzureServiceBus();
            }).InstancePerDependency();

            builder.RegisterType<TelegramMessagesParser>().AsImplementedInterfaces();

            var container = builder.Build();
            return container;
        }
    }
}