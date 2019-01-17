using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marvin.Serialization
{
    /// <summary>
    /// Json settings for serializing and deserializing objects
    /// </summary>
    public enum JsonSettings
    {
        /// <summary>
        /// Settings for minimal JSON string size and redundant information.
        /// Best used for serialized objects in database
        /// </summary>
        Minimal,

        /// <summary>
        /// Optimal settings for readable JSON. Best used for config files
        /// </summary>
        Readable,

        /// <summary>
        /// Readable JSON that replaces constructor created objects with values from the file.
        /// Ideal for configs that use default parameters.
        /// </summary>
        ReadableReplace,
    }

    /// <summary>
    /// Wrapper around 
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Json settings for optimal performance and minimal number of characters
        /// </summary>
        public static JsonSerializerSettings Minimal => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            NullValueHandling = NullValueHandling.Ignore,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        };

        /// <summary>
        /// Json settings for human-readable text files
        /// </summary>
        public static JsonSerializerSettings Readable => new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Include,
            Converters = new JsonConverter[] {new StringEnumConverter()}
        };

        /// <summary>
        /// Json settings for human-readable text files 
        /// </summary>
        public static JsonSerializerSettings ReadableReplace
        {
            get
            {
                var readable = Readable;
                readable.ObjectCreationHandling = ObjectCreationHandling.Replace;
                return readable;
            }
        }

        /// <summary>
        /// Map from Json enum to settings object
        /// </summary>
        private static readonly IDictionary<JsonSettings, JsonSerializerSettings> SettingsMap = new Dictionary<JsonSettings, JsonSerializerSettings>
            {
                {
                    JsonSettings.Minimal, Minimal
                },
                {
                    JsonSettings.Readable, Readable
                },
                {
                    JsonSettings.ReadableReplace, ReadableReplace
                }
            };
        
        #region Serialize

        /// <summary>
        /// Serializes the specified object to a json string. Uses <see cref="JsonSettings.Minimal"/> settings.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>JSON string</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Serialize object typed to include the type name when necessary. This uses
        /// <see cref="JsonSettings.Minimal"/>.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="type">Type to assume for deserialization</param>
        /// <returns>JSON string</returns>
        public static string Serialize(object obj, Type type)
        {
            return JsonConvert.SerializeObject(obj, type, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Serializes the specified object to a json string. Uses defined settings.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="settings">Settings for the serialization</param>
        /// <returns>JSON string</returns>
        public static string Serialize(object obj, JsonSettings settings)
        {
            return JsonConvert.SerializeObject(obj, SettingsMap[settings]);
        }

        /// <summary>
        /// Serialize object typed to include the type name when necessary.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="type">Type to assume for deserialization</param>
        /// <param name="settings">Settings to use for serialization</param>
        /// <returns>JSON string</returns>
        public static string Serialize(object obj, Type type, JsonSettings settings)
        {
            return JsonConvert.SerializeObject(obj, type, SettingsMap[settings]);
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Deserializes the given <see cref="string"/> to an object of the given type. Uses <see cref="JsonSettings.Minimal"/> settings
        /// </summary>
        /// <typeparam name="T">Object type to deserialize</typeparam>
        /// <param name="jsonStr">Json string to deserialize</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Deserializes the given <see cref="string"/> to an object of the given type. Uses defined settings.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize</typeparam>
        /// <param name="jsonStr">Json string to deserialize</param>
        /// <param name="settings">Settings for the deserialization</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(string jsonStr, JsonSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr, SettingsMap[settings]);
        }

        /// <summary>
        /// Deserializes the given <see cref="string"/> to an object of the given type. Uses <see cref="JsonSettings.Minimal"/> settings
        /// </summary>
        /// <param name="jsonStr">Json string to deserialize</param>
        /// <param name="type">Type the JSON represents</param>
        /// <returns>Deserialized object</returns>
        public static object Deserialize(string jsonStr, Type type)
        {
            return JsonConvert.DeserializeObject(jsonStr, type, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Deserializes the given <see cref="string"/> to an object of the given type. Uses <see cref="JsonSettings.Minimal"/> settings
        /// </summary>
        /// <param name="jsonStr">Json string to deserialize</param>
        /// <param name="type">Type the JSON represents</param>
        /// <param name="settings">Settings for the deserialization</param>
        /// <returns>Deserialized object</returns>
        public static object Deserialize(string jsonStr, Type type, JsonSettings settings)
        {
            return JsonConvert.DeserializeObject(jsonStr, type, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Populates the properties of the <paramref name="target"/> with values from the <paramref name="jsonStr"/>.
        /// Uses <see cref="JsonSettings.Minimal"/> settings.
        /// </summary>
        /// <param name="target">Target object to populate</param>
        /// <param name="jsonStr">Json string as data source</param>
        public static void Populate(object target, string jsonStr)
        {
            JsonConvert.PopulateObject(jsonStr, target, SettingsMap[JsonSettings.Minimal]);
        }

        /// <summary>
        /// Populates the properties of the <paramref name="target"/> with values from the <paramref name="jsonStr"/>.
        /// Uses the given settings.
        /// </summary>
        /// <param name="target">Target object to populate</param>
        /// <param name="jsonStr">Json string as data source</param>
        /// <param name="settings">Settings to pass to the serializer</param>
        public static void Populate(object target, string jsonStr, JsonSettings settings)
        {
            JsonConvert.PopulateObject(jsonStr, target, SettingsMap[settings]);
        }

        #endregion

    }
}