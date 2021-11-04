// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.UI.Tests
{
    [DetailsComponentRegistration(DetailsConstants.EmptyType)]
    public class EmptyDetailsMock : EmptyDetailsViewModelBase, IDetailsComponent
    {
        public bool InitializeCalled { get; private set; }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);
            InitializeCalled = true;
        }
    }
}
