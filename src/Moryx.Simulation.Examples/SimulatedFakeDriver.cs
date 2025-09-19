// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Cells;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;
using Moryx.Simulation.Examples.Messages;
using Moryx.ControlSystem.Simulation;

namespace Moryx.Simulation.Examples
{
    [ResourceRegistration]
    public class SimulatedFakeDriver : Driver, IMessageDriver<object>, ISimulationDriver
    {
        public bool HasChannels => false;

        public IDriver Driver => this;

        public string Identifier => Name;

        private SimulationState _simulatedState;
        public SimulationState SimulatedState
        {
            get => _simulatedState;
            private set
            {
                _simulatedState = value;
                SimulatedStateChanged?.Invoke(this, value);
            }
        }

        [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
        public AssemblyCell Cell { get; set; }

        public IEnumerable<ICell> Usages => new[] { Cell };

        protected override void OnStart()
        {
            base.OnStart();

            SimulatedState = SimulationState.Idle;
        }

        public IMessageChannel<TChannel> Channel<TChannel>(string identifier)
        {
            throw new NotImplementedException();
        }

        public IMessageChannel<TSend, TReceive> Channel<TSend, TReceive>(string identifier)
        {
            throw new NotImplementedException();
        }

        public void Send(object payload)
        {
            switch (payload)
            {
                case AssembleProductMessage assemble:
                    SimulatedState = SimulationState.Executing;
                    break;
                case ReleaseWorkpieceMessage release:
                    SimulatedState = SimulationState.Idle;
                    break;
            }
        }

        public Task SendAsync(object payload)
        {
            return Task.CompletedTask;
        }

        public void Ready(IActivity activity)
        {
            SimulatedState = SimulationState.Requested;

            Received?.Invoke(this, new WorkpieceArrivedMessage { ProcessId = activity.Process.Id });
        }

        public void Result(SimulationResult result)
        {
            Received?.Invoke(this, new AssemblyCompletedMessage { Result = result.Result });
        }

        public event EventHandler<object> Received;

        public event EventHandler<SimulationState> SimulatedStateChanged;
    }

    public class FakeDriverState : DriverState<SimulatedFakeDriver>
    {
        [StateDefinition(typeof(FakeDriverState), IsInitial = true)]
        public int InitialState = 0;

        public FakeDriverState(SimulatedFakeDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Offline)
        {
        }

        public void ForceState(StateClassification classification)
        {
            Classification = classification;
        }
    }
}

