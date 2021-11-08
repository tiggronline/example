using Battleship.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Battleship.Model.Models
{

    /// <summary>
    /// Turret model.
    /// </summary>
    public class Turret
    {

        /// <summary>
        /// Gets or sets the ID of the turret.
        /// </summary>
        /// <value>Integer between 102mm and 405mm</value>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the caliber of the turret (in mm).
        /// </summary>
        /// <value>Integer between 102mm and 405mm</value>
        /// <example>120</example>
        [Required]
        public int Caliber { get; set; }

        /// <summary>
        /// Gets or sets the location of the turret.
        /// </summary>
        /// <value><see cref="Location"/></value>
        /// <example>Bow</example>
        [Required]
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the start angle (in degrees).
        /// </summary>
        /// <value>Integer between 0 and 180 degrees</value>
        /// <example>51</example>
        [Required]
        public int RotationStartAngle { get; set; }

        /// <summary>
        /// Gets or sets the end angle (in degrees).
        /// </summary>
        /// <value example="122">Integer between 0 and 180 degrees</value>
        /// <example>123</example>
        [Required]
        public int RotationEndAngle { get; set; }

    }

}