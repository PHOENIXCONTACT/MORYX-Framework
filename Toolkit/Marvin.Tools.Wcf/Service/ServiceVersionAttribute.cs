using System;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Attribute used to declare a services version for clients to check compliance
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceVersionAttribute : Attribute
    {
        private string _serverVersion = "1.0.0.0";
        /// <summary>
        /// Version of the server
        /// </summary>
        public string ServerVersion
        {
            get { return _serverVersion; }
            set { _serverVersion = value; }
        }

        private string _minClientVersion = "1.0.0.0";
        /// <summary>
        /// Min required version of the server
        /// </summary>
        public string MinClientVersion
        {
            get { return _minClientVersion; }
            set { _minClientVersion = value; }
        }
    }
}
