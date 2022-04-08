using Microsoft.AspNetCore.SignalR;
using Moryx.AbstractionLayer.Recipes;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    public class ProductManagementHub: Hub<IProductManagementClient>
    {
    }

    public interface IProductManagementClient
    {
        /// <summary>
        /// Client is notified when <paramref name="recipe"/> was changed
        /// </summary>
        /// <param name="recipe">Recipe, which was changed</param>
        /// <returns></returns>
        Task RecipeChanged(IRecipe recipe);

        /// <summary>
        /// Client is notified when <paramref name="type"/> was changed
        /// </summary>
        /// <param name="type">Product type, which was changed</param>
        /// <returns></returns>
        Task TypeChanged(IProductType type);
    }
}
