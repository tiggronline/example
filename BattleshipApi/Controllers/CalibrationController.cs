using Battleship.Api.Services;
using Battleship.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Battleship.Api.Controllers
{

    /// <summary>
    /// Controller for the turret.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        /// Saves the requested calibration settings.
        /// </summary>
        /// <param name="settings"><see cref="CalibrationSettings"/></param>
        /// <returns>Boolean</returns>
        [HttpPut("Settings")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<ActionResult<bool>> SaveCalibrationSettingsAsync(
            [Required, FromBody] CalibrationSettings settings
            )
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(SaveCalibrationSettingsAsync)}=>");

            bool result = default; // default==false

            try
            {
                if (settings is null)
                    throw new ArgumentNullException(nameof(settings));


                result = await _calibrationSvc.SaveCalibrationSettingsAsync(settings);
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(SaveCalibrationSettingsAsync)}<=");
            }

            return Ok(result);
        }

        /// <summary>
        /// Runs the configured calibration tests.
        /// </summary>
        /// <returns>IEnumerable of <see cref="CalibrationTestResult"/>s</returns>
        [HttpPost("Run")]
        [Produces("application/json", Type = typeof(IEnumerable<CalibrationTestResult>))]
        public async Task<ActionResult<IEnumerable<CalibrationTestResult>>> RunCalibrationAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(RunCalibrationAsync)}=>");

            IEnumerable<CalibrationTestResult> result = default;

            try
            {
                result = await _calibrationSvc.RunCalibrationAsync();
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(RunCalibrationAsync)}<=");
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the configured calibration tests.
        /// </summary>
        /// <returns><see cref="CalibrationSettings"/></returns>
        [HttpGet()]
        [Produces("application/json", Type = typeof(IEnumerable<CalibrationTestResult>))]
        public async Task<ActionResult<CalibrationSettings>> GetCalibrationAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(GetCalibrationAsync)}=>");

            CalibrationSettings result = default;

            try
            {
                result = await _calibrationSvc.GetCalibrationAsync();
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(GetCalibrationAsync)}<=");
            }

            return Ok(result);
        }

        #endregion PUBLIC MEMBERS

    }

}