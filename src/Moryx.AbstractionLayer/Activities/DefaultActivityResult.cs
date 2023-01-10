// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Default results for activities. 
    /// </summary>
    public enum DefaultActivityResult
    {
        /// <summary>
        /// Activity was successfull
        /// </summary>
        [OutputType(OutputType.Success)]
        Success = 0,

        /// <summary>
        /// The activity was failed
        /// </summary>
        [OutputType(OutputType.Failure)]
        Failed = 1,

        /// <summary>
        /// The activity was failed because of an technical error
        /// </summary>
        [OutputType(OutputType.Failure)]
        TechnicalError = 2
    }
}
