// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Implementation of <see cref="IRecipe"/> with an defined <see cref="IWorkplan"/>
    /// </summary>
    public interface IWorkplanRecipe : IRecipe
    {
        /// <summary>
        /// Workplan of this recipe
        /// </summary>
        IWorkplan Workplan { get; set; }

        /// <summary>
        /// Steps that shall be skipped
        /// </summary>
        ICollection<long> DisabledSteps { get; }

        /// <summary>
        /// Pre-assigned resources for tasks from the workplan
        /// </summary>
        IDictionary<long, long> TaskAssignment { get; }
    }
}
