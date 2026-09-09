// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests;

public delegate void CustomHandler(object sender, int code, string message);

public interface ICustomDelegateResource : IResource
{
    event CustomHandler StatusReport;
}

public class CustomDelegateResource : Resource, ICustomDelegateResource
{
    public event CustomHandler StatusReport;
}
