// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model.Attributes
{
    public class DateTimeKindAttribute : Attribute
    {
        public DateTimeKindAttribute(DateTimeKind kind)
            => Kind = kind;

        public DateTimeKind Kind { get; }
    }
}