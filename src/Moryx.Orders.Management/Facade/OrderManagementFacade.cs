// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Orders.Advice;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Advice;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Users;
using Newtonsoft.Json;

namespace Moryx.Orders.Management
{
    internal class OrderManagementFacade : IOrderManagement, IFacadeControl
    {
        public IOperationDataPool OperationDataPool { get; set; }

        public IOperationManager OperationManager { get; set; }

        public IUserManagement UserManagement { get; set; }

        public IRecipeAssignment RecipeAssignment { get; set; }

        public IOperationLoggerProvider LoggerProvider { get; set; }

        public IAdviceManager AdviceManager { get; set; }

        public Action ValidateHealthState { get; set; }

        public void Activate()
        {
            OperationDataPool.OperationStarted += OnOperationStarted;
            OperationDataPool.OperationInterrupted += OnOperationInterrupted;
            OperationDataPool.OperationCompleted += OnOperationCompleted;

            OperationDataPool.OperationProgressChanged += OnOperationProgressChanged;
            OperationDataPool.OperationUpdated += OnOperationUpdated;

            OperationDataPool.OperationAdviced += OnOperationAdviced;
            OperationDataPool.OperationPartialReport += OnOperationPartialReport;

            OperationManager.BeginRequest += OnOperationBeginRequest;
            OperationManager.ReportRequest += OnOperationReportRequest;
        }

        public void Deactivate()
        {
            OperationDataPool.OperationStarted -= OnOperationStarted;
            OperationDataPool.OperationInterrupted -= OnOperationInterrupted;
            OperationDataPool.OperationCompleted -= OnOperationCompleted;

            OperationDataPool.OperationUpdated -= OnOperationUpdated;
            OperationDataPool.OperationProgressChanged -= OnOperationProgressChanged;

            OperationDataPool.OperationAdviced -= OnOperationAdviced;
            OperationDataPool.OperationPartialReport -= OnOperationPartialReport;

            OperationManager.BeginRequest -= OnOperationBeginRequest;
            OperationManager.ReportRequest -= OnOperationReportRequest;
        }

        private void OnOperationStarted(object sender, StartedEventArgs e)
        {
            OperationStarted?.Invoke(this, new OperationStartedEventArgs(e.OperationData.Operation, e.User));
        }

        private void OnOperationInterrupted(object sender, OperationEventArgs e)
        {
            OperationInterrupted?.Invoke(this, new OperationChangedEventArgs(e.OperationData.Operation));
        }

        private void OnOperationCompleted(object sender, ReportEventArgs e)
        {
            OperationCompleted?.Invoke(this, new OperationReportEventArgs(e.OperationData.Operation, e.Report));
        }

        private void OnOperationPartialReport(object sender, ReportEventArgs e)
        {
            OperationPartialReport?.Invoke(this, new OperationReportEventArgs(e.OperationData.Operation, e.Report));
        }

        private void OnOperationProgressChanged(object sender, OperationEventArgs e)
        {
            OperationProgressChanged?.Invoke(this, new OperationChangedEventArgs(e.OperationData.Operation));
        }

        private void OnOperationUpdated(object sender, OperationEventArgs e)
        {
            OperationUpdated?.Invoke(this, new OperationChangedEventArgs(e.OperationData.Operation));
        }

        private void OnOperationAdviced(object sender, AdviceEventArgs e)
        {
            OperationAdviced?.Invoke(this, new OperationAdviceEventArgs(e.OperationData.Operation, e.Advice));
        }

        private void OnOperationBeginRequest(object sender, BeginRequestEventArgs e)
        {
            if (OperationBeginRequest == null)
            {
                return;
            }

            var request = new OperationBeginRequestEventArgs(e.OperationData.Operation);

            OperationBeginRequest(this, request);

            foreach (var restriction in request.Restrictions)
            {
                e.AddRestriction(restriction);
            }
        }

        private void OnOperationReportRequest(object sender, ReportRequestEventArgs e)
        {
            if (OperationReportRequest == null)
                return;

            var request = new OperationReportRequestEventArgs(e.OperationData.Operation, e.ReportType);

            OperationReportRequest(this, request);

            foreach (var restriction in request.Restrictions)
                e.AddRestriction(restriction);
        }

        public IReadOnlyList<Operation> GetOperations(Func<Operation, bool> filter)
        {
            ValidateHealthState();

            var filtered = OperationDataPool.GetAll(data => filter(data.Operation));
            return filtered.Select(o => o.Operation).ToArray();
        }

        public Operation GetOperation(string orderNumber, string operationNumber)
        {
            ValidateHealthState();

            var operationData = OperationDataPool.Get(orderNumber, operationNumber);
            return operationData?.Operation;
        }

        public Operation GetOperation(Guid identifier)
        {
            ValidateHealthState();

            var operationData = OperationDataPool.Get(identifier);
            return operationData?.Operation;
        }

