// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Activity interface
    /// </summary>
    public interface IActivity : IDisposable
    {
        /// <summary>
        /// The ID of the Activity itself.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Name of the activity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameters of the activity
        /// </summary>
        IParameters Parameters { get; set; }

        /// <summary>
        /// The execution process this activity belongs to.
        /// </summary>
        IProcess Process { get; set; }

        /// <summary>
        /// All activity trace information
        /// </summary>
        Tracing Tracing { get; }

        /// <summary>
        /// Specifies the special process requirements of this type
        /// </summary>
        ProcessRequirement ProcessRequirement { get; }

        /// <summary>
        /// The capabilities needed for this activity.
        /// </summary>
        ICapabilities RequiredCapabilities { get; }

        /// <summary>
        /// This Activity's result. Will be <c>null</c> until the state changes to <c>Finished</c>.
        /// </summary>
        ActivityResult Result { get; }

        /// <summary>
        /// Creates and sets a typed result object for this result number
        /// </summary>
        ActivityResult Complete(long resultNumber);

        /// <summary>
        /// Create and sets a typed result object for a technical failure.
        /// </summary>
        ActivityResult Fail();
    }

    /// <summary>
    /// Activity interface with typed parameter object
    /// </summary>
    /// <typeparam name="TParam">Type of the parameters object</typeparam>
    public interface IActivity<out TParam> : IActivity
        where TParam : IParameters
    {
        /// <summary>
        /// Typed parameters object for this activity
        /// </summary>
        new TParam Parameters { get; }
    }
}
