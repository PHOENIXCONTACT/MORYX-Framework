// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.UI.Tests
{
    [DetailsComponentRegistration(TypeName)]
    public class DetailsComponentMock : EditModeViewModelBase, IDetailsComponent
    {
        public const string TypeName = "MockiMock";

        public bool InitializeCalled { get; private set; }

        public void Initialize(string typeName)
        {
            InitializeCalled = true;
        }

        public void SetBusy(bool to)
        {
            IsBusy = to;
        }
    }

    [DetailsComponentRegistration(DetailsConstants.DefaultType)]
    public class DefaultDetailsMock : DetailsComponentMock
    {

    }
}
