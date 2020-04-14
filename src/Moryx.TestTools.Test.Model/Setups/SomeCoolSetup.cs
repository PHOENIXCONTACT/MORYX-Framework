// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    [ModelSetup(TestModelConstants.Namespace)]
    public class SomeCoolSetup : IModelSetup
    {
        public int SortOrder => 1;

        public string Name => "Really cool setup";

        public string Description => "Creates some cars";

        public string SupportedFileRegex => string.Empty;

        public void Execute(IUnitOfWork openContext, string setupData)
        {
            var carRepo = openContext.GetRepository<ICarEntityRepository>();
            var wheelRepo = openContext.GetRepository<IWheelEntityRepository>();

            CarEntity lastCar = null;
            for (var i = 0; i < 20; i++)
            {
                var carEntity = carRepo.Create("Car " + i, i + 1000);

                wheelRepo.Create(WheelType.FrontLeft, carEntity);
                wheelRepo.Create(WheelType.FrontRight, carEntity);
                wheelRepo.Create(WheelType.RearLeft, carEntity);
                wheelRepo.Create(WheelType.RearRight, carEntity);

                lastCar = carEntity;
            }

            openContext.Save();

            carRepo.Remove(lastCar);

            openContext.Save();

            // All cars with exact name "Car 1"
            var allNamedCar1 = carRepo.GetAllBy("Car 1");

            var firstContains = carRepo.GetFirstContains("Car");

            var allContains = carRepo.GetAllContains("Car");

            // All cars with exact name "Car 1" and price 1001
            var allNamedCar1WithPrice1001 = carRepo.GetAllBy("Car 1", 1001);

            var get = carRepo.Get("Car");

            var getAllByName = carRepo.GetAllByName("Car 1");

            var getAllContainsName = carRepo.GetAllContainsName("Car");

            var bynameAndPrice = carRepo.GetAllByNameAndPrice("Car 1", 1001);

        }
    }
}
