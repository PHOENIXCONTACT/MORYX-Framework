// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    public class OperationReportEntity : EntityBase
    {
        public virtual long OperationId { get; set; }

        public virtual int ConfirmationType { get; set; }

        public virtual int SuccessCount { get; set; }

        public virtual int FailureCount { get; set; }

        public virtual string Comment { get; set; }

        public virtual global::System.DateTime ReportedDate { get; set; }

        public virtual string UserIdentifier { get; set; }

        #region Navigation Properties

        public virtual OperationEntity Operation { get; set; }

        #endregion
    }
}

