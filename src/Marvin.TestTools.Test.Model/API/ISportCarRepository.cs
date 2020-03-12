// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public interface ISportCarRepository : IRepository<SportCarEntity>
    {
        SportCarEntity GetSingleBy(string name);
    }

    public class SportCarRepository : ModificationTrackedRepository<SportCarEntity>, ISportCarRepository
    {
        public virtual SportCarEntity GetSingleBy(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
