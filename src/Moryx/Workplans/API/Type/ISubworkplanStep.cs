// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Special kind of step that operates on a workplan template
    /// </summary>
    public interface ISubworkplanStep : IWorkplanStep
    {
        /// <summary>
        /// Id of the subworkplan
        /// </summary>
        long WorkplanId { get; }

        /// <summary>
        /// Subworkplan referenced in this step
        /// </summary>
        IWorkplan Workplan { get; set; }
    }
}
