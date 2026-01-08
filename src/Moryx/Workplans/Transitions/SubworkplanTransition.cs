// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Transitions;

/// <summary>
/// Transition that executes the given workplan transparent to the executing engine
/// </summary>
public class SubworkplanTransition : TransitionBase<WorkplanSnapshot>, IObservableTransition
{
    // Fields
    private readonly IWorkplanEngine _engine;
    private readonly IIndexResolver _indexResolver;

    /// <summary>
    /// Create a new instance of the sub-workplan transition
    /// </summary>
    public SubworkplanTransition(IWorkplanEngine engine, IIndexResolver indexResolver)
    {
        _engine = engine;
        _indexResolver = indexResolver;
    }

    ///
    public override void Initialize()
    {
        _engine.Completed += SubworkplanCompleted;
        _engine.TransitionTriggered += TransitionTriggered;

        base.Initialize();
    }

    ///
    protected override void InputTokenAdded(object sender, IToken token)
    {
        StoredTokens.Add(token);
        ((IPlace)sender).Remove(token);

        // Start sub engine
        _engine.Start();
    }

    /// <summary>
    /// Redirect the <see cref="Triggered"/> events of the embedded transitions
    /// </summary>
    private void TransitionTriggered(object sender, ITransition e)
    {
        Triggered(e, EventArgs.Empty);
    }

    private void SubworkplanCompleted(object sender, IPlace place)
    {
        var outputIndex = _indexResolver.Resolve(place.Id);
        Executing(() => PlaceToken(Outputs[outputIndex], StoredTokens.First()));
    }

    /// <summary>
    /// Event which will occure when the transition is triggerd.
    /// </summary>
    public event EventHandler Triggered;

    /// <summary>
    /// Write sub-workplan snapshot to our special token
    /// </summary>
    public override void Pause()
    {
        State = _engine.Pause();
    }

    /// <summary>
    /// Resume execution of this transition using the given tokens. Transition that trigger instantly and don't
    /// store any tokens or state information can ignore this method
    /// </summary>
    public override void Resume()
    {
        _engine.Restore(State);
        _engine.Start();
    }
}
