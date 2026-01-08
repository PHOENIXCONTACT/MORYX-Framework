// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Configuration for the user assignment
    /// </summary>
    [DataContract]
    public class UsersConfig
    {
        /// <summary>
        /// Flag if the shift manager is enabled
        /// </summary>
        [DataMember, Description("Flag if users are required")]
        public bool UserRequired { get; set; }
    }
}
