// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.ProcessData.SpreadsheetsListener.Tests
{
    internal static class TestDirectory
    {
        internal static string[] GetOrderedFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern)
                .OrderBy(s => s)
                .ToArray();
        }
    }
}

