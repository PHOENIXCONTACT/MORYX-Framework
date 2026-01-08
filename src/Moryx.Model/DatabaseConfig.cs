// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Model.Attributes;

namespace Moryx.Model
{
    /// <inheritdoc />
    [DataContract]
    public class DatabaseConfig : ConfigBase
    {
        /// <summary>
        /// Connection string for the specified data provider
        /// </summary>
        [DataMember, Required]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Fully qualified name of the configurator type
        /// </summary>
        [DataMember]
        public virtual string ConfiguratorType { get; }

        /// <summary>
        /// Updates the properties decorated with <see cref="ConnectionStringKeyAttribute"/> from the current <see cref="ConnectionString"/>.
        /// </summary>
        public void UpdatePropertiesFromConnectionString()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            var keyPropertyMap = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttribute<ConnectionStringKeyAttribute>()
                })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => x.Attribute.Key, x => x.Property);

            var stringPairs = ConnectionString.Split([';'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var stringPair in stringPairs)
            {
                var keyValue = stringPair.Split(['='], 2);
                if (keyValue.Length != 2)
                    continue;

                if (keyPropertyMap.TryGetValue(keyValue[0], out var property))
                {
                    if (property.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(property.PropertyType, keyValue[1], true);
                        property.SetValue(this, enumValue);
                        continue;
                    }

                    var convertedValue = Convert.ChangeType(keyValue[1], property.PropertyType);
                    property.SetValue(this, convertedValue);
                }
            }
        }

        /// <summary>
        /// Updates the <see cref="ConnectionString"/> from the properties decorated with <see cref="ConnectionStringKeyAttribute"/>.
        /// </summary>
        public void UpdateConnectionString()
        {
            var keyPropertyMap = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    Property = p,
                    Attribute = p.GetCustomAttribute<ConnectionStringKeyAttribute>()
                })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => x.Attribute.Key, x => x.Property);

            var updates = keyPropertyMap.Select(kp => kp.Key + "=" + $"{kp.Value.GetValue(this)}").ToArray();

            // Parse original connection string
            var dict = ConnectionString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('=', 2))
                .ToDictionary(
                    p => p[0].Trim(),
                    p => p.Length > 1 ? p[1] : "",
                    StringComparer.OrdinalIgnoreCase
                );

            // Apply updates
            foreach (var update in updates)
            {
                var parts = update.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    dict[key] = parts[1];
                }
            }

            // Rebuild connection string
            ConnectionString = string.Join(";", dict.Select(kv => $"{kv.Key}={kv.Value}")) + ";";
        }
    }
}
