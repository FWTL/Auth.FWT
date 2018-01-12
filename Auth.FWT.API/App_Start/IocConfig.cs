using System;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Auth.FWT.API.Providers;
using Auth.FWT.Core;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Services.Dapper;
using Auth.FWT.CQRS;
using Auth.FWT.Data;
using Auth.FWT.Data.Dapper;
using Auth.FWT.Infrastructure.Logging;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation;
using GitGud.API.Providers;
using GitGud.Web.Core.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Rws.Web.Core.CQRS;

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

            _container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            return _container;
        }
    }
}