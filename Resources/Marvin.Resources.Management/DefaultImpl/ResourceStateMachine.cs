using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Default state machine
    /// </summary>
    [ExpectedConfig(typeof(StatesConfig))]
    [Plugin(LifeCycle.Transient, typeof(IResourceStateMachine), Name = ComponentName)]
    internal class ResourceStateMachine : IResourceStateMachine
    {
        public const string ComponentName = "ResourceStateMachine";

        /// <summary>
        /// Resource types this plugin supports
        /// </summary>
        public string[] SupportedTypes { get; private set; }

        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(StateMachineConfig config)
        {
            var casted = (StatesConfig)config;

            SupportedTypes = casted.ResourceTypes.Select(rt => rt.ResourceType).ToArray();
            PossibleStates = casted.PossibleStates.Select(pt => pt.ResourceState).ToArray();
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// States this resource can have
        /// </summary>
        public string[] PossibleStates { get; private set; }

        /// <summary>
        /// Current states of the resources
        /// </summary>
        private readonly Dictionary<long, string> _currentStates = new Dictionary<long, string>();

        /// <summary>
        /// Set state of the resource
        /// </summary>
        public void SetState(IUnitOfWork uow, ResourceContext resource, string state)
        {
            _currentStates[resource.Id] = state;
        }

        /// <summary>
        /// Get state of the resource
        /// </summary>
        public string GetState(IUnitOfWork uow, ResourceContext resource)
        {
            if (_currentStates.ContainsKey(resource.Id))
                return _currentStates[resource.Id];

            var state = PossibleStates.First();
            _currentStates[resource.Id] = state;
            return state;
        }

        [DataContract]
        public class StatesConfig : StateMachineConfig
        {
            /// <summary>
            /// Name of the component represented by this entry
            /// </summary>
            public override string PluginName { get { return ComponentName; } }

            public StatesConfig()
            {
                ResourceTypes = new List<SupportedResourceType>
                {
                    new SupportedResourceType{ResourceType = Resources.ResourceTypes.Machine},
                    new SupportedResourceType {ResourceType = Resources.ResourceTypes.MachinePart}
                };
                PossibleStates = new List<PossibleState>
                {
                    new PossibleState {ResourceState = "Stopped"},
                    new PossibleState {ResourceState = "Running"},
                    new PossibleState {ResourceState = "Maintenance"}
                };
            }

            /// <summary>
            /// Resource types supported by this state machine
            /// </summary>
            [DataMember]
            public List<SupportedResourceType> ResourceTypes { get; set; }

            /// <summary>
            /// Possible states this resource can have
            /// </summary>
            [DataMember]
            public List<PossibleState> PossibleStates { get; set; }
        }

        [DataContract]
        public class SupportedResourceType
        {
            [DataMember]
            public string ResourceType { get; set; }

            public override string ToString()
            {
                return ResourceType;
            }
        }

        [DataContract]
        public class PossibleState
        {
            [DataMember]
            public string ResourceState { get; set; }

            public override string ToString()
            {
                return ResourceState;
            }
        }
    }
}