// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;

namespace Marvin.Configuration
{
    /// <summary>
    /// Default value attribute which provides the current count of CPU cores
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CpuCountAttribute : DefaultValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CpuCountAttribute"/>
        /// All cores will be used
        /// </summary>
        public CpuCountAttribute() : this(0)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CpuCountAttribute"/>
        /// Number of cores which will be reserved
        /// </summary>
        public CpuCountAttribute(int reserve) : base(Environment.ProcessorCount > reserve ? Environment.ProcessorCount - reserve : 1)
        {
        }
    }
}
