namespace Marvin.Modules
{
    /// <summary>
    /// Based on <see cref="IInitializable"/> and <see cref="IPlugin"/> it offers a simple three-stage lifecycle. 
    /// Initialize, start and dispose.
    /// So with this interface a module plugins can be initialized before their start.
    /// </summary>
    public interface IInitializablePlugin : IInitializable, IPlugin
    {        
    }
}
