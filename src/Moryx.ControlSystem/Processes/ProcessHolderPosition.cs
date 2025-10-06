// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Implementation of <see cref="IProcessHolderPosition"/>
    /// </summary>
    public class ProcessHolderPosition : Resource, IProcessHolderPosition
    {
        [DataMember]
        private long _processId;

        private Session _session;

        #region EntrySerialize

        /// <summary>
        /// Gets the current process id for the ui.
        /// </summary>
        [EntrySerialize]
        [Description("Id of the carried process")]
        public long CurrentProcess => _processId;

        /// <summary>
        /// Gets the running activity.
        /// </summary>
        [EntrySerialize]
        [Description("Information about the current activity of the current process.")]
        public string CurrentActivity => Process?.CurrentActivity() == null
            ? string.Empty : $"{Process.CurrentActivity().Id} - {Process.CurrentActivity().GetType().Name}";

        /// <inheritdoc />
        [EntrySerialize, DataMember]
        public string Identifier { get; set; }

        #endregion

        /// <inheritdoc />
        public IProcess Process
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public Session Session
        {
            get => _session;
            set
            {
                _session = value;
                // Sync process reference with control system kernel
                if (_session?.Process?.Id == _processId)
                    Process = _session.Process;
            }
        }

        /// <inheritdoc />
        public MountInformation MountInformation => new(Process, Session);

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (_processId == EmptyProcess.ProcessId)
                Process = new EmptyProcess();
            else if (_processId != 0) // Everything is unknown until we receive a response from the control system
                Process = new UnknownProcess(_processId);
        }

        /// <summary>
        /// Start a session on the position
        /// </summary>
        public ReadyToWork StartSession() => StartSession(null);

        /// <summary>
        /// Start a session on the position
        /// </summary>
        public ReadyToWork StartSession(IConstraint[] constraints)
        {
            // Pick the correct over load based on process and constraint
            // It looks big, but reduces the memory impact per carrier and better uses the different overloads on session
            if (Process == null && constraints == null)
                Session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull);
            else if (Process == null && constraints != null)
                Session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, constraints);
            else if (Process != null && constraints == null)
                Session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, Process.Id);
            else
                Session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, Process.Id, constraints);

            return (ReadyToWork)Session;
        }

        /// <summary>
        /// Assign new process to holder position and write to database
        /// </summary>
        /// <param name="process"></param>
        public void AssignProcess(IProcess process)
        {
            if (Process == process)
                return;

            Process = process;
            _processId = process?.Id ?? 0;

            RaiseResourceChanged();

            ProcessChanged?.Invoke(this, process);
        }

        /// <summary>
        /// Complete a running activity on the holder and update the session property
        /// </summary>
        public ActivityCompleted CompleteActivity(long result)
        {
            if (!(Session is ActivityStart activityStart))
                throw new InvalidOperationException("Can only complete the activity if the current session is ActivityStart");

            Session = activityStart.CreateResult(result);
            return (ActivityCompleted)Session;
        }

        /// <inheritdoc />
        public virtual void Mount(MountInformation mountInformation)
        {
            if (Process != null)
                throw new InvalidOperationException("Can not mount a position currently holding a process");

            Session = mountInformation.Session;
            AssignProcess(mountInformation.Process);
        }

        /// <inheritdoc />
        public virtual void Unmount()
        {
            ClearPosition();
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            ClearPosition();
            RaisePositionReset();
        }

        /// <summary>
        /// Clear all information on this position
        /// </summary>
        protected internal void ClearPosition()
        {
            Session = null;
            AssignProcess(null);
        }

        /// <inheritdoc />
        public event EventHandler<IProcess> ProcessChanged;

        /// <inheritdoc />
        public event EventHandler ResetExecuted;

        /// <summary>
        /// Raise the <see cref="ResetExecuted"/> event
        /// </summary>
        protected void RaisePositionReset()
        {
            ResetExecuted?.Invoke(this, EventArgs.Empty);
        }
    }
}
