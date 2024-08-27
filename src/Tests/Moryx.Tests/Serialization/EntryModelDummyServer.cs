// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Serialization;

namespace Moryx.Tests
{
    public class EntryModelDummyServer
    {
        public int Value { get; set; }

        public string Text { get; set; }

        public bool HasAnything { get; set; }

        public EntryModelSubClassDummyServer Class { get; set; }

        public List<EntryModelSubClassDummyServer> Collection { get; set; }

        public Dictionary<int, EntryModelSubClassDummyServer> Dictionary { get; set; }
    }

    public class EntryModelSubClassDummyServer
    {
        public float Value { get; set; }

        public DummyEnum Enum { get; set; }
    }

    public class EntryModelDummyClient
    {
        public float Value { get; set; }

        public bool HasAnything { get; set; }

        public EntryModelSubClassDummyClient Class { get; set; }

        public EntryCollection<EntryModelSubClassDummyClient> Collection { get; set; }

        public EntryDictionary<EntryModelSubClassDummyClient> Dictionary { get; set; }
    }

    public class EntryModelSubClassDummyClient
    {
        public string Value { get; set; }

        public string Enum { get; set; }
    }

    public class EntryModelListHelperDummy
    {
        public List<EntryModelSubClassDummyClient> Collection { get; set; }
    }

    public class EntryModelDictionaryHelperDummy
    {
        public Dictionary<int, EntryModelSubClassDummyClient> Dictionary { get; set; }
    }
}
