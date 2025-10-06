// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    public interface IWheelEntityRepository : IRepository<WheelEntity>
    {
        WheelEntity Create(WheelType wheelType);

        WheelEntity Create(WheelType wheelType, CarEntity car);
    }
}
