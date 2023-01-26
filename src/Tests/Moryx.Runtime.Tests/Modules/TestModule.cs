// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
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

        protected override void OnInitialize()
        {
            LastInvoke = InvokedMethod.Initialize;
            switch (CurrentMode)
            {
                case TestMode.MoryxException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
        }

        protected override void OnStart()
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
        }

        protected override void OnStop()
        {
            LastInvoke = InvokedMethod.Stop;
            switch (CurrentMode)
            {
                case TestMode.MoryxException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
        }
    }
}
