namespace Marvin.Model
{
    /// <inheritdoc />
    public class DatabaseUpdate : IDatabaseUpdate
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public int From { get; set; }

        /// <inheritdoc />
        public int To { get; set; }
    }
}
