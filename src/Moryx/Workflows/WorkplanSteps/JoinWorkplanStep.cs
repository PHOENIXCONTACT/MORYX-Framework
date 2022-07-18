// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workflows.Transitions;

namespace Moryx.Workflows.WorkplanSteps
{
    /// <summary>
    /// Workplan step to join multiple inputs
    /// </summary>
    [DataContract]    
    [Display(Name = "Join", Description = "Rejoins the incoming paths into a single output")]
    public class JoinWorkplanStep : WorkplanStepBase
    {
        private JoinWorkplanStep()
        {
        }

        /// 
        public override string Name => "Join";

        /// <summary>
        /// Create new join step for certain number of inputs
        /// </summary>
        /// <param name="inputs">Number of inputs</param>
        public JoinWorkplanStep(ushort inputs = 2)
        {
            Inputs = new IConnector[inputs];
        }

        /// 
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            return new JoinTransition();
        }
    }
}
