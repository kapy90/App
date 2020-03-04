using System;
using System.Collections.Generic;
using System.Linq;
using App.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace App.Swagger
{
    public static class SwaggerConfigurer
    {
        private static List<OpenApiInfo> openApiInfos = new List<OpenApiInfo>();
        private static ConfigOptions swaggerOptions;

        private static void FindSwaggerApiExplorerAttribute(this IServiceCollection services)
        {
            services.LocateController();
        }

        private static void LocateController(this IServiceCollection services)
        {
            var typeFinder = services.BuildServiceProvider().GetService<ITypeFinder>();
            var controllers = typeFinder.FindClassesOfType<Microsoft.AspNetCore.Mvc.Controller>();
            controllers.ToList().ForEach(x =>
            {
                FindAttribute(x);
            });
        }

        private static void FindAttribute(Type x)
        {
            var swaggerApiExplorerAttributes = (SwaggerApiExplorerAttribute[])x.GetCustomAttributes(typeof(SwaggerApiExplorerAttribute), false);

            if (swaggerApiExplorerAttributes != null && swaggerApiExplorerAttributes.Length > 0)
            {
                var groupName = swaggerApiExplorerAttributes[0].GroupName;
                var ignoreApi = swaggerApiExplorerAttributes[0].IgnoreApi;
                if (!ignoreApi)
                {
                    if (openApiInfos.Count(info => info.Description == groupName) > 0) return;
                    openApiInfos.Add(new OpenApiInfo
                    {
                        Title = swaggerOptions.Title,
                        Version = swaggerOptions.Version,
                        Description = groupName
                    });
                }
            }
        }


        public static void AddAppSwagger(this IServiceCollection services, IConfiguration configuration, Action<SwaggerGenOptions> setupAction = null)
        {
            swaggerOptions = services.ConfigureStartupConfig<ConfigOptions>(configuration.GetSection("Swagger"));

            if (!swaggerOptions.IsEnabled) return;

            services.FindSwaggerApiExplorerAttribute();

            if (openApiInfos.Count == 0) return;

            services.AddScoped<SwaggerGenerator>();
            services.AddSwaggerGen(options =>
            {
              
                openApiInfos.ForEach(x =>
                {
                    options.SwaggerDoc(x.Description, x);
                });

                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.EnableAnnotations();

                options.DocInclusionPredicate((docName, apiDesc) => docName == apiDesc.GroupName);

                if(setupAction !=null)
                    setupAction(options);
            });
        }

        public static void UseAppSwagger(this IApplicationBuilder app, Action<SwaggerUIOptions> setupAction = null)
        {
             
            if (!swaggerOptions.IsEnabled) return;
            if (openApiInfos.Count == 0) return;

            if (!string.IsNullOrWhiteSpace(swaggerOptions.SwaggerLockToken))
            {
                app.UseSwaggerLock(swaggerOptions.SwaggerLockToken);
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                openApiInfos.ForEach(x =>
                {
                    c.SwaggerEndpoint($"/swagger/{x.Description}/swagger.json", x.Description);
                });

                if (setupAction != null)
                    setupAction(c);
            });
        }

    }
}
