// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Serialization.PossibleValues;

namespace Moryx.Tests.Serialization;

public class RuntimeValuesClass
{
    public List<string> AllowedValues { get; set; }

    [StringListPossibleValue]
    public string SelectedValue { get; set; }
}

public class StringListPossibleValueAttribute : RuntimePossibleValuesAttribute
{
    public override bool OverridesConversion => true;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(object instance, IContainer localContainer, IServiceProvider serviceProvider)
    {

        if (instance is not RuntimeValuesClass value)
        {
            return [];
        }

        return value.AllowedValues;
    }

    public override object Parse(object instance, IContainer container, IServiceProvider serviceProvider, string value)
    {
        return new();
    }
}
