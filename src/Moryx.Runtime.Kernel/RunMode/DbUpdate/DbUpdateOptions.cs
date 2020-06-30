// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using CommandLine;

namespace Moryx.Runtime.Kernel.DbUpdate
{
    /// <summary>
    /// Option class for the <see cref="DbUpdateRunMode"/>
    /// </summary>
    [Verb("dbUpdate", HelpText = "Updates all existing databases.")]
    public class DbUpdateOptions : RuntimeOptions
    {
    }
}
