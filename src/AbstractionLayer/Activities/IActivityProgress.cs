namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Typed interface for the activity progress
    /// </summary>
    public interface IActivityProgress : IActivityTracing
    {
        /// <summary>
        /// Relative progress in per-cent as value between 0 and 100
        /// </summary>
        double Relative { get; }
    }
}