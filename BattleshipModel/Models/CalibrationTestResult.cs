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

    }

}