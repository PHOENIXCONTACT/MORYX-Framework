// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Configuration
{
    /// <summary>
    /// Interface for updating databases from a specified version to
    /// an specified version.
    /// </summary>
    public interface IDatabaseUpdate
    {
        /// <summary>
        /// Description of this setup script
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version from which the database should be migrated
        /// </summary>
        int From { get; }

        /// <summary>
        /// Version to which the database should be migrated
        /// </summary>
        int To { get; }
    }

    /// <summary>
    /// Context for the database update
    /// </summary>
    public interface IUpdateContext
    {
        /// <summary>
        /// Execute sql commands
        /// </summary>
        void ExecuteSql(string sqlText);

        /// <summary>
        /// Execute embedded script located in folder scripts
        /// </summary>
        void ExecuteScript(string fileName);
    }
}
