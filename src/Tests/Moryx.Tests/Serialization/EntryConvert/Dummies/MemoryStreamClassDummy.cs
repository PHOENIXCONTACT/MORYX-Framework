// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;

namespace Moryx.Tests.Serialization;

public class MemoryStreamClassDummy
{
    public MemoryStreamClassDummy(string testString)
    {
        MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
    }

    public MemoryStream MemoryStream { get; set; }
}
