// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Simulation;

namespace Moryx.ControlSystem.Simulator
{
    internal class ProcessMovement
    {
        public IProcess Process { get; set; }

        public IActivity NextActivity { get; set; }

        public ISimulationDriver Source { get; set; }

        public ISimulationDriver Target { get; set; }

        public DateTime Started { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
