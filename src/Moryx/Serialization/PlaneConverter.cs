// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="Plane"/> since its Normal and D are fields, not properties.
/// </summary>
internal class PlaneConverter : JsonConverter<Plane>
{
    public override void WriteJson(JsonWriter writer, Plane value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Normal");
        serializer.Serialize(writer, value.Normal);
        writer.WritePropertyName("D");
        writer.WriteValue(value.D);
        writer.WriteEndObject();
    }

    public override Plane ReadJson(JsonReader reader, Type objectType, Plane existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var normal = obj["Normal"]?.ToObject<Vector3>(serializer) ?? Vector3.Zero;
        var d = obj["D"]?.Value<float>() ?? 0f;
        return new Plane(normal, d);
    }
}
