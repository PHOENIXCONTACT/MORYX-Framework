// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// Configuration value for the JSON format
/// </summary>
public enum JsonFormat
{
    /// <summary>
    /// Keep the default formatting
    /// </summary>
    Default = 0,
#pragma warning disable CA1069
    /// <summary>
    /// Keep the default formatting which normally is PascalCase for C#
    /// </summary>
    [Obsolete("If you want to keep the same behavior switch to default")]
    PascalCase = 0,
#pragma warning restore CA1069
    /// <summary>
    /// Format object keys as camelCase
    /// </summary>
    CamelCase = 1,
    /// <see cref="JsonNamingPolicy.KebabCaseUpper"/>
    KebapCaseUpper,
    /// <see cref="JsonNamingPolicy.KebabCaseLower"/>
    KebapCaseLower,
    /// <see cref="JsonNamingPolicy.SnakeCaseLower"/>
    SnakeCaseLower,
    /// <see cref="JsonNamingPolicy.SnakeCaseUpper"/>
    SnakeCaseUpper
    // TODO: Add proper PascalCase support once it's available in the framework
    //  https://github.com/dotnet/runtime/issues/34114
}

