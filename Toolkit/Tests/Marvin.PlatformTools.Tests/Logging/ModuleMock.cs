using Marvin.Logging;

namespace Marvin.PlatformTools.Tests.Logging
{
    public class ModuleMock : ILoggingHost
    {
        public string Name { get { return GetType().Name; } }

        public IModuleLogger Logger { get; set; }
    }
}