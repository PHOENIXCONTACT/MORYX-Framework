// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
    public sealed class NullActivityParameters : IParameters
    {
        /// <see cref="IParameters"/>
        public IParameters Bind(Process process)
        {
            return this;
        }
    }
}
