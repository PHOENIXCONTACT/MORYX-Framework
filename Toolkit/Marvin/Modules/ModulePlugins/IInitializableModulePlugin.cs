namespace Marvin.Modules.ModulePlugins
{
    /// <summary>
    /// Based on IInitializable and IModulePlugin it offers a simple three-stage lifecycle. 
    /// Initialize, start and dispose.
    /// So with this interface a module plugins can be initialized before their start.
    /// </summary>
    public interface IInitializableModulePlugin : IInitializable, IModulePlugin
    {        
    }
}
