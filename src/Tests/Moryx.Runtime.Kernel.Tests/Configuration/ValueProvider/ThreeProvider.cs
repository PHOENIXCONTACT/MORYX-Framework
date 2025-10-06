// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using System.Reflection;

namespace Moryx.Tests.Configuration.ValueProvider
{
    internal class ThreeProvider : IValueProvider
    {
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            property.SetValue(parent, 3);
            return ValueProviderResult.Handled;
        }
    }
}