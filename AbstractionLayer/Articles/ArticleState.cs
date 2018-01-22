namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// The state of an Article. This will not occupy more than 4 Bits. Other enums can be added by bit-shifting
    /// </summary>
    public enum ArticleState : byte
    {
        /// <summary>
        /// Initial state
        /// </summary>
        Initial = 0,

        /// <summary>
        /// The article is currently in production
        /// </summary>
        InProduction = 1,

        /// <summary>
        /// The production on this article was paused
        /// </summary>
        Paused = 2,

        /// <summary>
        /// The production process succeeded.
        /// </summary>
        Success = 3,

        /// <summary>
        /// The production process failed.
        /// </summary>
        Failure = 4,

        /// <summary>
        /// State of part is inherited from the parent article
        /// </summary>
        Inherited = 5
    }
}