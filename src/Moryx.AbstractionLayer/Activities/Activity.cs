// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.Tools;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Base class for all <see cref="IActivity"/>
    /// </summary>
    public abstract class Activity : IActivity
    {
        #region Activity definition

        ///
        public abstract ProcessRequirement ProcessRequirement { get; }

        ///
        public abstract ICapabilities RequiredCapabilities { get; }

        /// <summary>
        /// Untyped parameters object
        /// </summary>
        public IParameters Parameters { get; set; }

        #endregion

        #region Members

        ///
        public long Id { get; set; }

        ///
        public IProcess Process { get; set; }

        ///
        public Tracing Tracing { get; set; }

        ///
        public ActivityResult Result { get; set; }

        /// <summary>
        /// Name of the activity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the corresponding workplan step
        /// </summary>
        public long StepId { get; set; }

        #endregion

        /// <summary>
        /// Initialize activity with <see cref="Tracing"/>
        /// </summary>
        internal Activity()
        {
            Tracing = new Tracing();

            var displayName = GetType().GetDisplayName();
            Name = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
        }

        /// <inheritdoc />
        public ActivityResult Complete(long resultNumber)
        {
            return Result = CreateResult(resultNumber);
        }

        /// <inheritdoc />
        public ActivityResult Fail()
        {
            return Result = CreateFailureResult();
        }

        /// <summary>
        /// Create a typed result object for this result number
        /// </summary>
        protected abstract ActivityResult CreateResult(long resultNumber);

        /// <summary>
        /// Create a typed result object for a technical failure.
        /// </summary>
        protected abstract ActivityResult CreateFailureResult();

        /// <seealso cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
        }

        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return $"{GetType().Name} - Process = {Process?.Id ?? 0}";
        }
    }

    /// <summary>
    /// Base class for all <see cref="IActivity"/> with parameters and tracing
    /// </summary>
    /// <typeparam name="TParam">
    ///     Type of the parameters object.
    ///     Use <see cref="NullActivityParameters"/> if your activity does not require parameters
    /// </typeparam>
    /// <typeparam name="TTracing">Type of the tracing object.</typeparam>
    public abstract class Activity<TParam, TTracing> : Activity, IActivity<TParam>
        where TParam : IParameters
        where TTracing : Tracing, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="Activity{TParam,TTracing}"/>
        /// </summary>
        protected Activity()
        {
            Tracing = new TTracing();
        }

        ///
        public new TParam Parameters
        {
            get => (TParam)base.Parameters;
            set => base.Parameters = value;
        }

        ///
        public new TTracing Tracing
        {
            get => (TTracing)base.Tracing;
            set => base.Tracing = value;
        }
    }

    /// <summary>
    /// Base class for all <see cref="IActivity"/> with parameters
    /// </summary>
    /// <typeparam name="TParam">
    ///     Type of the parameters object.
    ///     Use <see cref="NullActivityParameters"/> if your activity does not require parameters
    /// </typeparam>
    public abstract class Activity<TParam> : Activity<TParam, Tracing>
        where TParam : IParameters
    {
    }
}
