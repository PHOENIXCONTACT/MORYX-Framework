using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    public interface IAnotherPlugin : IConfiguredPlugin<AnotherPluginConfig>
    {
         
    }

    public interface IAnotherSubPlugin : IConfiguredPlugin<AnotherSubConfig>
    {
        
    }
}