using Marvin.Workflows;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Tests.Workflows
{
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var step = new DummyStep(2, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var left = Workflow.CreateConnector("Left");
            workplan.Add(left);
            step.Outputs[0] = left;
            var right = Workflow.CreateConnector("Right");
            workplan.Add(right);
            step.Outputs[1] = right;

            var rightOnly = new DummyStep(1, "B");
            rightOnly.Inputs[0] = right;
            workplan.Add(rightOnly);
            rightOnly.Outputs[0] = left;

            var merge = new DummyStep(2, "C");
            merge.Inputs[0] = left;
            workplan.Add(merge);
            merge.Outputs[0] = merge.Outputs[1] = complete;

            return workplan;
        }

        public static WorkplanDummy CreateBig()
        {
            var workplan = new WorkplanDummy();

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            var failed = Workflow.CreateConnector("Failed", NodeClassification.Failed);
            workplan.Add(inital, complete, failed);

            var step = new DummyStep(2, "Feed case");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var left = Workflow.CreateConnector("Left");
            workplan.Add(left);
            step.Outputs[0] = left;
            var right = Workflow.CreateConnector("Right");
            workplan.Add(right);
            step.Outputs[1] = right;

            step = new DummyStep(3, "Mount");
            step.Inputs[0] = left;
            workplan.Add(step);
            step.Outputs[2] = right;
            left = Workflow.CreateConnector("Merge");
            workplan.Add(left);
            step.Outputs[0] = step.Outputs[1] = left;

            step = new DummyStep(2, "Set pole");
            step.Inputs[0] = left;
            workplan.Add(step);
            left = Workflow.CreateConnector("Pole set");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var subPlan = CreateSub();
            subPlan.Id = 42;
            var sub = new SubworkflowStep(subPlan);
            sub.Inputs[0] = inital;
            sub.Outputs[0] = sub.Outputs[0] = complete;
            workplan.Add(sub);

            return workplan;
        }

        public static WorkplanDummy CreateSub()
        {
            var workplan = new WorkplanDummy();

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            var failed = Workflow.CreateConnector("Failed", NodeClassification.Failed);
            workplan.Add(inital, complete, failed);

            var step = new DummyStep(1, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var aComplete = Workflow.CreateConnector("A1");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var step = new DummyStep(1, "A");
            step.Inputs[0] = inital;
            // workplan.Add(step);  // Add first step at the end

            var aComplete = Workflow.CreateConnector("A1");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var step = new DummyStep(1, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var aComplete = Workflow.CreateConnector("A1");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital);
            workplan.Add(complete);

            var step = new DummyStep(1, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var a1 = Workflow.CreateConnector("Before pause");
            workplan.Add(a1);
            step.Outputs[0] = a1;

            var rightOnly = new PausableStep();
            rightOnly.Inputs[0] = a1;
            workplan.Add(rightOnly);
            step.Outputs[0] = a1;

            var b1 = Workflow.CreateConnector("Right complete");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var step = new DummyStep(2, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var left = Workflow.CreateConnector("Left");
            workplan.Add(left);
            step.Outputs[0] = left;
            var right = Workflow.CreateConnector("Right");
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

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            workplan.Add(inital, complete);

            var step = new DummyStep(2, "A");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var left = Workflow.CreateConnector("Left");
            workplan.Add(left);
            step.Outputs[0] = left;
            var right = Workflow.CreateConnector("DeadEnd");
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

        public static WorkplanDummy WithLoop()
        {
            var workplan = new WorkplanDummy();

            var inital = Workflow.CreateConnector("Start", NodeClassification.Start);
            var complete = Workflow.CreateConnector("End", NodeClassification.End);
            var failed = Workflow.CreateConnector("Failed", NodeClassification.Failed);
            workplan.Add(inital, complete, failed);

            var step = new DummyStep(2, "Feed case");
            step.Inputs[0] = inital;
            workplan.Add(step);

            var left = Workflow.CreateConnector("Left");
            workplan.Add(left);
            step.Outputs[0] = left;
            var right = Workflow.CreateConnector("Right");
            workplan.Add(right);
            step.Outputs[1] = right;

            step = new DummyStep(3, "Mount");
            step.Inputs[0] = left;
            workplan.Add(step);
            step.Outputs[2] = right;
            left = Workflow.CreateConnector("Merge");
            workplan.Add(left);
            step.Outputs[0] = step.Outputs[1] = left;

            step = new DummyStep(3, "Set pole");
            step.Inputs[0] = left;
            workplan.Add(step);
            var oldLeft = left;
            left = Workflow.CreateConnector("Pole set");
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
}