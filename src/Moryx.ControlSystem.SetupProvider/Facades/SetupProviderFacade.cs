using System;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Runtime.Modules;

namespace Moryx.ControlSystem.SetupProvider
{
    internal class SetupProviderFacade : ISetupProvider, IFacadeControl
    {
        public Action ValidateHealthState { get; set; }

        #region Dependencies

        public ISetupManager SetupManager { get; set; }

        #endregion

        /// <inheritdoc cref="IFacadeControl"/>
        public void Activate()
        {
        }

        /// <inheritdoc cref="IFacadeControl"/>
        public void Deactivate()
        {
        }

        public string Name => "SetupProvider";

        public IRecipe LoadRecipe(long id)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return null;
#endif
            ValidateHealthState();

            // TODO: Setup restore --> SetupRecipes are not saved
            return new SetupRecipe
            {
                Id = id,
                Origin = this
            };
        }

        public ISetupRecipe RequiredSetup(SetupExecution execution, IProductionRecipe recipe, ISetupTarget targetSystem)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return null;
#endif

            ValidateHealthState();

            var setup = SetupManager.RequiredSetup(execution, recipe, targetSystem);
            if (setup != null)
                setup.Origin = this;
            return setup;
        }

        public event EventHandler<IRecipe> RecipeChanged;
    }
}