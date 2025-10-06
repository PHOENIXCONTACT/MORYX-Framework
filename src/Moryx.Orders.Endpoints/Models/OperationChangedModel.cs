// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models
{
    [DataContract]
    public class OperationChangedModelBase
    {
        [DataMember]
        public OperationModel OperationModel { get; set; }
    }

    [DataContract]
    public class OperationStartedModel : OperationChangedModelBase
    {
        [DataMember]
        public string UserId { get; set; }
    }

    [DataContract]
    public class OperationReportedModel : OperationChangedModelBase
    {
        [DataMember]
        public ReportModel Report { get; set; }
    }

    [DataContract]
    public class OperationAdvicedModel : OperationChangedModelBase
    {
        [DataMember]
        public AdviceModel Advice { get; set; }
    }

    /// <summary>
    /// Only for automatic geneation of Swagger definition.
    /// DO NOT use for productive UI
    /// </summary>
    [DataContract]
    public abstract class OperationChangedModel
    {
        [DataMember]
        public OperationStartedModel StartedModel { get; set; }

        [DataMember]
        public OperationReportedModel ReportedModel { get; set; }

        [DataMember]
        public OperationAdvicedModel AdvicedModel { get; set; }
    }

    public enum OperationTypes
    {
        Start,
        Progress,
        Completed,
        Interrupted,
        Report,
        Advice,
        Update
    }
}

