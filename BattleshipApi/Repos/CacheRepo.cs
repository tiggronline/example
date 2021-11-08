using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Battleship.Api.Repos
{

    /// <summary>
    /// Abstraction for caching repository.
    /// </summary>
    public class CacheRepo
    {

        #region ==================== PRIVATE FIELDS ====================

        /// <summary>
        /// ILogger of <seealso cref="CacheRepo"/> to use for logging.
        /// </summary>
        private readonly ILogger<CacheRepo> _logger;

        /// <summary>
        /// <seealso cref="IDistributedCache"/>.
        /// </summary>
        /// <value><seealso cref="IDistributedCache"/></value>
        private static IDistributedCache _cache;

        #endregion PRIVATE FIELDS


        /// <summary>
        /// Initialises this class with the supplied dependency-injected parameters.
        /// </summary>
        /// <param name="logger">ILogger of <see cref="CacheRepo"/> to use for logging</param>
        /// <param name="cache">IDistributedCache</param>
        public CacheRepo(
            ILogger<CacheRepo> logger,
            IDistributedCache cache
            )
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));

            _cache = cache ??
                throw new ArgumentNullException(nameof(cache));
        }


        #region ==================== PUBLIC MEMBERS ====================

        /// <summary>
        /// Returns the requested untyped cache entry asynchronously.
        /// </summary>
        /// <param name="key">String key</param>
        /// <returns>String entry if found; otherwise null</returns>
        public async Task<string> GetAsync(
            string key
            )
        {
            _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(GetAsync)}=>");

            string result = default;

            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));


                var jsonStringValue = await _cache.GetStringAsync(key);


                result = jsonStringValue;
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(GetAsync)}<=");
            }

            return result;
        }

        /// <summary>
        /// Returns the requested typed cache entry asynchronously.
        /// </summary>
        /// <param name="key">String key</param>
        /// <returns><typeparamref name="T"/> value if found; otherwise null</returns>
        /// <typeparam name="T">Type of Object</typeparam>
        public async Task<T> GetAsync<T>(
            string key
            )
        {
            _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(GetAsync)}<T>=>");

            T typedValue = default;

            try
            {
                var valueType = typeof(T);

                var jsonStringValue = await GetAsync(key);

                if (jsonStringValue is null)
                    typedValue = default;

                else if (IsSimpleDataType(valueType))
                    typedValue = (T)(object)jsonStringValue;

                else
                {
                    try
                    {
                        object deserializedValue = JsonConvert.DeserializeObject<T>(
                            jsonStringValue,
                            JsonSettingsForDeserializing
                            );

                        typedValue = (T)deserializedValue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to de-serialize cache entry '{key}' to a {valueType.Name} ({ex.Message})!");
                        typedValue = default;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(GetAsync)}<T><=");
            }

            return typedValue;
        }

        /// <summary>
        /// Adds or updates the requested cache entry asynchronously to the <paramref name="value"/> serialized
        /// to a JSON string if a class otherwise a string representation of the <paramref name="value"/>.
        /// </summary>
        /// <param name="key">String key</param>
        /// <param name="value">Object value</param>
        /// <param name="cacheEntryOptions">DistributedCacheEntryOptions detailing when the key should expire (optional;
        /// default=set in AppSettings.json)</param>
        public async Task SetAsync(
            string key,
            object value,
            DistributedCacheEntryOptions cacheEntryOptions = null
            )
        {
            _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(SetAsync)}=>");

            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException(nameof(key));

                if (value is null)
                    throw new ArgumentNullException(nameof(value));


                var jsonStringValue =
                    IsSimpleDataType(value.GetType())
                        ? value.ToString()
                        : JsonConvert.SerializeObject(
                            value,
                            JsonSettingsForSerializing
                            );


                await _cache.SetStringAsync(
                     key,
                     jsonStringValue
                     );
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            finally
            {
                _logger.LogTrace($"{nameof(CacheRepo)}.{nameof(SetAsync)}<=");
            }
        }

        #endregion PUBLIC MEMBERS


        #region ==================== WORKER FUNCTIONS ====================

        /// <summary>
        /// Gets the JsonSerializerSettings for deserializing JSON into objects.
        /// </summary>
        /// <value>JsonSerializerSettings</value>
        private static JsonSerializerSettings JsonSettingsForDeserializing { get; }
            = new() { };

        /// <summary>
        /// Gets the JsonSerializerSettings for serializing objects into JSON.
        /// </summary>
        /// <value>JsonSerializerSettings</value>
        private static JsonSerializerSettings JsonSettingsForSerializing { get; }
        = new()
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Returns whether or not the <paramref name="type"/> is a 'simple' data type
        ///  i.e. a primitive, a string (which is a Char Array i.e. complex)
        ///  or a DateTime (which is a struct i.e. complex).
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Boolean True if <paramref name="type"/> is 'simple'; otherwise False</returns>
        private static bool IsSimpleDataType(Type type)
            => (type.IsPrimitive || type.Equals(typeof(string)) || type.Equals(typeof(DateTime)));

        #endregion WORKER FUNCTIONS
    }

}