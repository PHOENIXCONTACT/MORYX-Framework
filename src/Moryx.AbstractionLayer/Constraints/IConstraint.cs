// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Constraints
{
    /// <summary>
    /// Constraint for the context provided.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Checks if the context matches to the constraints.
        /// </summary>
        /// <returns>True: all is ok and valid, False: nothing is ok and not valid.</returns>
        bool Check(IConstraintContext context);
    }
}
