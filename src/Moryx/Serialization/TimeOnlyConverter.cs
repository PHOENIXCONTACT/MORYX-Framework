// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Newtonsoft.Json;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="TimeOnly"/> which is not natively supported.
/// </summary>
internal class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("O", CultureInfo.InvariantCulture));
    }

    public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return TimeOnly.ParseExact((string)reader.Value!, "O", CultureInfo.InvariantCulture);
    }
}
