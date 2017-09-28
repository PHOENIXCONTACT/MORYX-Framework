using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// A UserModel.
    /// </summary>
    [DataContract]
    public class UserModel
    {
        /// <summary>
        /// Constructor for the user model.
        /// </summary>
        public UserModel()
        {
            Groups = new List<string>();
        }

        /// <summary>
        /// The name of the user.
        /// </summary>
        [DataMember]
        public string Username { get; set; }
        
        /// <summary>
        /// The groups where the user is assigned to.
        /// </summary>
        [DataMember]
        public List<string> Groups { get; set; }
    }
}
