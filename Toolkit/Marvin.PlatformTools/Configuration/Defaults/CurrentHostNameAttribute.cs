using System;
using System.ComponentModel;
using System.Net;

namespace Marvin.Configuration
{
    /// <summary>
    /// Default value attribute which provides the current HostName of this computer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CurrentHostNameAttribute : DefaultValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CurrentHostNameAttribute"/>
        /// </summary>
        public CurrentHostNameAttribute() : base(Dns.GetHostName())
        {
        }
    }
}
