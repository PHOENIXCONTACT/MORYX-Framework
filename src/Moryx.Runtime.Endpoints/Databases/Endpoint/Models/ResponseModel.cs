using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// General model for API responses
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// Result of the response. `null` on error
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// List of errors. `null` if none
        /// </summary>
        public string[] Error { get; set; }
    }
}
