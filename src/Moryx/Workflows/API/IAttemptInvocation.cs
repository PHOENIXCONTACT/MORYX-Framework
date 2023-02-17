// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Interface for a transition handling wrapper
    /// </summary>
    public interface IAttemptInvocation
    {
        /// <summary>
        /// Tries to map the transition to a certain type. If successful it invokes
        /// the target and returns true, otherwise it returns false.
        /// </summary>
        bool TryInvoke(ITransition transition);
    }
}
