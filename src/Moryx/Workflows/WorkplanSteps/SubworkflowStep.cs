// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows.WorkplanSteps
{
    /// <summary>
    /// Step that creates <see cref="SubworkflowTransition"/> with the given workplan
    /// </summary>
    [DataContract]
    public class SubworkflowStep : SubWorkplanStep
    {
        private SubworkflowStep()
        {
            // Empty constructor for JSON
        }

        /// <summary>
        /// Create step from workplan
        /// </summary>
        public SubworkflowStep(IWorkplan workplan) : base(workplan)
        {
        }
        
        private IIndexResolver _indexResolver;
        /// <summary>
        /// Instantiate transition from this step
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            var engine = Workflow.CreateEngine(Workplan, context);
            var indexResolver = _indexResolver ?? (_indexResolver = TransitionBase.CreateIndexResolver(OutputDescriptions));
            var transition = new SubworkflowTransition(engine, indexResolver);
            return transition;
        }
    }
}
