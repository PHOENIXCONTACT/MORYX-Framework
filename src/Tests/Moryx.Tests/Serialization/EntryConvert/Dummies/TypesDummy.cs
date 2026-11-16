// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class TypesDummy
{
    public Type PlainType { get; set; }

    [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
    public Type SelectableType { get; set; }

    [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
    public string TypeName { get; set; }

    [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
    public List<Type> TypeList { get; set; }

    public Type ReceivedType { get; private set; } = typeof(object);

    public void Select([PossibleTypes([typeof(string), typeof(int)], UseFullname = true)] Type type) => ReceivedType = type;

    public void SelectAny(Type type) => ReceivedType = type;
}
