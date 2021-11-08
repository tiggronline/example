using Battleship.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Battleship.API.Infrastructure
{

    /// <summary>
    /// HttpInterceptor Middleware
    /// </summary>
    public class HttpInterceptorMiddleware
    {

        /// <summary>
        /// RequestDelegate
        /// </summary>
        private readonly RequestDelegate _nextDelegate;

        /// <summary>
        /// ILogger of <see cref="HttpInterceptorMiddleware"/> for logging.
        /// </summary>
        private readonly ILogger<HttpInterceptorMiddleware> _logger;


        /// <summary>
        /// Initialise this class.
        /// </summary>
        /// <param name="nextDelegate">Request Delegate to continue request pipeline</param>
        /// <param name="logger">ILogger of <see cref="HttpInterceptorMiddleware"/> for logging</param>
        public HttpInterceptorMiddleware(
            RequestDelegate nextDelegate,
            ILogger<HttpInterceptorMiddleware> logger
            )
        {
            _nextDelegate = nextDelegate ??
                throw new ArgumentNullException(nameof(nextDelegate));

            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Middleware invoked as part of HttpRequest pipeline asynchronously.
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <remarks>EVERY request is intercepted here, so any unhandled exception in the
        ///  request call stack will bubble and be caught in this CATCH</remarks>
        public async Task InvokeAsync(
            HttpContext context
            )
        {
            // This is the last bastion of defence so EVERYTHING inside the TryCatch!
            try
            {
                if (context is null)
                    throw new ArgumentNullException(nameof(context));

                // Continue with Request pipeline
                await _nextDelegate.Invoke(context);
            }
            catch (Exception ex)
            {
                _ = HandleContextException(ex, context);
            }
        }


        /// <summary>
        /// Handles the <paramref name="exception"/> in the HttpRequest pipeline into a desensitized
        ///  structured response.
        /// We only want to expose information that can help a user resolve an issue with no details
        ///  on the implmentation of the code
        /// For example we want a user to know that a filename was too long but not that an AutoMapper 
        ///  could not map the filename into a smaller field.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="context">HttpContext</param>
        private async Task HandleContextException(
            Exception exception,
            HttpContext context
            )
        {
            // This is the last bastion of defence so EVERYTHING inside the TryCatch!
            try
            {
                if (exception is null)
                    exception = new Exception($"{nameof(exception)} cannot be null!", exception); // Do NOT use ArgumentNullException so that a 500 error is returned

                if (context is null)
                    exception = new Exception($"{nameof(context)} cannot be null!", exception); // Do NOT use ArgumentNullException so that a 500 error is returned


                _logger.LogError(exception, exception.Message);

                //TODO: Review API exception types and provide detailed responses accordingly

                // If we have a silly exception, try to get a more meaningful exception from it's InnerException
                var usefulException = exception.GetUsefulException();

                var statusCode = usefulException switch
                {
                    HttpRequestException => usefulException.Message.Equals("The object specified does not belong to a list.", StringComparison.OrdinalIgnoreCase) ?
                                                HttpStatusCode.NotFound :
                                                ((HttpRequestException)usefulException).StatusCode.Value,
                    ArgumentNullException => HttpStatusCode.BadRequest,
                    ArgumentException => HttpStatusCode.BadRequest,
                    FileNotFoundException => HttpStatusCode.NotFound,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    //LockedException => HttpStatusCode.Locked,
                    NotImplementedException => HttpStatusCode.NotImplemented,
                    TaskCanceledException => context.RequestAborted.IsCancellationRequested ?
                                                    HttpStatusCode.RequestTimeout :
                                                    HttpStatusCode.GatewayTimeout,
                    _ => HttpStatusCode.InternalServerError
                };

                var message = statusCode switch
                {
                    HttpStatusCode.BadRequest => $"Bad request - {usefulException.Message}!",                               // 400
                    HttpStatusCode.Forbidden => "You do not have access to that feature!",                                  // 403
                    HttpStatusCode.NotFound => "Not found - the requested item may have been moved, renamed or deleted!",   // 404
                    HttpStatusCode.RequestTimeout => "The request was cancelled - please try again",                        // 408
                    HttpStatusCode.Locked =>                                                                                // 423
                        usefulException.InnerException?.Message is null ? 
                            $"Locked for use by someone else!" :
                            usefulException.InnerException?.Message, //Return actual error
                    HttpStatusCode.UnsupportedMediaType => $"Unsupported media type - {usefulException.Message}!",          // 415
                    HttpStatusCode.GatewayTimeout => $"The request timed out - please try again later",                     // 504
                    HttpStatusCode.NotImplemented => $"That feature has not been implemented - {usefulException.Message}!", // 501
                    HttpStatusCode.InternalServerError => "Something really unexpected just occurred!",                     // 500
                    _ => exception.Message
                };

                var result = JsonConvert.SerializeObject(
                    new
                    {
                        StatusCode = (int)statusCode,
                        Message = message
                    },
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    }
                    );

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
#if DEBUG
                // Stop the code here for ALL developers because the code above has not been extensively tested (very difficult to test all of this)
                // Because we're not using Windows the message will be logged to the Output Window (instead of a MessageBox)
                Debug.Assert(false, $"Unhandled exception in {nameof(HttpInterceptorMiddleware)}!");
#endif

                _logger.LogError(ex, $"Failed to handle exception ({ex?.Message}) in middleware ({ex?.Message})!");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Something REALLY unexpected went wrong!");
            }
        }

    }


    /// <summary>
    /// IApplicationBuilder Middleware extensions.
    /// </summary>
    public static class HttpInterceptorMiddlewareExtensions
    {

        /// <summary>
        /// Registers the <see cref="HttpInterceptorMiddleware"/> class as Middleware for the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">IApplicationBuilder</param>
        public static void UseHttpInterceptorMiddleware(
            this IApplicationBuilder builder
            )
            => builder.UseMiddleware<HttpInterceptorMiddleware>();

    }

}