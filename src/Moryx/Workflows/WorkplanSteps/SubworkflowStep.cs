// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workflows.Transitions;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Workflows.WorkplanSteps
{
    /// <summary>
    /// Step that creates <see cref="SubworkflowTransition"/> with the given workplan
    /// </summary>
    [DataContract]
    [ClassDisplay(Name = "Sub-Workplan", Description = "Nests a sub-workplan in the current workplan")]
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
            var indexResolver = _indexResolver ??= TransitionBase.CreateIndexResolver(OutputDescriptions);
            var transition = new SubworkflowTransition(engine, indexResolver);
            return transition;
        }
    }
}
