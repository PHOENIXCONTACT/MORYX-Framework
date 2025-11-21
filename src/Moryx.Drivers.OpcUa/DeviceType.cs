// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// https://reference.opcfoundation.org/DI/v104/docs/4.7
/// </summary>
internal class DeviceType
{
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public string DeviceRevision { get; set; }
    public string SoftwareRevision { get; set; }
    public string HardwareRevision { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int RevisionCounter { get; set; }
    public string DeviceManual { get; set; }
    public string ManufacturerUri { get; set; }
    public string ProductCode { get; set; }
    public string DeviceClass { get; set; }
    public string ProductInstanceUri { get; set; }

    public override string ToString()
    {
        return Name;
    }

}
