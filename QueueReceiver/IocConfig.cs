using Autofac;
using StackExchange.Redis;

namespace QueueReceiver
{
    public class IocConfig
    {
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

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