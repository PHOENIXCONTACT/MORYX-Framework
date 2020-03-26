// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Marvin.Products.Management.Modification
{
    [DataContract]
    internal class RecipeTypeModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool HasWorkplans { get; set; }
    }
}
