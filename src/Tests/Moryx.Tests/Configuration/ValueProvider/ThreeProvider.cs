// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Configuration;

namespace Moryx.Tests.Configuration.ValueProvider;

internal class ThreeProvider : IValueProvider
{
    public ValueProviderResult Handle(object parent, PropertyInfo property)
    {
        property.SetValue(parent, 3);
        return ValueProviderResult.Handled;
    }
}