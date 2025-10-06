// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Simulation;
using Moryx.Serialization;
using System.ComponentModel;

namespace Moryx.Drivers.Simulation.InOutDriver
{
    /// <summary>
    /// Base class for simulation drivers that implement the IInOutDriver interface
    /// </summary>
    public abstract class SimulatedInOutDriver<TIn, TOut> : Driver, IInOutDriver<TIn, TOut>, ISimulationDriver
    {
        [ResourceReference(ResourceRelationType.Driver, ResourceReferenceRole.Source)]
        public Cell Cell { get; set; }

        public IEnumerable<ICell> Usages => new[] { Cell };

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

        protected override void OnStart()
        {
            base.OnStart();

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

        protected SimulatedInput<TIn> SimulatedInput { get; } = new SimulatedInput<TIn>();
        [EntrySerialize]
        public List<ValueModel> Inputs
        {
            get => SimulatedInput.Values.Select(pair => new ValueModel { Key = pair.Key, Value = pair.Value.ToString() }).ToList();
            set
            {
                foreach (var item in value)
                {
                    // Update with type conversion
                    if (SimulatedInput.Values.ContainsKey(item.Key))
                        SimulatedInput.Values[item.Key] = (TIn)Convert.ChangeType(item.Value, SimulatedInput.Values[item.Key].GetType());
                    else
                        SimulatedInput.Values[item.Key] = (TIn)Convert.ChangeType(item.Value, typeof(TIn));

                    SimulatedInput.RaiseInputChanged(item.Key);
                }
            }
        }
        public IInput<TIn> Input => SimulatedInput;

        protected SimulatedOutput<TOut> SimulatedOutput { get; } = new SimulatedOutput<TOut>();
        [EntrySerialize, ReadOnly(true)]
        public List<ValueModel> Outputs
        {
            get => SimulatedOutput.Values.Select(pair => new ValueModel { Key = pair.Key, Value = pair.Value.ToString() }).ToList();
        }
        public IOutput<TOut> Output => SimulatedOutput;

        /// <summary>
        /// React to output changes from the cell. Remember to set the simulation state correctly
        /// </summary>
        protected abstract void OnOutputSet(object sender, string key);

        [EntrySerialize]
        public void SetBool(string key, bool value)
        {
            if (typeof(TIn).IsAssignableFrom(typeof(bool)))
                SimulatedInput.Values[key] = (TIn)(object)value;
            else
                SimulatedInput.Values[key] = (TIn)Convert.ChangeType(value, typeof(TIn));
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetFloat(string key, float value)
        {
            if (typeof(TIn).IsAssignableFrom(typeof(float)))
                SimulatedInput.Values[key] = (TIn)(object)value;
            else
                SimulatedInput.Values[key] = (TIn)Convert.ChangeType(value, typeof(TIn));
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetInteger(string key, int value)
        {
            if (typeof(TIn).IsAssignableFrom(typeof(int)))
                SimulatedInput.Values[key] = (TIn)(object)value;
            else
                SimulatedInput.Values[key] = (TIn)Convert.ChangeType(value, typeof(TIn));
            SimulatedInput.RaiseInputChanged(key);
        }

        [EntrySerialize]
        public void SetString(string key, string value)
        {
            if (typeof(TIn).IsAssignableFrom(typeof(string)))
                SimulatedInput.Values[key] = (TIn)(object)value;
            else
                SimulatedInput.Values[key] = (TIn)Convert.ChangeType(value, typeof(TIn));
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

