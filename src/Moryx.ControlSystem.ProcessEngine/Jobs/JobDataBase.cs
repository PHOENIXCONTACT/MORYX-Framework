// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Logging;
using Moryx.StateMachines;

namespace Moryx.ControlSystem.ProcessEngine.Jobs;

internal abstract class JobDataBase : IStateContext, IPersistentObject, IJobData, ILoggingComponent
{
    #region Dependencies

    /// <summary>
    /// Dispatcher to dispatch new processes
    /// Will handle the complete communication with the <see cref="IProcessController"/>
    /// </summary>
    public IJobDispatcher Dispatcher { get; set; }

    /// <summary>
    /// Logger to log issues with state transitions
    /// </summary>
    public IModuleLogger Logger { get; set; }

    #endregion

    /// <inheritdoc cref="IJobData"/>
    public long Id { get; private set; }

    /// <inheritdoc />
    public int Amount { get; protected set; }

    public EngineJob Job { get; }

    /// <summary>
    /// Explicit implementation of <see cref="IPersistentObject"/>
    /// Can set the <see cref="Id"/> of the job externally
    /// </summary>
    long IPersistentObject.Id
    {
        get => Id;
        set
        {
            Id = value;
            Job.Id = value;
        }
    }

    /// <inheritdoc />
    IJobState IJobData.State => State;

    /// <inheritdoc cref="IJobData"/>
    public IWorkplanRecipe Recipe { get; protected set; }

    /// <inheritdoc />
    public DateTime Started { get; protected set; } = DateTime.Now;

    /// <inheritdoc />
    public DateTime Completed => DateTime.MaxValue;

    internal List<ProcessData> AllProcesses { get; }

    /// <summary>
    /// Externally accessible list of all processes
    /// </summary>
    IReadOnlyList<ProcessData> IJobData.AllProcesses => AllProcesses;

    /// <inheritdoc cref="IJobData"/>
    public IRecipeProvider RecipeProvider => Recipe.Origin;

    /// <inheritdoc cref="IJobData"/>
    public JobClassification Classification => State.Classification;

    /// <inheritdoc />
    public bool CanComplete => State.CanComplete;

    /// <inheritdoc />
    public bool CanAbort => State.CanAbort;

    /// <inheritdoc />
    public bool IsStable => State.IsStable;

    /// <summary>
    /// Private list of running processes.
    /// </summary>
    internal List<ProcessData> RunningProcesses { get; } = new ProcessList<ProcessData>();

    /// <summary>
    /// Externally accessible list of running processes
    /// </summary>
    IReadOnlyList<ProcessData> IJobData.RunningProcesses => RunningProcesses;

    /// <summary>
    /// Lock object to lock state handling
    /// </summary>
    protected readonly object StateLock = new();

    /// <summary>
    /// General representation of the current state of the Job
    /// </summary>
    protected JobStateBase State { get; private set; }

    /// <summary>
    /// Creates a new instance. Will initialize the a new state machine
    /// </summary>
    /// <param name="recipe">Recipe of the job</param>
    /// <param name="amount">Current amount of the job</param>
    protected JobDataBase(IWorkplanRecipe recipe, int amount)
    {
        Job = new EngineJob(recipe, amount);

        Recipe = recipe;
        Amount = amount;

        AllProcesses = new ProcessList<ProcessData>(amount);
    }

    /// <summary>
    /// Creates a new instance. Will use data from the <see cref="JobEntity"/>
    /// to restore the job data. Will be used to restore data on start of the system
    /// </summary>
    /// <param name="entity">Data source to restore the job</param>
    /// <param name="recipe">Recipe of the job</param>
    protected JobDataBase(IWorkplanRecipe recipe, JobEntity entity) : this(recipe, entity.Amount)
    {
        Job.Id = Id = entity.Id;
    }

