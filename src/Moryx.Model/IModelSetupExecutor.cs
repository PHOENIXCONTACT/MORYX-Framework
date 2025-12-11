// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Configuration;

namespace Moryx.Model
{
    /// <summary>
    /// Handles and executes model setups for a certain database context
    /// </summary>
    public interface IModelSetupExecutor
    {
        /// <summary>
        /// Returns all possible setups for the given DbContext
        /// </summary>
        IReadOnlyList<IModelSetup> GetAllSetups();

        /// <summary>
        /// Executes the given setup
        /// </summary>
        Task ExecuteAsync(DatabaseConfig config, IModelSetup setup, string setupData);
    }
}
