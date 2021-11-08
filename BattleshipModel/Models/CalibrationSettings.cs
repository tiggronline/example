using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        /// <value example='[1,3,2,3]'>IEnumerable of Integer turret IDs</value>
        [Required]
        public IEnumerable< int> Sequence { get; set; }

        /// <summary>
        /// Gets or sets the turrets that can be tested.
        /// </summary>
        /// <value>IEnumerable of <see cref="Turret"/>s</value>
        [Required]
        public IEnumerable<Turret> Turrets { get; set; }

    }

}