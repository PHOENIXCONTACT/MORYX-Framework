// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Tracing for wpc based activities
    /// </summary>
    [DataContract]
    public class ProcessHolderTracing : Tracing
    {
        /// <summary>
        /// Id of the process holder where the activity was executed
        /// </summary>
        public long HolderId { get; set; }

        ///
        public virtual double Relative => Progress;
    }
}
