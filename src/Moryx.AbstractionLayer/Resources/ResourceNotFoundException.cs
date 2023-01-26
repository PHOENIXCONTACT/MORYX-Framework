// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Exception which can be thrown iff a resource was not found
    /// </summary>
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ResourceNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="requiredCapabilities">The required capabilities.</param>
        public ResourceNotFoundException(ICapabilities requiredCapabilities)
            : base(string.Format(Strings.ResourceNotFoundException_ByCapabilities_Message, requiredCapabilities))
        {
        }

        /// <summary>
        /// Initialize the exception with a database id
        /// </summary>
        /// <param name="id">Id that was not found</param>
        public ResourceNotFoundException(long id)
            : base(string.Format(Strings.ResourceNotFoundException_ById_Message, id))
        {
        }
    }
}
