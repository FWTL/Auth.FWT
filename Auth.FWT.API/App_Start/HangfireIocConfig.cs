using Auth.FWT.API.Controllers.Job;
using Auth.FWT.Core;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Providers;
using Auth.FWT.Core.Services.Dapper;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Data;
using Auth.FWT.Data.Dapper;
using Auth.FWT.Infrastructure.Logging;
using Auth.FWT.Infrastructure.ServiceBus;
using Auth.FWT.Infrastructure.Telegram;
using Autofac;
using NodaTime;
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

            builder.RegisterType(typeof(GetMessages)).InstancePerDependency();

            var container = builder.Build();
            return container;
        }
    }
}