    /// <inheritdoc />
    public void Ready()
    {
        InvokeStateMachine(s => s.Ready());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Start()
    {
        InvokeStateMachine(s => s.Start());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Stop()
    {
        InvokeStateMachine(s => s.Stop());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Abort()
    {
        InvokeStateMachine(s => s.Abort());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Load()
    {
        InvokeStateMachine(s => s.Load());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Interrupt()
    {
        InvokeStateMachine(s => s.Interrupt());
    }

    /// <inheritdoc cref="IJobData"/>
    public void Complete()
    {
        InvokeStateMachine(s => s.Complete());
    }

    /// <inheritdoc cref="IJobData"/>
    public void ProcessChanged(ProcessData processData, ProcessState trigger)
    {
        InvokeStateMachine(s => s.ProcessChanged(processData, trigger));
    }

    // TODO: Remove this method as soon as Platform 3 offers proper locking
    protected void InvokeStateMachine(Action<JobStateBase> stateMachine)
    {
        JobStateBase oldState;
        lock (StateLock)
        {
            oldState = State;
            stateMachine(State);
        }
        // Only publish changes of classification
        if (oldState != State)
            StateChanged?.Invoke(this, new JobStateEventArgs(this, oldState, State));
    }

    /// <inheritdoc />
    public abstract void FailurePredicted(ProcessData processData);

    /// <inheritdoc cref="IJobData"/>
    public void AddProcesses(IReadOnlyList<ProcessData> processes)
    {
        var domainProcesses = processes
            .Select(p => p.Process).ToList();

        AllProcesses.AddRange(processes);
        Job.TotalProcesses.AddRange(domainProcesses);

        RunningProcesses.AddRange(processes);
        Job.Running.AddRange(domainProcesses);
    }

    /// <inheritdoc cref="IJobData"/>
    public virtual void AddProcess(ProcessData processData)
    {
        AllProcesses.Add(processData);
        Job.TotalProcesses.Add(processData.Process);

        RunningProcesses.Add(processData);

        RaiseProgressChanged();
    }

    /// <summary>
    /// Used by the state machine.
    /// Loads processes of this job
    /// </summary>
    internal virtual void LoadProcesses()
    {
        Dispatcher.LoadProcesses(this);
    }

    /// <summary>
    /// Used by the state machine.
    /// Starts new processes on the <see cref="IJobDispatcher"/>
    /// </summary>
    internal void StartProcess()
    {
        Dispatcher.StartProcess(this);
    }

    /// <summary>
    /// Used by the state machine.
    /// Resumes processes on the <see cref="IJobDispatcher"/>
    /// </summary>
    internal void ResumeProcesses()
    {
        Dispatcher.Resume(this);
    }

    /// <summary>
    /// Used by the state machine.
    /// Aborts all running processes on the <see cref="IJobDispatcher"/>
    /// </summary>
    internal void AbortProcesses()
    {
        Dispatcher.Abort(this);
    }

    /// <summary>
    /// Used by the state machine.
    /// Removes created processes on the <see cref="IJobDispatcher"/>
    /// Creates unmountActivities if necessary
    /// </summary>
    internal void CleanupProcesses()
    {
        Dispatcher.Cleanup(this);
    }

    /// <summary>
    /// Used by the state machine.
    /// Interrupts processes on the <see cref="IJobDispatcher"/>
    /// No new activities will be dispatched. Running activities will be finished
    /// </summary>
    internal void InterruptProcesses()
    {
        Dispatcher.Interrupt(this);
    }

    /// <summary>
    /// Removes the given process from the running of the job.
    /// </summary>
    internal virtual void ProcessCompleted(ProcessData processData)
    {
        // Only raise progress changed while we have running processes
        if (RunningProcesses.Count > 0)
            RaiseProgressChanged();
    }

    /// <inheritdoc />
    void IStateContext.SetState(StateBase state)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        // No lock required. Locked before state change.
        State = (JobStateBase)state;
        Job.Classification = State.Classification;
        Job.UpdateStateDisplayName(State);
    }

    /// <summary>
    /// Raises the <see cref="ProgressChanged"/> event
    /// </summary>
    internal void RaiseProgressChanged()
    {
        ProgressChanged?.Invoke(this, EventArgs.Empty);
    }

    public override int GetHashCode()
    {
        return Id > 0 ? Id.GetHashCode() : base.GetHashCode();
    }

    /// <inheritdoc cref="IJobData"/>
    public event EventHandler ProgressChanged;

    /// <inheritdoc cref="IJobData"/>
    public event EventHandler<JobStateEventArgs> StateChanged;
}