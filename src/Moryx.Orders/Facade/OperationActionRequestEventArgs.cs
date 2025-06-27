// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Orders.Restrictions;

namespace Moryx.Orders
{
    /// <summary>
    /// Base class for operation depending actions events
    /// </summary>
    public class OperationActionRequestEventArgs<TRestrictionType> : OperationChangedEventArgs where TRestrictionType : IOperationRestriction
    {
        private readonly List<TRestrictionType> _restrictions = new();

        /// <summary>
        /// Restrictions to execute the operation action
        /// </summary>
        public IReadOnlyCollection<TRestrictionType> Restrictions => _restrictions;

        /// <summary>
        /// Creates a new instance of <see cref="OperationActionRequestEventArgs{TRestrictionType}"/>
        /// </summary>
        public OperationActionRequestEventArgs(Operation operation) : base(operation)
        {
        }

        /// <summary>
        /// Adds restriction to the request
        /// </summary>
        public void AddRestriction(TRestrictionType restriction)
        {
            _restrictions.Add(restriction);
        }
    }
}