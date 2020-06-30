// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;

namespace Marvin.Tests
{
    public class FileStreamDummy
    {
        public FileStreamDummy(string filePath, FileMode mode)
        {
            FileStream = new FileStream(filePath, mode);
        }

        public FileStream FileStream { get; set; }
    }
}
