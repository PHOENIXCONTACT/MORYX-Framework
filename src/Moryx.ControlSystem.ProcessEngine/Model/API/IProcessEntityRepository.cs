// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public interface IProcessEntityRepository : IRepository<ProcessEntity>
    {
        ICollection<ProcessEntity> GetAllByState(int state);
    }
}

