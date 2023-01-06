// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Implementation of a null constraint which will be the default implementation which is always ok.
    /// </summary>
    public class NullConstraint: IConstraint
    {
        /// <summary>
        /// Implementation of a null constraint. Will always be true and will check nothing.
        /// </summary>
        public bool Check(IConstraintContext context)
        {
            return true;
        }
    }
}
