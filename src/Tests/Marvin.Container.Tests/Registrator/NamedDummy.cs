namespace Marvin.Container.Tests
{
    [Registration(LifeCycle.Transient, Name = "Dummy")]
    internal class NamedDummy
    {
    }

    [Registration(LifeCycle.Transient)]
    internal class UnnamedDummy
    {
        
    }
}
