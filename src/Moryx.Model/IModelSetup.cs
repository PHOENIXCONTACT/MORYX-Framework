// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity;

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
        /// <param name="dbContext">Context for db access</param>
        /// <param name="setupData">Any data for the setup, excel or sql etc</param>
        void Execute(DbContext dbContext, string setupData);
    }

    /// <summary>
    /// Setup to initialize a database
    /// </summary>
    public interface IModelSetup<in TContext> : IModelSetup where TContext : DbContext
    {
        /// <summary>
        /// Execute setup in this context
        /// </summary>
        /// <param name="dbContext">Context for db access</param>
        /// <param name="setupData">Any data for the setup, excel or sql etc</param>
        void Execute(TContext dbContext, string setupData);
    }

    /// <summary>
    /// Base class for model setups
    /// </summary>
    public abstract class ModelSetupBase<TContext> : IModelSetup<TContext> where TContext : DbContext
    {
        /// <inheritdoc />
        public abstract int SortOrder { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public abstract string SupportedFileRegex { get; }

        /// <inheritdoc />
        public void Execute(DbContext dbContext, string setupData)
        {
            Execute((TContext) dbContext, setupData);
        }

        /// <inheritdoc />
        public abstract void Execute(TContext dbContext, string setupData);
    }
}
