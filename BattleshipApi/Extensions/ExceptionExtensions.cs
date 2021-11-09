using System;
using System.Net.Http;
using System.Reflection;

namespace Battleship.API.Extensions
{

    /// <summary>
    /// Extensions for Exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {

        /// <summary>
        /// Returns a more useful exception if <paramref name="ex"/> is lacking.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Exception</returns>
        public static Exception GetUsefulException(
            this Exception ex
            )
        {
            if (ex is AggregateException aggEx)
                return aggEx.InnerException.GetUsefulException(); // Grab the first thing to go wrong

            else if (ex is TargetInvocationException invocEx)
                return invocEx.InnerException.GetUsefulException();

            else if (
                ex.Message.Contains("See the inner exception for details", StringComparison.OrdinalIgnoreCase) &&
                ex.InnerException is not null
                )
                return ex.InnerException.GetUsefulException();

            else if (
                ex is HttpRequestException &&
                ex.Message.Equals("Internal Server Error", StringComparison.OrdinalIgnoreCase) &&
                ex.InnerException is not null
                )
                return ex.InnerException.GetUsefulException();

            else
                return ex;
        }

    }

}