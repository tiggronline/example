using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Battleship.Web
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


            Program.WriteToDebugAndConsole("Configuring services...");


            Program.WriteToDebugAndConsole(" Configuring Razor...");

            services.AddRazorPages();

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
            Program.WriteToDebugAndConsole($" {logger.GetType().Name}:");
            Program.WriteToDebugAndConsole($"  Trace={logger.IsEnabled(LogLevel.Trace)}");
            Program.WriteToDebugAndConsole($"  Debug={logger.IsEnabled(LogLevel.Debug)}");
            Program.WriteToDebugAndConsole($"  Information={logger.IsEnabled(LogLevel.Information)}");
            Program.WriteToDebugAndConsole($"  Warning={logger.IsEnabled(LogLevel.Warning)}");
            Program.WriteToDebugAndConsole($"  Error={logger.IsEnabled(LogLevel.Error)}");
            Program.WriteToDebugAndConsole($"  Critical={logger.IsEnabled(LogLevel.Critical)}");


            // Use SeriLog for streamlined HTTP request logging.
            // Must be called before handlers such as MVC (will not time or log components that appear before it in the pipeline).
            logger.LogInformation("Configuring SerilogRequestLogging...");

            app.UseSerilogRequestLogging(); //Start the logging before anything else so as much as possible gts logged


            logger.LogInformation(" Configuring error handling...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
            }


            logger.LogInformation(" Configuring pipeline...");

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints =>
                    endpoints.MapRazorPages()
                    );

        }

    }

}