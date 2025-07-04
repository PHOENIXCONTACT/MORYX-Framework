// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Moryx.Serialization
{
    /// <summary>
    /// Wrapper around
    /// </summary>
    public static class JsonSettings
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
        /// Overwrite one of the values from a predefined setting
        /// </summary>
        public static JsonSerializerSettings Overwrite<T>(this JsonSerializerSettings current,
            Func<JsonSerializerSettings, T> overwrite)
        {
            overwrite(current);
            return current;
        }
    }
}
