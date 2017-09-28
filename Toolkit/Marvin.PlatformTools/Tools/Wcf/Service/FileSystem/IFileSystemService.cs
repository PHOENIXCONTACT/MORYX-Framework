using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Marvin.Tools.Wcf.FileSystem
{
    /// <summary>
    /// Service contract for the file system service.
    /// </summary>
    [ServiceContract]
    public interface IFileSystemService
    {
        /// <summary>
        /// Get a stream to the file configured.
        /// </summary>
        /// <returns>A stream to the file.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "*")]
        Stream StreamFile();

        /// <summary>
        /// Get the content of the path from the parameters.
        /// </summary>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <returns>List of <see cref="FileModel"/> of the path.</returns>
        [OperationContract]
        List<FileModel> GetContentOfPath(string relativePath);

        /// <summary>
        /// Upload a file to the web folder.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>True: file uploaded, false: an error occured while uploading. File was not uploaded.</returns>
        [OperationContract]
        bool UploadFile(RemoteFile file);

        /// <summary>
        /// Delete a file on the given path.
        /// </summary>
        /// <param name="relativePath">relative path to the file which should be deleted.</param>
        /// <returns>True: file deleted, false: an error occured and the file was not deleted.</returns>
        [OperationContract]
        bool DeleteFile(string relativePath);
    }
}
