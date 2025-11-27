// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Simulation;
using Moryx.Serialization;
using System.ComponentModel;
using Moryx.AbstractionLayer.Activities;

namespace Moryx.Drivers.Simulation.InOutDriver
{
    /// <summary>
    /// Base class for simulation drivers that implement the IInOutDriver interface
    /// </summary>
    public abstract class SimulatedInOutDriver : Driver, IInOutDriver, ISimulationDriver
    {
        [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
        public Cell Cell { get; set; }

        public IEnumerable<ICell> Usages => [Cell];

        private SimulationState _simulatedState;
        [EntrySerialize]
        public SimulationState SimulatedState
        {
            get => _simulatedState;
            set
            {
                _simulatedState = value;
                SimulatedStateChanged?.Invoke(this, value);
            }
        }

        protected override async Task OnStartAsync()
        {
            await base.OnStartAsync();

            SimulatedOutput.OutputSet += OnOutputSet;

            SimulatedState = SimulationState.Idle;
        }

        /// <summary>
        /// Driver should publish application specific Ready event
        /// Remember to set the SimulationState
        /// </summary>
        public abstract void Ready(IActivity activity);

        /// <summary>
        /// Driver should publish the result for the currently executed activity
        /// </summary>
        public abstract void Result(SimulationResult result);

        protected SimulatedInput SimulatedInput { get; } = new SimulatedInput();

        [EntrySerialize]
        public List<ValueModel> Inputs
        {
            get => SimulatedInput.Values.Select(pair => new ValueModel { Key = pair.Key, Value = pair.Value.ToString() }).ToList();
            set
            {
                foreach (var item in value)
                {
                    SimulatedInput.Values[item.Key] = item.Value;
                    SimulatedInput.RaiseInputChanged(item.Key);
                }
            }
        }
        public IInput Input => SimulatedInput;

        protected SimulatedOutput SimulatedOutput { get; } = new SimulatedOutput();
        [EntrySerialize, ReadOnly(true)]
        public List<ValueModel> Outputs
        {
            get => SimulatedOutput.Values.Select(pair => new ValueModel { Key = pair.Key, Value = pair.Value.ToString() }).ToList();
        }
        public IOutput Output => SimulatedOutput;

        /// <summary>
        /// React to output changes from the cell. Remember to set the simulation state correctly
        /// </summary>
        protected abstract void OnOutputSet(object sender, string key);

        [EntrySerialize]
        public void SetBool(string key, bool value)
        {
            SimulatedInput.Values[key] = value;
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetFloat(string key, float value)
        {
            SimulatedInput.Values[key] = value;
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetInteger(string key, int value)
        {
            SimulatedInput.Values[key] = value;
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetString(string key, string value)
        {
            SimulatedInput.Values[key] = value;
            SimulatedInput.RaiseInputChanged(key);
        }

        /// <summary>
        /// Event raised when the state has changed
        /// </summary>
        public event EventHandler<SimulationState> SimulatedStateChanged;
    }

    public class ValueModel
    {
        /// <summary>
        /// Key from interal dictionary
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Converted value
        /// </summary>
        public string Value { get; set; }

        public override string ToString() => $"{Key}: {Value}";
    }
}

