// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Marvin.Configuration;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class NoStringValueProviderFilter : IValueProviderFilter
    {
        public bool CheckProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType != typeof(string);
        }
    }
}
