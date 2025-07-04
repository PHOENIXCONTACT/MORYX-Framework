﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Orders.Management.Model
{
    public class OperationRecipeReferenceEntity : EntityBase
    {
        public virtual long RecipeId { get; set; }

        public virtual long OperationId { get; set; }

        #region Navigation Properties
    
        /// <summary>
        /// There are no comments for Operation in the schema.
        /// </summary>
        public virtual OperationEntity Operation { get; set; }

        #endregion
    }

}
