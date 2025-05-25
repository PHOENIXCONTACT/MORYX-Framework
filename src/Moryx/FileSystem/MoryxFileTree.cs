// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.FileSystem;

/// <summary>
/// File that contains a list of files and is the foundation for the recursive tree
/// </summary>
public class MoryxFileTree : MoryxFile
{
    private readonly List<MoryxFile> _files = new List<MoryxFile>();

    public override FileType FileType => FileType.Tree;

    public IReadOnlyList<MoryxFile> Files => _files;

    public void AddFile(MoryxFile file)
    {
        file.ParentTree = this;
        _files.Add(file);
    }

    public bool RemoveFile(MoryxFile file)
    {
        var found = _files.Remove(file);
        if (found)
            file.ParentTree = null;
        return found;
    }
}
