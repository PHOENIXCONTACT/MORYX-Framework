using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Management;
using Marvin.Products.Model;
using Marvin.Products.Samples.Recipe;

namespace Marvin.Products.Samples
{
    [Plugin(LifeCycle.Singleton, typeof(IProductStorage))]
    public class WatchProductStorage : ProductStorageBase
    {
        public override IUnitOfWorkFactory Factory { get; set; }

        protected override IProductTypeStrategy[] BuildMap()
        {
            return new IProductTypeStrategy[]
            {
                new WatchPackageStrategy(), 
                new WatchStrategy(),
                new WatchfaceStrategy(),
                new DefaultProductStrategy<NeedleProduct>(true, ParentLoadBehaviour.Ignore)
            };
        }

        protected override IProductRecipe LoadCustomRecipe(IUnitOfWork uow, ProductRecipeEntity recipeEntity)
        {
            var watchRecipe = new WatchProductRecipe
            {
                Case = new CaseDescription
                {
                    CaseColorCode = 90,
                    CaseMaterial = 7
                },
                CoresInstalled = 2,
            };

            return watchRecipe;
        }

        protected override ProductRecipeEntity SaveRecipe(IUnitOfWork uow, IProductRecipe recipe)
        {
            return base.SaveRecipe(uow, recipe);
        }
    }
}
