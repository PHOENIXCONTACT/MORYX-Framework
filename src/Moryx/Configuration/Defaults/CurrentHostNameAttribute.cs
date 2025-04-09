// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Moryx.Configuration
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
        public CurrentHostNameAttribute() : base("localhost")
        {
            try
            {
                SetValue(Dns.GetHostName());
            }
            catch (SocketException)
            {
                // ignored -> default is "localhost"
            }
        }
    }
}
