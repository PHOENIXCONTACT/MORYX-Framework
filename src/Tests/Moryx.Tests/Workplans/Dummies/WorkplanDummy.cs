// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using Moryx.Workplans.WorkplanSteps;
using System.Drawing;

namespace Moryx.Tests.Workplans;

public class WorkplanDummy : Workplan
{
    public WorkplanDummy()
    {
        Id = 1;
        Name = "DummyWorkplan";
    }

    public static WorkplanDummy CreateFull()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var step = new DummyStep(2, "A");
        step.Inputs[0] = inital;
        step.Position = new Point(1, 1);
        workplan.Add(step);

        var left = WorkplanInstance.CreateConnector("Left");
        left.Position = new Point(0, 2);
        workplan.Add(left);
        step.Outputs[0] = left;
        var right = WorkplanInstance.CreateConnector("Right");
        right.Position = new Point(2, 2);
        workplan.Add(right);
        step.Outputs[1] = right;

        var rightOnly = new DummyStep(1, "B");
        rightOnly.Inputs[0] = right;
        rightOnly.Position = new Point(2, 3);
        workplan.Add(rightOnly);
        rightOnly.Outputs[0] = left;

        var merge = new DummyStep(2, "C");
        merge.Inputs[0] = left;
        merge.Position = new Point(1, 4);
        workplan.Add(merge);
        merge.Outputs[0] = merge.Outputs[1] = complete;

