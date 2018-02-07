using Auth.FWT.Core.Data;
using Auth.FWT.Data;
using Autofac;

namespace Auth.Manage
{
    internal class Program
    {
        internal static ContainerBuilder ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>));
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork));

            builder.Register<IEntitiesContext>(b =>
            {
                var context = new AppContext("name=AppContext");
                return context;
            });

            builder.RegisterType<Application>().As<IApplication>();
            return builder;
        }

        private static void Main(string[] args)
        {
            IContainer _container = ConfigureContainer(new ContainerBuilder()).Build();
            _container.Resolve<IApplication>().Run();
        }
    }
}
