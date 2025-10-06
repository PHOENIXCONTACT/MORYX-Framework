// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Context for workplans that execute a process
    /// </summary>
    public class ProcessContext : IWorkplanContext
    {
        /// <summary>
        /// Create process context for process
        /// </summary>
        public ProcessContext(IProcess process)
        {
            Process = process;
        }

        /// <summary>
        /// Process the workplan is executed on
        /// </summary>
        public IProcess Process { get; }

        /// <inheritdoc />
        public virtual bool IsDisabled(IWorkplanStep step)
        {
            return ((IWorkplanRecipe)Process.Recipe).DisabledSteps.Contains(step.Id);
        }
    }
}
