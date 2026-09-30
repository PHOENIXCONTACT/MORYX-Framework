// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.ResourceExtensions.Properties;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.AbstractionLayer.ResourceExtensions;

/// <summary>
/// Extension resource that observes an <see cref="IDriver"/> and logs state changes at appropriate log levels.
/// </summary>
[Display(Name = nameof(Strings.DriverStateLogger_Name), Description = nameof(Strings.DriverStateLogger_Description), ResourceType = typeof(Strings))]
[ResourceRegistration]
public class DriverStateLogger : Resource
{
    #region Resource Reference

    /// <summary>
    /// The driver this resource is logging state changes for
    /// </summary>
    [ReferenceOverride(nameof(Parent))]
    [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Source, IsRequired = true)]
    public required IDriver Driver { get; set; }

    #endregion

    #region LifeCycle

    /// <inheritdoc/>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = Driver.Name + " State-Logger";
        }

        Driver.StateChanged += OnDriverStateChanged;
        if (Driver.CurrentState != null)
        {
            LogState(Driver.CurrentState);
        }

        return base.OnStartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Driver.StateChanged -= OnDriverStateChanged;
        return base.OnStopAsync(cancellationToken);
    }

    #endregion

    private void OnDriverStateChanged(object? sender, IDriverState state) => LogState(state);

    private void LogState(IDriverState state)
    {
        var driverResource = Driver as Resource;
        var driverId = driverResource?.Id.ToString(CultureInfo.InvariantCulture) ?? "?";
        var driverName = driverResource?.Name ?? Driver.GetType().Name;
        var classification = state.Classification;

        if (classification.HasFlag(StateClassification.Error))
        {
            Logger.LogError("Driver {DriverId}-{DriverName} state changed to {Classification}", driverId, driverName, classification);
        }
        else if (classification == StateClassification.Offline)
        {
            Logger.LogWarning("Driver {DriverId}-{DriverName} state changed to {Classification}", driverId, driverName, classification);
        }
        else if (classification.HasFlag(StateClassification.Maintenance))
        {
            Logger.LogWarning("Driver {DriverId}-{DriverName} state changed to {Classification}", driverId, driverName, classification);
        }
        else
        {
            Logger.LogInformation("Driver {DriverId}-{DriverName} state changed to {Classification}", driverId, driverName, classification);
        }
    }
}
