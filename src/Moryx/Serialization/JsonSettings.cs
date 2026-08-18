// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Moryx.Serialization;

/// <summary>
/// Wrapper around
/// </summary>
public static class JsonSettings
{
    private static readonly StringEnumConverter _stringEnum = new();
    private static readonly Vector2Converter _vector2 = new();
    private static readonly Vector3Converter _vector3 = new();
    private static readonly QuaternionConverter _quaternion = new();

    /// <summary>
    /// Json settings for optimal performance and minimal number of characters
    /// </summary>
    public static JsonSerializerSettings Minimal => new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        NullValueHandling = NullValueHandling.Ignore,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        Converters = { _vector2, _vector3, _quaternion },
    };

    /// <summary>
    /// Json settings for human-readable text files
    /// </summary>
    public static JsonSerializerSettings Readable => new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        DefaultValueHandling = DefaultValueHandling.Include,
        NullValueHandling = NullValueHandling.Include,
        Converters = [_stringEnum, _vector2, _vector3, _quaternion]
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
