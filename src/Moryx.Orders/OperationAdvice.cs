// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// Representation of an advice in a defined tote box
    /// </summary>
    public abstract class OperationAdvice : IPersistentObject
    {
        /// <summary>
        /// Creates a new instance of <see cref="OperationAdvice"/>
        /// </summary>
        protected OperationAdvice(string toteBoxNumber)
        {
            ToteBoxNumber = toteBoxNumber;
        }

        /// <summary>
        /// Id of the current advice
        /// </summary>
        long IPersistentObject.Id { get; set; }

        /// <summary>
        /// The number of the tote box which was used to advice
        /// </summary>
        public string ToteBoxNumber { get; set; }
    }

    /// <summary>
    /// Advice for pick parts in a defined tote box
    /// </summary>
    public class PickPartAdvice : OperationAdvice
    {
        /// <summary>
        /// Typed part for the pick part advice
        /// </summary>
        public ProductPart Part { get; set; }

        /// <inheritdoc />
        // ReSharper disable once SuggestBaseTypeForParameter
        public PickPartAdvice(ProductPart part, string toteBoxNumber) : base(toteBoxNumber)
        {
            Part = part;
        }
    }

    /// <summary>
    /// Advice for an order with an amount of parts within a defined tote box
    /// </summary>
    public class OrderAdvice : OperationAdvice
    {
        /// <summary>
        /// Amount of parts which are adviced in the given tote box
        /// </summary>
        public int Amount { get; set; }

        /// <inheritdoc />
        public OrderAdvice(string toteBoxNumber, int amount) : base(toteBoxNumber)
        {
            Amount = amount;
        }
    }
}
