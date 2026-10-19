// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.Resources.VisualInstructions;

internal class PossibleInstructionsAttribute : PossibleValuesAttribute
{
    public override bool OverridesConversion => false;

    public override bool UpdateFromPredecessor => false;

    public PossibleInstructionsAttribute()
    {
        RequiresPossibleValuesContext = true;
    }

    public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<string> GetValues(PossibleValuesContext context = null)
    {
        if (context!.Instance is not VisualInstructor instructor)
        {
            throw new InvalidOperationException($"Context instance is not a {nameof(VisualInstructor)}");
        }

        return instructor.CurrentInstructionIds.Select(i => i.ToString(CultureInfo.InvariantCulture));
    }

    public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        var isParsed = long.TryParse(value, CultureInfo.InvariantCulture, out var result);
        return isParsed ? result : throw new InvalidOperationException($"Could not parse {value} to long");
    }
}
