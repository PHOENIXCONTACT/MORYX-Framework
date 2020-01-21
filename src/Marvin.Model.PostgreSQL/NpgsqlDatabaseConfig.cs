using System.Runtime.Serialization;

namespace Marvin.Model.PostgreSQL
{
    /// <summary>
    /// Database config for the Npgsql databases
    /// </summary>
    [DataContract]
    public class NpgsqlDatabaseConfig : DatabaseConfig
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NpgsqlDatabaseConfig"/>
        /// </summary>
        public NpgsqlDatabaseConfig()
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