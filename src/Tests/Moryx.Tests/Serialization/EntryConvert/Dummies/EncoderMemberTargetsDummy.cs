// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class EncoderMemberTargetsDummy
{
    public EncoderTypeTargetDummy ByType { get; set; }

    [EntrySerialize(EncoderType = typeof(PropertyConverterDummy))]
    public EncoderTypeTargetDummy ByProperty { get; set; }

    public List<EncoderTypeTargetDummy> ByItemType { get; set; }

    [EntrySerialize(EncoderType = typeof(PropertyConverterDummy), EncoderTarget = EntryEncoderTarget.Items)]
    public List<EncoderTypeTargetDummy> ByItemEncoder { get; set; }

    [EntrySerialize(EncoderType = typeof(CollectionConverterDummy))]
    public List<EncoderTypeTargetDummy> ByMemberEncoder { get; set; }

    public EncoderTypeTargetDummy Received { get; private set; }

    public void ByParameter([EntrySerialize(EncoderType = typeof(PropertyConverterDummy))] EncoderTypeTargetDummy tagged) => Received = tagged;
}
