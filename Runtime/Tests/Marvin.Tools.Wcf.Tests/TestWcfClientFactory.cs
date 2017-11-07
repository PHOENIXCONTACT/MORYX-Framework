namespace Marvin.Tools.Wcf.Tests
{
    internal class TestWcfClientFactory : BaseWcfClientFactory
    {
        public void Initialize(IWcfClientFactoryConfig config)
        {
            Initialize(config, null, new SimpleThreadContext());
        }
    }
}