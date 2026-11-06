// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Serialization;

class DictionaryClassDummy
{
    public string Name { get; set; }

    public IDictionary<string, int> SubDictionary { get; set; }

    public IDictionary<string, EnumDummy> EnumDictionary { get; set; }
}
