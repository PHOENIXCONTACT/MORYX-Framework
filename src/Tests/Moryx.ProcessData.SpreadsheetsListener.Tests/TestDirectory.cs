// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;

namespace Moryx.ProcessData.SpreadsheetsListener.Tests;

internal static class TestDirectory
{
    internal static string[] GetOrderedFiles(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern)
            .OrderBy(s => s)
            .ToArray();
    }
}