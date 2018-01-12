using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Marvin;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Marvin.Model.Npgsql")]
[assembly: AssemblyDescription("EntityFramework DataModel based on Npgsql")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("70964330-f80e-4e1e-a33b-2021bde37b2f")]

// Force System.Threading.Tasks.Extensions assembly copied to the output directory
[assembly: ForceAssemblyReference(typeof(AsyncMethodBuilderAttribute))]