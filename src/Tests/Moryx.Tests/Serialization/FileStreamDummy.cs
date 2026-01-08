// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;

namespace Moryx.Tests.Serialization;

public class FileStreamDummy
{
    public FileStreamDummy(string filePath, FileMode mode)
    {
        FileStream = new FileStream(filePath, mode);
    }

    public FileStream FileStream { get; set; }
}