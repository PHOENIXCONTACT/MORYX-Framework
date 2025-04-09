// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Recipes
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
    }
}
