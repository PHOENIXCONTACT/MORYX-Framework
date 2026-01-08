// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Recipes
{
    /// <summary>
    /// A Recipe's state
    /// </summary>
    public enum RecipeState
    {
        /// <summary>
        /// The recipe is new and may be used for test jobs only.
        /// </summary>
        New = 0,

        /// <summary>
        /// The recipe is released and may be used now for regular jobs.
        /// </summary>
        Released = 1,

        /// <summary>
        /// The recipe is revoked and must not be for regular jobs anymore.
        /// </summary>
        Revoked = 2
    }
}
