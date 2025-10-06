// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Attributes
{
    public class DateTimeKindAttribute : Attribute
    {
        public DateTimeKindAttribute(DateTimeKind kind)
            => Kind = kind;

        public DateTimeKind Kind { get; }
    }
}