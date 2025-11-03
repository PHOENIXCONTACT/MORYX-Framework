// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Model.Attributes;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Attribute to identify SqlServer specific contexts
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SqlServerDatabaseContextAttribute : DatabaseSpecificContextAttribute
{

}