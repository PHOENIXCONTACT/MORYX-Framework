using System.Threading;
using Marvin.Container;
using Marvin.Threading;
using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    [Plugin(LifeCycle.Singleton, typeof(IHelloWorldWcfSvcMgr), Name = ComponentName)]
    public class HelloWorldWcfSvcMgr : ServiceManager<IHelloWorldWcfService>, IHelloWorldWcfSvcMgr
    {
        internal const string ComponentName = "HelloWorldWcfSvcMgr";

        public IParallelOperations ParallelOperations { get; set; }

        protected override void Initialize(IHelloWorldWcfService connection)
        {
        }

        public void Initialize()
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
            
        }
        
        public string Hello(string name)
        {
            return string.IsNullOrEmpty(name) ? "Hello world!" : string.Format("Hello {0}!", name);
        }

        public string Throw(string name)
        {
            // The exception here is on purpose!

            throw new System.NotImplementedException();
        }

        public void TriggerHelloCallback(string name)
        {
            ParallelOperations.ScheduleExecution(() => HelloCallback(name), 100, Timeout.Infinite);
        }

        public void HelloCallback(string name)
        {
            string message = Hello(name);

            foreach (IHelloWorldWcfService service in Services)
            {
                service.HelloCallback(message);
            }
        }

        public void TriggerThrowCallback(string name)
        {
            ParallelOperations.ScheduleExecution(() => ThrowCallback(name), 100, Timeout.Infinite);
        }

        public void ThrowCallback(string name)
        {
            string message = Hello(name);

            foreach (IHelloWorldWcfService service in Services)
            {
                service.ThrowCallback(message);
            }
        }
    }
}