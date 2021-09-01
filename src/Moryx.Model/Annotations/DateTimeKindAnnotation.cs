// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moryx.Model.Attributes;

namespace Moryx.Model.Annotations
{
    public static class DateTimeKindAnnotation
    {
        private const string AnnotationName = "DateTimeKind";

        private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
            new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime, DateTime> LocalConverter =
            new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Local));

        private static readonly ValueConverter<DateTime, DateTime> UnspecifiedConverter =
            new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified));

        /// <summary>
        /// Adds the date time kind converter to the model builder.
        /// The default for datetime will change to Utc
        /// </summary>
        public static void ApplyDateTimeKindConverter(this ModelBuilder builder, DateTimeKind defaultKind = DateTimeKind.Utc)
        {
            var dateTimeProperties = builder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()).Where(property =>
                property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?));

            foreach (var property in dateTimeProperties)
            {
                var kind = FindDateTimeKind(property) ?? defaultKind;
                switch (kind)
                {
                    case DateTimeKind.Utc:
                        property.SetValueConverter(UtcConverter);
                        break;
                    case DateTimeKind.Local:
                        property.SetValueConverter(LocalConverter);
                        break;
                    case DateTimeKind.Unspecified:
                        property.SetValueConverter(UnspecifiedConverter);
                        break;
                    default:
                        throw new NotSupportedException($"Kind \"{kind}\" unsupported");
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="DateTimeKind"/> for this property.
        /// </summary>
        public static PropertyBuilder<DateTime> HasDateTimeKind(this PropertyBuilder<DateTime> builder, DateTimeKind kind) =>
            builder.HasAnnotation(AnnotationName, kind);

        /// <summary>
        /// Sets the <see cref="DateTimeKind"/> for this property.
        /// </summary>
        public static PropertyBuilder<DateTime?> HasDateTimeKind(this PropertyBuilder<DateTime?> builder, DateTimeKind kind) =>
            builder.HasAnnotation(AnnotationName, kind);

        private static DateTimeKind? FindDateTimeKind(IPropertyBase property)
        {
            var attribute = property.PropertyInfo.GetCustomAttribute<DateTimeKindAttribute>();
            if (attribute != null)
                return attribute.Kind;

            return (DateTimeKind?)property.FindAnnotation(AnnotationName)?.Value;
        }
    }
}