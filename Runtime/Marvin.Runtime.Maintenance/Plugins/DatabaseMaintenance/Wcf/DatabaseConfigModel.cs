using System.Runtime.Serialization;
using Marvin.Model;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    [DataContract]
    public class DatabaseConfigModel
    {
        /// <summary>
        /// Databse server
        /// </summary>
        [DataMember]
        public string Server { get; set; }

        /// <summary>
        /// Port on server
        /// </summary>
        [DataMember]
        public int Port { get; set; }

        /// <summary>
        /// Database to use
        /// </summary>
        [DataMember]
        public string Database { get; set; }

        /// <summary>
        /// Schema in server
        /// </summary>
        [DataMember]
        public string Schema { get; set; }

        /// <summary>
        /// Databse user
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        /// Password for <see cref="IDatabaseConfig.User"/>
        /// </summary>
        [DataMember]
        public string Password { get; set; }
    }
}