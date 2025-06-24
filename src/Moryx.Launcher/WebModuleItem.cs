using Moryx.Asp.Integration;

namespace Moryx.Launcher
{
    /// <summary>
    /// Contains properties holding related information of a WebModule to process in the Shell
    /// </summary>
    public class WebModuleItem
    {
        /// <summary>
        ///  Unique route of the module
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Sort index of the module
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// Title of the module
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Recognizable icon of the module
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Description of the module
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category of the module
        /// </summary>
        public ModuleCategory Category { get; set; }

        /// <summary>
        /// Optional URL to an event stream
        /// </summary>
        public string EventStream { get; set; }
    }
}