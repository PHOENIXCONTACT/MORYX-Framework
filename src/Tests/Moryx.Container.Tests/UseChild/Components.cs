// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests;

internal sealed class ParentOnly
{
    public Parent Parent { get; set; }
}

internal sealed class NamedParentOnly
{
    [Named("Parent2")]
    public Parent Parent { get; set; }
}

internal sealed class TypedChild
{
    [UseChild]
    public Child Child { get; set; }
}

internal sealed class NamedChild
{
    [UseChild("Child")]
    public Child Child { get; set; }
}

internal sealed class NamedChildNamedParent
{
    [UseChild("Child", "Parent2")]
    public Child Child { get; set; }
}
