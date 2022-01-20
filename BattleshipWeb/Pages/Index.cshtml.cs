using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Battleship.Web.Pages
{

    /// <summary>
    /// 
    /// </summary>
    public class IndexModel : PageModel
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public IndexModel(
            ILogger<IndexModel> logger
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