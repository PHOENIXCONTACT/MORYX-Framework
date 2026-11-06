// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

[EntrySerialize(EncoderType = typeof(TypeConverterDummy))]
public class EncoderTypeTargetDummy
{
    public string Tag { get; set; }
}
