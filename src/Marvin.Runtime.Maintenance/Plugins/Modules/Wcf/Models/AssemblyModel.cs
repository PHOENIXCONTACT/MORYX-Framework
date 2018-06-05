namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Holds importats information about an assembly.
    /// </summary>
    public class AssemblyModel
    {
        /// <summary>
        /// Name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the assembly.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Name of the bundle where the assembly belongs to.
        /// </summary>
        public string Bundle { get; set; }
    }
}
