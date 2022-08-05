// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.Runtime.Tests.Modules
{
    internal class DelayedExceptionModule : ServerModuleBase<TestConfig>
    {
        public DelayedExceptionModule(IModuleContainerFactory containerFactory, IConfigManager configManager, IServerLoggerManagement loggerManagement) 
            : base(containerFactory, configManager, loggerManagement)
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
