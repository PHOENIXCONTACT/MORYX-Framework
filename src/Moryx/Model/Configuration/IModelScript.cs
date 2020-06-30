// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Model
{
    /// <summary>
    /// Interface for all 
    /// </summary>
    public interface IModelScript
    {
        /// <summary>
        /// Name of this script
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag if this script is intended for database creation
        /// </summary>
        bool IsCreationScript { get; }

        /// <summary>
        /// Get script text
        /// </summary>
        string GetSql();
    }
}
