using Battleship.Api.Repos;
using Battleship.Api.Services;
using Battleship.API.Infrastructure;
using Battleship.Model;
using Battleship.Model.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NeoSmart.Caching.Sqlite;
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
        /// <remarks>Called by the runtime</remarks>
        public void ConfigureServices(
            IServiceCollection services
            )
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));


            Program.WriteToDebugAndConsole("Configuring services...");


            Program.WriteToDebugAndConsole(" Configuring settings...");

            services
                .AddOptions<OptionsForTurrets>()
                    .Bind(_configuration.GetSection(OptionsForTurrets.SectionPath))
                        .ValidateDataAnnotations();

            services
                .AddOptions<OptionsForJson>()
                    .Bind(_configuration.GetSection(OptionsForJson.SectionPath))
                        .ValidateDataAnnotations();


            Program.WriteToDebugAndConsole(" Configuring caching...");

            services.AddSqliteCache(
                options =>
                {
                    options.CachePath = "Battleships.db";
                    options.CleanupInterval = new TimeSpan(1, 0, 0);
                }
                );


            Program.WriteToDebugAndConsole(" Configuring controllers...");

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        // Settings for all serialized return values
                        OptionsForJson jsonOptions = new();
                        _configuration
                            .GetSection(OptionsForJson.SectionPath)
                                .Bind(jsonOptions);

                        options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        };
                        options.SerializerSettings.CheckAdditionalContent = jsonOptions.Serializing.CheckAdditionalContent;
                        options.SerializerSettings.ConstructorHandling = jsonOptions.Serializing.ConstructorHandling;
                        options.SerializerSettings.DateFormatHandling = jsonOptions.Serializing.DateFormatHandling;
                        options.SerializerSettings.DateTimeZoneHandling = jsonOptions.Serializing.DateTimeZoneHandling;
                        options.SerializerSettings.DateFormatString = jsonOptions.Serializing.DateFormatString;
                        options.SerializerSettings.DateParseHandling = jsonOptions.Serializing.DateParseHandling;
                        options.SerializerSettings.DefaultValueHandling = jsonOptions.Serializing.DefaultValueHandling;
                        options.SerializerSettings.Formatting = jsonOptions.Serializing.Formatting;
                        options.SerializerSettings.FloatFormatHandling = jsonOptions.Serializing.FloatFormatHandling;
                        options.SerializerSettings.FloatParseHandling = jsonOptions.Serializing.FloatParseHandling;
                        options.SerializerSettings.MaxDepth = jsonOptions.Serializing.MaxDepth;
                        options.SerializerSettings.MetadataPropertyHandling = jsonOptions.Serializing.MetadataPropertyHandling;
                        options.SerializerSettings.MissingMemberHandling = jsonOptions.Serializing.MissingMemberHandling;
                        options.SerializerSettings.NullValueHandling = jsonOptions.Serializing.NullValueHandling;
                        options.SerializerSettings.ObjectCreationHandling = jsonOptions.Serializing.ObjectCreationHandling;
                        options.SerializerSettings.PreserveReferencesHandling = jsonOptions.Serializing.PreserveReferencesHandling;
                        options.SerializerSettings.ReferenceLoopHandling = jsonOptions.Serializing.ReferenceLoopHandling;
                        options.SerializerSettings.StringEscapeHandling = jsonOptions.Serializing.StringEscapeHandling;
                        options.SerializerSettings.TypeNameAssemblyFormatHandling = jsonOptions.Serializing.TypeNameAssemblyFormatHandling;
                        options.SerializerSettings.TypeNameHandling = jsonOptions.Serializing.TypeNameHandling;
                        if (jsonOptions.Serializing.SerializeEnumsAsStrings)
                            options.SerializerSettings.Converters.Add(
                                new Newtonsoft.Json.Converters.StringEnumConverter()
                                );
                    }
                        );


            Program.WriteToDebugAndConsole(" Configuring API versioning...");

            services
                .AddApiVersioning(
                   options =>
                   {
                       // Provide api-supported_versions and api-deprecated-versions in the response headers
                       options.ReportApiVersions = true;

                       // Default to the latest version
                       options.DefaultApiVersion = new ApiVersion(2, 0);
                       options.AssumeDefaultVersionWhenUnspecified = true;

                       // Use a header value to allow callers to target a specific API version in the header other than the default set above
                       options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                   }
                    )
                .AddVersionedApiExplorer( // Add a version-aware explorer
                    options =>
                    {
                        //options.GroupNameFormat = "'v'VVV"; // Format the version as "'v'major[.minor][-status]"
                        //options.SubstituteApiVersionInUrl = true; // Subsitute the version when using url segments (the SubstitutionFormat can also be used to control the format of the API version in route templates)
                    }
                    );


            Program.WriteToDebugAndConsole(" Configuring Swagger...");

            services
                .AddOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>()
                .Configure<IApiVersionDescriptionProvider>((options, provider) =>
                {
                    // Generate swagger documentation for every discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
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
                });

            services.AddSwaggerGen(options =>
                {
                    //HACK: Map the IFormCollection in Swagger to a normal file upload so Swagger will show a file upload button (Angular Flow uploads multiple form parts including the normal upload)
                    options.MapType(
                        typeof(IFormCollection),
                        () => new OpenApiSchema()
                        {
                            Type = "file",
                            Format = "binary"
                        }
                        );

                    // Add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();


                    // Use the .Net generated code comments for annotations (location is set in the project Properties->Build)
                    var asmDocFile = $"{AppInfo.RootPath}\\{AppInfo.Name}.xml";

                    if (!File.Exists(asmDocFile))
                        asmDocFile = $"{AppInfo.Name}.xml";

                    if (!File.Exists(asmDocFile))
                        Program.WriteToDebugAndConsole($"WARNING: XML Documentation file '{asmDocFile}' not found for Swagger!");

                    else
                    {
                        Program.WriteToDebugAndConsole($"Using XML Documentation file '{asmDocFile}' for Swagger");

                        options.IncludeXmlComments(
                            asmDocFile,
                            includeControllerXmlComments: true
                            );
                    }
                }
                );


            Program.WriteToDebugAndConsole(" Configuring DI...");

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
        /// <param name="apiProv">IApiVersionDescriptionProvider</param>
        /// <param name="logger">ILogger of <see cref="Startup"/> to test the logging configuration</param>
        /// <remarks>Called by the runtime</remarks>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiProv,
            ILogger<Startup> logger
            )
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            if (env is null)
                throw new ArgumentNullException(nameof(env));

            if (apiProv is null)
                throw new ArgumentNullException(nameof(apiProv));

            if (logger is null)
                throw new ArgumentNullException(nameof(logger));


            Program.WriteToDebugAndConsole(" Configuring HTTP request pipeline...");


            Program.WriteToDebugAndConsole($" {logger.GetType().Name}:");
            Program.WriteToDebugAndConsole($"  Trace={logger.IsEnabled(LogLevel.Trace)}");
            Program.WriteToDebugAndConsole($"  Debug={logger.IsEnabled(LogLevel.Debug)}");
            Program.WriteToDebugAndConsole($"  Information={logger.IsEnabled(LogLevel.Information)}");
            Program.WriteToDebugAndConsole($"  Warning={logger.IsEnabled(LogLevel.Warning)}");
            Program.WriteToDebugAndConsole($"  Error={logger.IsEnabled(LogLevel.Error)}");
            Program.WriteToDebugAndConsole($"  Critical={logger.IsEnabled(LogLevel.Critical)}");


            // Use SeriLog for streamlined HTTP request logging.
            // Must be called before handlers such as MVC (will not time or log components that appear before it in the pipeline).
            logger.LogInformation(" Configuring Serilog request logging...");

            app.UseSerilogRequestLogging(); //Start the logging before anything else so as much as possible gts logged


            if (env.IsDevelopment())
            {
                //Do NOT use the DEP because it interferes with the HttpInterceptorMiddleware
                //app.UseDeveloperExceptionPage()

                logger.LogInformation(" Configuring Swagger...");

                app.UseSwagger()
                    .UseSwaggerUI(
                        options =>
                        {
                            // Generate a swagger endpoint for every discovered API version order by latest first (so it appears first in the Swagger dropdown)
                            foreach (var description in apiProv.ApiVersionDescriptions.OrderByDescending(d => d.ApiVersion))
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
            logger.LogInformation(" Configuring error handling...");

            app.UseHttpInterceptorMiddleware();


            logger.LogInformation(" Configuring pipeline...");

            app.UseHttpsRedirection()
                .UseRouting()
                .UseEndpoints(
                    endpoints => endpoints.MapControllers() // Handle requests matching MVC Endpoints
                    );

        }

    }

}