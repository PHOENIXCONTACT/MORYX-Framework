namespace Marvin.Container.Tests
{
    public interface ILocalComponent
    {
    }

    [Plugin(LifeCycle.Transient, typeof(ILocalComponent))]
    internal class LocalComponent : ILocalComponent
    {
    }

    [PluginFactory]
    public interface ILocalFactory
    {
        ILocalComponent Create();
    }
}