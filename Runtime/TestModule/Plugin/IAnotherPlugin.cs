using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    public interface IAnotherPlugin : IConfiguredModulePlugin<AnotherPluginConfig>
    {
         
    }

    public interface IAnotherSubPlugin : IConfiguredModulePlugin<AnotherSubConfig>
    {
        
    }
}