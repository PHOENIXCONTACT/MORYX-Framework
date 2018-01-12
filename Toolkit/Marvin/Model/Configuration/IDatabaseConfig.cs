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
        string Host { get; set; }

        /// <summary>
        /// Port to access the server
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Database to use
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// Databse user
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Password for <see cref="Username"/>
        /// </summary>
        string Password { get; set; }
    }
}