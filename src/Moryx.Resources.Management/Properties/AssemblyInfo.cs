using System.Runtime.CompilerServices;
using Moryx.AbstractionLayer;
using Moryx.Modules;
using Moryx.Resources.Management;

[assembly: Bundle(AbstractionLayerBundle.Name, AbstractionLayerBundle.Version)]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo(ResourceProxyBuilder.AssemblyName)]
[assembly: InternalsVisibleTo("Moryx.Resources.IntegrationTests")]
[assembly: InternalsVisibleTo("Moryx.Resources.Management.Tests")]