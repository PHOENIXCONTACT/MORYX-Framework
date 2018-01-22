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
[assembly: AssemblyConfiguration("MarvinPlatform-CI #854")]
[assembly: AssemblyCompany("PHOENIX CONTACT GmbH & Co. KG")]
[assembly: AssemblyProduct("Marvin.Resources.Management")]
[assembly: AssemblyCopyright("Copyright © PHOENIX CONTACT GmbH & Co. KG 2016")]
[assembly: AssemblyTrademark("")]
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
[assembly: AssemblyVersion(ModuleVersion.Version)]
[assembly: AssemblyFileVersion(ModuleVersion.Version)]

// Marvin attributes
[assembly: ResourcesBundle]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo(ResourceTypeController.AssemblyName)]
[assembly: InternalsVisibleTo("Marvin.Resources.IntegrationTests")]
[assembly: InternalsVisibleTo("Marvin.Resources.Management.Tests")]
