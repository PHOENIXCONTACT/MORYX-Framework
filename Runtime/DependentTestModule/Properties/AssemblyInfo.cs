using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Marvin.Modules;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TestModule")]
[assembly: AssemblyDescription("Marvin Runtime Plugin: TestModule")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("f31704fe-753b-45e8-a7fd-adc4bdbaac24")]


// Marvin attributes
[assembly: Bundle("Marvin.TestModule", "1.0.0.0")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
