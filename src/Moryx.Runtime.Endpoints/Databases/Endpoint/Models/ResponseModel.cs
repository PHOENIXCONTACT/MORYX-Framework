namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// General model for API responses
    /// </summary>
    public class ResponseModel<T>
    {
        /// <summary>
        /// Result of the response. `null` on error
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// List of errors. `null` if none
        /// </summary>
        public string[] Errors { get; set; }
    }
}
