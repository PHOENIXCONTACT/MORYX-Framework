// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Serialization;

namespace Moryx.Tests
{
    public class EditorBrowsableMixed
    {
        [EditorBrowsable]
        public string Property1 { get; set; } = "123456";

        [EditorBrowsable]
        private string Property2 { get; set; } = "987654";

        [EditorBrowsable]
        public bool Property3 { get; set; } = true;

        public int Property4 { get; set; } = 98;

        [EditorBrowsable]
        public bool Property5 { get; } = true;

        [EditorBrowsable]
        public bool Method1()
        {
            return true;
        }

        public string Method2()
        {
            return "1234";
        }

        [EditorBrowsable]
        private bool Method3()
        {
            return true;
        }
    }

    public class NoEditorBrowsableSet
    {
        public string Property1 { get; set; } = "123456";

        public bool Property2 { get; set; } = true;

        public int Property3 { get; set; } = 98;

        public bool Method1()
        {
            return true;
        }

        public string Method2()
        {
            return "1234";
        }
    }
}
