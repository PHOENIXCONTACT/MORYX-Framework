// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Logging;
using Moryx.Orders.Management.Properties;
using Moryx.Orders.Restrictions;
using Moryx.Tools;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Handles actions on operations which have more dependencies than the operation itself
    /// It is the third pillar between the pool and other components
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IOperationManager))]
    internal class OperationManager : IOperationManager, ILoggingComponent
    {
        /// <inheritdoc />
        [UseChild(nameof(OperationManager))]
        public IModuleLogger Logger { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        public IOperationDataPool OperationDataPool { get; set; }

        /// <inheritdoc />
        public Task Assign(IOperationData operationData)
        {
            return operationData.Assign();
        }

        /// <inheritdoc />
        public BeginContext GetBeginContext(IOperationData operationData)
        {
            var state = operationData.State;
            var restrictions = new List<BeginRestriction>();

            AddBeginRestrictions(restrictions, state);

            AddOverDeliveryRestriction(restrictions, operationData, state);

            AddExternalRestrictions(restrictions, operationData);

            var beginContext = operationData.GetBeginContext();
            beginContext.CanBegin = restrictions.All(r => r.CanBegin);
            beginContext.Restrictions = restrictions.Where(r => r.Description != null).Select(r => r.Description).ToArray();

            if (beginContext.CanReduce)
            {
                beginContext.CanReduce = restrictions.All(r => r.CanReduce);
                beginContext.MinimalTargetAmount = restrictions.Max(r => r.MinimalTargetAmount);
            }

            return beginContext;
        }

        private void AddBeginRestrictions(IList<BeginRestriction> restrictions, IOperationState state)
        {
            // Operation State
            if (!state.CanBegin)
            {
                restrictions.Add(new BeginRestriction(false, Strings.OperationManager_StateErrorText, RestrictionSeverity.Error));
            }

            // Max Running Operations
            var runningOperations = OperationDataPool.GetAll(o =>
                o.State.Classification is OperationStateClassification.Running or OperationStateClassification.Interrupting);
            if (state.Classification != OperationStateClassification.Running && state.Classification != OperationStateClassification.Interrupting &&
                ModuleConfig.MaxRunningOperations >= 0 && runningOperations.Count >= ModuleConfig.MaxRunningOperations)
            {
                restrictions.Add(new BeginRestriction(false, Strings.OperationManager_MaxRunningErrorText, RestrictionSeverity.Error));
            }
        }

        private static void AddOverDeliveryRestriction(IList<BeginRestriction> restrictions, IOperationData operationData, IOperationState state)
        {
            if (state.CanBegin && operationData.Operation.State.HasFlag(OperationStateClassification.IsAmountReached))
            {
                restrictions.Add(new BeginRestriction(canBegin: true, canReduce: false, operationData.Operation.Progress.SuccessCount,
                    Strings.OperationManager_OverDeliveryNotice, RestrictionSeverity.Warning));
            }
            else if ((state.Classification == OperationStateClassification.Running
                && !operationData.Operation.State.HasFlag(OperationStateClassification.IsAmountReached))
                || state.Classification == OperationStateClassification.Interrupting)
            {
                restrictions.Add(new BeginRestriction(canBegin: true, canReduce: true, operationData.Operation.Progress.SuccessCount,
                    Strings.OperationManager_OverDeliveryNotice, RestrictionSeverity.Info));
            }
        }

        private void AddExternalRestrictions(IList<BeginRestriction> restrictions, IOperationData operationData)
        {
            var externalRequest = new BeginRequestEventArgs(operationData);
            BeginRequest?.Invoke(this, externalRequest);
            restrictions.AddRange(externalRequest.Restrictions);
        }

        public ReportContext GetReportContext(IOperationData operationData) =>
            GetReportContext(operationData, ReportType.Report);

        public ReportContext GetInterruptContext(IOperationData operationData) =>
            GetReportContext(operationData, ReportType.Interrupt);

        /// <inheritdoc />
        public Task Adjust(IOperationData operationData, User user, int amount)
        {
            if (!user.SignedIn)
            {
                throw new InvalidOperationException("User for the begin of the operation was not signed in.");
            }

            return operationData.Adjust(amount, user);
        }

        private ReportContext GetReportContext(IOperationData operationData, ReportType reportType)
        {
            var restrictions = new List<ReportRestriction>();

            // Operation State
            var canPartial = operationData.State.CanPartialReport;
            var canFinal = operationData.State.CanFinalReport;

            if (canPartial == false && canFinal == false)
                restrictions.Add(new ReportRestriction(false, false, Strings.OperationManager_NoReportErrorText, RestrictionSeverity.Error));

            if (!canPartial && canFinal)
                restrictions.Add(new ReportRestriction(false, true, Strings.OperationManager_NoPartialReportErrorText, RestrictionSeverity.Warning));

            if (canPartial && !canFinal)
                restrictions.Add(new ReportRestriction(true, false, Strings.OperationManager_NoFinalReportErrorText, RestrictionSeverity.Warning));

            // Collect external restrictions
            var externalRequest = new ReportRequestEventArgs(operationData, reportType);
            ReportRequest?.Invoke(this, externalRequest);
            restrictions.AddRange(externalRequest.Restrictions);

            var reportContext = operationData.GetReportContext();
            reportContext.CanPartial = restrictions.All(r => r.CanPartialReport);
            reportContext.CanFinal = restrictions.All(r => r.CanFinalReport);
            reportContext.Restrictions = restrictions.Where(r => r.Description != null).Select(r => r.Description).ToArray();

            return reportContext;
        }

        /// <inheritdoc />
        public Task Report(IOperationData operationData, OperationReport report)
        {
            if (!report.User.SignedIn)
                throw new InvalidOperationException("User for the report of the operation was not signed in.");

            return operationData.Report(report);
        }

        /// <inheritdoc />
        public Task Interrupt(IOperationData operationData, User user)
        {
            return operationData.Interrupt(user);
        }

        /// <inheritdoc />
        public Task Abort(IOperationData operationData)
        {
            return operationData.Abort();
        }

        public event EventHandler<BeginRequestEventArgs> BeginRequest;

        public event EventHandler<ReportRequestEventArgs> ReportRequest;
    }
}

