using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using FWTL.Infrastructure.Configuration;
using FWTL.Infrastructure.Filters;
using FWTL.Infrastructure.IdentityServer;
using FWTL.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace FWTL.Auth
{


    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(hostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            _configuration = configuration.Build();

            configuration.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true);
            _configuration = configuration.Build();

            if (!hostingEnvironment.IsDevelopment())
            {
                configuration.Add(new AzureSecretsVaultSource(_configuration["AzureKeyVault:App:BaseUrl"], _configuration["AzureKeyVault:App:ClientId"], _configuration["AzureKeyVault:App:SecretId"]));
                _configuration = configuration.Build();
            }

            _hostingEnvironment = hostingEnvironment;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FWT.Auth");
                c.DisplayRequestDuration();
            });

            app.UseMvc(routes =>
            {
            });

            ValidatorOptions.LanguageManager.Enabled = false;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ApiExceptionAttribute));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "FWT.Auth", Version = "v1" });
                c.MapType<Guid>(() => new Schema() { Type = "string", Format = "text", Description = "GUID" });

                c.OperationFilter<AuthorizeOperationFilter>();
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
                options.DescribeAllEnumsAsStrings();
            });

            var identityServerBuilder = services.AddIdentityServer()
                .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients(_configuration))
                .AddCustomTokenRequestValidator<TokenRequestValidator>();

            if (_hostingEnvironment.IsDevelopment())
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                identityServerBuilder.AddSigningCredentialFromAzureKeyVault(
                    _configuration["AzureKeyVault:App:BaseUrl"],
                    _configuration["AzureKeyVault:App:ClientId"],
                    _configuration["AzureKeyVault:App:SecretId"],
                    "SigningCert",
                    30);
            }

            IContainer applicationContainer = IocConfig.RegisterDependencies(services, _hostingEnvironment, _configuration);
            return new AutofacServiceProvider(applicationContainer);
        }
    }
}
