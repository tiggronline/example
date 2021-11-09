using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;

namespace Battleship.Web
{

    /// <summary>
    /// Entry class for the application.
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">String array of command line arguments pass when starting the app</param>
        public static int Main(
            string[] args
            )
        {
            try
            {
                WriteToDebugAndConsole($"Creating host...");

                var host = CreateHostBuilder(args)
                    .Build();

                WriteToDebugAndConsole($"Starting host...");

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteToDebugAndConsole(ex.ToString());
                Console.ResetColor();

#if DEBUG
                WriteToDebugAndConsole($"Press any key to exit");
                Console.ReadKey();
#endif

                return 1;
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }


        /// <summary>
        /// Creates and configures a host builder.
        /// </summary>
        /// <param name="args">String array of command line arguments pass when starting the app</param>
        /// <returns>IHostBuilder</returns>
        private static IHostBuilder CreateHostBuilder(
            string[] args
            )
            => Host
                .CreateDefaultBuilder(
                    args
                    )
                .ConfigureLogging(
                    logging => logging.ClearProviders()
                    )
                .UseSerilog(
                    (context, services, configuration) =>
                        configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .ReadFrom.Services(services),
                    writeToProviders: true
                    )
                .ConfigureWebHostDefaults(
                    webBuilder => webBuilder.UseStartup<Startup>()
                    );


        /// <summary>
        /// Writes the <paramref name="message"/> to the Debug Logger and Console.
        /// </summary>
        /// <param name="message">String message (optional)</param>
        /// <param name="args">Parameter Array of Object arguments</param>
        public static void WriteToDebugAndConsole(
            string message,
            params object[] args
            )
        {
            // Make the message as similar to our logging template as possible
            var formattedMessage = (args.Length == 0) ? message : string.Format(message, args);
            var fullMessage = $"{DateTime.Now:HH:mm:ss} INF {formattedMessage}";

            Debug.WriteLine(fullMessage);
            Console.WriteLine(fullMessage);
        }

    }

}