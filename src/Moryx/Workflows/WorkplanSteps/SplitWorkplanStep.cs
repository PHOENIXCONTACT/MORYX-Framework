// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Properties;
using Moryx.Workplans.Transitions;

namespace Moryx.Workplans.WorkplanSteps
{
    /// <summary>
    /// Workplanstep to split execution
    /// </summary>
    [DataContract]
    [ClassDisplay(ResourceType = typeof(Strings), Name = "SplitWorkplanStep_Name", Description = "SplitWorkplanStep_Description")]
    public class SplitWorkplanStep : WorkplanStepBase
    {
        private SplitWorkplanStep()
        {
            Name = "Split";
        }

        /// <summary>
        /// Create new split instance
        /// </summary>
        /// <param name="outputs"></param>
        public SplitWorkplanStep([Display(Name = "Outputs", Description = "Number of parallel paths")]int outputs = 2)
        {
            if (outputs <= 1)
                throw new ArgumentException("Split must have at least two outputs!");

            Outputs = new IConnector[outputs];
            OutputDescriptions = new OutputDescription[outputs];
            for (var i = 0; i < outputs; i++)
            {
                OutputDescriptions[i] = new OutputDescription{OutputType = OutputType.Success};
            }
        }

        /// <summary>
        /// Create transistion instance
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new SplitTransition();
        }
    }
}
