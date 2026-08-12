// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.Management;
using Moryx.TestTools.IntegrationTest;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Material.IntegrationTests;

[TestFixture]
internal abstract class TestBase
{
    protected MoryxTestEnvironment _env = null!;
    protected IResourceManagement _resourceManagement = null!;

    [SetUp]
    public virtual async Task SetUp()
    {
        ReflectionTool.TestMode = true;
        await SetupResourceManagement();
    }

    /// <summary>
    /// Setup the resource management module for integration tests of the save
    /// and reload behaviour of a material container
    /// </summary>
    private async Task SetupResourceManagement()
    {
        var config = new ModuleConfig();
        config.Initialize();
        _env = new MoryxTestEnvironment(typeof(ModuleController), [], config);

        await _env.StartTestModuleAsync();

        _resourceManagement = _env.GetTestModule<IResourceManagement>();
    }

    [TearDown]
    public virtual Task TearDown()
    {
        return _env.StopTestModuleAsync();
    }

    protected async Task RestartResourceManagementAsync()
    {
        await _env.StopTestModuleAsync();
        await _env.StartTestModuleAsync();
        _resourceManagement = _env.GetTestModule<IResourceManagement>();
    }

    /// <summary>
    /// Helper to create and persist a <see cref="TestContainerHost"/> through the
    /// resource management facade so that subsequent operations use the resource's
    /// <see cref="Resource.Graph"/> just like a real resource would.
    /// </summary>
    protected async Task<long> CreateContainerHostAsync() =>
        await _resourceManagement.CreateUnsafeAsync(typeof(TestContainerHost), _ => Task.CompletedTask);
}
