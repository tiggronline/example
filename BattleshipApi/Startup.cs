using Battleship.Api.Repos;
using Battleship.Api.Services;
using Battleship.API.Infrastructure;
using Battleship.Model.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

namespace Battleship.Api
{

    /// <summary>
    /// Application startup.
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>IConfiguration</value>
        private IConfiguration Configuration { get; }


        /// <summary>
        /// Initialises this class.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(
            IConfiguration configuration
            )
        {
            Configuration = configuration;
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
                    .Bind(Configuration.GetSection(OptionsForTurrets.SectionPath))
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
                    options.SwaggerDoc(
                        "v1", // MUST be v1
                        new OpenApiInfo
                        {
                            Title = AppInfo.Product,
                            Description = AppInfo.Description,
                            Version = AppInfo.Version.ToString(fieldCount: 3),
                            Contact = new OpenApiContact()
                            {
                                Name = AppInfo.CompanyName
                            }
                        }
                        );
                    //options.UseInlineDefinitionsForEnums();

                    // Use the .Net generated code comments for annotations (location is set in the project Properties->Build)
                    var asmDocFile = $"{AppInfo.RootPath}\\{AppInfo.Name}.xml";
                    if (!File.Exists(asmDocFile))
                        asmDocFile = $"{AppInfo.Name}.xml";

                    if (File.Exists(asmDocFile))
                        options.IncludeXmlComments(
                            asmDocFile,
                            includeControllerXmlComments: true
                            );
                }
                );


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
        /// <remarks>Called by the runtime.</remarks>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger
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
                Program.WriteToDebugAndConsole($"Configuring Dev Environment (exceptions & Swagger)...");

                app //.UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(
                        c => c.SwaggerEndpoint(
                                "/swagger/v1/swagger.json",
                                $"{AppInfo.Product} v{AppInfo.Version.ToString(2)}"
                                )
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