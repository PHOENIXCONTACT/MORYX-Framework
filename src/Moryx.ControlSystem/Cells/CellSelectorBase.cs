// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;

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
        public virtual void Initialize(CellSelectorConfig config)
        {
            Config = (TConfig)config;
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
        public abstract IReadOnlyList<ICell> SelectCells(IActivity activity, IReadOnlyList<ICell> availableCells);
    }
}
