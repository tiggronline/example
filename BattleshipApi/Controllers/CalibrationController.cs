﻿using Battleship.Api.Services;
using Battleship.Model.Config;
using Battleship.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Battleship.Api.Controllers
{

    /// <summary>
    /// Controller for the turret.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
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
        /// <param name="settings" example="{'abc':'def'}"><see cref="CalibrationSettings"/></param>
        /// <returns>Boolean</returns>
        [HttpPut("Settings")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<ActionResult<bool>> SaveCalibrationSettingsAsync(
            [Required, FromBody] CalibrationSettings settings
            )
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(SaveCalibrationSettingsAsync)}=>");


            if (settings is null)
                throw new ArgumentNullException(nameof(settings));


            var result = await _calibrationSvc.SaveCalibrationSettingsAsync(settings);


            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(SaveCalibrationSettingsAsync)}<=");

            return Ok(result);
        }

        /// <summary>
        /// Runs the configured calibration tests.
        /// </summary>
        /// <returns>IEnumerable of <see cref="CalibrationTestResult"/>s</returns>
        [HttpPut("Run")]
        [Produces("application/json", Type = typeof(IEnumerable<CalibrationTestResult>))]
        public async Task<ActionResult<IEnumerable<CalibrationTestResult>>> RunCalibrationAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(RunCalibrationAsync)}=>");


            var result = await _calibrationSvc.RunCalibrationAsync();


            _logger.LogTrace($"{nameof(CalibrationController)}.{nameof(RunCalibrationAsync)}<=");

            return Ok(result);
        }

        #endregion PUBLIC MEMBERS

    }

}