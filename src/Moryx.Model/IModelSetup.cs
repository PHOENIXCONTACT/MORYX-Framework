// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Model
{
    /// <summary>
    /// Setup to initialize a database
    /// </summary>
    public interface IModelSetup
    {
        /// <summary>
        /// For this data model unique setup id
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Display name of this setup
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Short description what data this setup contains
        /// </summary>
        string Description { get; }

        /// <summary>
        /// FileType supported by this setup
        /// </summary>
        string SupportedFileRegex { get; }

        /// <summary>
        /// Execute setup in this context
        /// </summary>
        /// <param name="openContext">Context for db access</param>
        /// <param name="setupData">Any data for the setup, excel or sql etc</param>
        void Execute(IUnitOfWork openContext, string setupData);
    }
}
