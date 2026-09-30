// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class InstanceAwareValuesClass
{
    public List<string> AllowedValues { get; set; }

    [StringListPossibleValue(RequiresPossibleValuesContext = true)]
    public string SelectedValue { get; set; }
}

public class StringListPossibleValueAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider, PossibleValuesContext context)
    {
        if (context.Instance is not InstanceAwareValuesClass value)
        {
            return [];
        }

        return value.AllowedValues;
    }
}
