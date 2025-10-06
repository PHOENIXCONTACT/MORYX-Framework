// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;
using Moryx.Logging;
using Moryx.Workplans;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Base class for all implementations of <see cref="ISetupTrigger"/>
    /// </summary>
    public abstract class SetupTriggerBase<TConfig> : ISetupTrigger
        where TConfig : SetupTriggerConfig
    {
        /// <summary>
        /// Factory to resolve bindings recipe based
        /// </summary>
        protected IBindingResolverFactory RecipeResolverFactory { get; } = new RecipeBindingResolverFactory();

        /// <summary>
        /// Component to create log entries.
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Typed config instance
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <inheritdoc />
        public int SortOrder
        {
            get => Config.SortOrder;
            set { }
        }

        /// <inheritdoc />
        public abstract SetupExecution Execution { get; }

        /// <inheritdoc />
        public virtual void Initialize(SetupTriggerConfig config)
        {
            Config = (TConfig) config;
            Logger = Logger?.GetChild(GetType().Name, GetType());
        }

        /// <inheritdoc />
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <inheritdoc />
        public abstract SetupEvaluation Evaluate(IProductRecipe recipe);

        /// <inheritdoc />
        public abstract IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe);
    }
}
