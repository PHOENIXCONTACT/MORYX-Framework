// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    [ModelSetup(typeof(TestModelContext))]
    public class AnySetup : ModelSetupBase<TestModelContext>
    {
        public override int SortOrder => 1;

        public override string Name => "Any setup";

        public override string Description => "Creates some cars";

        public override string SupportedFileRegex => string.Empty;

        public override void Execute(TestModelContext dbContext, string setupData)
        {
            var carSet = dbContext.Cars;
            var wheelSet = dbContext.Wheels;

            CarEntity lastCar = null;
            for (var i = 0; i < 1; i++)
            {
                var carEntity = carSet.Create();
                carEntity.Name = "Car " + i;
                carEntity.Price = i + 100;

                void CreateWheel(WheelType wheelType)
                {
                    var wheelEntity = wheelSet.Create();
                    wheelEntity.WheelType = wheelType;
                    carEntity.Wheels.Add(wheelEntity);
                }

                CreateWheel(WheelType.FrontLeft);
                CreateWheel(WheelType.FrontRight);
                CreateWheel(WheelType.RearLeft);
                CreateWheel(WheelType.RearRight);

                carSet.Add(carEntity);

                lastCar = carEntity;
            }

            dbContext.SaveChanges();

            dbContext.Cars.Remove(lastCar);

            dbContext.SaveChanges();

            // All cars with exact name "Car 1"
            var allNamedCar1 = carSet.Where(c => c.Name == "Car 1");

            var firstContains = carSet.First(c => c.Name.Contains("Car"));

            var allContains = carSet.Where(c => c.Name.Contains("Car"));
        }
    }
}
