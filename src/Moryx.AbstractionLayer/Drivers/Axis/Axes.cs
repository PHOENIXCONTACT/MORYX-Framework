// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis
{
    /// <summary>
    /// Enum which contains hardware specific axes
    /// </summary>
    [Flags]
    public enum Axes : uint
    {
        /// <summary>
        /// Axis A of the system
        /// </summary>
        A = 1 << 0,
        /// <summary>
        /// Axis B of the system
        /// </summary>
        B = 1 << 1,
        /// <summary>
        /// Axis C of the system
        /// </summary>
        C = 1 << 2,
        /// <summary>
        /// Axis D of the system
        /// </summary>
        D = 1 << 3,
        /// <summary>
        /// Axis E of the system
        /// </summary>
        E = 1 << 4,
        /// <summary>
        /// Axis F of the system
        /// </summary>
        F = 1 << 5,
        /// <summary>
        /// Axis G of the system
        /// </summary>
        G = 1 << 6,
        /// <summary>
        /// Axis H of the system
        /// </summary>
        H = 1 << 7,
        /// <summary>
        /// Axis I of the system
        /// </summary>
        I = 1 << 8,
        /// <summary>
        /// Axis J of the system
        /// </summary>
        J = 1 << 9,
        /// <summary>
        /// Axis K of the system
        /// </summary>
        K = 1 << 10,
        /// <summary>
        /// Axis L of the system
        /// </summary>
        L = 1 << 11,
        /// <summary>
        /// Axis M of the system
        /// </summary>
        M = 1 << 12,
        /// <summary>
        /// Axis N of the system
        /// </summary>
        N = 1 << 13,
        /// <summary>
        /// Axis O of the system
        /// </summary>
        O = 1 << 14,
        /// <summary>
        /// Axis P of the system
        /// </summary>
        P = 1 << 15,
        /// <summary>
        /// Axis Q of the system
        /// </summary>
        Q = 1 << 16,
        /// <summary>
        /// Axis R of the system
        /// </summary>
        R = 1 << 17,
        /// <summary>
        /// Axis S of the system
        /// </summary>
        S = 1 << 18,
        /// <summary>
        /// Axis T of the system
        /// </summary>
        T = 1 << 19,
        /// <summary>
        /// Axis U of the system
        /// </summary>
        U = 1 << 20,
        /// <summary>
        /// Axis V of the system
        /// </summary>
        V = 1 << 21,
        /// <summary>
        /// Axis W of the system
        /// </summary>
        W = 1 << 22,
        /// <summary>
        /// Axis X of the system
        /// </summary>
        X = 1 << 23,
        /// <summary>
        /// Axis Y of the system
        /// </summary>
        Y = 1 << 24,
        /// <summary>
        /// Axis Z of the system
        /// </summary>
        Z = 1 << 25,
        /// <summary>
        /// Door is a specialized axis
        /// </summary>
        Door = 1 << 26,
        /// <summary>
        /// Rotating plate is another specialized axis
        /// </summary>
        RotationPlate = 1 << 27,
        /// <summary>
        /// All axes of system
        /// </summary>
        All = uint.MaxValue,
    }
}
