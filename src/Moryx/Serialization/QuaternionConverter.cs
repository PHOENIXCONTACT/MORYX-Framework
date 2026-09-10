// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="Quaternion"/> since its X, Y, Z, W are fields, not properties.
/// </summary>
internal class QuaternionConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
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

    public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        return new Quaternion(
            obj["X"]?.Value<float>() ?? 0f,
            obj["Y"]?.Value<float>() ?? 0f,
            obj["Z"]?.Value<float>() ?? 0f,
            obj["W"]?.Value<float>() ?? 0f
        );
    }
}
