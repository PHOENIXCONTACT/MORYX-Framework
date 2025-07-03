#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
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
