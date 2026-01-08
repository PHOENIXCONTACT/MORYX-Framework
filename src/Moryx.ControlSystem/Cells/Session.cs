// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Activities;

namespace Moryx.ControlSystem.Cells;

/// <summary>
/// Base class for all steps in the production process
/// </summary>
public abstract class Session
{
    /// <summary>
    /// Empty array of constraints
    /// </summary>
    protected static readonly IConstraint[] EmptyConstraints = [];

    /// <summary>
    /// Initialize a new resource request for a certain resource
    /// </summary>
    protected Session(ActivityClassification classification, ProcessReference reference)
    {
        _context = new SessionContext(classification, Guid.NewGuid(), reference);
    }

    /// <summary>
    /// Internal constructor to forward the production context
    /// </summary>
    protected Session(Session currentSession)
    {
        _context = currentSession._context;
    }

    /// <summary>
    /// Context class holding all session information
    /// </summary>
    private SessionContext _context;

    /// <summary>
    /// Unique id of the current production transaction
    /// </summary>
    public Guid Id => _context.SessionId;

    /// <summary>
    /// The resource accepted activity classification
    /// </summary>
    public ActivityClassification AcceptedClassification => _context.Classification;

    /// <summary>
    /// Id of the process the cell is working on
    /// </summary>
    public Process Process
    {
        get => _context.Process;
        internal set => _context.Process = value;
    }

    /// <summary>
    /// Id of the process the cell is working on
    /// </summary>
    public ProcessReference Reference
    {
        get => _context.Reference;
        internal set => _context.Reference = value;
    }

    /// <summary>
    /// User object to identify a session or to carry information until a session response.
    /// </summary>
    public object Tag
    {
        get => _context.Tag;
        set => _context.Tag = value;
    }

    #region Factory methods
    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type)
    {
        return CreateSession(classification, type, ProcessReference.Empty, EmptyConstraints);
    }

    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type, long processId)
    {
        return CreateSession(classification, type, ProcessReference.ProcessId(processId), EmptyConstraints);
    }

    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type, IIdentity identity)
    {
        return CreateSession(classification, type, ProcessReference.InstanceIdentity(identity), EmptyConstraints);
    }

    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type, params IConstraint[] constraints)
    {
        return CreateSession(classification, type, ProcessReference.Empty, constraints);
    }

    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type, long processId, params IConstraint[] constraints)
    {
        return CreateSession(classification, type, ProcessReference.ProcessId(processId), constraints);
    }

    /// <summary>
    /// Start a new production session
    /// </summary>
    public static ReadyToWork StartSession(ActivityClassification classification, ReadyToWorkType type, IIdentity identity, params IConstraint[] constraints)
    {
        return CreateSession(classification, type, ProcessReference.InstanceIdentity(identity), constraints);
    }

    /// <summary>
    /// Creates a new <see cref="Session"/> for the <paramref name="unknown"/> activity
    /// with a new session context and marks the activity as failed.
    /// </summary>
    /// <param name="unknown"></param>
    public static UnknownActivityAborted WrapUnknownActivity(Activity unknown)
    {
        var wrapper = StartSession(ActivityClassification.Unknown, ReadyToWorkType.Unset, unknown.Process.Id)
            .CompleteSequence(null, false, []);
        return new UnknownActivityAborted(unknown, wrapper);
    }

    private static ReadyToWork CreateSession(ActivityClassification classification, ReadyToWorkType type, ProcessReference reference, IConstraint[] constraints)
    {
        ArgumentNullException.ThrowIfNull(constraints);
        return new ReadyToWork(classification, type, reference, constraints);
    }

    #endregion

    private struct SessionContext
    {
        internal SessionContext(ActivityClassification classification, Guid sessionId, ProcessReference reference)
        {
            Classification = classification;
            SessionId = sessionId;
            Reference = reference;

            Tag = null;
            Process = null;
        }

        public Guid SessionId;

        public Process Process;

        public ProcessReference Reference;

        public ActivityClassification Classification;

        public object Tag;
    }
}