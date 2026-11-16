// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tests.Configuration.ValueProvider;

internal sealed class ClassWithoutParamLessCtor
{
    public ClassWithoutParamLessCtor(string a)
    {

    }
}

internal sealed class TestConfig5
{
    [DefaultValue(null)]
    public ClassWithoutParamLessCtor Field1 { get; set; }

    [DefaultValue(null)]
    public Func<string, bool> Field2 { get; set; }
}
