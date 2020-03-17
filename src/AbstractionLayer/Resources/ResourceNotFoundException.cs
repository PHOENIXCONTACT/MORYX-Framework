// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.AbstractionLayer.Resources
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
            : base($"No resource found providing capabilities or too many matches: {requiredCapabilities}")
        {
        }

        /// <summary>
        /// Initialize the exception with a database id
        /// </summary>
        /// <param name="id">Id that was not found</param>
        public ResourceNotFoundException(long id)
            : base($"No resource found with id: {id}")
        {
        }
    }
}