        public Operation AddOperation(OperationCreationContext context)
        {
            return AddOperation(context, new NullOperationSource());
        }

        public Operation AddOperation(OperationCreationContext context, IOperationSource source)
        {
            ValidateHealthState();

            // Validate source
            source ??= new NullOperationSource();

            if (source is not NullOperationSource)
            {
                try
                {
                    // Try serialization
                    JsonConvert.SerializeObject(source, typeof(IOperationSource), JsonSettings.Minimal);
                }
                catch
                {
                    throw new ArgumentException("Operation source must be serializable", nameof(source));
                }
            }

            // Add to pool
            var operationData = OperationDataPool.Add(context, source);

            return operationData.Operation;
        }

        public BeginContext GetBeginContext(Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            return OperationManager.GetBeginContext(operationData);
        }

        public void BeginOperation(Operation operation, int amount)
        {
            BeginOperation(operation, amount, UserManagement.DefaultUser);
        }

        public void BeginOperation(Operation operation, int amount, User user)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            OperationManager.Adjust(operationData, user, amount);
        }

        public void AbortOperation(Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            OperationManager.Abort(operationData);
        }

        public void SetOperationSortOrder(int sortOrder, Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            operationData.SortOrder = sortOrder;
        }

        public void UpdateSource(IOperationSource source, Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            if (operationData.Operation.Source.Type != source.Type)
                throw new InvalidOperationException("Type of the operation source cannot be changed");

            operationData.UpdateSource(source);
        }

        public ReportContext GetReportContext(Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            var reportContext = OperationManager.GetReportContext(operationData);
            return reportContext;
        }

        public void ReportOperation(Operation operation, OperationReport report)
        {
            ValidateHealthState();

            // Get default user if there is no in the report
            report.User ??= UserManagement.DefaultUser;

            var operationData = GetOperationDataSave(operation);
            OperationManager.Report(operationData, report);
        }

        public ReportContext GetInterruptContext(Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            return OperationManager.GetInterruptContext(operationData);
        }

        public void InterruptOperation(Operation operation)
        {
            InterruptOperation(operation, UserManagement.DefaultUser);
        }

        public void InterruptOperation(Operation operation, User user)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            OperationManager.Interrupt(operationData, user ?? UserManagement.DefaultUser);
        }

        private IOperationData GetOperationDataSave(Operation operation)
        {
            var operationData = OperationDataPool.Get(operation);
            if (operationData == null)
                throw new ArgumentException($"Operation with identifier {operation.Identifier} not found");

            return operationData;
        }

        public void Reload(Operation operation)
        {
            ValidateHealthState();

            OperationManager.Assign(OperationDataPool.Get(operation));
        }

        public AdviceContext GetAdviceContext(Operation operation)
        {
            ValidateHealthState();

            var operationData = GetOperationDataSave(operation);
            return operationData.GetAdviceContext();
        }

        public async Task<AdviceResult> TryAdvice(Operation operation, OperationAdvice advice)
        {
            ValidateHealthState();

            var operationData = OperationDataPool.Get(operation);
            if (advice is OrderAdvice)
            {
                var orderAdvice = advice as OrderAdvice;
                return await AdviceManager.OrderAdvice(operationData, orderAdvice.ToteBoxNumber, orderAdvice.Amount);
            }
            else
            {
                var pickPartAdvice = advice as PickPartAdvice;
                return await AdviceManager.PickPartAdvice(operationData, pickPartAdvice.ToteBoxNumber, pickPartAdvice.Part);
            }
        }

        public IReadOnlyCollection<OperationLogMessage> GetLogs(Operation operation)
        {
            ValidateHealthState();

            var operationData = OperationDataPool.Get(operation);
            var operationLogger = LoggerProvider.GetLogger(operationData);
            if (operationLogger == null)
                return Array.Empty<OperationLogMessage>();

            return operationLogger.Messages;
        }

        public Task<IReadOnlyList<IProductRecipe>> GetAssignableRecipes(ProductIdentity identity)
        {
            ValidateHealthState();

            return RecipeAssignment.PossibleRecipes(identity);
        }

        public event EventHandler<OperationStartedEventArgs> OperationStarted;

        public event EventHandler<OperationReportEventArgs> OperationCompleted;

        public event EventHandler<OperationChangedEventArgs> OperationInterrupted;

        public event EventHandler<OperationReportEventArgs> OperationPartialReport;

        public event EventHandler<OperationChangedEventArgs> OperationProgressChanged;

        public event EventHandler<OperationChangedEventArgs> OperationUpdated;

        public event EventHandler<OperationAdviceEventArgs> OperationAdviced;

        public event EventHandler<OperationBeginRequestEventArgs> OperationBeginRequest;

        public event EventHandler<OperationReportRequestEventArgs> OperationReportRequest;
    }
}

