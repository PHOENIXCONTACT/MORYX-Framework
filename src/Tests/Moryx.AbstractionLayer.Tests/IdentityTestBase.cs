// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.AbstractionLayer.Tests;

public abstract class IdentityTestBase
{
    protected static readonly IIdentity _aIdentity = new BatchIdentity("A");
    protected static readonly IIdentity _bIdentity = new BatchIdentity("B");
    protected static readonly IIdentity _cIdentity = new BatchIdentity("C");
    protected static readonly IIdentity _abIdentity = CombinedIdentity.From(_aIdentity, _bIdentity);
    protected static readonly IIdentity _abcIdentity = CombinedIdentity.From(_aIdentity, _bIdentity, _cIdentity);
}
