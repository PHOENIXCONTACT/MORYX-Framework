// Copyright (c) 2024, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Shifts
{
    /// <summary>
    /// Enum representing the days a shift can be assigned.
    /// </summary>
    [Flags]
    public enum AssignedDays
    {
        All,
        First = 1 << 0,
        Second = 1 << 1,
        Third = 1 << 2,
        Fourth = 1 << 3,
        Fifth = 1 << 4,
        Sixth = 1 << 5,
        Seventh = 1 << 6,
        Eighth = 1 << 7,
        Ninth = 1 << 8,
        Tenth = 1 << 9,
        Eleventh = 1 << 10,
        Twelfth = 1 << 11,
        Thirteenth = 1 << 12,
        Fourteenth = 1 << 13,
        Fifteenth = 1 << 14,
        Sixteenth = 1 << 15,
        Seventeenth = 1 << 16,
        Eighteenth = 1 << 17,
        Nineteenth = 1 << 18,
        Twentieth = 1 << 19,
        TwentyFirst = 1 << 20,
        TwentySecond = 1 << 21,
        TwentyThird = 1 << 22,
        TwentyFourth = 1 << 23,
        TwentyFifth = 1 << 24,
        TwentySixth = 1 << 25,
        TwentySeventh = 1 << 26,
        TwentyEighth = 1 << 27,
        TwentyNinth = 1 << 28,
        Thirtieth = 1 << 29,
        ThirtyFirst = 1 << 30
    }
}
