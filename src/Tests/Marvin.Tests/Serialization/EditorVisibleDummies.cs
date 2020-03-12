// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Serialization;

namespace Marvin.Tests
{
    public class EditorVisibleMixed
    {
        [EditorVisible]
        public string Property1 { get; set; } = "123456";
        [EditorVisible]
        private string Property2 { get; set; } = "987654";
        [EditorVisible]
        public bool Property3 { get; set; } = true;
        public int Property4 { get; set; } = 98;
        [EditorVisible]
        public bool Property5 { get; } = true;

        [EditorVisible]
        public bool Method1()
        {
            return true;
        }

        public string Method2()
        {
            return "1234";
        }

        [EditorVisible]
        private bool Method3()
        {
            return true;
        }
    }

    public class NoEditorVisibleSet
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
