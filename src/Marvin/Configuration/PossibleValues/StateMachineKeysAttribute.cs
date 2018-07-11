using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Container;
using Marvin.StateMachines;

namespace Marvin.Configuration
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
        public override IEnumerable<string> GetValues(IContainer container)
        {
            return GetFields().Select(f => f.Name);
        }

        /// <inheritdoc />
        public override object Parse(IContainer container, string value)
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