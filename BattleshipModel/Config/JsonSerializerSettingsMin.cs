using Newtonsoft.Json;

namespace Battleship.Model.Config
{

    /// <summary>
    /// JsonSerializer settings with only required properties (to avoid Swagger dying).
    /// </summary>
    public class JsonSerializerSettingsMin

    {

        /// <summary>
        /// Gets or sets how strings are escaped when writing JSON text.
        /// </summary>
        /// <value>StringEscapeHandling</value>
        public StringEscapeHandling StringEscapeHandling { get; set; }

        /// <summary>
        /// Gets or sets how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
        /// </summary>
        /// <value>FloatParseHandling</value>
        public FloatParseHandling FloatParseHandling { get; set; }

        /// <summary>
        /// Gets or sets how special floating point numbers, e.g. System.Double.NaN,
        ///  System.Double.PositiveInfinity and System.Double.NegativeInfinity, are written as JSON.
        /// </summary>
        /// <value>FloatFormatHandling</value>
        public FloatFormatHandling FloatFormatHandling { get; set; }

        /// <summary>
        /// Gets or sets how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z",
        ///  are parsed when reading JSON
        /// </summary>
        /// <value>DateParseHandling</value>
        public DateParseHandling DateParseHandling { get; set; }

        /// <summary>
        /// Gets or sets how System.DateTime time zones are handled during serialization and
        ///  deserialization.
        /// </summary>
        /// <value>DateTimeZoneHandling</value>
        public DateTimeZoneHandling DateTimeZoneHandling { get; set; }

        /// <summary>
        /// Gets or sets how dates are written to JSON text.
        /// </summary>
        /// <value>DateFormatHandling</value>     
        public DateFormatHandling DateFormatHandling { get; set; }

        /// <summary>
        /// Gets or whether how JSON text output is formatted.
        /// </summary>
        /// <value>Formatting</value>     
        public Formatting Formatting { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth allowed when reading JSON. Reading past this depth
        ///  will throw a Newtonsoft.Json.JsonReaderException.
        ///  A null value means there is no maximum.
        /// </summary>
        /// <value>Nullable Integer</value>     
        public int? MaxDepth { get; set; }

        /// <summary>
        /// Gets or sets how System.DateTime and System.DateTimeOffset values are formatted
        ///  when writing JSON text, and the expected date format when reading JSON text.
        /// </summary>
        /// <value>String</value>     
        public string DateFormatString { get; set; }

        /// <summary>
        /// Gets or sets how constructors are used during deserialization.
        /// </summary>
        /// <value>ConstructorHandling</value>     
        public ConstructorHandling ConstructorHandling { get; set; }

        /// <summary>
        /// Gets or sets how a type name assembly is written and resolved by the serializer.
        /// </summary>
        /// <value>TypeNameAssemblyFormatHandling</value>     
        public TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandling { get; set; }

        /// <summary>
        /// Gets or sets how metadata properties are used during deserialization.
        /// </summary>
        /// <value>MetadataPropertyHandling</value>     
        public MetadataPropertyHandling MetadataPropertyHandling { get; set; }

        /// <summary>
        /// Gets or sets how type name writing and reading is handled by the serializer.
        /// </summary>
        /// <value>MetadataPropertyHandling</value>     
        /// <remarks>
        /// Newtonsoft.Json.JsonSerializerSettings.TypeNameHandling should be used with caution
        ///  when your application deserializes JSON from an external source. Incoming types
        ///  should be validated with a custom Newtonsoft.Json.JsonSerializerSettings.SerializationBinder
        ///  when deserializing with a value other than Newtonsoft.Json.TypeNameHandling.None.
        /// </remarks>
        public TypeNameHandling TypeNameHandling { get; set; }

        /// <summary>
        /// Gets or sets how object references are preserved by the serializer.
        /// </summary>
        /// <value>PreserveReferencesHandling</value>     
        public PreserveReferencesHandling PreserveReferencesHandling { get; set; }

        /// <summary>
        /// Gets or sets how default values are handled during serialization and deserialization.
        /// </summary>
        /// <value>DefaultValueHandling</value>     
        public DefaultValueHandling DefaultValueHandling { get; set; }

        /// <summary>
        /// Gets or sets how null values are handled during serialization and deserialization.
        /// </summary>
        /// <value>NullValueHandling</value>     
        public NullValueHandling NullValueHandling { get; set; }

        /// <summary>
        /// Gets or sets how objects are created during deserialization.
        /// </summary>
        /// <value>ObjectCreationHandling</value>     
        public ObjectCreationHandling ObjectCreationHandling { get; set; }

        /// <summary>
        /// Gets or sets how missing members (e.g. JSON contains a property that isn't a
        ///  member on the object) are handled during deserialization.
        /// </summary>
        /// <value>MissingMemberHandling</value>     
        public MissingMemberHandling MissingMemberHandling { get; set; }

        /// <summary>
        /// Gets or sets how reference loops (e.g. a class referencing itself) are handled.
        /// </summary>
        /// <value>ReferenceLoopHandling</value>     
        public ReferenceLoopHandling ReferenceLoopHandling { get; set; }

        /// <summary>
        /// Gets or sets whether or not there will be a check for additional content
        ///  after deserializing an object.
        /// </summary>
        /// <value>Boolean True if there will be a check for additional content after deserializing an object;
        ///  False if there will not be a check</value>     
        public bool CheckAdditionalContent { get; set; }


        /// <summary>
        /// Gets or sets whether or not enum values are serialized as their string name.
        /// </summary>
        /// <value>Boolean True to serialize enum values as their string name;
        ///  False to serialize as their numeric value</value>     
        public bool SerializeEnumsAsStrings { get; set; }

    }

}