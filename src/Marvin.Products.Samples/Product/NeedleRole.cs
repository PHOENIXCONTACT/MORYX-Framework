namespace Marvin.Products.Samples
{
    /// <summary>
    /// Role of the referenced <see cref="NeedleProduct"/> in the <see cref="WatchProduct"/>
    /// </summary>
    public enum NeedleRole
    {
        /// <summary>
        /// Needle shows the hours
        /// </summary>
        Hours,

        /// <summary>
        /// Needle shows the minutes
        /// </summary>
        Minutes,

        /// <summary>
        /// Needle shows the seconds
        /// </summary>
        Seconds,

        /// <summary>
        /// Needle is used for a dedicated stopwatch
        /// </summary>
        Stopwatch
    }
}