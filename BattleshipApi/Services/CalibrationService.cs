using Battleship.Api.Repos;
using Battleship.Model.Config;
using Battleship.Model.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Battleship.Api.Services
{

    /// <summary>
    /// Business logic for manipulating turrets.
    /// </summary>
    public class CalibrationService
    {

        #region ==================== PRIVATE FIELDS ====================

        /// <summary>
        /// ILogger of <see cref="CalibrationService"/>.
        /// </summary>
        private readonly ILogger<CalibrationService> _logger;

        /// <summary>
        /// IOptions of <see cref="OptionsForTurrets"/>.
        /// </summary>
        private readonly OptionsForTurrets _turretOptions;

        /// <summary>
        /// <see cref="CacheRepo"/>.
        /// </summary>
        private readonly CacheRepo _cacheRepo;

        /// <summary>
        /// String key for calibration settings.
        /// </summary>
        private const string _calibrationSettingsKey = "CalibrationSettings";

        #endregion PRIVATE FIELDS



        /// <summary>
        /// Initialises this controller.
        /// </summary>
        /// <param name="logger">ILogger of <see cref="CalibrationService"/></param>
        /// <param name="turretOptions"><see cref="OptionsForTurrets"/></param>
        /// <param name="cacheRepo">CacheRepo</param>
        public CalibrationService(
            ILogger<CalibrationService> logger,
            IOptions<OptionsForTurrets> turretOptions,
            CacheRepo cacheRepo
            )
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            _turretOptions = turretOptions?.Value ??
                throw new ArgumentNullException(nameof(turretOptions));

            _cacheRepo = cacheRepo ??
                throw new ArgumentNullException(nameof(cacheRepo));
        }


        #region ==================== PUBLIC MEMBERS ====================

        /// <summary>
        /// Saves the requested calibration settings.
        /// </summary>
        /// <returns>Boolean</returns>
        public async Task<bool> SaveCalibrationSettingsAsync(
            CalibrationSettings settings
            )
        {
            _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(SaveCalibrationSettingsAsync)}=>");

            bool result = default; // default==false

            try
            {
                if (settings is null)
                    throw new ArgumentNullException(nameof(settings));


                if (settings.Turrets is null || !settings.Turrets.Any())
                    throw new ArgumentException($"No turret details were supplied!");

                if (settings.Sequence is null || !settings.Sequence.Any())
                    throw new ArgumentException($"No turret test sequence was supplied!");

                if (settings.Turrets.Select(t => t.Id).Distinct().Count() != settings.Turrets.Count())
                    throw new ArgumentException($"Duplicate {nameof(Turret)}.{nameof(Turret.Id)} found!", nameof(settings));

                foreach (var turret in settings.Turrets)
                    ValidateTurret(turret);

                ValidateTurretSequencing(
                    turrets: settings.Turrets,
                    sequenceTurretIds: settings.Sequence
                    );


                await _cacheRepo.SetAsync(_calibrationSettingsKey, settings);


                result = true;
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(SaveCalibrationSettingsAsync)}<=");
            }

            return result;
        }

        /// <summary>
        /// Runs the configured calibration tests.
        /// </summary>
        /// <returns>IEnumerable of <see cref="CalibrationTestResult"/>s</returns>
        public async Task<IEnumerable<CalibrationTestResult>> RunCalibrationAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(RunCalibrationAsync)}=>");

            IEnumerable<CalibrationTestResult> result = default;

            try
            {
                var settings = await _cacheRepo.GetAsync<CalibrationSettings>(
                    _calibrationSettingsKey
                    );

                if (settings is null)
                    throw new InvalidOperationException("No calibration settings have been configured!");


                Dictionary<int, CalibrationTestResult> testResults = new();

                var turrets = settings.Turrets.ToDictionary(
                    t => t.Id,
                    t => t
                    );

                foreach (var turretId in settings.Sequence)
                {
                    _logger.LogDebug($"Testing calibration for turret {turretId}...");

                    var testResult = DoTest( // TODO: Handle test failures and feed this back to the user
                        turrets[turretId]
                        );

                    if (testResults.TryGetValue(turretId, out var existingTestResult))
                        testResults[turretId] = testResult + existingTestResult;
                    else
                        testResults.Add(turretId, testResult);

                    _logger.LogDebug($"Testing calibration for turret {turretId} completed successfully");
                }

                result = testResults.Values; //TODO: We could apply some kind of ordering here.
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(RunCalibrationAsync)}<=");
            }

            return result;
        }

        /// <summary>
        /// Returns the configured calibration tests.
        /// </summary>
        /// <returns><see cref="CalibrationSettings"/></returns>
        public async Task<CalibrationSettings> GetCalibrationAsync()
        {
            _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(GetCalibrationAsync)}=>");

            CalibrationSettings result = default;

            try
            {
                result = await _cacheRepo.GetAsync<CalibrationSettings>(
                    _calibrationSettingsKey
                    );
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CalibrationService)}.{nameof(GetCalibrationAsync)}<=");
            }

            return result;
        }

        #endregion PUBLIC MEMBERS


        #region ==================== WORKER FUNCTIONS ====================

        /// <summary>
        /// Validates the properties of the requested <paramref name="turret"/>.
        /// </summary>
        /// <param name="turret"><see cref="Turret"/></param>
        /// <returns>Boolean True if the properties are all valid</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="turret"/> is blank</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the properties exceed the specifications in <see cref="_turretOptions"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="turret"/> start angle is greater than the end angle</exception>
        private bool ValidateTurret(
            Turret turret
            )
        {
            if (turret is null)
                throw new ArgumentNullException(nameof(turret));


            if (turret.Caliber < _turretOptions.MinCaliber)
                throw new ArgumentOutOfRangeException($"Caliber for turret {turret.Id} ({turret.Caliber}) must be greater than or equal to {_turretOptions.MinCaliber}!");

            if (turret.Caliber > _turretOptions.MaxCaliber)
                throw new ArgumentOutOfRangeException($"Caliber for turret {turret.Id} ({turret.Caliber}) must be less than or equal to {_turretOptions.MaxCaliber}!");


            if (turret.RotationStartAngle < _turretOptions.MinAngle)
                throw new ArgumentOutOfRangeException($"Rotation start angle for turret {turret.Id} ({turret.RotationStartAngle}) must be greater than or equal to {_turretOptions.MinAngle}!");

            if (turret.RotationStartAngle > _turretOptions.MaxAngle)
                throw new ArgumentOutOfRangeException($"Rotation start angle for turret {turret.Id} ({turret.RotationStartAngle}) must be less than or equal to {_turretOptions.MaxAngle}!");

            if (turret.RotationEndAngle < _turretOptions.MinAngle)
                throw new ArgumentOutOfRangeException($"Rotation end angle for turret {turret.Id} ({turret.RotationEndAngle}) must be greater than or equal to {_turretOptions.MinAngle}!");

            if (turret.RotationEndAngle > _turretOptions.MaxAngle)
                throw new ArgumentOutOfRangeException($"Rotation end angle for turret {turret.Id} ({turret.RotationEndAngle}) must be less than or equal to {_turretOptions.MaxAngle}!");

            if (turret.RotationStartAngle > turret.RotationEndAngle)
                throw new InvalidOperationException($"Rotation start end angle for turret {turret.Id} ({turret.RotationEndAngle}) must be less than or equal to the end angle ({turret.RotationEndAngle})!");


            return true;
        }

        /// <summary>
        /// Validates the <paramref name="sequenceTurretIds"/> against <paramref name="turrets"/>.
        /// </summary>
        /// <param name="turrets">IEnumerable of Integer valid turret Ids</param>
        /// <param name="sequenceTurretIds">IEnumerable of Integer turret Ids in the sequence</param>
        /// <returns>Boolean True if the properties are all valid</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="turrets"/> or <paramref name="sequenceTurretIds"/> is blank</exception>
        /// <exception cref="KeyNotFoundException">Thrown if any value in <paramref name="sequenceTurretIds"/> does not exist in <paramref name="turrets"/></exception>
        private static bool ValidateTurretSequencing(
            IEnumerable<Turret> turrets,
            IEnumerable<int> sequenceTurretIds
            )
        {
            if (turrets is null || !turrets.Any())
                throw new ArgumentNullException(nameof(turrets));

            if (sequenceTurretIds is null || !sequenceTurretIds.Any())
                throw new ArgumentNullException(nameof(sequenceTurretIds));


            var distinctOrderedTurretIds = turrets
                .Select(t => t.Id)
                .Distinct()
                .OrderBy(i => i)
                .ToList();

            foreach (var id in sequenceTurretIds.Distinct())
                if (!distinctOrderedTurretIds.Contains(id))
                    throw new KeyNotFoundException($"The turrent Id {id} in {nameof(sequenceTurretIds)} does not exist in the {nameof(turrets)}!");

            return true;
        }

        /// <summary>
        /// Tests the requested <paramref name="turret"/> and returns the result.
        /// </summary>
        /// <param name="turret"><see cref="Turret"/> to test</param>
        /// <returns><see cref="CalibrationTestResult"/></returns>
        private static CalibrationTestResult DoTest(
            Turret turret
            )
        {

            if (turret is null)
                throw new ArgumentNullException(nameof(turret));

            var result = new CalibrationTestResult()
            {
                TurretId = turret.Id,
                Rotated = turret.RotationEndAngle - turret.RotationStartAngle,
                TimesTested = 1
            };

            return result;
        }

        #endregion WORKER FUNCTIONS

    }

}