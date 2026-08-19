// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Globalization;

namespace Moryx.Serialization;

/// <summary>
/// Type conversion, struct serializer registry and string formatting for EntryConvert
/// </summary>
public static partial class EntryConvert
{
    /// <summary>
    /// Registry of serializers for structs that are decomposed into sub-entries
    /// </summary>
    private static readonly Dictionary<Type, IStructSerializer> _structSerializers = new IStructSerializer[]
    {
        new Vector2EntrySerializer(),
        new Vector3EntrySerializer(),
        new QuaternionEntrySerializer()
    }.ToDictionary(s => s.TargetType);

    /// <summary>
    /// Try to find a registered <see cref="IStructSerializer"/> for the given type
    /// </summary>
    private static bool TryGetSerializer(Type type, out IStructSerializer serializer)
    {
        return _structSerializers.TryGetValue(type, out serializer);
    }

    /// <summary>
    /// Converts given value typed instance to a string with the given <see cref="IFormatProvider"/>
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="formatProvider">Format provider used to convert the value to string</param>
    /// <returns></returns>
    internal static string ConvertToString(object value, IFormatProvider formatProvider)
    {
        return value switch
        {
            DateTime dt => dt.ToUniversalTime().ToString("O", formatProvider),
            DateOnly d => d.ToString("O", formatProvider),
            TimeOnly t => t.ToString("O", formatProvider),
            TimeSpan ts => ts.ToString("c", formatProvider),
            IConvertible convertible => convertible.ToString(formatProvider),
            _ => value?.ToString()
        };
    }

    /// <summary>
    /// Transform type of entry
    /// </summary>
    /// <returns>Enum representation of the property type</returns>
    public static EntryValueType TransformType(Type propertyType)
    {
        var valueType = EntryValueType.String;

        if (propertyType == typeof(byte))
        {
            valueType = EntryValueType.Byte;
        }
        else if (propertyType == typeof(bool))
        {
            valueType = EntryValueType.Boolean;
        }
        else if (propertyType == typeof(short))
        {
            valueType = EntryValueType.Int16;
        }
        else if (propertyType == typeof(ushort))
        {
            valueType = EntryValueType.UInt16;
        }
        else if (propertyType == typeof(int))
        {
            valueType = EntryValueType.Int32;
        }
        else if (propertyType == typeof(uint))
        {
            valueType = EntryValueType.UInt32;
        }
        else if (propertyType == typeof(long))
        {
            valueType = EntryValueType.Int64;
        }
        else if (propertyType == typeof(ulong))
        {
            valueType = EntryValueType.UInt64;
        }
        else if (propertyType == typeof(float))
        {
            valueType = EntryValueType.Single;
        }
        else if (propertyType == typeof(double))
        {
            valueType = EntryValueType.Double;
        }
        else if (propertyType.IsEnum)
        {
            valueType = EntryValueType.Enum;
        }
        else if (typeof(Stream).IsAssignableFrom(propertyType))
        {
            valueType = EntryValueType.Stream;
        }
        else if (propertyType == typeof(DateOnly))
        {
            valueType = EntryValueType.Date;
        }
        else if (propertyType == typeof(TimeOnly))
        {
            valueType = EntryValueType.Time;
        }
        else if (propertyType == typeof(TimeSpan))
        {
            valueType = EntryValueType.TimeSpan;
        }
        else if (propertyType == typeof(DateTime))
        {
            valueType = EntryValueType.DateTime;
        }
        else if (_structSerializers.ContainsKey(propertyType))
        {
            valueType = EntryValueType.Struct;
        }
        else if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
        {
            valueType = EntryValueType.Collection;
        }
        else if (propertyType.IsClass && propertyType != typeof(string))
        {
            valueType = EntryValueType.Class;
        }

        return valueType;
    }

