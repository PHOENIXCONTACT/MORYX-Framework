// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Serialization;

public enum EnumDummy
{
    [Display(Name = DisplayNameEnumStrings.UnsetDisplayName, Description = DisplayNameEnumStrings.UnsetDescription)]
    Unset,

    [Display(Name = DisplayNameEnumStrings.ValueADisplayName)]
    ValueA,

    ValueB
}

public static class DisplayNameEnumStrings
{
    public const string UnsetDisplayName = "No Value";
    public const string UnsetDescription = "Nothing selected";
    public const string ValueADisplayName = "First Value";
}
