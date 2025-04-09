// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    public interface IHouseEntityRepository : IRepository<HouseEntity>
    {
        ICollection<HouseEntity> GetMethLabratories();
    }

    public class HouseEntityRepository : ModificationTrackedRepository<HouseEntity>, IHouseEntityRepository
    {
        public virtual ICollection<HouseEntity> GetMethLabratories()
        {
            // This method should not be proxied
            return DbSet.Where(h => h.IsMethLabratory).ToList();
        }
    }
}
