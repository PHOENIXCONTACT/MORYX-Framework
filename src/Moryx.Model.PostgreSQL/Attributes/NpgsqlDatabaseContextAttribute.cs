// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Attributes;

namespace Moryx.Model.PostgreSQL.Attributes
{
    /// <summary>
    /// Attribute to identify Npgsql specific contexts
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NpgsqlDatabaseContextAttribute : DatabaseSpecificContextAttribute { }
}
