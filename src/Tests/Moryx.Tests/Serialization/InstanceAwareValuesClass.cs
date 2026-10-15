// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class InstanceAwareValuesClass
{
    public List<string> AllowedValues { get; set; }

    [StringListPossibleValue]
    public string SelectedValue { get; set; }

    [EntrySerialize]
    public void DoSomething([StringListPossibleValue] string value)
    {
    }
}

public class StringListPossibleValueAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public StringListPossibleValueAttribute()
    {
        RequiresPossibleValuesContext = true;
    }

    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetValues(PossibleValuesContext context = null)
    {
        if (context!.Instance is not InstanceAwareValuesClass value)
        {
            throw new InvalidOperationException();
        }

        return value.AllowedValues;
    }
}
