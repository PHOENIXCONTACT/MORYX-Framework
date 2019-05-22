using Marvin.Container;
using Marvin.Logging;

namespace Marvin.TestTools.UnitTest
{
    /// <summary>
    /// Installer registering the interceptor
    /// </summary>
    public class BasicInterceptorInstaller : IContainerInstaller
    {
        /// <inheritdoc />
        public void Install(IComponentRegistrator registrator)
        {
            registrator.Register(typeof(EmptyInterceptor));
            registrator.Register(typeof(DummyLogger), new []{typeof(IModuleLogger)});
        }
    }
}