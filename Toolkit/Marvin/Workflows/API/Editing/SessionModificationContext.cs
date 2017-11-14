using System.Collections.Generic;
using System.Linq;
using Marvin.Model;

namespace Marvin.Workflows
{
    /// <summary>
    /// Internal helper class for fluent API to keep <see cref="SessionModification"/> clean
    /// </summary>
    internal class SessionModificationContext
    {
        /// <summary>
        /// Operation the user performed
        /// </summary>
        private readonly UserOperation _operation;
        /// <summary>
        /// Hold all steps that were modified
        /// </summary>
        private readonly IList<ModifiedStep> _modifiedSteps = new List<ModifiedStep>(); 
        /// <summary>
        /// Hold all connectors that were modified
        /// </summary>
        private readonly IList<ModifiedConnector> _modifiedConnectors = new List<ModifiedConnector>();

        public SessionModificationContext(UserOperation operation)
        {
            _operation = operation;
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Added(WorkplanStep step)
        {
            return AddStep(step, ModificationType.Insert);
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Added(ConnectorDto connector, ConnectionPoint source, ConnectionPoint target)
        {
            return AddConnector(connector, ModificationType.Insert, source, target);
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Updated(WorkplanStep step)
        {
            return AddStep(step, ModificationType.Update);
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Updated(ConnectorDto connector, ConnectionPoint source, ConnectionPoint target)
        {
            return AddConnector(connector, ModificationType.Update, source, target);
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Deleted(WorkplanStep step)
        {
            return AddStep(step, ModificationType.Delete);
        }

        /// <summary>
        /// Step was added to the session
        /// </summary>
        public SessionModificationContext Deleted(ConnectorDto connector)
        {
            return AddConnector(connector, ModificationType.Delete, new ConnectionPoint(), new ConnectionPoint());
        }

        private SessionModificationContext AddStep(WorkplanStep step, ModificationType type)
        {
            _modifiedSteps.Add(new ModifiedStep
            {
                Data = step,
                Modification = type
            });
            return this;
        }

        private SessionModificationContext AddConnector(ConnectorDto connector, ModificationType type, ConnectionPoint source, ConnectionPoint target)
        {
            _modifiedConnectors.Add(new ModifiedConnector
            {
                Data = connector,
                Modification = type,
                Source = source,
                Target = target
            });
            return this;
        }

        /// <summary>
        /// Extract session modification from context
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Session modification publish to listeners</returns>
        public static implicit operator SessionModification(SessionModificationContext context)
        {
            return context == null ? null : new SessionModification
            {
                Operation = context._operation,
                StepModifications = context._modifiedSteps.ToArray(),
                ConnectorModifications = context._modifiedConnectors.ToArray()
            };
        }
    }
}