// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Container;
using Moryx.StateMachines;

namespace Moryx.Serialization
{
    /// <summary>
    /// <see cref="PossibleValuesAttribute"/> to provide possible StateMachine keys
    /// </summary>
    public class StateMachineKeysAttribute : PossibleValuesAttribute
    {
        private readonly Type _stateBase;

        /// <summary>
        /// Creates a new instance if <see cref="StateMachineKeysAttribute"/>
        /// Uses the StateMachine base type to read the possible keys
        /// </summary>
        public StateMachineKeysAttribute(Type stateBase)
        {
            _stateBase = stateBase;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IContainer container, IServiceProvider serviceProvider)
        {
            return GetFields().Select(f => f.Name);
        }

        /// <inheritdoc />
        public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
        {
            return GetFields().Single(f => f.Name == value).GetValue(null);
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            return StateBase.GetStateFields(_stateBase);
        }

        /// <inheritdoc />
        public override bool OverridesConversion => true;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;
    }
}
