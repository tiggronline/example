using Newtonsoft.Json;

namespace Battleship.Model.Config
{

    /// <summary>
    /// Strongly-typed configuration options for JSON settings in the API "appsettings*.json".
    /// </summary>
    public class OptionsForJson : OptionsBase
    {

        /// <summary>
        /// String configuration section path.
        /// </summary>
        public new const string SectionPath = "Json";


        /// <summary>
        /// Gets or sets the settings for serializing.
        /// </summary>
        /// <value><see cref="JsonSerializerSettingsMin"/></value>
        public JsonSerializerSettingsMin Serializing { get; set; } =
            new JsonSerializerSettingsMin()
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };

        /// <summary>
        /// Gets or sets the settings for deserializing.
        /// </summary>
        /// <value><see cref="JsonSerializerSettingsMin"/></value>
        public JsonSerializerSettingsMin Deserializing { get; set; } =
            new JsonSerializerSettingsMin();

    }

}