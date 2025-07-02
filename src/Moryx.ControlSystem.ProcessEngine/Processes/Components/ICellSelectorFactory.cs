using Moryx.Container;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Factory to create resource sorters
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface ICellSelectorFactory
    {
        /// <summary>
        /// Create <see cref="ICellSelector"/> instance based on config
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        ICellSelector Create(CellSelectorConfig config);

        /// <summary>
        /// Destroy instance of <see cref="ICellSelector"/>
        /// </summary>
        /// <param name="instance"></param>
        void Destroy(ICellSelector instance);
    }
}