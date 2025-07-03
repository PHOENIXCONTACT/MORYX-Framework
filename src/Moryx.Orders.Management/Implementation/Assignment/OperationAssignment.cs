using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignment))]
    internal class OperationAssignment : IOperationAssignment
    {
        #region Dependencies

        public IAssignStepFactory AssignStepFactory { get; set; }

        public IOperationLoggerProvider LoggerProvider { get; set; }

        #endregion

        #region Fields

        private IOperationAssignStep _productAssignStep;
        private IOperationAssignStep _partsAssignStep;
        private IOperationAssignStep _recipeAssignStep;
        private IOperationAssignStep _userAssignStep;
        private IOperationAssignStep _documentAssignStep;
        private IOperationAssignStep _validateAssignStep;

        #endregion

        public void Start()
        {
            _productAssignStep = AssignStepFactory.Create(nameof(ProductAssignStep));
            _productAssignStep.Start();

            _partsAssignStep = AssignStepFactory.Create(nameof(PartsAssignStep));
            _partsAssignStep.Start();

            _recipeAssignStep = AssignStepFactory.Create(nameof(RecipeAssignStep));
            _recipeAssignStep.Start();

            _documentAssignStep = AssignStepFactory.Create(nameof(DocumentAssignStep));
            _documentAssignStep.Start();

            _userAssignStep = AssignStepFactory.Create(nameof(UserAssignStep));
            _userAssignStep.Start();

            _validateAssignStep = AssignStepFactory.Create(nameof(ValidateAssignStep));
            _validateAssignStep.Start();
        }

        public void Stop()
        {
            _productAssignStep?.Stop();
            _partsAssignStep?.Stop();
            _recipeAssignStep?.Stop();
            _documentAssignStep?.Stop();
            _userAssignStep?.Stop();
            _validateAssignStep?.Stop();
        }

        public void Assign(IOperationData operationData) =>
            Task.Run(() => RunAssignment(operationData));

        public void Reassign(IOperationData operationData) =>
            Task.Run(() => RunReassignment(operationData));

        private async Task RunAssignment(IOperationData operationData)
        {
            var result = false;
            var operationLogger = LoggerProvider.GetLogger(operationData);

            operationLogger.Log(LogLevel.Information, Strings.OperationAssignment_Started);
            try
            {
                var assignSteps = new[]
                {
                    _productAssignStep,
                    _partsAssignStep,
                    _recipeAssignStep,
                    _documentAssignStep,
                    _validateAssignStep
                };

                // Process residual steps
                foreach (var createStep in assignSteps)
                {
                    result = await createStep.AssignStep(operationData, operationLogger);
                    if (!result)
                        break;

                    operationLogger.Log(LogLevel.Information, Strings.OperationAssignment_Successful, createStep.GetType().Name); 
                }
            }
            catch (Exception ex)
            {
                operationLogger.LogException(LogLevel.Error, ex, Strings.OperationAssignment_Exception_Assign);
                result = false;
            }
            finally
            {
                operationData.AssignCompleted(result);
            }
        }

        private async Task RunReassignment(IOperationData operationData)
        {
            var result = false;
            var operationLogger = LoggerProvider.GetLogger(operationData);

            try
            {
                result = await _recipeAssignStep.AssignStep(operationData, operationLogger);
                operationLogger.Log(LogLevel.Information, Strings.OperationAssignment_RecipeReassigned, operationData.Operation.Recipes.FirstOrDefault());
            }
            catch (Exception ex)
            {
                operationLogger.LogException(LogLevel.Error, ex, Strings.OperationAssignment_Exception_Assign);
                result = false;
            }
            finally
            {
                operationData.AssignCompleted(result);
            }
        }

        public async Task Restore(IOperationData operationData)
        {
            var restoreSteps = new[]
            {
                _productAssignStep,
                _partsAssignStep,
                _recipeAssignStep,
                _documentAssignStep,
                _userAssignStep
            };

            var operationLogger = LoggerProvider.GetLogger(operationData);

            // Process residual restore steps
            foreach (var restoreStep in restoreSteps)
            {
                var result = await restoreStep.RestoreStep(operationData, operationLogger);
                if (result)
                    continue;

                throw new InvalidOperationException(restoreStep.GetType().Name + " was failed");
            }
        }
    }
}
