// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    public class OrderEntity : ModificationTrackedEntityBase
    {
        public virtual string Number { get; set; }

        public virtual string Type { get; set; }

        #region Navigation Properties

        public virtual ICollection<OperationEntity> Operations { get; set; }

        #endregion
    }
}

