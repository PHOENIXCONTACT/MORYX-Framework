using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// Model for a user config.
    /// </summary>
    [DataContract]
    public class ClientConfigModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfigModel"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="jsonText">The json text.</param>
        public ClientConfigModel(string typeName, string jsonText)
        {
            TypeName = typeName;
            JsonText = jsonText;
        }

        /// <summary>
        /// The config in a JsonString.
        /// </summary>
        [DataMember]
        public string JsonText { get; set; }

        /// <summary>
        /// TypeName of configuration
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        /// <summary>
        /// Returns the type of config
        /// </summary>
        public override string ToString()
        {
            return TypeName;
        }
    }
}