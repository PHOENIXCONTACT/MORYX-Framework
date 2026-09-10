// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="Vector4"/> since its X, Y, Z, W are fields, not properties.
/// </summary>
internal class Vector4Converter : JsonConverter<Vector4>
{
    public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteValue(value.Y);
        writer.WritePropertyName("Z");
        writer.WriteValue(value.Z);
        writer.WritePropertyName("W");
        writer.WriteValue(value.W);
        writer.WriteEndObject();
    }

    public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        return new Vector4(
            obj["X"]?.Value<float>() ?? 0f,
            obj["Y"]?.Value<float>() ?? 0f,
            obj["Z"]?.Value<float>() ?? 0f,
            obj["W"]?.Value<float>() ?? 0f
        );
    }
}
