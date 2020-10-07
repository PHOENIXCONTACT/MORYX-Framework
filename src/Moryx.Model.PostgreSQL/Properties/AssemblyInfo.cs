using System.Runtime.CompilerServices;
using Moryx;

// Force System.Threading.Tasks.Extensions assembly copied to the output directory
[assembly: ForceAssemblyReference(typeof(AsyncMethodBuilderAttribute))]
