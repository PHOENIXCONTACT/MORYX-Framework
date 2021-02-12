namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Additional interface for named task steps
    /// </summary>
    public interface INamedTaskStep
    {
        /// <summary>
        /// Name of the task step
        /// </summary>
        string Name { get; set; }
    }
}