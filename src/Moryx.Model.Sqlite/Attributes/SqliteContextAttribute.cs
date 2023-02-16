// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Attributes;
using System;

namespace Moryx.Model.Sqlite.Attributes
{
    /// <summary>
    /// Attribute to identify Sqlite specific contexts
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SqliteContextAttribute : DatabaseSpecificContextAttribute { }
}
