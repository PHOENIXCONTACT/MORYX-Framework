// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;

namespace Moryx.Drivers.OpcUa;

internal static partial class OpcUaNodeHelpers
{
    [GeneratedRegex(";ns=\\d+")]
    public static partial Regex NodeIdWithNamespaceIndexRegex();
}
