// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public interface IHouseEntityRepository : IRepository<HouseEntity>
    {
        ICollection<HouseEntity> GetMethLabratories();
    }

    public class HouseEntityRepository : Repository<HouseEntity>, IHouseEntityRepository
    {
        public virtual ICollection<HouseEntity> GetMethLabratories()
        {
            // This method should not be proxied
            return DbSet.Where(h => h.IsMethLabratory).ToList();
        }
    }
}
