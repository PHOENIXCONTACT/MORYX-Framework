using System;
using Marvin.Runtime.Modules;
using Marvin.Threading;

namespace Marvin.Runtime.Tests.Modules
{
    internal class TestModule : ServerModuleBase<TestConfig>
    {
        /// <summary>
        /// Unique name for this module within the platform it is designed for
        /// </summary>
        public override string Name => "TestModule";

        /// <summary>
        /// Current test mode to execute
        /// </summary>
        public TestMode CurrentMode { get; set; }

        /// <summary>
        /// Access to the last invoked method to validate correct behaviour
        /// </summary>
        public InvokedMethod LastInvoke { get; set; }

        public TestConfig MyConfig => Config;

        protected override void OnInitialize()
        {
            LastInvoke = InvokedMethod.Initialize;
            switch (CurrentMode)
            {
                case TestMode.MarvinException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
        }

        public int RetryCount { get; set; }
        protected override void OnStart()
        {
            LastInvoke = InvokedMethod.Start;
            RetryCount++;
            switch (CurrentMode)
            {
                case TestMode.RetryException:
                    throw new MissingDependencyException("Missing dependency!", 3, 100);
                case TestMode.MarvinException:
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
                case TestMode.MarvinException:
                    throw new TestException();
                case TestMode.SystemException:
                    throw new Exception("I am done here!");
            }
        }

        public void RaiseException(bool critical)
        {
            var parallelOps = Container.Resolve<IParallelOperations>();
            parallelOps.ExecuteParallel(() =>
            {
                throw new Exception("Parallel ops failed");
            }, critical);
        }
    }
}
