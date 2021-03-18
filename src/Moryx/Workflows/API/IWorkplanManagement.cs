using System;

namespace Moryx.Workflows
{
    /// <summary>
    /// Additional facade interface for components that store and provide workplans
    /// </summary>
#pragma warning disable 618
    public interface IWorkplanManagement : IWorkplans
#pragma warning restore 618
    {
        /// <summary>
        /// A workplan was changed
        /// </summary>
        event EventHandler<Workplan> WorkplanChanged;
    }
}