// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Tests.Workplans;

[DataContract]
public class DummyStep : WorkplanStepBase
{
    private DummyStep()
    {

    }

    public DummyStep(int outputs)
        : this(outputs, "DummyStep")
    {
    }

    public DummyStep(int outputs, string name)
    {
        Outputs = new IConnector[outputs];
        Name = name;
    }

    [EntrySerialize]
    public int Number { get; set; }

    ///
    protected override TransitionBase Instantiate(IWorkplanContext context)
    {
        return new DummyTransition { Context = context, Name = Name };
    }
}

public class DummyTransition : TransitionBase, IObservableTransition
{
    public string Name { get; set; }

    public IWorkplanContext Context { get; set; }

    public int ResultOutput { get; set; }

    ///
    protected override void InputTokenAdded(object sender, IToken token)
    {
        ((IPlace)sender).Remove(token);
        StoredTokens.Add(token);
        Triggered(this, new EventArgs());
        if (ResultOutput >= 0) // Resume directly
            PlaceToken(Outputs[ResultOutput], StoredTokens.First());
    }

    public override void Resume()
    {
        if (StoredTokens.Any())
            Triggered(this, new EventArgs());
    }

    public void ResumeAsync(int result)
    {
        PlaceToken(Outputs[result], StoredTokens.First());
    }

    public event EventHandler Triggered;
}