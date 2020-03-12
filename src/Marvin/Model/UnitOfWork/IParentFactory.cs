// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Model
{
    /// <summary>
    /// Interface for all <see cref="T:Marvin.Model.IUnitOfWorkFactory" /> to give inherited factories
    /// the chance to register themselves
    /// </summary>
    public interface IParentFactory
    {
        /// <summary>
        /// Register a new child for this parent
        /// </summary>
        void RegisterChild(IUnitOfWorkFactory childFactory, string childModelName);
    }
}
