namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Interface for writing output
    /// </summary>
    public interface IOutput<TOut>
    {
        /// <summary>
        /// Access flags for the output
        /// </summary>
        SupportedAccess Access { get; }

        /// <summary>
        /// Single value output
        /// Only available for <see cref="SupportedAccess.Single"/>
        /// </summary>
        TOut Value { get; set;  }

        /// <summary>
        /// Index based output
        /// Only available for <see cref="SupportedAccess.Index"/>
        /// </summary>
        TOut this[int index] { get; set; }

        /// <summary>
        /// Key based output
        /// Only available for <see cref="SupportedAccess.Key"/>
        /// </summary>
        TOut this[string key] { get; set; }
    }
}