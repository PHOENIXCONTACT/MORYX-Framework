// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Enum representing the transaction progress
    /// </summary>
    public enum MovementProgress
    {
        /// <summary>
        /// Default value
        /// </summary>
        Unset,

        /// <summary>
        /// Default start value if process information were retrieved from the source
        /// </summary>
        InformationRetrieved,

        /// <summary>
        /// Information was written to the target
        /// </summary>
        MountedOnTarget,

        /// <summary>
        /// Information was removed from the source
        /// </summary>
        RemovedFromSource,

        /// <summary>
        /// Transaction was confirmed and completed
        /// </summary>
        Completed,

        /// <summary>
        /// Transaction was aborted and all information is restored
        /// </summary>
        Aborted
    }

    /// <summary>
    /// Class that wraps process movement within a transaction
    /// </summary>
    public class ProcessMovement : IDisposable
    {
        private readonly IProcessHolderPosition _source;
        private readonly IProcessHolderPosition _target;

        /// <summary>
        /// Progress of the transaction
        /// </summary>
        public MovementProgress Progress { get; private set; }

        /// <summary>
        /// Current mount information moved from one <see cref="IProcessHolderPosition"/> to another one
        /// </summary>
        public MountInformation MountInformation { get; }

        private ProcessMovement(IProcessHolderPosition source, IProcessHolderPosition target)
        {
            _source = source;
            _target = target;

            MountInformation = _source.MountInformation;
            Progress = MountInformation.Process == null ? MovementProgress.Aborted : MovementProgress.InformationRetrieved;
        }

        /// <summary>
        /// Initiate a process movement transaction
        /// </summary>
        public static ProcessMovement Transaction(IProcessHolderPosition source, IProcessHolderPosition target)
        {
            return new ProcessMovement(source, target);
        }

        /// <summary>
        /// Execute the movement from source to target
        /// </summary>
        /// <param name="source">Current holder of the process and its session</param>
        /// <param name="target">Desired target of the process and session</param>
        /// <returns></returns>
        public static bool Move(IProcessHolderPosition source, IProcessHolderPosition target)
        {
            using (var transaction = new ProcessMovement(source, target))
            {
                if (transaction.Progress == MovementProgress.Aborted)
                    return false;

                transaction.MountOnTarget();

                transaction.RemoveOnSource();

                transaction.Confirm();

                return transaction.Progress == MovementProgress.Completed;
            }
        }

        /// <summary>
        /// Mount information on the target <see cref="IProcessHolderPosition"/>
        /// </summary>
        public void MountOnTarget()
        {
            if (Progress < MovementProgress.InformationRetrieved)
                throw new InvalidOperationException("Can not mount before initiating the transaction");

            _target.Mount(MountInformation);
            Progress = MovementProgress.MountedOnTarget;
        }

        /// <summary>
        /// Remove mount information from the source <see cref="IProcessHolderPosition"/>
        /// </summary>
        public void RemoveOnSource()
        {
            if (Progress < MovementProgress.MountedOnTarget)
                throw new InvalidOperationException("Can not remove before mounting on target");

            _source.Unmount();
            Progress = MovementProgress.RemovedFromSource;
        }

        /// <summary>
        /// Confirm transaction
        /// </summary>
        public void Confirm()
        {
            if (Progress < MovementProgress.MountedOnTarget)
                throw new InvalidOperationException("Can not confirm before everything was completed");

            Progress = MovementProgress.Completed;
        }

        /// <summary>
        /// Something went wrong, so we cancel the transaction
        /// </summary>
        public void RollBack()
        {
            switch (Progress)
            {
                case MovementProgress.MountedOnTarget:
                    _target.Unmount();
                    break;
                case MovementProgress.RemovedFromSource:
                    _source.Mount(MountInformation);
                    _target.Unmount();
                    break;
            }

            Progress = MovementProgress.Aborted;
        }

        /// <summary>
        /// Dispose the transaction. Roll back any chances if it was not confirmed
        /// </summary>
        public void Dispose()
        {
            if (Progress < MovementProgress.Completed)
                RollBack();
        }
    }
}
