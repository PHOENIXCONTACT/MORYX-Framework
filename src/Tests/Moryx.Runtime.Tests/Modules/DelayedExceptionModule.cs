// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.Runtime.Tests.Modules
{
    internal class DelayedExceptionModule : ServerModuleBase<TestConfig>
    {
        public DelayedExceptionModule(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerFactory)
            : base(containerFactory, configManager, loggerFactory)
        {
        }

        /// <inheritdoc />
        public override string Name => "DelayedException";

        public ManualResetEvent WaitEvent { get; set; }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            WaitEvent = new ManualResetEvent(false);
            return Task.CompletedTask;
        }

        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            Container.Resolve<IParallelOperations>().ExecuteParallel(delegate
            {
                WaitEvent.WaitOne();
                throw new Exception("Test");
            });
            return Task.CompletedTask;
        }
    }
}
