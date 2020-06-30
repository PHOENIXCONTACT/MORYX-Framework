using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moryx.Tests.Workflows;

namespace Moryx.Workflows.Benchmark
{
    /// TODO: migrate to nunit test
    class Program
    {
        private static readonly Random RandomGen = new Random();
        private static readonly Stopwatch StopWatch = new Stopwatch();

        static void Main(string[] args)
        {
            // Run everything once for JIT
            Console.Write("JIT compilation...");
            JITCompiler();
            Console.WriteLine("...done!");

            ExecuteBenchmark();

            Console.ReadLine();
        }

        private static void ExecuteBenchmark()
        {
            // Get number of transitions
            Console.Write("Executed transitions: ");
            var transCount = int.Parse(Console.ReadLine());

            // Generate Workplan
            Console.WriteLine();
            Console.Write("Generating workplan...");
            var workplan = GenerateWorkplan(transCount);
            Console.WriteLine("done!");

            // Prepare benchmarks
            IWorkflowEngine engine = null;
            var benchmarks = new[]
            {
                new
                {
                    message = "Creating engine",
                    operation = new Action(() => engine = CreateEngine(workplan))
                },
                new
                {
                    message = "Running on default path",
                    operation = new Action(() =>
                    {
                        engine.TransitionTriggered += (sender, transition) => { };
                        engine.Start();
                    })
                },
                new
                {
                    message = "Creating engine",
                    operation = new Action(() => engine = CreateEngine(workplan))
                },
                new
                {
                    message = "Running on random path",
                    operation = new Action(() =>
                    {
                        engine.TransitionTriggered += FindPath;
                        engine.Start();
                    })
                }
            };

            // Run benchmarks
            foreach (var benchmark in benchmarks)
            {
                Console.Write(benchmark.message + "...");
                StopWatch.Restart();
                benchmark.operation();
                StopWatch.Stop();
                Console.WriteLine("done! Elapsed time: {0}ms", StopWatch.ElapsedMilliseconds);
            }
        }

        private static void JITCompiler()
        {
            // Workplan
            var workplan = GenerateWorkplan(3);

            // Compile engine
            var engine = Workflow.CreateEngine(workplan, new NullContext());
            engine.TransitionTriggered += (sender, transition) => { };
            engine.Completed += (sender, place) => { };
            engine.Start();
            Workflow.Destroy(engine);
        }

        private static IWorkplan GenerateWorkplan(int transCount)
        {
            var workplan = new WorkplanDummy();

            // Prepare connector variables
            var initial = Workflow.CreateConnector("Start", NodeClassification.Start);
            var left = Workflow.CreateConnector("Left0", NodeClassification.Intermediate);
            var right = Workflow.CreateConnector("Right0", NodeClassification.Intermediate);
            var final = Workflow.CreateConnector("Final", NodeClassification.End);
            workplan.Add(initial, left, right, final);

            // Prepare step variable
            var step = new DummyStep(2, "Split");
            step.Inputs[0] = initial;
            step.Outputs[0] = left;
            step.Outputs[1] = right;
            workplan.Add(step);

            // Create big maze
            DummyStep stepLeft = null, stepRight = null;
            for (int i = 0; i < transCount - 2; i++)
            {
                stepLeft = new DummyStep(2, string.Format("Step{0}Left", i));
                stepLeft.Inputs[0] = left;
                stepRight = new DummyStep(2, string.Format("Step{0}Right", i));
                stepRight.Inputs[0] = right;
                workplan.Add(stepLeft, stepRight);

                left = Workflow.CreateConnector(string.Format("Left{0}", i + 1), NodeClassification.Intermediate);
                right = Workflow.CreateConnector(string.Format("Right{0}", i + 1), NodeClassification.Intermediate);
                workplan.Add(left, right);

                stepLeft.Outputs[0] = stepRight.Outputs[1] = left;
                stepLeft.Outputs[1] = stepRight.Outputs[0] = right;
            }

            // Final join
            stepLeft.Outputs[1] = stepRight.Outputs[0] = left;
            var join = new DummyStep(1, "Complete");
            join.Inputs[0] = left;
            workplan.Add(join);
            join.Outputs[0] = final;
            return workplan;
        }

        private static IWorkflowEngine CreateEngine(IWorkplan workplan)
        {
            var engine = Workflow.CreateEngine(workplan, new NullContext());
            engine.Completed += (sender, place) => { };
            return engine;
        }

        private static void FindPath(object sender, ITransition transition)
        {
            // Console.WriteLine("{0} triggered", transition.Id);
            var trans = (DummyTransition)transition;
            trans.ResultOutput = RandomGen.Next(transition.Outputs.Length);
        }
    }
}
