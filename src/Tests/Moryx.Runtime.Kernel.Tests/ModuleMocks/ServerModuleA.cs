// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

public class ServerModuleA : MockServerModuleBase, IFacadeContainer<IFacadeA>
{
    public ServerModuleA(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    public override string Name => "ServerModuleA";

    public IFacadeA Facade { get; } = new FacadaA();
}
