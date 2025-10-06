// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Logging;
using Moryx.StateMachines;
using System.Runtime.CompilerServices;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal abstract class JobStateBase : StateBase<JobDataBase>, IJobState
    {
        internal const int CompletedKey = 999;
        internal const int InitialKey = 0;

        int IJobState.Key => Key;

        public JobClassification Classification { get; }

        public virtual bool CanComplete => false;

        public virtual bool CanAbort => false;

        public virtual bool IsStable => false;

        protected JobStateBase(JobDataBase context, StateMap stateMap, JobClassification classification)
            : base(context, stateMap)
        {
            Classification = classification;
        }

        /// <summary>
        /// Logs an error to the context
        /// </summary>
        protected void InvalidJobState([CallerMemberName] string methodName = "")
        {
            var error = $"The state with the name '{GetType().Name}' cannot handle the method '{methodName}'.";
            (Context as ILoggingComponent)?.Logger.Log(LogLevel.Error, error);
        }

        public virtual void Ready()
        {
            InvalidJobState();
        }

        public virtual void Start()
        {
            InvalidJobState();
        }

        public virtual void Stop()
        {
            InvalidJobState();
        }

        public virtual void Complete()
        {
            InvalidJobState();
        }

        public virtual void Abort()
        {
            InvalidJobState();
        }

        public virtual void Interrupt()
        {
            InvalidJobState();
        }

        public virtual void Load()
        {
            InvalidJobState();
        }

        public virtual void ProcessChanged(ProcessData processData, ProcessState trigger)
        {
            InvalidJobState();
        }
    }
}
