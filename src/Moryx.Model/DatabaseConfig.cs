// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using System.Text;
using Moryx.Configuration;

namespace Moryx.Model
{
    public class ConnectionStringKeyAttribute : Attribute
    {
        public ConnectionStringKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    /// <inheritdoc />
    [DataContract]
    public class DatabaseConfig : ConfigBase
    {
        /// <summary>
        /// Connection string for the specified data provider
        /// </summary>
        [DataMember]
        public virtual string ConnectionString { get; set; }

        [DataMember]
        public virtual string ConfiguratorType { get; }

        public void UpdatePropertiesFromConnectionString()
        {
            // if (string.IsNullOrEmpty(ConnectionString))
            //     return;
            //
            // var stringPairs = ConnectionString.Split([';'], StringSplitOptions.RemoveEmptyEntries);
            // foreach (var stringPair in stringPairs)
            // {
            //     var keyValue = stringPair.Split(['='], 2);
            //     if (keyValue.Length != 2)
            //         continue;
            //
            //     var connectionStringKey = ConnectionStringKeys.FirstOrDefault(k => k.Value.Equals(keyValue[0], StringComparison.InvariantCultureIgnoreCase));
            //     if (connectionStringKey.Key == null)
            //         continue;
            //
            //     var property = GetType().GetProperty(connectionStringKey.Key);
            //     if (property == null)
            //         continue;
            //
            //     var convertedValue = Convert.ChangeType(keyValue[1], property.PropertyType);
            //     property.SetValue(this, convertedValue);
            // }
        }

        public void UpdateConnectionString()
        {
            // var sb = new StringBuilder();
            // foreach (var connectionStringKey in ConnectionStringKeys)
            // {
            //     var property = GetType().GetProperty(connectionStringKey.Key);
            //     if (property == null)
            //         continue;
            //
            //     var value = property.GetValue(this)?.ToString();
            //     if (string.IsNullOrEmpty(value))
            //         continue;
            //
            //     var stringPair = $"{connectionStringKey}={value};";
            //     sb.Append(stringPair);
            // }
        }
    }
}
