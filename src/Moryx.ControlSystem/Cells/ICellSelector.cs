// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.Modules;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Plugin interface for the resource selector
    /// </summary>
    public interface ICellSelector : IAsyncConfiguredPlugin<CellSelectorConfig>
    {
        /// <summary>
        /// Sort or filter the available resources for an activity to determine
        /// the target resources
        /// </summary>
        Task<IReadOnlyList<ICell>> SelectCellsAsync(Activity activity, IReadOnlyList<ICell> availableCells, CancellationToken cancellationToken);
    }
}
