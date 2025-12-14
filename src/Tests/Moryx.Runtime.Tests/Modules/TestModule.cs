// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Tests.Modules
{
    internal class TestModule : ServerModuleBase<TestConfig>
    {
        public TestModule(IModuleContainerFactory containerFactory, IConfigManager configManager, ILoggerFactory loggerManagement)
            : base(containerFactory, configManager, loggerManagement)
        {
        }

        /// <inheritdoc />
        public override string Name => "TestModule";

        /// <summary>
        /// Current test mode to execute
        /// </summary>
        public TestMode CurrentMode { get; set; }

        /// <summary>
        /// Access to the last invoked method to validate correct behaviour
        /// </summary>
        public InvokedMethod LastInvoke { get; set; }

        public int RetryCount { get; set; }

        public TestConfig MyConfig => Config;

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            LastInvoke = InvokedMethod.Initialize;
            switch (CurrentMode)
            {
                case TestMode.MoryxException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }

            return Task.CompletedTask;
        }

        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            LastInvoke = InvokedMethod.Start;
            RetryCount++;
            switch (CurrentMode)
            {
                case TestMode.MoryxException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            LastInvoke = InvokedMethod.Stop;
            switch (CurrentMode)
            {
                case TestMode.MoryxException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
            return Task.CompletedTask;
        }
    }
}
