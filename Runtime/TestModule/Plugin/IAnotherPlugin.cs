using Marvin.Modules;

namespace Marvin.TestModule
{
    public interface IAnotherPlugin : IConfiguredPlugin<AnotherPluginConfig>
    {
         
    }

    public interface IAnotherSubPlugin : IConfiguredPlugin<AnotherSubConfig>
    {
        
    }
}