namespace Moryx.AbstractionLayer
{
    // TODO: Remove if https://github.com/PHOENIXCONTACT/MORYX-Platform/issues/103 is done
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