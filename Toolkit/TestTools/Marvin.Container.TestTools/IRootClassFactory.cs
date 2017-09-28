namespace Marvin.Container.TestTools
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IRootClassFactory
    {
        IRootClass Create(RootClassFactoryConfig config);

        void Destroy(IRootClass instance);
    }
}