using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Regex operation validation
    /// Validates the operation number and the amount
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IOperationValidation), Name = nameof(RegexOperationValidation))]
    [ExpectedConfig(typeof(RegexOperationValidationConfig))]
    public class RegexOperationValidation : IOperationValidation
    {
        /// <summary>
        /// A Regex created from operation validators configuration property <c>RegularExpression</c>
        /// </summary>
        private Regex _operationNumberRegex;

        private RegexOperationValidationConfig _config;

        /// <inheritdoc />
        public void Initialize(OperationValidationConfig config)
        {
            _config = (RegexOperationValidationConfig) config;
            _operationNumberRegex = new Regex(_config.RegularExpression);
        }

        /// <inheritdoc />
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <inheritdoc />
        public Task<bool> Validate(Operation operation, IOperationLogger operationLogger)
        {
            var numberResult = ValidateOperationNumber(operation.Number);
            var amountResult = ValidateOperationAmount(operation.TotalAmount);
            var recipesResult = operation.Recipes.Any() && operation.Recipes.All(ValidateRecipe);

            if (numberResult && amountResult && recipesResult)
                return Task.FromResult(true);

            if (!numberResult)
                operationLogger.Log(LogLevel.Error, Strings.RegexOperationValidation_Regex_Match_Failed, _config.RegularExpression);

            if (!amountResult)
                operationLogger.Log(LogLevel.Error, Strings.RegexOperationValidation_Wrong_Amount);

            if (!recipesResult)
                operationLogger.Log(LogLevel.Error, Strings.RegexOperationValidation_No_Valid_Recipe);

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public virtual bool ValidateCreationContext(OrderCreationContext orderContext)
        {
            foreach (var operation in orderContext.Operations)
            {
                var validationResult = ValidateOperationNumber(operation.Number) &&
                                       ValidateOperationAmount(operation.TotalAmount);

                if (validationResult == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// The Operation number must exists and has to match the regular expression.
        /// </summary>
        /// <param name="operationNumber">The operation number to validate</param>
        /// <returns><c>True</c> if the operation number is valid or <c>false</c> otherwise.</returns>
        private bool ValidateOperationNumber(string operationNumber)
        {
            return !string.IsNullOrWhiteSpace(operationNumber) && _operationNumberRegex.IsMatch(operationNumber);
        }

        /// <summary>
        /// The amount must be greater than 0 because than is no production possible
        /// </summary>
        /// <param name="amount">the current primary yield amount</param>
        /// <returns><c>True</c> if the amount is valid or <c>false</c> otherwise.</returns>
        private static bool ValidateOperationAmount(int amount)
        {
            return amount > 0;
        }

        /// <summary>
        /// The recipe must be at least not null
        /// </summary>
        /// <param name="recipe">The recipe at the operation</param>
        /// <returns><c>True</c> if the recipe is valid or <c>false</c> otherwise.</returns>
        private static bool ValidateRecipe(IRecipe recipe)
        {
            return recipe != null;
        }
    }
}