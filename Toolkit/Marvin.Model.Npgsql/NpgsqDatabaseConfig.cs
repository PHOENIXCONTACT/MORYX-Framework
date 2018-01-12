using System.Runtime.Serialization;

namespace Marvin.Model.Npgsql
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqDatabaseConfig : DatabaseConfig
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NpgsqDatabaseConfig"/>
        /// </summary>
        public NpgsqDatabaseConfig()
        {
            // Set default values
            Host = "localhost";
            Port = 5432;
            Database = "";
            Username = "postgres";
            Password = "postgres";
        }
    }
}