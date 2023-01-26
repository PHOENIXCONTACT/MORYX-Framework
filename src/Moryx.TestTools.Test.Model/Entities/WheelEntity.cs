// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.TestTools.Test.Model
{
    public enum WheelType
    {
        FrontLeft,
        FrontRight,
        RearLeft,
        RearRight
    }

    public class WheelEntity : EntityBase
    {
        public virtual CarEntity Car { get; set; }

        public virtual WheelType WheelType { get; set; }
    }
}
