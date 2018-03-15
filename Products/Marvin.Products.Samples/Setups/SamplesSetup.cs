using System;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Products.Samples.Model;

namespace Marvin.Products.Samples.Setups
{
    [ModelSetup(WatchProductsConstants.Namespace)]
    public class SamplesSetup : IModelSetup
    {
        public void Execute(IUnitOfWork uow, string setupData)
        {
            var prodRepo = uow.GetRepository<IProductEntityRepository>();
            var propertiesRepo = uow.GetRepository<IProductPropertiesRepository>();
            var linkRepo = uow.GetRepository<IPartLinkRepository>();

            // Create products
            var watchProduct = prodRepo.Create("1234", 1, nameof(WatchProduct));
            var watchProperties = propertiesRepo.Create("Cool Watch");
            //watchProperties.OperatingSystem = OperatingSystem.Windows2012Server;
            watchProduct.SetCurrentVersion(watchProperties);

            var watchfaceProduct = prodRepo.Create("5678", 2, nameof(WatchFaceProduct));
            var watchfaceProperties = propertiesRepo.Create("Cool watchface");
            watchfaceProduct.SetCurrentVersion(watchfaceProperties);

            var hourProduct = prodRepo.Create("567871", 3, nameof(NeedleProduct));
            var hourProperties = propertiesRepo.Create("Hour");
            hourProduct.SetCurrentVersion(hourProperties);

            var secondsProduct = prodRepo.Create("3455644", 3, nameof(NeedleProduct));
            var secondsProperties = propertiesRepo.Create("Seconds");
            secondsProduct.SetCurrentVersion(secondsProperties);

            // Create part links
            var singleLink = linkRepo.Create(nameof(WatchProduct.Watchface));
            singleLink.Parent = watchProduct;
            singleLink.Child = watchfaceProduct;

            var multiLink1 = linkRepo.Create(nameof(WatchProduct.Needles));
            multiLink1.Parent = watchProduct;
            multiLink1.Child = hourProduct;

            var multiLink2 = linkRepo.Create(nameof(WatchProduct.Needles));
            multiLink2.Parent = watchProduct;
            multiLink2.Child = secondsProduct;

            uow.Save();
        }

        public int SortOrder => 1;
        public string Name => "Sample";
        public string Description => "Create sample data";
        public string SupportedFileRegex => string.Empty;
    }
}
