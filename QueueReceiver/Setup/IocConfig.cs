using System.Reflection;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Data;
using Auth.FWT.Events.Dispatcher;
using Autofac;
using NodaTime;
using StackExchange.Redis;

namespace QueueReceiver
{
    public class IocConfig
    {
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork)).InstancePerDependency();

            builder.Register<IEntitiesContext>(b =>
            {
                var context = new AppContext("name=AppContext");
                return context;
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

            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().InstancePerDependency();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>)).InstancePerDependency();

            builder.RegisterType<Job>().InstancePerDependency();

            return builder.Build();
        }
    }
}