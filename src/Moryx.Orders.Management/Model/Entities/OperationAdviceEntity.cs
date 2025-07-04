// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    public class OperationAdviceEntity : EntityBase
    {
        public virtual string LoadingEquipment { get; set; }

        public virtual int Amount { get; set; }

        public virtual string ToteBoxNumber { get; set; }

        public virtual long? PartId { get; set; }

        public virtual long OperationId { get; set; }

        #region Navigation Properties

        public virtual OperationEntity Operation { get; set; }

        #endregion
    }

}

