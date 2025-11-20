// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.StateMachines;
using Moryx.ControlSystem.Simulation;
using Moryx.Resources.Benchmarking.Messages;
using Moryx.Resources.Benchmarking;

namespace Moryx.Simulation.Tests
{
    public class SimulatedDummyTestDriver : Driver, IMessageDriver<object>, ISimulationDriver
    {
        public bool HasChannels => false;

        public IDriver Driver => this;

        public string Identifier => Name;

        private SimulationState _simulatedState;
        public virtual SimulationState SimulatedState
        {
            get => _simulatedState;
            private set
            {
                _simulatedState = value;
                SimulatedStateChanged?.Invoke(this, value);
            }
        }

        [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
        public virtual AssemblyTestCell Cell { get; set; }

        public virtual IEnumerable<ICell> Usages => [Cell];

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

        public virtual void Send(object payload)
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

        public Task SendAsync(object payload, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public virtual void Ready(IActivity activity)
        {
            SimulatedState = SimulationState.Requested;

            Received?.Invoke(this, new WorkpieceArrivedMessage { ProcessId = activity.Process.Id });
        }

        public virtual void Result(SimulationResult result)
        {
            Received?.Invoke(this, new AssemblyCompletedMessage { Result = result.Result });
        }

        public virtual event EventHandler<object> Received;

        public virtual event EventHandler<SimulationState> SimulatedStateChanged;
    }

    public class DummyDriverState : DriverState<SimulatedDummyTestDriver>
    {
        [StateDefinition(typeof(FakeDriverState), IsInitial = true)]
        public int InitialState = 0;

        public DummyDriverState(SimulatedDummyTestDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Offline)
        {
        }

        public void ForceState(StateClassification classification)
        {
            Classification = classification;
        }
    }
}

