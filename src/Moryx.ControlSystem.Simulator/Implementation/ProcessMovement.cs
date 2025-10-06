// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
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
