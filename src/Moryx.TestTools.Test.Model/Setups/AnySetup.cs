// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    [ModelSetup(typeof(TestModelContext))]
    public class AnySetup : IModelSetup
    {
        public int SortOrder => 1;

        public string Name => "Some setup";

        public string Description => "Creates some cars";

        public string SupportedFileRegex => string.Empty;

        public async Task ExecuteAsync(IUnitOfWork openContext, string setupData, CancellationToken cancellationToken)
        {
            var carRepo = openContext.GetRepository<ICarEntityRepository>();
            var wheelRepo = openContext.GetRepository<IWheelEntityRepository>();

            CarEntity lastCar = null;
            for (var i = 0; i < 1; i++)
            {
                var carEntity = await carRepo.CreateAsync(cancellationToken);
                carEntity.Name = "Car " + i;
                carEntity.Price = i + 100;

                async Task CreateWheel(WheelType wheelType)
                {
                    var wheelEntity = await wheelRepo.CreateAsync(cancellationToken);
                    wheelEntity.WheelType = wheelType;
                    wheelEntity.Car = carEntity;
                }

                await CreateWheel(WheelType.FrontLeft);
                await CreateWheel(WheelType.FrontRight);
                await CreateWheel(WheelType.RearLeft);
                await CreateWheel(WheelType.RearRight);

                lastCar = carEntity;
            }

            await openContext.SaveChangesAsync(cancellationToken);

            carRepo.Remove(lastCar);

            await openContext.SaveChangesAsync(cancellationToken);

            var allCarsWithLazyWheels = await carRepo.Linq.ToListAsync(cancellationToken);

            var allCarsWithWheels = await carRepo.Linq.Include(c => c.Wheels).ToListAsync(cancellationToken);

            // All cars with exact name "Car 1"
            var allNamedCar1 = carRepo.Linq.Where(c => c.Name == "Car 1");

            var firstContains = await carRepo.Linq.FirstAsync(c => c.Name.Contains("Car"), cancellationToken);

            var allContains = carRepo.Linq.Where(c => c.Name.Contains("Car"));
        }
    }
}
