using System;
using System.ComponentModel;
using System.Net;
using Marvin.Testing;

namespace Marvin.Runtime.Configuration
{
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Property)]
    public class CurrentHostNameAttribute : DefaultValueAttribute
    {
        public CurrentHostNameAttribute() : base(Dns.GetHostName())
        {
        }
    }
}
