using Battleship.Model.Enums;

namespace Battleship.Model.Models
{

    /// <summary>
    /// Turret model.
    /// </summary>
    public class Turret
    {

        /// <summary>
        /// Gets or sets the caliber of the turret (in mm).
        /// </summary>
        /// <value>Integer between 102mm and 405mm</value>
        public int Caliber { get; set; }

        /// <summary>
        /// Gets or sets the location of the turret.
        /// </summary>
        /// <value><see cref="Location"/></value>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the start angle (in degrees).
        /// </summary>
        /// <value>Integer between 0 and 180 degrees</value>
        public int RotationStartAngle { get; set; }

        /// <summary>
        /// Gets or sets the end angle (in degrees).
        /// </summary>
        /// <value>Integer between 0 and 180 degrees</value>
        public int RotationEndAngle { get; set; }

    }

}