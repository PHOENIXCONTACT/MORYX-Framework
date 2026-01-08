// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.Orders.Endpoints.Models
{
    /// <summary>
    /// OrderOperation-DTO to be used with the endpoints
    /// </summary>
    [DataContract(IsReference = true)]
    public class OperationModel
    {
        public OperationModel()
        {
            JobIds = [];
        }

        [DataMember]
        public Guid Identifier { get; set; }

        [DataMember]
        public int TotalAmount { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int OverDeliveryAmount { get; set; }

        [DataMember]
        public int UnderDeliveryAmount { get; set; }

        [DataMember]
        public DateTime PlannedStart { get; set; }

        [DataMember]
        public DateTime PlannedEnd { get; set; }

        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public double TargetCycleTime { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public string Order { get; set; }

        [DataMember]
        public OperationStateClassification Classification { get; set; }

        [DataMember]
        public string StateDisplayName { get; set; }

        [DataMember]
        public long ProductId { get; set; }

        [DataMember]
        public string ProductIdentifier { get; set; }

        [DataMember]
        public short ProductRevision { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string RecipeName { get; set; }

        [DataMember]
        public long[] RecipeIds { get; set; }

        [DataMember]
        public long[] JobIds { get; set; }

        [DataMember]
        public int ReportedSuccessCount { get; set; }

        [DataMember]
        public int ReportedFailureCount { get; set; }

        [DataMember]
        public bool CanAssign { get; set; }

        [DataMember]
        public bool CanBegin { get; set; }

        [DataMember]
        public bool CanInterrupt { get; set; }

        [DataMember]
        public bool CanReport { get; set; }

        [DataMember]
        public bool CanAdvice { get; set; }

        [DataMember]
        public bool IsCreated { get; set; }

        [DataMember]
        public bool IsFailed { get; set; }

        [DataMember]
        public bool IsAssigning { get; set; }

        [DataMember]
        public bool IsAborted { get; set; }

        [DataMember]
        public bool IsAmountReached { get; set; }

        [DataMember]
        public int RunningCount { get; set; }

        [DataMember]
        public int SuccessCount { get; set; }

        [DataMember]
        public int ScrapCount { get; set; }

        [DataMember]
        public int PendingCount { get; set; }

        [DataMember]
        public int ProgressRunning { get; set; }

        [DataMember]
        public int ProgressSuccess { get; set; }

        [DataMember]
        public int ProgressScrap { get; set; }

        [DataMember]
        public int ProgressPending { get; set; }

        [DataMember]
        public bool HasDocuments { get; set; }

        [DataMember]
        public bool HasPartList { get; set; }

        public Entry OperationSource { get; set; }
    }
}
