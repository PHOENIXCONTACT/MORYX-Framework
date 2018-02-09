namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for parameters used to import files as products
    /// </summary>
    public interface IFileImportParameters : IImportParameters
    {
        /// <summary>
        /// File name or content of the file
        /// </summary>
        string File { get; set; }

        /// <summary>
        /// Extension of the file
        /// </summary>
        string FileExtension { get; set; }
    }
}