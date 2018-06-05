using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Marvin.Tests.Workflows;
using Marvin.Workflows.Compiler;

namespace Marvin.Workflows.Benchmark
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

            Console.WriteLine();
            Console.WriteLine("1. Execute benchmark");
            Console.WriteLine("2. Demonstrate compiler");
            Console.Write("Selection: ");
            var selection = int.Parse(Console.ReadLine());

            Console.WriteLine();
            switch (selection)
            {
                case 1:
                    ExecuteBenchmark();
                    break;
                case 2:
                    DemonstrateCompiler();
                    break;
            }

            Console.ReadLine();
        }

        private static void DemonstrateCompiler()
        {
            Console.WriteLine("1: Simple plan");
            Console.WriteLine("2: Split plan");
            Console.WriteLine("3: Generate workplan");
            Console.WriteLine("4: Real plan");
            Console.Write("Selection: ");
            var selection = int.Parse(Console.ReadLine());

            IWorkplan workplan = null;
            ICompiler<CompiledDummyTransition> compiler = null;
            switch (selection)
            {
                case 1:
                    workplan = WorkplanDummy.CreateSub();
                    break;
                case 2:
                    workplan = WorkplanDummy.CreateFull();
                    break;
                case 3:
                    Console.Write("Executed transitions: ");
                    var transCount = int.Parse(Console.ReadLine());
                    workplan = GenerateWorkplan(transCount);
                    break;
                case 4:
                    workplan = WorkplanDummy.CreateBig();
                    compiler = new StationMapCompiler(GenerateStationMap(workplan));
                    break;
            }

            Console.Write("Compiling workplan...");
            var compiled = Workflow.Compile(workplan, new NullContext(), compiler ?? new DummyStepCompiler());
            Console.WriteLine("done!");
            Console.WriteLine();

            Console.WriteLine("Steps:");
            Console.WriteLine("|  Id  |  Outfeed  |  Station  |  Name");
            Console.WriteLine("|------+-----------+-----------+----------");
            foreach (var step in compiled.Steps)
            {
                Console.WriteLine("|  {0:D2}  |   {1}   |    {2:D2}     |  {3} ", 
                    step.Id, step.IsOutfeed.ToString().PadRight(5), step.Station, step.Name);
            }

            Console.WriteLine();
            Console.WriteLine("Decision matrix:");
            // Write header
            Console.Write("|   |");   
            foreach (var step in compiled.Steps.Where(s => !s.IsOutfeed))
            {
                Console.Write(" {0:D2} |", step.Id);   
            }
            Console.WriteLine();
            Console.Write("|---+");
            foreach (var step in compiled.Steps.Where(s => !s.IsOutfeed))
            {
                Console.Write("----+");
            }
            Console.WriteLine();

            // Write rows
            for (int i = 0; i < compiled.DecisionMatrix.GetLength(1); i++)
            {
                Console.Write("| {0} |", i);
                // Rows
                for (int j = 0; j < compiled.DecisionMatrix.GetLength(0); j++)
                {
                    Console.Write(" {0:D2} |", compiled.DecisionMatrix[j, i]);
                }
                Console.WriteLine();
            }
        }

        private static IDictionary<long, int> GenerateStationMap(IWorkplan workplan)
        {
            var dictionary = new Dictionary<long, int>();
            dictionary[0] = 5;

            foreach (var step in workplan.Steps)
            {
                var station = 0;
                switch (step.Name)
                {
                    case "Feed case":
                        station = 1;
                        break;
                    case "Mount":
                        station = 1;
                        break;
                    case "Set pole":
                        station = 2;
                        break;
                    case "Set screw":
                        station = 3;
                        break;
                    case "Remove case":
                        station = 4;
                        break;
                }
                dictionary[step.Id] = station;
            }
            return dictionary;
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
                    message = "Compiling workplan",
                    operation = new Action(() => Workflow.Compile(workplan, new NullContext(), new DummyStepCompiler()))
                },
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

            // Compile compiler :-)
            var compiled = Workflow.Compile(workplan, new NullContext(), new DummyStepCompiler());
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
