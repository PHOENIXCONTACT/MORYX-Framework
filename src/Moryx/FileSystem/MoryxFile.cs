// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;

namespace Moryx.FileSystem;

public class MoryxFile
{
    public int Mode { get; set; } = (int)new MoryxFileMode();

    public virtual FileType FileType { get; } = FileType.Blob;

    public string MimeType { get; set; }

    public string Hash { get; set; }

    public string FileName { get; set; }

    public MoryxFileTree ParentTree { get; set; }
}
