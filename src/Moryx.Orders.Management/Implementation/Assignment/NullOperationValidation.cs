// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.Container;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationValidation), Name = nameof(NullOperationValidation))]
    internal class NullOperationValidation : IOperationValidation
    {
        public void Initialize(OperationValidationConfig config)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public bool ValidateCreationContext(OrderCreationContext orderContext)
        {
            return true;
        }

        public Task<bool> Validate(Operation operation, IOperationLogger operationLogger)
        {
            return Task.FromResult(true);
        }
    }
}
