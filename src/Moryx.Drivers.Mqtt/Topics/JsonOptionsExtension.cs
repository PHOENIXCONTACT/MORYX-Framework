// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Encodings.Web;
using System.Text.Json;

namespace Moryx.Drivers.Mqtt.Topics;

/// <summary>
/// Helper functions to convert the Json serialization options in this namespace into their System.Text.Json equivalent
/// </summary>
public static class JsonOptionsExtension
{
    extension(JsonIgnoreCondition ignoreCondition) {
        /// <summary>
        /// Converts to specific JsonIgnoreCondition for System.Text.Json
        /// </summary>
        public System.Text.Json.Serialization.JsonIgnoreCondition ForSystemTextJson() => ignoreCondition switch
        {
            JsonIgnoreCondition.Never => System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            JsonIgnoreCondition.WhenWritingDefault => System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            JsonIgnoreCondition.WhenWritingNull => System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            _ => throw new NotImplementedException(),
        };
    }

    extension(JsonEncoderOption encoderOption)
    {
        /// <summary>
        /// Converts to specific JavaScriptEncoder for System.Text.Json
        /// </summary>
        public JavaScriptEncoder ForSystemTextJson() => encoderOption switch
        {
            JsonEncoderOption.Default => JavaScriptEncoder.Default,
            JsonEncoderOption.UnsafeRelaxedJsonEscaping => JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            _ => throw new NotImplementedException($"EncoderOption {encoderOption} is not defined"),
        };
    }

    extension(JsonFormat format)
    {
        /// <summary>
        /// Converts to specific JsonNamingPolicy for System.Text.Json
        /// </summary>
        public JsonNamingPolicy ForSystemTextJson() => format switch
        {
            JsonFormat.Default => null,
            JsonFormat.CamelCase => JsonNamingPolicy.CamelCase,
            JsonFormat.KebapCaseUpper => JsonNamingPolicy.KebabCaseUpper,
            JsonFormat.KebapCaseLower => JsonNamingPolicy.KebabCaseLower,
            JsonFormat.SnakeCaseLower => JsonNamingPolicy.SnakeCaseLower,
            JsonFormat.SnakeCaseUpper => JsonNamingPolicy.SnakeCaseUpper,
            _ => throw new NotImplementedException()
        };
    }
}
