using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Battleship.Web.Pages
{

    /// <summary>
    /// 
    /// </summary>
    public class PrivacyModel : PageModel
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<PrivacyModel> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public PrivacyModel(
            ILogger<PrivacyModel> logger
            )
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
        }

    }

}