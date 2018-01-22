namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Adapter for product files. Use this property 
    /// </summary>
    public class ProductFile
    {
        /// <summary>
        /// Original name of the file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of this file
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// The complete file
        /// </summary>
        public byte[] File { get; set; }
    }
}