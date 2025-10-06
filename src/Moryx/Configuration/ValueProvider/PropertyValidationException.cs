// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Configuration
{
    [Serializable]
    internal class PropertyValidationException : Exception
    {
        private PropertyInfo property;
        private ValueProviderExecutorSettings settings;

        public PropertyValidationException()
        {
        }

        public PropertyValidationException(string message) : base(message)
        {
        }

        public PropertyValidationException(PropertyInfo property, ValueProviderExecutorSettings settings)
            : this($"Failed to provide acceptable value for property {property.Name} on type {property.DeclaringType}")
        {
            this.property = property;
            this.settings = settings;
        }

    }
}
