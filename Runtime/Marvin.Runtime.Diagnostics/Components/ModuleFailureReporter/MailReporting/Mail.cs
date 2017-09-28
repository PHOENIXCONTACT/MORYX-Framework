using System.Collections.Generic;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    /// <summary>
    /// Represents a mail object which contains the properties which a mail should have.
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// Recipient of mail
        /// </summary>
        public List<string> Recipients { get; set; }
        /// <summary>
        /// Subject of mail
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Full message body - may be html
        /// </summary>
        public string MessageBody { get; set; }
        /// <summary>
        /// Optional path to message attachment
        /// </summary>
        public string AttachmentPath { get; set; } 
    }
}
