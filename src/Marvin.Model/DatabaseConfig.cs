using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Model
{
    /// <inheritdoc />
    [DataContract]
    public class DatabaseConfig : IDatabaseConfig
    {
        /// <inheritdoc />
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <inheritdoc />
        public string LoadError { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Host { get; set; }

        /// <inheritdoc />
        [DataMember]
        public int Port { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Database { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Username { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Password { get; set; }
    }
}