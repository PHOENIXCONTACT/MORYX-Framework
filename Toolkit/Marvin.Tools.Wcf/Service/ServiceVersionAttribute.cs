using System;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Attribute used to declare a services version for clients to check compliance
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceVersionAttribute : Attribute
    {
        /// <summary>
        /// Version of the server
        /// </summary>
        public string ServerVersion { get; set; } = "1.0.0.0";

        /// <summary>
        /// Min required version of the server
        /// </summary>
        public string MinClientVersion { get; set; } = "1.0.0.0";
    }
}
