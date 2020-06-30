// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.UI.Tests
{
    [DetailsComponentRegistration(DetailsConstants.EmptyType)]
    public class EmptyDetailsMock : EmptyDetailsViewModelBase, IDetailsComponent
    {
        public bool InitializeCalled { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeCalled = true;
        }
    }
}
