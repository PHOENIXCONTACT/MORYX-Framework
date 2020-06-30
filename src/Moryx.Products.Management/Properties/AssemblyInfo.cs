using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Moryx.AbstractionLayer;
using Moryx.Modules;
using Moryx.Products;
using Moryx.Products.Management;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Moryx.Products.Management")]
[assembly: AssemblyDescription("Moryx Runtime Module: ProductManagement")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f31704fe-753b-45e8-a7fd-adc4bdbaac24")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

// Moryx attributes
[assembly: Bundle(AbstractionLayerBundle.Name, AbstractionLayerBundle.Version)]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Moryx.Products.IntegrationTests")]
