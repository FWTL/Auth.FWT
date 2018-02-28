using System.Reflection;
using Auth.FWT.API.CQRS;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Infrastructure.Handlers;
using Autofac;
using StackExchange.Redis;

namespace QueueReceiver
{
    public class IocConfig
    {
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().InstancePerDependency();
            builder.RegisterGeneric(typeof(RedisJsonHandler<,>)).As(typeof(IWriteCacheHandler<,>)).InstancePerDependency();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>)).InstancePerDependency();

            builder.Register(b =>
            {
                return ConnectionMultiplexer.Connect(ConfigKeys.RedisConnectionString);
            }).SingleInstance();

            builder.Register(b =>
            {
                var redis = b.Resolve<ConnectionMultiplexer>();
                return redis.GetDatabase();
            }).InstancePerDependency();

            builder.RegisterType<Job>().InstancePerDependency();

            return builder.Build();
        }
    }
}