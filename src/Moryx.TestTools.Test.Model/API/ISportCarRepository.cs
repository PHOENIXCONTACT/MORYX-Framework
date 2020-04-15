// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public interface ISportCarRepository : IRepository<SportCarEntity>
    {
        SportCarEntity GetSingleBy(string name);
    }

    public class SportCarRepository : Repository<SportCarEntity>, ISportCarRepository
    {
        public virtual SportCarEntity GetSingleBy(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
