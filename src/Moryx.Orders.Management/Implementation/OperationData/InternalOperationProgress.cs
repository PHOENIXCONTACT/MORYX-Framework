// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    internal class InternalOperationProgress : OperationProgress
    {
        public new int RunningCount
        {
            get => base.RunningCount;
            set => base.RunningCount = value;
        }

        public new int SuccessCount
        {
            get => base.SuccessCount;
            set => base.SuccessCount = value;
        }

        public new int FailureCount
        {
            get => base.FailureCount;
            set => base.FailureCount = value;
        }

        public new int ReworkedCount
        {
            get => base.ReworkedCount;
            set => base.ReworkedCount = value;
        }

        public new int ScrapCount
        {
            get => base.ScrapCount;
            set => base.ScrapCount = value;
        }

        public new int PendingCount
        {
            get => base.PendingCount;
            set => base.PendingCount = value;
        }

        public new int ProgressRunning
        {
            get => base.ProgressRunning;
            set => base.ProgressRunning = value;
        }

        public new int ProgressSuccess
        {
            get => base.ProgressSuccess;
            set => base.ProgressSuccess = value;
        }

        public new int ProgressScrap
        {
            get => base.ProgressScrap;
            set => base.ProgressScrap = value;
        }

        public new int ProgressPending
        {
            get => base.ProgressPending;
            set => base.ProgressPending = value;
        }
    }
}
