// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moryx.FileSystem;

/// <summary>
/// Moryx file types in owner tree file
/// </summary>
public enum FileType
{
    /// <summary>
    /// Binary file, unspecified
    /// </summary>
    Blob = 0,

    /// <summary>
    /// Tree file with references to files
    /// </summary>
    Tree = 1,
}
