// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Recipes;

namespace Moryx.ControlSystem.Tests.Mocks
{
    public class DummyRecipe : ProductionRecipe, IOrderBasedRecipe
    {
        public string OrderNumber { get; set; }
        public string OperationNumber { get; set; }

        public DummyRecipe()
        {
        }
    }
}
