#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    public class OperationEntity : ModificationTrackedEntityBase
    {
        public virtual long OrderId { get; set; }

        public virtual long ProductId { get; set; }

        public virtual Guid Identifier { get; set; }

        public virtual string Number { get; set; }

        public virtual string Name { get; set; }

        public virtual int AssignState { get; set; }

        public virtual int State { get; set; }

        public virtual int TotalAmount { get; set; }

        public virtual int TargetAmount { get; set; }

        public virtual int OverDeliveryAmount { get; set; }

        public virtual int UnderDeliveryAmount { get; set; }

        public virtual string Source { get; set; }

        public virtual double TargetCycleTime { get; set; }

        public virtual DateTime PlannedStart { get; set; }

        public virtual DateTime PlannedEnd { get; set; }

        public virtual DateTime? ActualStart { get; set; }

        public virtual DateTime? ActualEnd { get; set; }

        public virtual string TargetStock { get; set; }

        public virtual string Unit { get; set; }

        #region Navigation Properties

        public virtual OrderEntity Order { get; set; }

        public virtual ICollection<OperationJobReferenceEntity> JobReferences { get; set; }

        public virtual ICollection<OperationReportEntity> Reports { get; set; }

        public virtual ICollection<OperationRecipeReferenceEntity> RecipeReferences { get; set; }

        public virtual ICollection<ProductPartEntity> ProductParts { get; set; }

        public virtual ICollection<OperationAdviceEntity> Advices { get; set; }

        #endregion

        public OperationEntity()
        {
            JobReferences = new List<OperationJobReferenceEntity>();
            Reports = new List<OperationReportEntity>();
            RecipeReferences = new List<OperationRecipeReferenceEntity>();
            ProductParts = new List<ProductPartEntity>();
            Advices = new List<OperationAdviceEntity>();
        }
    }

}
