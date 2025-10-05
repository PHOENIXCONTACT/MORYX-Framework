// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Workplans.Endpoint
{
  /// <summary>
  /// DTO representation of a <see cref="IWorkplanStep"/>
  /// </summary>
  public sealed class WorkplanNodeModel
    {
        /// <summary>
        /// Id of the represented step
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Server side type of this step
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Transition Name of this step
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Displayed Name of this step
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Top 
        /// </summary>
        [DataMember]
        public int PositionTop { get; set; }

        /// <summary>
        /// Second position value
        /// </summary>
        [DataMember]
        public int PositionLeft { get; set; }

        /// <summary>
        /// Classification of the step
        /// </summary>
        [DataMember]
        public WorkplanNodeClassification Classification { get; set; }

        /// <summary>
        /// Inputs of this step
        /// </summary>
        [DataMember]
        public NodeConnectionPoint[] Inputs { get; set; }

        /// <summary>
        /// Outputs of this step
        /// </summary>
        [DataMember]
        public NodeConnectionPoint[] Outputs { get; set; }

        /// <summary>
        /// Serialized properties of the step
        /// </summary>
        [DataMember]
        public Entry Properties { get; set; }

        /// <summary>
        /// Reference to a subworkplan
        /// </summary>
        [DataMember]
        public long SubworkplanId { get; set; }
    }
}

