namespace Battleship.Model.Config
{

    /// <summary>
    /// Strongly-typed configuration options base class for settings in the API "appsettings*.json".
    /// </summary>
    public abstract class OptionsBase
    {

        /// <summary>
        /// Placeholder for the configuration section path.
        /// Needs to be hidden in derived classes using
        ///     public new const string SectionPath = "ActualSectionPath";
        ///  where ActualSectionPath is the relavant section path in appsettings.json.
        /// </summary>
        /// <value>String</value>
        public static string SectionPath = null;

    }

}