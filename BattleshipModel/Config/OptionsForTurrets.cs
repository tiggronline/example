namespace Battleship.Model.Config
{

    /// <summary>
    /// Strongly-typed configuration options for turrets.
    /// </summary>
    public class OptionsForTurrets:OptionsBase
    {

        /// <summary>
        /// The name of the main section in AppSettings.json.
        /// </summary>
        public new const string SectionPath = "Turrets";


        /// <summary>
        /// Gets or sets the minimum possible angle of a turret.
        /// </summary>
        /// <value>Integer</value>
        public int MinAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum possible angle of a turret.
        /// </summary>
        /// <value>Integer</value>
        public int MaxAngle { get; set; }


        /// <summary>
        /// Gets or sets the maximum possible caliber of a turret.
        /// </summary>
        /// <value>Integer</value>
        public int MinCaliber { get; set; }

        /// <summary>
        /// Gets or sets the maximum possible caliber of a turret.
        /// </summary>
        /// <value>Integer</value>
        public int MaxCaliber { get; set; }

    }

}