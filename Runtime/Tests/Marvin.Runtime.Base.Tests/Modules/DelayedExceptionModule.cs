using System;
using System.Threading;
using Marvin.Threading;

namespace Marvin.Runtime.Base.Tests.Modules
{
    internal class DelayedExceptionModule : ServerModuleBase<TestConfig>
    {
        /// <summary>
        /// Unique name for this module within the platform it is designed for
        /// </summary>
        public override string Name
        {
            get { return "DelayedException"; }
        }

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