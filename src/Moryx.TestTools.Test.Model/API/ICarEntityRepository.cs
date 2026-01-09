// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model;

public interface ICarEntityRepository : IRepository<CarEntity>
{
    CarEntity Create(string name);
    CarEntity Create(string name, int price);
    CarEntity Create(string name, byte[] image);

    //TODO: Navigation property collection
    //CarEntity Create(string name, IEnumerable<WheelEntity> wheels);

    IEnumerable<CarEntity> GetAllBy(string name);
    List<CarEntity> GetAllBy(int price);
    ICollection<CarEntity> GetAllBy(string name, int price);

    ICollection<CarEntity> GetAllContains(string name);
    ICollection<CarEntity> GetAllContains(string name, int price);

    CarEntity GetSingleBy(string name);
    CarEntity GetSingleContains(string name);
    CarEntity GetSingleOrDefaultBy(string name);
    CarEntity GetSingleOrDefaultContains(string name);

    CarEntity GetFirstBy(string name);
    CarEntity GetFirstContains(string name);
    CarEntity GetFirstOrDefaultBy(string name);
    CarEntity GetFirstOrDefaultContains(string name);

    CarEntity GetBy(string name); // same as GetFirstOrDefaultBy
    CarEntity GetContains(string name); // same as GetFirstOrDefaultContains
    CarEntity Get(string name); // same as GetFirstOrDefaultBy

    ICollection<CarEntity> GetAllByName(string name); // postfix does not change behavior
    ICollection<CarEntity> GetAllContainsName(string name); // postfix does not change behavior
    ICollection<CarEntity> GetAllByNameAndPrice(string name, int price); // postfix does not change behavior
}