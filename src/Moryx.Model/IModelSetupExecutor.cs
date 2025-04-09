// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
        Task Execute(IDatabaseConfig config, IModelSetup setup, string setupData);
    }
}