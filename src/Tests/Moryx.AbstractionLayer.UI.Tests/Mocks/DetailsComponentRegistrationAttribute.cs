// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DetailsComponentRegistrationAttribute : DetailsRegistrationAttribute
    {
        public DetailsComponentRegistrationAttribute(string typeName) : base(typeName, typeof(IDetailsComponent))
        {
        }
    }
}
