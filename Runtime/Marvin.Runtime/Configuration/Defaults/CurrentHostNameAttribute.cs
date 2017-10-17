using System;
using System.ComponentModel;
using System.Net;

namespace Marvin.Runtime.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CurrentHostNameAttribute : DefaultValueAttribute
    {
        public CurrentHostNameAttribute() : base(Dns.GetHostName())
        {
        }
    }
}
