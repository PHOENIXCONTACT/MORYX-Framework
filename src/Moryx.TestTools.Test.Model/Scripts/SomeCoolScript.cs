// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    [ModelScript(TestModelConstants.Namespace)]
    public class SomeCoolScript : IModelScript
    {
        public string Name => "Some cool name";

        public bool IsCreationScript => true;

        public string GetSql()
        {
            return $"UPDATE {TestModelConstants.Schema.ToLower()}.\"{nameof(HouseEntity)}\" SET \"{nameof(HouseEntity.Name)}\" = 'From CreationScript';";
        }
    }
}
