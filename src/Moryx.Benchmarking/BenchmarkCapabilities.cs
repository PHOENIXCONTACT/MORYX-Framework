// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Benchmarking
{
    [DataContract]
    public class BenchmarkCapabilities : CapabilitiesBase
    {
        [DataMember]
        public int Step { get; set; }

        public BenchmarkCapabilities() : this(0)
        {
        }

        public BenchmarkCapabilities(int step)
        {
            Step = step;
        }

        /// <summary>
        /// Checks whether the provided capabilities comply to the requested ones.
        /// </summary>
        /// <param name="provided">The provided capabilities. The argument may be casted directly to the required type because a type
        ///             check is already done and if there is an unexpected cast exception, it will be caught by the the calling code.</param>
        /// <returns>
        /// <c>True</c>, if the provided capabilities comply to this one of <c>false</c>, if not.
        /// </returns>
        protected override bool ProvidedBy(ICapabilities provided)
        {
            var benchmark = provided as BenchmarkCapabilities;
            return benchmark != null && benchmark.Step == Step;
        }
    }
}
