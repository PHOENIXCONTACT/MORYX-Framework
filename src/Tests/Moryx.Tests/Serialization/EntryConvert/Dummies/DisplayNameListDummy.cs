// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tests.Serialization;

public class DisplayNameListDummy
{
    public List<DisplayNameItemDummy> Items { get; set; }
}

[DisplayName(ItemDisplayName), Description(ItemDescription)]
public class DisplayNameItemDummy
{
    public const string ItemDisplayName = "Named item";

    public const string ItemDescription = "Item with display name";

    public int Value { get; set; }
}