    /// <summary>
    /// Transform string to typed object based on the type value
    /// </summary>
    /// <param name="type">Desired property type</param>
    /// <param name="value">String value</param>
    /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing value</param>
    /// <returns>Typed object with parsed value</returns>
    public static object ToObject(Type type, string value, IFormatProvider formatProvider)
    {
        // Traditional re-transformation
        object result = null;
        if (type == typeof(string))
        {
            result = value;
        }
        else if (type == typeof(byte))
        {
            result = byte.Parse(value, formatProvider);
        }
        else if (type == typeof(bool))
        {
            result = bool.Parse(value);
        }
        else if (type == typeof(short))
        {
            result = short.Parse(value, formatProvider);
        }
        else if (type == typeof(ushort))
        {
            result = ushort.Parse(value, formatProvider);
        }
        else if (type == typeof(int))
        {
            result = int.Parse(value, formatProvider);
        }
        else if (type == typeof(uint))
        {
            result = uint.Parse(value, formatProvider);
        }
        else if (type == typeof(long))
        {
            result = long.Parse(value, formatProvider);
        }
        else if (type == typeof(ulong))
        {
            result = ulong.Parse(value, formatProvider);
        }
        else if (type == typeof(float))
        {
            result = ParseWithFallback<float>(
                value, formatProvider, NumberStyles.Float,
                float.TryParse);
        }
        else if (type == typeof(double))
        {
            result = ParseWithFallback<double>(
                value, formatProvider, NumberStyles.Float,
                double.TryParse);
        }
        else if (type == typeof(decimal))
        {
            result = ParseWithFallback<decimal>(
                value, formatProvider, NumberStyles.Float,
                decimal.TryParse);
        }
        else if (type.IsEnum)
        {
            result = Enum.Parse(type, value);
        }
        else if (type == typeof(DateTime))
        {
            result = ConvertToUtc(DateTime.Parse(value, formatProvider));
        }
        else if (type == typeof(DateOnly))
        {
            result = DateOnly.Parse(value, formatProvider);
        }
        else if (type == typeof(TimeOnly))
        {
            result = TimeOnly.Parse(value, formatProvider);
        }
        else if (type == typeof(TimeSpan))
        {
            result = TimeSpan.Parse(value, formatProvider);
        }

        return result;
    }

    private delegate bool TryParseDelegate<T>(string input, NumberStyles style, IFormatProvider provider, out T result);

    private static T ParseWithFallback<T>(string input, IFormatProvider provider, NumberStyles styles, TryParseDelegate<T> parser)
    {
        if (parser(input, styles, provider, out var parsedValue))
        {
            return parsedValue;
        }

        if (parser(input, styles, CultureInfo.InvariantCulture, out parsedValue))
        {
            return parsedValue;
        }

        throw new FormatException($"Cannot parse '{input}' to {typeof(T).Name}.");
    }

    private static DateTime? ConvertToUtc(DateTime? dateTime)
    {
        if (dateTime == null)
        {
            return null;
        }

        var nonNullDateTime = (DateTime)dateTime;
        if (nonNullDateTime.Kind == DateTimeKind.Utc)
        {
            return nonNullDateTime;
        }
        else
        {
            return TimeZoneInfo.ConvertTimeToUtc(nonNullDateTime, TimeZoneInfo.Local);
        }
    }

    /// <summary>
    /// Transform string to typed object based on the type enum value
    /// </summary>
    /// <param name="type">Entry value type</param>
    /// <param name="value">String value</param>
    /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing value</param>
    /// <returns>Typed object with parsed value</returns>
    public static object ToObject(EntryValueType type, string value, IFormatProvider formatProvider)
    {
        // Traditional re-transformation
        object result = null;

        switch (type)
        {
            case EntryValueType.String:
                result = value;
                break;

            case EntryValueType.Byte:
                result = byte.Parse(value, formatProvider);
                break;

            case EntryValueType.Boolean:
                result = bool.Parse(value);
                break;

            case EntryValueType.Int16:
                result = short.Parse(value, formatProvider);
                break;

            case EntryValueType.UInt16:
                result = ushort.Parse(value, formatProvider);
                break;

            case EntryValueType.Int32:
                result = int.Parse(value, formatProvider);
                break;

            case EntryValueType.UInt32:
                result = uint.Parse(value, formatProvider);
                break;

            case EntryValueType.Int64:
                result = long.Parse(value, formatProvider);
                break;

            case EntryValueType.UInt64:
                result = ulong.Parse(value, formatProvider);
                break;

            case EntryValueType.Single:
                result = ParseWithFallback<float>(
                    value, formatProvider, NumberStyles.Float,
                    float.TryParse);
                break;

            case EntryValueType.Double:
                result = ParseWithFallback<double>(
                    value, formatProvider, NumberStyles.Float,
                    double.TryParse);
                break;

            case EntryValueType.Date:
                result = DateOnly.Parse(value, formatProvider);
                break;

            case EntryValueType.Time:
                result = TimeOnly.Parse(value, formatProvider);
                break;

            case EntryValueType.TimeSpan:
                result = TimeSpan.Parse(value, formatProvider);
                break;

            case EntryValueType.DateTime:
                result = ConvertToUtc(DateTime.Parse(value, formatProvider));
                break;
        }
        return result;
    }
}
