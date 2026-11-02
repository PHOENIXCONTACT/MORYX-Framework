// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class PossibleValuesWithContextClassDummy
{
    public List<string> AllowedValues { get; set; }

    [PossibleValuesWithContextDummy]
    public string SelectedValue { get; set; }

    [EntrySerialize]
    public void DoSomething([PossibleValuesWithContextDummy] string value)
    {
    }
}
