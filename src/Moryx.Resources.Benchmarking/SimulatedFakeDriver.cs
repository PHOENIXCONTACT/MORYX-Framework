// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;
using Moryx.ControlSystem.Simulation;
using Moryx.Resources.Benchmarking.Messages;

namespace Moryx.Resources.Benchmarking
{
    [ResourceRegistration]
    public class SimulatedFakeDriver : Driver, IMessageDriver, ISimulationDriver
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

        public IEnumerable<ICell> Usages => [Cell];

        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await base.OnStartAsync(cancellationToken);

            SimulatedState = SimulationState.Idle;
        }

        public IMessageChannel Channel(string identifier)
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

        public Task SendAsync(object payload, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Ready(Activity activity)
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

    public class FakeDriverState : SyncDriverState<SimulatedFakeDriver>
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

