// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Products.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Interface for business object mapping onto an <see cref="IGenericColumns"/> entity
    /// </summary>
    public interface IGenericMapper
    {
        /// <summary>
        /// Check if the objects value has changed compared to the current database state
        /// </summary>
        bool HasChanged(IGenericColumns current, object updated);

        /// <summary>
        /// Read the value from the database and write it to the object
        /// </summary>
        void ReadValue(IGenericColumns source, object target);

        /// <summary>
        /// Write the value from the object to the database entity
        /// </summary>
        void WriteValue(object source, IGenericColumns target);
    }
}
