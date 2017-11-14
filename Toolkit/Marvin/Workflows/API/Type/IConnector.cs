namespace Marvin.Workflows
{
    /// <summary>
    /// Connector of one or more transistions
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Workplan unique element id of this connector
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Optional name for the place
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Classification of this connector
        /// </summary>
        NodeClassification Classification { get; set; }

        /// <summary>
        /// Create a place instance
        /// </summary>
        IPlace CreateInstance();
    }
}