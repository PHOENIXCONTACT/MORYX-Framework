// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;

namespace Moryx.Runtime.Configuration
{
    /// <summary>
    /// Attribute used to reference another <see cref="IConfig"/>. Do not decorate the property with data member as it will write a copy of the config to your xml.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SharedConfigAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SharedConfigAttribute"/>
        /// </summary>
        /// <param name="useCopy">Flag if the injected object shall be a copy</param>
        public SharedConfigAttribute(bool useCopy)
        {
            UseCopy = useCopy;
        }

        /// <summary>
        /// Flag if the injected object shall be a copy
        /// </summary>
        public bool UseCopy { get; private set; }
    }
}
