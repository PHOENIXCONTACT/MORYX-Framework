// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model.Setups
{
    [ModelSetup("Moryx.TestTools.Test.Model")]
    public class CarSetup : IModelSetup
    {
        public string Name => "Basic cars";

        public int SortOrder => 1;

        public string Description => "Creates a basic list of cars";

        public string SupportedFileRegex => string.Empty;

        public void Execute(IUnitOfWork openContext, string setupData)
        {
            
        }
    }
}
