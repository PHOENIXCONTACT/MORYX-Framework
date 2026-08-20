// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="Vector2"/> since its X, Y are fields, not properties.
/// </summary>
internal class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteValue(value.X);
        writer.WritePropertyName("Y");
        writer.WriteValue(value.Y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        return new Vector2(
            obj["X"]?.Value<float>() ?? 0f,
            obj["Y"]?.Value<float>() ?? 0f
        );
    }
}
