// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the StepEntity repository.
    /// </summary>
    public interface IStepEntityRepository : IRepository<StepEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        StepEntity Create(long stepId, string name, string assembly, string nameSpace, string classname);
    }
}
