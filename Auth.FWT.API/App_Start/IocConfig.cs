using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Auth.FWT.Core;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Services.Dapper;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Data;
using Auth.FWT.Data.Dapper;
using Auth.FWT.Infrastructure.Logging;
using Auth.FWT.Infrastructure.ServiceBus;
using Auth.FWT.Infrastructure.Telegram;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation;
using GitGud.API.Providers;
using GitGud.Web.Core.Providers;
using NodaTime;
using Rws.Web.Core.CQRS;
using StackExchange.Redis;
using TLSharp.Core;

namespace Auth.FWT.API
{
    public class IocConfig
    {
        private static IContainer _container;

        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>)).InstancePerRequest();
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork)).InstancePerRequest();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICommandHandler<>)).InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICommandHandler<,>)).InstancePerRequest();

            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>)).InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(AbstractValidator<>)).InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICachableHandler<,>)).InstancePerRequest();

            builder.Register<IDapperConnector>(b =>
            {
                var connectionString = ConfigKeys.ConnectionString;
                return new DapperConnector(connectionString);
            }).InstancePerRequest();

            builder.Register<IEntitiesContext>(b =>
            {
                var context = new Data.AppContext("name=AppContext");
                return context;
            }).InstancePerRequest();

            builder.Register(b => NLogLogger.Instance).SingleInstance();

            builder.Register<IUserProvider>(b =>
            {
                HttpRequestMessage httpRequestMessage = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
                return new UserProvider(httpRequestMessage);
            }).InstancePerRequest();

            builder.RegisterType<SQLSessionStore>().As<ISessionStore>().InstancePerRequest();

            builder.Register<ITelegramClient>(b =>
            {
                return new NewTelegramClient(b.Resolve<IUserSessionManager>(), b.Resolve<ISessionStore>(), ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash);
            }).InstancePerRequest();

            builder.Register<IUserSessionManager>(b =>
            {
                return AppUserSessionManager.Instance.UserSessionManager;
            }).SingleInstance();

            builder.Register(b =>
            {
                var manager = b.Resolve<IUserSessionManager>();
                var currentUserId = b.Resolve<IUserProvider>()?.CurrentUserId;
                return manager.Get(currentUserId.ToString(), b.Resolve<ISessionStore>());
            }).InstancePerRequest();

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
            }).InstancePerRequest();

            builder.Register<IServiceBus>(b =>
            {
                return new AzureServiceBus();
            }).InstancePerRequest();

            _container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            return _container;
        }
    }
}