// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.AbstractionLayer;
using Moryx.Modules;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Plugin interface for the resource selector
    /// </summary>
    public interface ICellSelector : IConfiguredPlugin<CellSelectorConfig>
    {
        /// <summary>
        /// Sort or filter the available resources for an activity to determine
        /// the target resources
        /// </summary>
        IReadOnlyList<ICell> SelectCells(IActivity activity, IReadOnlyList<ICell> availableCells);
    }
}
