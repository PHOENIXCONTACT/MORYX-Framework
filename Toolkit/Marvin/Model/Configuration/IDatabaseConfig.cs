using System.Security.Cryptography.X509Certificates;
using Marvin.Configuration;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for database configuration
    /// </summary>
    public interface IDatabaseConfig : IConfig
    {
        /// <summary>
        /// Databse server
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Port to access the server
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Database to use
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// Schema to store the model
        /// </summary>
        string Schema { get; set; }

        /// <summary>
        /// Databse user
        /// </summary>
        string User { get; set; }

        /// <summary>
        /// Password for <see cref="User"/>
        /// </summary>
        string Password { get; set; }
    }
}