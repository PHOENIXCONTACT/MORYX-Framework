// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine
{
    /// <summary>
    /// Generates database ids for processes and activities based on their
    /// parents id and children count at the time of creation
    /// </summary>
    internal static class IdShiftGenerator
    {
        /// <summary>
        /// Number of bits for each generation shift (parent id -> child id)
        /// </summary>
        public const int ShiftSpace = 14;

        /// <summary>
        /// Maximum amount of children is limited by the shift space
        /// </summary>
        public const int MaxAmount = (int.MaxValue >> (31 - ShiftSpace));

        /// <summary>
        /// Shifts the parent id to the left and adds the children count
        /// </summary>
        public static long Generate(long parentId, int count)
        {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
            return parentId << ShiftSpace | count;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
        }

        /// <summary>
        /// Extract the parent id from a combined key
        /// </summary>
        public static long ExtractParent(long combinedKey)
        {
            return combinedKey >> ShiftSpace;
        }

        /// <summary>
        /// Extract the child index from the combined key
        /// </summary>
        public static long ExtractChild(long combinedKey)
        {
            var mask = uint.MaxValue;
            mask >>= (32 - ShiftSpace);
            return combinedKey & mask;
        }
    }
}