        return workplan;
    }

    public static WorkplanDummy CreateBig()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(inital, complete, failed);

        var step = new DummyStep(2, "Feed case");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        step.Outputs[0] = left;
        var right = WorkplanInstance.CreateConnector("Right");
        workplan.Add(right);
        step.Outputs[1] = right;

        step = new DummyStep(3, "Mount");
        step.Inputs[0] = left;
        workplan.Add(step);
        step.Outputs[2] = right;
        left = WorkplanInstance.CreateConnector("Merge");
        workplan.Add(left);
        step.Outputs[0] = step.Outputs[1] = left;

        step = new DummyStep(2, "Set pole");
        step.Inputs[0] = left;
        workplan.Add(step);
        left = WorkplanInstance.CreateConnector("Pole set");
        workplan.Add(left);
        step.Outputs[0] = left;
        step.Outputs[1] = failed;

        step = new DummyStep(3, "Set screw");
        step.Inputs[0] = left;
        workplan.Add(step);
        step.Outputs[0] = complete;
        step.Outputs[1] = right;
        step.Outputs[2] = failed;

        var rightOnly = new DummyStep(1, "Remove case");
        rightOnly.Inputs[0] = right;
        rightOnly.Outputs[0] = failed;
        workplan.Add(rightOnly);

        return workplan;
    }

    public static WorkplanDummy CreateWithSub()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var subPlan = CreateSub();
        subPlan.Id = 42;
        var sub = new SubworkplanStep(subPlan);
        sub.Inputs[0] = inital;
        sub.Outputs[0] = sub.Outputs[0] = complete;
        workplan.Add(sub);

        return workplan;
    }

    public static WorkplanDummy CreateSub()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(inital, complete, failed);

        var step = new DummyStep(1, "A");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var aComplete = WorkplanInstance.CreateConnector("A1");
        workplan.Add(aComplete);
        step.Outputs[0] = aComplete;

        var finalStep = new DummyStep(2, "B");
        finalStep.Inputs[0] = aComplete;
        workplan.Add(finalStep);
        finalStep.Outputs[0] = complete;
        finalStep.Outputs[1] = failed;

        return workplan;
    }

    public static WorkplanDummy CreateMixed()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var step = new DummyStep(1, "A");
        step.Inputs[0] = inital;
        // workplan.Add(step);  // Add first step at the end

        var aComplete = WorkplanInstance.CreateConnector("A1");
        workplan.Add(aComplete);
        step.Outputs[0] = aComplete;

        var finalStep = new DummyStep(1, "B");
        finalStep.Inputs[0] = aComplete;
        workplan.Add(finalStep);
        finalStep.Outputs[0] = complete;

        workplan.Add(step); // Mix order of steps

        return workplan;
    }

    public static WorkplanDummy CreatePausableSub()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var step = new DummyStep(1, "A");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var aComplete = WorkplanInstance.CreateConnector("A1");
        workplan.Add(aComplete);
        step.Outputs[0] = aComplete;

        var finalStep = new PausableStep();
        finalStep.Inputs[0] = aComplete;
        workplan.Add(finalStep);
        finalStep.Outputs[0] = complete;

        return workplan;
    }

    public static WorkplanDummy CreatePausable()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital);
        workplan.Add(complete);

        var step = new DummyStep(1, "A");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var a1 = WorkplanInstance.CreateConnector("Before pause");
        workplan.Add(a1);
        step.Outputs[0] = a1;

        var rightOnly = new PausableStep();
        rightOnly.Inputs[0] = a1;
        workplan.Add(rightOnly);
        step.Outputs[0] = a1;

        var b1 = WorkplanInstance.CreateConnector("Right complete");
        workplan.Add(b1);
        rightOnly.Outputs[0] = b1;

        var merge = new DummyStep(1, "C");
        merge.Inputs[0] = b1;
        workplan.Add(merge);
        merge.Outputs[0] = complete;

        return workplan;
    }

    public static WorkplanDummy CreateLoneWolf()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var step = new DummyStep(2, "A");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        step.Outputs[0] = left;
        var right = WorkplanInstance.CreateConnector("Right");
        workplan.Add(right);
        // step.Outputs[1] = right;  <-- This causes a Lone Wolf validation error

        var rightOnly = new DummyStep(1, "LoneWolf");
        rightOnly.Inputs[0] = right;
        workplan.Add(rightOnly);
        rightOnly.Outputs[0] = left;

        var merge = new DummyStep(2);
        merge.Inputs[0] = left;
        workplan.Add(merge);
        merge.Outputs[0] = merge.Outputs[1] = complete;

        return workplan;
    }

    public static WorkplanDummy CreateDeadEnd()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(inital, complete);

        var step = new DummyStep(2, "A");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        step.Outputs[0] = left;
        var right = WorkplanInstance.CreateConnector("DeadEnd");
        workplan.Add(right);
        step.Outputs[1] = right;

        var rightOnly = new DummyStep(1);
        workplan.Add(rightOnly);
        rightOnly.Outputs[0] = left;

        var merge = new DummyStep(2);
        merge.Inputs[0] = left;
        workplan.Add(merge);
        merge.Outputs[0] = merge.Outputs[1] = complete;

        return workplan;
    }

    public static WorkplanDummy CreateNoLuckyStreak()
    {
        var workplan = new WorkplanDummy();

        var initial = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(initial, complete, failed);

        // Step with only a failure output — no success path to End
        var step = new DummyStep(1, "FailOnly");
        step.Inputs[0] = initial;
        step.Outputs[0] = failed;
        step.OutputDescriptions = [new OutputDescription { Name = "Failed", OutputType = OutputType.Failure }];
        workplan.Add(step);

        return workplan;
    }

    public static WorkplanDummy CreateInfiniteLoop()
    {
        var workplan = new WorkplanDummy();

        var initial = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        workplan.Add(initial, complete);

        // Step A: Start -> Left
        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        var stepA = new DummyStep(1, "A");
        stepA.Inputs[0] = initial;
        stepA.Outputs[0] = left;
        workplan.Add(stepA);

        // Step B: Left -> Right (forms a cycle with Step C, no exit)
        var right = WorkplanInstance.CreateConnector("Right");
        workplan.Add(right);
        var stepB = new DummyStep(1, "InfiniteB");
        stepB.Inputs[0] = left;
        stepB.Outputs[0] = right;
        workplan.Add(stepB);

        // Step C: Right -> Left (closes the loop back to Left)
        var stepC = new DummyStep(1, "InfiniteC");
        stepC.Inputs[0] = right;
        stepC.Outputs[0] = left;
        workplan.Add(stepC);

        return workplan;
    }

    public static WorkplanDummy CreateSuccessPathCycle()
    {
        var workplan = new WorkplanDummy();

        var initial = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(initial, complete, failed);

        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        var right = WorkplanInstance.CreateConnector("Right");
        workplan.Add(right);

        // Step A: output 0 (success) -> Left, output 1 (alternative) -> Failed
        var stepA = new DummyStep(2, "CycleA");
        stepA.Inputs[0] = initial;
        stepA.Outputs[0] = left;
        stepA.Outputs[1] = failed;
        workplan.Add(stepA);

        // Step B: output 0 (success) -> Right, output 1 (alternative) -> Failed
        var stepB = new DummyStep(2, "CycleB");
        stepB.Inputs[0] = left;
        stepB.Outputs[0] = right;
        stepB.Outputs[1] = failed;
        workplan.Add(stepB);

        // Step C: output 0 (success) -> Left (closes cycle), output 1 (alternative) -> Failed
        var stepC = new DummyStep(2, "CycleC");
        stepC.Inputs[0] = right;
        stepC.Outputs[0] = left;
        stepC.Outputs[1] = failed;
        workplan.Add(stepC);

        return workplan;
    }

    public static WorkplanDummy WithLoop()
    {
        var workplan = new WorkplanDummy();

        var inital = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var complete = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var failed = WorkplanInstance.CreateConnector("Failed", NodeClassification.Failed);
        workplan.Add(inital, complete, failed);

        var step = new DummyStep(2, "Feed case");
        step.Inputs[0] = inital;
        workplan.Add(step);

        var left = WorkplanInstance.CreateConnector("Left");
        workplan.Add(left);
        step.Outputs[0] = left;
        var right = WorkplanInstance.CreateConnector("Right");
        workplan.Add(right);
        step.Outputs[1] = right;

        step = new DummyStep(3, "Mount");
        step.Inputs[0] = left;
        workplan.Add(step);
        step.Outputs[2] = right;
        left = WorkplanInstance.CreateConnector("Merge");
        workplan.Add(left);
        step.Outputs[0] = step.Outputs[1] = left;

        step = new DummyStep(3, "Set pole");
        step.Inputs[0] = left;
        workplan.Add(step);
        var oldLeft = left;
        left = WorkplanInstance.CreateConnector("Pole set");
        workplan.Add(left);
        step.Outputs[0] = left;
        step.Outputs[1] = oldLeft;
        step.Outputs[2] = failed;

        step = new DummyStep(3, "Set screw");
        step.Inputs[0] = left;
        workplan.Add(step);
        step.Outputs[0] = right;
        step.Outputs[1] = complete;
        step.Outputs[2] = failed;

        var rightOnly = new DummyStep(1, "Remove case");
        rightOnly.Inputs[0] = right;
        rightOnly.Outputs[0] = failed;
        workplan.Add(rightOnly);

        return workplan;
    }
}