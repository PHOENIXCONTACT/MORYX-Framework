// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using Moryx.Runtime.Modules;
using Moryx.Threading;

namespace Moryx.Runtime.Tests.Modules
{
    internal class DelayedExceptionModule : ServerModuleBase<TestConfig>
    {
        /// <summary>
        /// Unique name for this module within the platform it is designed for
        /// </summary>
        public override string Name => "DelayedException";

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

        public ManualResetEvent WaitEvent { get; set; }
    }
}
