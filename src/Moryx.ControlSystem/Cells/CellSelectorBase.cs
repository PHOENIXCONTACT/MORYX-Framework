// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Base class for <see cref="ICellSelector"/>
    /// </summary>
    public abstract class CellSelectorBase : CellSelectorBase<CellSelectorConfig>
    {
    }

    /// <summary>
    /// Base class for <see cref="ICellSelector"/> with typed config
    /// </summary>
    public abstract class CellSelectorBase<TConfig> : ICellSelector
        where TConfig : CellSelectorConfig
    {
        /// <summary>
        /// Configuration of the selector
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <inheritdoc />
        public virtual Task InitializeAsync(CellSelectorConfig config)
        {
            Config = (TConfig)config;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StartAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task StopAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public abstract Task<IReadOnlyList<ICell>> SelectCellsAsync(IActivity activity, IReadOnlyList<ICell> availableCells);
    }
}
