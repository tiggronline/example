using System;

namespace Battleship.Model.Models
{

    /// <summary>
    /// Calibration settings model.
    /// </summary>
    public class CalibrationTestResult
    {

        /// <summary>
        /// Gets or sets the IDs of turret tested.
        /// </summary>
        /// <value>Integer</value>
        public int TurretId { get; set; }

        /// <summary>
        /// Gets or sets the number of times the turret was tested.
        /// </summary>
        /// <value>IEnumerable of <see cref="Turret"/>s</value>
        public int TimesTested { get; set; }

        /// <summary>
        /// Gets or sets the degrees the turret was rotated.
        /// </summary>
        /// <value>Integer degrees</value>
        public int Rotated { get; set; }


        /// <summary>
        /// Adds the <paramref name="ctr1"/> to the <paramref name="ctr2"/> and returns the result.
        /// </summary>
        /// <param name="ctr1"><see cref="CalibrationTestResult"/></param>
        /// <param name="ctr2"><see cref="CalibrationTestResult"/></param>
        /// <returns><see cref="CalibrationTestResult"/></returns>
        public static CalibrationTestResult operator +(
            CalibrationTestResult ctr1,
            CalibrationTestResult ctr2
            )
        {
            if (ctr1 is null)
                throw new ArgumentNullException(nameof(ctr1));

            if (ctr2 is null)
                throw new ArgumentNullException(nameof(ctr2));

            if (ctr1.TurretId != ctr2.TurretId)
                throw new InvalidOperationException($"Cannot add {nameof(CalibrationTestResult)}s with different {nameof(CalibrationTestResult.TurretId)}s!");

            return new CalibrationTestResult()
            {
                TurretId = ctr1.TurretId,
                Rotated = ctr1.Rotated + ctr2.Rotated, //TODO: Strictly speaking the turret needs to move from the RotationEndAngle back toRotationStartAngle before traversing again
                TimesTested = ctr1.TimesTested + ctr2.TimesTested
            };
        }
    }

}