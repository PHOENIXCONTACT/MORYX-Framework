// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Message send by the resource when it not or no longer ready to work with a resource
    /// </summary>
    public class NotReadyToWork : Session
    {
        private readonly ReadyToWork _pausedSession;

        /// <summary>
        /// Signal not/no longer ready to work for an empty resource/wpc
        /// </summary>
        internal NotReadyToWork(ReadyToWork currentSession)
            : base(currentSession)
        {
            _pausedSession = currentSession;
        }

        /// <summary>
        /// Resume the paused session
        /// </summary>
        public ReadyToWork ResumeSession()
        {
            return _pausedSession;
        }
    }
}
