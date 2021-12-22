using Battleship.Api.Repos;
using Battleship.Api.Services;
using Battleship.API.Infrastructure;
using Battleship.Model;
using Battleship.Model.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NeoSmart.Caching.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace Battleship.Api
{

    /// <summary>
    /// Application startup.
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// IConfiguration
        /// </summary>
        private readonly IConfiguration _configuration;


        /// <summary>
        /// Initialises this class.
        /// </summary>
        /// <param name="configuration">IConfiguration</param>
        public Startup(
            IConfiguration configuration
            )
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// Adds services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>Called by the runtime.</remarks>
        public void ConfigureServices(
            IServiceCollection services
            )
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));


            Program.WriteToDebugAndConsole($"Configuring services...");


            services
                .AddOptions<OptionsForTurrets>()
                    .Bind(_configuration.GetSection(OptionsForTurrets.SectionPath))
                        .ValidateDataAnnotations();


            services.AddSqliteCache(
                options =>
                {
                    options.CachePath = "Battleships.db";
                    options.CleanupInterval = new TimeSpan(1, 0, 0);
                }
                );


            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        };
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
#if DEBUG
                        options.SerializerSettings.Formatting = Formatting.Indented;
#else
                        options.SerializerSettings.Formatting = Formatting.None;
#endif
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    }
                    );

            services.AddSwaggerGen(options =>
                {
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
                    var serviceProvider = services.BuildServiceProvider(); //HACK: Use the IApiVersionDescriptionProvider configured in AddApiVersioning above
                    var apiVersionProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

                    // Generate swagger documentation for every discovered API version
                    foreach (var description in apiVersionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(
                            description.ApiVersion.ToString(),
                            new OpenApiInfo
                            {
                                Version = description.ApiVersion.ToString(),
                                Title = AppInfo.Product,
                                Description = $"{AppInfo.Description} for version {description.ApiVersion}. {(description.IsDeprecated ? " (DEPRECATED)" : "")}",
                                Contact = new OpenApiContact()
                                {
                                    Name = AppInfo.CompanyName,
                                    Email = AppInfo.CompanyEmail,
                                    Url = AppInfo.CompanyUri
                                }
                            }
                            );
                    }

                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // Use the .Net generated code comments for annotations (location is set in the project Properties->Build)
                    var asmDocFile = $"{AppInfo.RootPath}\\{AppInfo.Name}.xml";

                    if (!File.Exists(asmDocFile))
                        asmDocFile = $"{AppInfo.Name}.xml";

                    if (!File.Exists(asmDocFile))
                        Program.WriteToDebugAndConsole($"WARNING: XML Documentation file '{asmDocFile}' not found for Swagger!");

                    if (File.Exists(asmDocFile))
                    {
                        Program.WriteToDebugAndConsole($"Using XML Documentation file '{asmDocFile}' for Swagger");
                        options.IncludeXmlComments(
                            asmDocFile,
                            includeControllerXmlComments: true
                            );
                    }
                }
                );


            // Consifure dependency injection
            services
                .AddSingleton<CacheRepo>();

            services
                .AddScoped<CalibrationService>();
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        /// <param name="logger">ILogger of <see cref="Startup"/></param>
        /// <param name="apiVersionProvider">IApiVersionDescriptionProvider</param>
        /// <remarks>Called by the runtime.</remarks>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            IApiVersionDescriptionProvider apiVersionProvider
            )
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            if (env is null)
                throw new ArgumentNullException(nameof(env));

            if (logger is null)
                throw new ArgumentNullException(nameof(logger));


            // Show the console what is being logged
            Program.WriteToDebugAndConsole($"Logging:");
            Program.WriteToDebugAndConsole($"  Trace={logger.IsEnabled(LogLevel.Trace)}");
            Program.WriteToDebugAndConsole($"  Debug={logger.IsEnabled(LogLevel.Debug)}");
            Program.WriteToDebugAndConsole($"  Information={logger.IsEnabled(LogLevel.Information)}");
            Program.WriteToDebugAndConsole($"  Warning={logger.IsEnabled(LogLevel.Warning)}");
            Program.WriteToDebugAndConsole($"  Error={logger.IsEnabled(LogLevel.Error)}");
            Program.WriteToDebugAndConsole($"  Critical={logger.IsEnabled(LogLevel.Critical)}");


            // Use SeriLog for streamlined HTTP request logging.
            // Must be called before handlers such as MVC (will not time or log components that appear before it in the pipeline).
            Program.WriteToDebugAndConsole($"Configuring SerilogRequestLogging...");

            app.UseSerilogRequestLogging(); //Start the logging before anything else so as much as possible gts logged


            if (env.IsDevelopment())
            {
                //Do NOT use the DEP because it interferes with the HttpInterceptorMiddleware
                //app.UseDeveloperExceptionPage()

                Program.WriteToDebugAndConsole($"Configuring Swagger...");

                app.UseSwagger()
                    .UseSwaggerUI(
                        options =>
                        {
                            // Generate a swagger endpoint for every discovered API version order by latest first (so it appears first in the Swagger dropdown)
                            foreach (var description in apiVersionProvider.ApiVersionDescriptions.OrderByDescending(d => d.ApiVersion))
                            {
                                options.SwaggerEndpoint(
                                    $"/swagger/{description.GroupName}/swagger.json",
                                    $"v{description.GroupName}{(description.IsDeprecated ? " (deprecated)" : "")}"
                                    );
                            }
                        }
                        );
            }


            //Use custom Exception Handler for structured error responses
            Program.WriteToDebugAndConsole($"Configuring error handling...");

            app.UseHttpInterceptorMiddleware();


            Program.WriteToDebugAndConsole($"Configuring pipeline...");

            app.UseHttpsRedirection()
                .UseRouting()
                .UseEndpoints(endpoints =>
                    endpoints.MapControllers() // Handle requests matching MVC Endpoints
                );

        }

    }

}