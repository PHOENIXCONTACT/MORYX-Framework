// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Runtime.Modules;

namespace Moryx.ControlSystem.ProcessEngine
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public IActivityDataPool ActivityPool { get; set; }

        public IJobDataList JobList { get; set; }

        public IActivityDispatcher ActivityDispatcher { get; set; }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            if (!args.Any())
                outputStream("ProcessEngine console requires arguments");

            switch (args[0])
            {
                case "processes":
                    PrintProcesses(args.Skip(1).ToArray(), outputStream);
                    break;
                case "jobs":
                    PrintJobs(args.Skip(1).ToArray(), outputStream);
                    break;
                case "sessions":
                    PrintSessions(args.Skip(1).ToArray(), outputStream);
                    return;
            }
        }

        private void PrintSessions(string[] args, Action<string> outputStream)
        {
            outputStream(" Resource Id : Name                     | Mode           | Type  | Constraint ");

            var sessions = ActivityDispatcher.ExportSessions();
            foreach (var session in sessions)
            {
                outputStream($"  {session.Resource.Id:D4} : {session.Resource.Name,-30}| {session.ReadyToWork.AcceptedClassification,-15}| {session.ReadyToWork.ReadyToWorkType, -6}| {session.ReadyToWork.Constraints.FirstOrDefault()?.GetType().Name}");
            }
        }

        private void PrintJobs(string[] args, Action<string> outputStream)
        {
            if (args.Length <= 0)
                return;

            var operation = args[0].ToLower();

            switch (operation)
            {
                case "list":
                    PrintJobList(outputStream);
                    break;
            }

        }

        private void PrintJobList(Action<string> outputStream)
        {
            var currentJobs = JobList.ToArray();
            if (!currentJobs.Any())
            {
                outputStream("Id | State | Running | Success | Failed | Reworked | Progress   ");
                return;
            }

            var stateLengthCount = currentJobs.Max(j => j.State.ToString().Length);

            var title = $" Id    |{" State ".PadRight(stateLengthCount + 2, ' ')}| Running | Success | Failed | Reworked | Progress   ";
            outputStream(title);
            outputStream("".PadRight(title.Length, '='));
            foreach (var jobData in currentJobs)
            {
                if (jobData is IProductionJobData productionJob)
                {
                    int finishedChars = 0;
                    if (productionJob.SuccessCount > 0 || productionJob.FailureCount > 0)
                    {
                        var percent = ((decimal)productionJob.SuccessCount + (decimal)productionJob.FailureCount) * 10 / jobData.Amount;
                        finishedChars = Convert.ToInt32(Math.Ceiling(percent));
                    }

                    outputStream($" {productionJob.Id,-5} |" +
                                 $" {jobData.State.ToString().PadRight(stateLengthCount, ' ')} |" +
                                 $" {productionJob.RunningProcesses.Count,-7} |" +
                                 $" {productionJob.SuccessCount,-7} |" +
                                 $" {productionJob.FailureCount,-6} |" +
                                 $" {productionJob.ReworkedCount,-8} |" +
                                 " ".PadRight(finishedChars, '#') + "");
                }

                if (jobData is ISetupJobData setupJob)
                {
                    outputStream($" {setupJob.Id,-5} |" +
                                 $" {jobData.State.ToString().PadRight(stateLengthCount, ' ')} |" +
                                 $" {setupJob.RunningCount,-7} |");
                }
            }
        }

        private void PrintProcesses(string[] args, Action<string> outputStream)
        {
            var processes = ActivityPool.Processes;

            outputStream("ProcessId    | State         | ActivityId      | ResourceIds    | ActivityState   | Type");
            foreach (var processData in processes)
            {
                outputStream($" {processData.Id, 11} | {processData.State, -13} |                 |                |                 |");
                foreach (var activityData in processData.Activities)
                {
                    if(args.Any(a => a == "-r") && activityData.State > ActivityState.Running)
                        continue;

                    var possibleResources = activityData.Targets
                        .Select(pr => pr.Id).ToArray();

                    outputStream($"                             | {activityData.Id, 15} | {string.Join(",", possibleResources), -14} | {activityData.State, -15} | {activityData.Activity.GetType().Name} ");
                }

                var reportedSessions = processData.ReportedSessions.ToArray();

                if (reportedSessions.Length == 0)
                    continue;

                foreach (var session in reportedSessions)
                    outputStream($"    {session.Resource.Id:D5} | {session.Resource.Name, -14}| {session.ReadyToWork.AcceptedClassification, -11}| {session.ReadyToWork.ReadyToWorkType}");
            }
        }
    }
}
