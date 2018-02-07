using Auth.FWT.API;
using Auth.FWT.Core;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Services.Dapper;
using Auth.FWT.Core.Services.Logging;
using Auth.FWT.CQRS;
using Auth.FWT.Data;
using Auth.FWT.Data.Dapper;
using Autofac;
using FluentValidation;
using Moq;
using Rws.Web.Core.CQRS;

namespace Auth.FWT.Tests
{
    public class TestIocConfig
    {
        public static ContainerBuilder RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(EntityRepository<,>)).As(typeof(IRepository<,>));
            builder.RegisterType(typeof(UnitOfWork)).As(typeof(IUnitOfWork));

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICommandHandler<>));
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(ICommandHandler<,>));

            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterAssemblyTypes(typeof(WebApiApplication).Assembly).AsClosedTypesOf(typeof(AbstractValidator<>));

            builder.Register<IDapperConnector>(b =>
            {
                var connectionString = ConfigKeys.ConnectionString;
                return new DapperConnector(connectionString);
            });

            builder.Register<IEntitiesContext>(b =>
            {
                var context = new FakeContext();
                return context;
            });

            builder.Register(b =>
            {
                var fakeLogger = new Mock<ILogger>();
                return fakeLogger.Object;
            }).SingleInstance();

            return builder;
        }
    }
}
