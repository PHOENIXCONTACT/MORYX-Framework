// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// View model for the management of available permissions
    /// </summary>
    public class PermissionsViewModel
    {
        /// <summary>
        /// Maps to <see cref="Permission.Id"/>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Maps to <see cref="Permission.Name"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Maps to <see cref="Permission.Roles"/>
        /// </summary>
        public string[] Roles { get; set; }
    }
}
