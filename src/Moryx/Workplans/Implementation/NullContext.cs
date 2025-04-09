namespace Moryx.Workplans
{
    /// <summary>
    /// Null object pattern for the workplan context
    /// </summary>
    public struct NullContext : IWorkplanContext
    {
        /// <see cref="IWorkplanContext"/>
        public bool IsDisabled(IWorkplanStep step)
        {
            return false;
        }
    }
}