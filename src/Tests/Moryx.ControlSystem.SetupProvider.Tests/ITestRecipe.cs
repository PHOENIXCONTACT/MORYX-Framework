using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    internal interface ITestRecipe : IProductRecipe
    {
        int SetupState { get; set; }
    }
}
