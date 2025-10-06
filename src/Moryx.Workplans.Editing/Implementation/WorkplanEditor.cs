// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Tools;
using Moryx.Workplans.Editing.Components;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.Workplans.Editing.Implementation
{
    [Component(LifeCycle.Singleton, typeof(IWorkplanEditor))]
    internal class WorkplanEditor : IWorkplanEditor, IPlugin
    {
        private Type[] _availableTasks;
        private List<IWorkplanEditingSession> _sessions = new();

        #region Dependencies
        public ModuleConfig Config { get; set; }

        #endregion

        #region Lifecycle
        public void Start()
        {
            _availableTasks = ReflectionTool.GetPublicClasses<WorkplanStepBase>(StepFilter).ToArray();
        }

        private bool StepFilter(Type stepType)
        {
            // Some step types are always allowed in editor because they will be used as template
            if (stepType == typeof(SubworkplanStep) || stepType == typeof(SplitWorkplanStep) || stepType == typeof(JoinWorkplanStep))
                return true;

            // Find match in settings
            var settings = Config.StepSettings.FirstOrDefault(ts => ts.StepType == stepType.FullName);
            return settings?.Enabled ?? !Config.HideUnknown;
        }

        public void Stop()
        {

        }
        #endregion

        public IWorkplanEditingSession this[string token] => _sessions.FirstOrDefault(s => s.Session.Token.Equals(token));

        public IReadOnlyList<Type> AvailableSteps => _availableTasks.ToList().AsReadOnly();

        public void CloseSession(string token)
        {
            var session = this[token];
            if (session == null)
                return;

            _sessions.Remove(session);
        }

        public IWorkplanEditingSession EditWorkplan(Workplan workplan, bool duplicate)
        {
            if (duplicate)
            {
                workplan.Id = 0;
                workplan.Version = 0;
                workplan.State = WorkplanState.New;
            }
            var session = new WorkplanEditingSession(workplan);
            _sessions.Add(session);
            return session;
        }
    }
}

