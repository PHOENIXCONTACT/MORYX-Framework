// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Constraints;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.ControlSystem.ProcessEngine;

[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    public IActivityDataPool ActivityPool { get; set; }

    public IJobDataList JobList { get; set; }

    public IActivityDispatcher ActivityDispatcher { get; set; }

    public class SessionExport
    {
        public long ResourceId { get; set; }

        public string ResourceName { get; set; }

        public ActivityClassification Mode { get; set; }

        public ReadyToWorkType Type { get; set; }

        public string[] Constraints { get; set; }

        public override string ToString()
        {
            return $"{ResourceName}({ResourceId}) | {Mode} | {Type}";
        }
    }

    [EntrySerialize]
    [Display(Name = "Export Sessions", Description = "Sessions that were reported during ProcessEngineAttached or as RTW.Push but could not yet be processed")]
    public List<SessionExport> ReportedSessions()
    {
        List<SessionExport> result = [];

        var sessions = ActivityDispatcher.ExportSessions();
        foreach (var session in sessions)
        {
            if (session.ReadyToWork is null)
            {
                // Technically the session can be a Session type besides a RTW that is not yet processed
                continue;
            }

            result.Add(new()
            {
                ResourceId = session.Resource.Id,
                ResourceName = session.Resource.Name,
                Mode = session.ReadyToWork.AcceptedClassification,
                Type = session.ReadyToWork.ReadyToWorkType,
                Constraints = session.ReadyToWork.Constraints.Select(c => c?.ToString() ?? "null").ToArray()
            });
        }
        return result;
    }
}
