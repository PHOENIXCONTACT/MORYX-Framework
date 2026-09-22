// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Serialization.PossibleValues;

namespace Moryx.Tests.Serialization;

public class RuntimeValuesClass
{
    public List<string> AllowedValues { get; set; }

    [StringListPossibleValue(Source = nameof(AllowedValues))]
    public string SelectedValue { get; set; }
}

public class StringListPossibleValueAttribute : RuntimePossibleValuesAttribute
{
    public override bool OverridesConversion => true;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(object instance, IContainer localContainer, IServiceProvider serviceProvider)
    {
        var sourceProperty = instance?.GetType().GetProperty(Source);

        if (sourceProperty is null)
        {
            return [];
        }

        var possible = sourceProperty.GetValue(instance, null);
        if (possible is null)
        {
            return [];
        }
        return possible as IEnumerable<string>;
    }

    public override object Parse(object instance, IContainer container, IServiceProvider serviceProvider, string value)
    {
        return new();
    }
}
