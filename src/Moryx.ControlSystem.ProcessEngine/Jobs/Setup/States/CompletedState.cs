// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    [DisplayName("Completed")]
    internal class CompletedState : SetupJobStateBase
    {
        public override bool IsStable => true;

        public CompletedState(JobDataBase context, StateMap stateMap)
            : base(context, stateMap, JobClassification.Completed)
        {
        }
    }
}
