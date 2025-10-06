// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public interface IJobEntityRepository : IRepository<JobEntity>
    {
        IEnumerable<JobEntity> GetAllByState(int state);
    }
}

