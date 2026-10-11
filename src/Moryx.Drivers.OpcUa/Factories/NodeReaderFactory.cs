// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Drivers.OpcUa.Factories;

internal class NodeReaderFactory
{
    public virtual IOpcUaNodeReader CreateNodeReader(IModuleLogger moduleLogger, IOpcUaDriver driver)
    {
        var browser = new OpcUaNodeReader(moduleLogger, driver);
        return browser;
    }
}
