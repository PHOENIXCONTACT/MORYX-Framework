// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to enforce the null object pattern
    /// </summary>
    [DataContract]
    public sealed class NullActivityParameters : IParameters
    {
        /// <see cref="IParameters"/>
        public IParameters Bind(IProcess process)
        {
            return this;
        }
    }
}
