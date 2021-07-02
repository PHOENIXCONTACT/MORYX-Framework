// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests
{
    internal interface IExplicitInterface
    {
        string ExplicitProperty { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class EntrySerialize_Explicit : IExplicitInterface
    {
        string IExplicitInterface.ExplicitProperty { get; set; }

        public int NormalProperty { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class EntrySerialize_BaseType
    {
        public int Property1 { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class EntrySerialize_DerivedType : EntrySerialize_BaseType
    {
        public int Property2 { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class EntrySerialize_NotClassMixed
    {
        [EntrySerialize(EntrySerializeMode.Always)]
        public string AlwaysProperty1 { get; set; } = "123456";

        [EntrySerialize(EntrySerializeMode.Never)]
        private string NeverProperty1 { get; set; } = "987654";

        public bool NullProperty1 { get; set; } = true;

        [EntrySerialize(EntrySerializeMode.Always)]
        public bool AlwaysMethod1() => true;

        [EntrySerialize(EntrySerializeMode.Never)]
        public string NeverMethod1() => "1234";

        private bool NullMethod1() => true;
    }

    // ReSharper disable once InconsistentNaming
    public class EntrySerialize_NoClassNoMember
    {
        public string NullProperty1 { get; set; } = "123456";

        public bool NullProperty2 { get; set; } = true;

        public bool NullMethod1() => true;

        public string NullMethod2() => "1234";
    }

    // ReSharper disable once InconsistentNaming
    [EntrySerialize(EntrySerializeMode.Never)]
    public class EntrySerialize_NeverClassNoMember
    {
        public string NullProperty1 { get; set; } = "123456";

        public bool NullProperty2 { get; set; } = true;

        public bool NullMethod1() => true;

        public string NullMethod2() => "1234";
    }

    // ReSharper disable once InconsistentNaming
    [EntrySerialize(EntrySerializeMode.Never)]
    public class EntrySerialize_NeverClassAlwaysMember
    {
        [EntrySerialize(EntrySerializeMode.Always)]
        public string AlwaysProperty1 { get; set; } = "123456";

        public bool NullProperty2 { get; set; } = true;

        [EntrySerialize(EntrySerializeMode.Always)]
        public bool AlwaysMethod1() => true;

        public string NullMethod2() => "1234";
    }

    [EntrySerialize(EntrySerializeMode.Never)]
    public class EntrySerialize_InheritedBase
    {
        public bool NullProperty1 { get; set; } = true;
    }

    public class EntrySerialize_Inherited : EntrySerialize_InheritedBase
    {
        public string NullProperty2 { get; set; } = "789456";

        public bool NullProperty3 { get; set; } = false;
    }
}
