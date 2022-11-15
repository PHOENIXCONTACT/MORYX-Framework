// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workplans.Transitions;
using System.ComponentModel.DataAnnotations;
using Moryx.Properties;

namespace Moryx.Workplans.WorkplanSteps
{
    /// <summary>
    /// Step that creates <see cref="SubworkflowTransition"/> with the given workplan
    /// </summary>
    [DataContract]
    [ClassDisplay(ResourceType = typeof(Strings), Name = "SubworkflowStep_Name", Description = "SubworkflowStep_Description")]
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
