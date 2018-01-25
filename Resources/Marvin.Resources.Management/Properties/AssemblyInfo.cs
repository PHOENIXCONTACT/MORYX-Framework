using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Marvin.AbstractionLayer.Resources;
using Marvin.Modules;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Marvin.Resources;
using Marvin.Resources.Management;

[assembly: AssemblyTitle("Marvin.Resources.Management")]
[assembly: AssemblyDescription("Marvin Runtime Module: ResourceManager")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f14d282b-a0e8-46d7-92b0-a173be7ce3da")]

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

// Marvin attributes
[assembly: ResourcesBundle]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo(ResourceProxyBuilder.AssemblyName)]
[assembly: InternalsVisibleTo("Marvin.Resources.IntegrationTests")]
[assembly: InternalsVisibleTo("Marvin.Resources.Management.Tests")]
