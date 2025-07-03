using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.ProcessEngine.Tests
{
    internal interface ITestRecipe : IProductRecipe
    {
        int SetupState { get; set; }
    }
}
