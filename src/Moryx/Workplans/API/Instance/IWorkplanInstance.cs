// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Interface of instances of an <see cref="IWorkplan"/>, executable by an implementation of <see cref="IWorkplanEngine"/>.
    /// </summary>
    public interface IWorkplanInstance
    {
        /// <summary>
        /// Instantiated workplan type
        /// </summary>
        IWorkplan Workplan { get; }

        /// <summary>
        /// All places of the <see cref="IWorkplanInstance"/>. See also <seealso href="https://en.wikipedia.org/wiki/Petri_net"/>.
        /// </summary>
        IReadOnlyList<IPlace> Places { get; }

        /// <summary>
        /// All transitions of the <see cref="IWorkplanInstance"/>. See also <seealso href="https://en.wikipedia.org/wiki/Petri_net"/>.
        /// </summary>
        IReadOnlyList<ITransition> Transitions { get; }
    }
}
