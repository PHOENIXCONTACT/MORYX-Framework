using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    /// <summary>
    /// Configuration of the mail client.
    /// </summary>
    [DataContract]
    public class MailClientConfig
    {
        /// <summary>
        /// Host name of mail server
        /// </summary>
        [DataMember]
        [DefaultValue("smtp-int.de.phoenixcontact.com")]
        public string MailServer { get; set; }
        /// <summary>
        /// Port on target system
        /// </summary>
        [DataMember]
        [DefaultValue(25)]
        public int Port { get; set; }
        /// <summary>
        /// Sender address to be used by client
        /// </summary>       
        [DataMember]
        [DefaultValue("energiemanagement@phoenixcopntact.com")]
        public string SenderAddress { get; set; }
        /// <summary>
        /// Name of sender
        /// </summary>
        [DataMember]
        [DefaultValue("Energiemanagement")]
        public string Sender { get; set; }
    }
}