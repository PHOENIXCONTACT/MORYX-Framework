// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Localizations;

namespace Moryx.ControlSystem.Recipes
{
    /// <summary>
    /// Implementation of an <see cref="IOrderBasedRecipe"/>
    /// </summary>
    [DebuggerDisplay(nameof(OrderBasedRecipe) + " <Id: {" + nameof(Id) + "}, Target: {" + nameof(Target) + "}, Workplan: {" + nameof(Workplan) + "}, OrderNumber: {" + nameof(OrderNumber) + "}, OperationNumber: {" + nameof(OperationNumber) + "}>")]
    public class OrderBasedRecipe : ProductionRecipe, IOrderBasedRecipe
    {
        /// <summary>
        /// Order number for this recipe instance
        /// </summary>
        [Display(Name = nameof(Strings.OrderBasedRecipe_OrderNumber), Description = nameof(Strings.OrderBasedRecipe_OrderNumber_Description), ResourceType = typeof(Strings))]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Operation number for this recipe instance
        /// </summary>
        [Display(Name = nameof(Strings.OrderBasedRecipe_OperationNumber), Description = nameof(Strings.OrderBasedRecipe_OperationNumber_Description), ResourceType = typeof(Strings))]
        public string OperationNumber { get; set; }

        /// <summary>
        /// Default constructor to create a new order based recipe
        /// </summary>
        public OrderBasedRecipe()
        {
        }

        /// <summary>
        /// Create a recipe clone for a different order
        /// </summary>
        protected OrderBasedRecipe(OrderBasedRecipe source)
            : base(source)
        {
            OrderNumber = source.OrderNumber;
            OperationNumber = source.OperationNumber;
        }

        /// <inheritdoc />
        public override IRecipe Clone()
        {
            return new OrderBasedRecipe(this);
        }
    }
}
