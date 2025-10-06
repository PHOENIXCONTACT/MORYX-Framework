// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Text;

namespace Moryx.Tests
{
    public class MemoryStreamDummy
    {
        public MemoryStreamDummy(string testString)
        {
            MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
        }

        public MemoryStream MemoryStream { get; set; }
    }
}
