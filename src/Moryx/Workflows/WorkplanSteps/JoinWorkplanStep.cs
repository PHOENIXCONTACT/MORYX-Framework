// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Workflows.Transitions;
using System.ComponentModel.DataAnnotations;
using Moryx.Properties;

namespace Moryx.Workflows.WorkplanSteps
{
    /// <summary>
    /// Workplan step to join multiple inputs
    /// </summary>
    [DataContract]    
    [ClassDisplay(ResourceType = typeof(Strings), Name = "JoinWorkplanStep_Name", Description = "JoinWorkplanStep_Description")]
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
