using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Battleship.Web.Pages
{

    /// <summary>
    /// Error PageNodel.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {

        /// <summary>
        /// ILogger of <see cref="ErrorModel"/> for logging.
        /// </summary>
        private readonly ILogger<ErrorModel> _logger;


        /// <summary>
        /// Initialises this class.
        /// </summary>
        /// <param name="logger">ILogger of <see cref="ErrorModel"/></param>
        public ErrorModel(
            ILogger<ErrorModel> logger
            )
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Gets or sets the ID of the request.
        /// </summary>
        /// <value>String</value>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets whether or not to show the <see cref="RequestId"/> (i.e. if it is non-blank).
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);


        /// <summary>
        /// Occurs when the user initiates a GET.
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ??
                HttpContext.TraceIdentifier;
        }

    }

}