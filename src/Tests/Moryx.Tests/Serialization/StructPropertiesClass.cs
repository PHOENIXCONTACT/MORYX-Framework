// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;

namespace Moryx.Tests.Serialization;

public class StructPropertiesClass
{
    public Vector2 Position2D { get; set; }

    public Vector3 Position3D { get; set; }

    public Vector4 Position4D { get; set; }

    public Quaternion Rotation { get; set; }

    public Plane Surface { get; set; }
}
