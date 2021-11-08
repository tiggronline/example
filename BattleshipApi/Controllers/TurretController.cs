using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Battleship.Api.Controllers
{

    /// <summary>
    /// Controller for the turret.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TurretController : ControllerBase
    {

        /// <summary>
        /// ILogger of <see cref="TurretController"/>.
        /// </summary>
        private readonly ILogger<TurretController> _logger;


        /// <summary>
        /// Initialises this controller.
        /// </summary>
        /// <param name="logger">ILogger of <see cref="TurretController"/></param>
        public TurretController(
            ILogger<TurretController> logger
            )
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task SaveSettingsAsync()
        {
            await Task.FromException(
                new NotImplementedException() //TODO: Implement SaveSettingsAsync
                );
        }

    }

}