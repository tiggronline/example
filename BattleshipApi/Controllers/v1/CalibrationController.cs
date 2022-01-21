using Battleship.Api.Services;
using Battleship.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Battleship.Api.Controllers.v1
{

    /// <summary>
    /// Controller for the turret.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")] // Need to specify this on EVERY class, parent & derived, to get picked up by the versioning
    public class CalibrationController : ControllerBase
    {

        #region ==================== PRIVATE FIELDS ====================

        /// <summary>
        /// ILogger of <see cref="CalibrationController"/>.
        /// </summary>
        private readonly ILogger<CalibrationController> _logger;

        /// <summary>
        /// <see cref="CalibrationService"/>.
        /// </summary>
        private readonly CalibrationService _calibrationSvc;

        #endregion PRIVATE FIELDS


        /// <summary>
        /// Initialises this controller.
        /// </summary>
        /// <param name="logger">ILogger of <see cref="CalibrationController"/></param>
        /// <param name="calibrationSvc"><see cref="CalibrationService"/></param>
        public CalibrationController(
            ILogger<CalibrationController> logger,
            CalibrationService calibrationSvc
            )
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            _calibrationSvc = calibrationSvc ??
                throw new ArgumentNullException(nameof(calibrationSvc));
        }


        #region ==================== PUBLIC MEMBERS ====================

        /// <summary>
        /// Gets the application information.
        /// </summary>
        /// <returns><see cref="CalibrationSettings"/></returns>
        [HttpGet()]
        [Produces("application/json", Type = typeof(string))]
        public async Task<ActionResult<CalibrationSettings>> GetAppInfoAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(GetAppInfoAsync)}=>");

            string result = default;

            try
            {
                result = await Task.FromResult("Battleship v1");
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(GetAppInfoAsync)}<=");
            }

            return Ok(result);
        }

        #endregion PUBLIC MEMBERS

    }

}