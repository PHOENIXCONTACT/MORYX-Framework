// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Workflows.Compiler
{
    /// <summary>
    /// A transition that was compiled from a workplan instance
    /// </summary>
    public abstract class CompiledTransition
    {
        /// <summary>
        /// Locally unique id of the step
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Id of the workplan element that was compiled to this step
        /// </summary>
        public long SourceId { get; internal set; }

        /// <summary>
        /// Follow up references of outputs
        /// </summary>
        internal OutputRelation[] OutputRelations { get; set; }
    }
}
