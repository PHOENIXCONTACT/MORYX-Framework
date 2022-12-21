// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workplans.Transitions;

namespace Moryx.Workplans.WorkplanSteps
{
    /// <summary>
    /// Base class for all workplanstep
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class WorkplanStepBase : IWorkplanStep
    {
        /// <summary>
        /// Create step and set id
        /// </summary>
        protected WorkplanStepBase()
        {
            Inputs = new IConnector[1];
            Outputs = new IConnector[1];
            OutputDescriptions = new[] { new OutputDescription { Name = "Complete", OutputType = OutputType.Success} };
        }

        /// <inheritdoc/>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Name of the step given by the user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public IConnector[] Inputs { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public IConnector[] Outputs { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public OutputDescription[] OutputDescriptions { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public NodePosition Position { get; set; }

        /// <inheritdoc/>
        public ITransition CreateInstance(IWorkplanContext context)
        {
            var transition = Instantiate(context);
            transition.Id = Id;
            transition.Inputs = new IPlace[Inputs.Length];
            transition.Outputs = new IPlace[Outputs.Length];
            return transition;
        }

        ///
        protected abstract TransitionBase Instantiate(IWorkplanContext context);
    }
}
