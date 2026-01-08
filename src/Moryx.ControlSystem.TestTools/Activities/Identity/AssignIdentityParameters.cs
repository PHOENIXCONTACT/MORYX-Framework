// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.Bindings;
using Moryx.ControlSystem.Capabilities;
using Moryx.Serialization;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Generic parameters used for identity assignment
    /// </summary>
    [DataContract]
    public class AssignIdentityParameters : Parameters, IAssignIdentityParameters
    {
        private object _target;
        private IBindingResolver _resolver;

        /// <inheritdoc />
        [EntrySerialize, DataMember]
        [Description("Identity type to assign")]
        public int Type { get; set; }

        /// <inheritdoc />
        [EntrySerialize, DataMember]
        [Description("Source of identity, should a new one be requested or assign an existent")]
        public IdentitySource Source { get; set; }

        /// <inheritdoc />
        [EntrySerialize, DataMember]
        [Description("Amount of identities to assign")]
        public int Amount { get; set; }

        /// <inheritdoc />
        public object Target => _target; //The ValueProvider tries to fill all properties of an object including the Target.
                                         //TODO: Fix ValueProviderExecutor. Must be fixed in MORYX-Core.

        /// <summary>
        /// Binding string for resolving the <see cref="Target"/>
        /// e.g. ProductInstance.Board (IIdentifiableObject) or ProductInstance.MacAddresses (List)
        /// </summary>
        [EntrySerialize, DataMember]
        [Description("Binding string for resolving the target. Default targes is the root product instance.")]
        public string TargetBinding { get; set; }

        /// <inheritdoc />
        protected override void Populate(Process process, Parameters instance)
        {
            var parameters = (AssignIdentityParameters)instance;

            if (_resolver == null && !string.IsNullOrWhiteSpace(TargetBinding))
                _resolver = ResolverFactory.Create(TargetBinding);

            var target = _resolver?.Resolve(process)
                         ?? (process as ProductionProcess)?.ProductInstance;

            parameters._target = target;
            parameters.Type = Type;
            parameters.Amount = Amount;
            parameters.Source = Source;
        }
    }
}
