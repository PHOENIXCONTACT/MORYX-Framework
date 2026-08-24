// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Newtonsoft.Json;

namespace Moryx.Serialization;

/// <summary>
/// Newtonsoft.Json converter for <see cref="DateOnly"/> which is not natively supported.
/// </summary>
internal class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("O", CultureInfo.InvariantCulture));
    }

    public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return DateOnly.ParseExact((string)reader.Value!, "O", CultureInfo.InvariantCulture);
    }
}
