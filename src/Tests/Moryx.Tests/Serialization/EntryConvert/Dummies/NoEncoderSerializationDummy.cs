// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class NoEncoderSerializationDummy : DefaultSerialization
{
    public override EntryEncoderBase GetEncoder(Type memberType, ICustomAttributeProvider attributeProvider) => null;
}
