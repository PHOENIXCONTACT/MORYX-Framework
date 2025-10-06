// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
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

        protected override void OnInitialize()
        {
            WaitEvent = new ManualResetEvent(false);
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
            Container.Resolve<IParallelOperations>().ExecuteParallel(delegate
            {
                WaitEvent.WaitOne();
                throw new Exception("Test");
            });
        }
    }
}
