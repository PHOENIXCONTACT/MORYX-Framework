// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.Dummies;
using Moryx.Runtime.Modules;
using Microsoft.Extensions.Logging;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks;

internal class RestartableServerModuleA : MockServerModuleBase, IFacadeContainer<IFacadeA>
{
    public RestartableServerModuleA(
        IModuleContainerFactory containerFactory,
        IConfigManager configManager,
        ILoggerFactory loggerFactory)
        : base(containerFactory, configManager, loggerFactory)
    {
    }

    public override string Name => "RestartableServerModuleA";

    public IFacadeA Facade { get; } = new FacadaA();
}

