using System.Collections.Generic;

namespace Battleship.Model.Models
{

    /// <summary>
    /// Calibration settings model.
    /// </summary>
    public class CalibrationSettings
    {

        /// <summary>
        /// Gets or sets the IDs of turrets to test in order.
        /// </summary>
        /// <value>IEnumerable of Integer turret IDs</value>
        public IEnumerable< int> Sequence { get; set; }

        /// <summary>
        /// Gets or sets the turrets that can be tested.
        /// </summary>
        /// <value>IEnumerable of <see cref="Turret"/>s</value>
        public IEnumerable<Turret> Turrets { get; set; }

    }

}