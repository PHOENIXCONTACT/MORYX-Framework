// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Capabilities;

/// <summary>
/// Capabilities needed for mounting activities
/// </summary>
[DataContract]
public class MountCapabilities : CapabilitiesBase
{
    /// <summary>
    /// <c>True</c> if the device can mount an article to a WPC
    /// </summary>
    [DataMember]
    public bool CanMount { get; set; }

    /// <summary>
    /// <c>True</c> if the device can unmount an article from a WPC
    /// </summary>
    [DataMember]
    public bool CanUnmount { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MountCapabilities()
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="canMount"><c>True</c> if the device can mount an article to a WPC</param>
    /// <param name="canUnmount"><c>True</c> if the device can unmount an article from a WPC</param>
    public MountCapabilities(bool canMount, bool canUnmount)
    {
        CanMount = canMount;
        CanUnmount = canUnmount;
    }

    ///
    protected override bool ProvidedBy(ICapabilities provided)
    {
        var capabilities = provided as MountCapabilities;
        if (capabilities == null)
            return false;

        if (CanMount && !capabilities.CanMount)
        {
            return false;
        }

        if (CanUnmount && !capabilities.CanUnmount)
        {
            return false;
        }

        return true;
    }
}