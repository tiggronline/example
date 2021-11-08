using Battleship.Model.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NeoSmart.Caching.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        /// Initialises this class.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>IConfiguration</value>
        private IConfiguration _configuration { get; }

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
                    { //Configure JSON
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
                .AddSingleton<CacheRepository>()
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        /// <remarks>Called by the runtime.</remarks>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BattleshipApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }

}