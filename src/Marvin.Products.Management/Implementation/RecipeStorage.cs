using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.AbstractionLayer.Recipes;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Tools;
using Marvin.Workflows;
using Marvin.Workflows.WorkplanSteps;
using Newtonsoft.Json;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Static helper to load recipes and workplans
    /// </summary>
    public static class RecipeStorage
    {
        /// <summary>
        /// Copy properties to recipe
        /// </summary>
        /// <param name="recipeEntity"></param>
        /// <param name="productRecipe"></param>
        public static void CopyToRecipe(ProductRecipeEntity recipeEntity, IProductRecipe productRecipe)
        {
            productRecipe.Id = recipeEntity.Id;
            productRecipe.Name = recipeEntity.Name;
            productRecipe.Classification = (RecipeClassification)recipeEntity.Classification;
            productRecipe.Revision = recipeEntity.Revision;
            productRecipe.State = (RecipeState)recipeEntity.State;
            productRecipe.Product = new ProductTypeReference(recipeEntity.ProductId);

            var workplanRecipe = productRecipe as IWorkplanRecipe;
            if (workplanRecipe != null)
                workplanRecipe.Workplan = LoadWorkplan(recipeEntity.Workplan);
        }

        /// <summary>
        /// Save recipe to given <see cref="IUnitOfWork"/>
        /// </summary>
        public static ProductRecipeEntity SaveRecipe(IUnitOfWork uow, IProductRecipe recipe)
        {
            var entity = uow.GetEntity<ProductRecipeEntity>(recipe);

            entity.Type = recipe.GetType().Name;
            entity.Revision = recipe.Revision;
            entity.Name = recipe.Name;
            entity.State = (int)recipe.State;
            entity.Classification = (int)recipe.Classification;
            entity.ProductId = recipe.Product.Id;

            var workplanRecipe = recipe as IWorkplanRecipe;
            if (workplanRecipe != null)
                entity.WorkplanId = workplanRecipe.Workplan.Id;

            return entity;
        }

        /// <summary>
        /// Loads a workplan from database
        /// </summary>
        public static Workplan LoadWorkplan(IUnitOfWork uow, long id)
        {
            var workplan = uow.GetRepository<IWorkplanEntityRepository>().GetByKey(id);
            return workplan == null ? null : LoadWorkplan(workplan);
        }

        /// <summary>
        /// Convert a workplan entity to a <see cref="Workplan"/> instance
        /// </summary>
        private static Workplan LoadWorkplan(WorkplanEntity workplanEntity)
        {
            // Load connectors and steps
            var connectors = LoadConnectors(workplanEntity);
            var steps = LoadSteps(workplanEntity, connectors);

            // Restore workplan and its properties
            var workplan = Workplan.Restore(connectors.Values.ToList(), steps);
            workplan.Id = workplanEntity.Id;
            workplan.Name = workplanEntity.Name;
            workplan.Version = workplanEntity.Version;
            workplan.State = (WorkplanState)workplanEntity.State;
            workplan.MaxElementId = workplanEntity.MaxElementId;

            return workplan;
        }

        /// <summary>
        /// Load connectors and return them as a map EntityId => IConnector instance
        /// </summary>
        private static Dictionary<long, IConnector> LoadConnectors(WorkplanEntity workplan)
        {
            return workplan.Connectors.ToDictionary(
                connector => connector.Id,
                connector => (IConnector)new Connector
                {
                    Id = connector.ConnectorId,
                    Name = connector.Name,
                    Classification = (NodeClassification)connector.Classification
                });
        }

        /// <summary>
        /// Restore <see cref="IWorkplanStep"/> instances from entities
        /// </summary>
        private static List<IWorkplanStep> LoadSteps(WorkplanEntity workplan, IDictionary<long, IConnector> connectors)
        {
            var steps = new List<IWorkplanStep>();

            foreach (var stepEntity in workplan.Steps)
            {
                var fullName = $"{stepEntity.NameSpace}.{stepEntity.Classname}, {stepEntity.Assembly}";
                var type = Type.GetType(fullName, true, true);
                // Create instance using public and private constructors
                var step = (WorkplanStepBase)Activator.CreateInstance(type, true);
                step.Id = stepEntity.StepId;

                // Restore output descriptions
                step.OutputDescriptions = new OutputDescription[stepEntity.OutputDescriptions.Count];
                for (int index = 0; index < step.OutputDescriptions.Length; index++)
                {
                    var descriptionEntity = stepEntity.OutputDescriptions.First(ode => ode.Index == index);
                    var description = new OutputDescription
                    {
                        OutputType = (OutputType)descriptionEntity.OutputType,
                        Name = descriptionEntity.Name,
                        MappingValue = descriptionEntity.MappingValue
                    };
                    step.OutputDescriptions[index] = description;
                }

                // Restore parameters from JSON
                var taskStep = step as ITaskStep<IParameters>;
                if (taskStep != null)
                {
                    JsonConvert.PopulateObject(stepEntity.Parameters, taskStep.Parameters);
                }

                // Restore Subworkplan if necessary
                var subworkplanStep = step as ISubworkplanStep;
                if (subworkplanStep != null)
                {
                    subworkplanStep.Workplan = LoadWorkplan(stepEntity.SubWorkplan);
                }

                // Link inputs and outputs
                step.Inputs = RestoreReferences(stepEntity, ConnectorRole.Input, connectors);
                step.Outputs = RestoreReferences(stepEntity, ConnectorRole.Output, connectors);

                steps.Add(step);
            }

            return steps;
        }

        /// <summary>
        /// Restore either <see cref="IWorkplanStep.Inputs"/> or <see cref="IWorkplanStep.Outputs"/>.
        /// </summary>
        private static IConnector[] RestoreReferences(StepEntity stepEntity, ConnectorRole role, IDictionary<long, IConnector> connectors)
        {
            var referenceEntities = stepEntity.Connectors.Where(c => c.Role == role).ToList();

            var references = new IConnector[referenceEntities.Count];
            for (int index = 0; index < referenceEntities.Count; index++)
            {
                var referenceEntity = referenceEntities.First(f => f.Index == index);
                if (referenceEntity.ConnectorId.HasValue)
                    references[index] = connectors[referenceEntity.ConnectorId.Value];
            }

            return references;
        }


        /// <summary>
        /// Save a workplan to the database
        /// </summary>
        public static WorkplanEntity SaveWorkplan(IUnitOfWork uow, Workplan workplan)
        {
            var workplanRepo = uow.GetRepository<IWorkplanEntityRepository>();
            var referenceRepo = uow.GetRepository<IWorkplanReferenceRepository>();

            // Try to get the current object
            var workplanEntity = workplanRepo.GetByKey(workplan.Id);
            // If it is a new plan we need a new object
            if (workplanEntity == null)
            {
                workplanEntity = workplanRepo.Create(workplan.Name, workplan.Version, (int)workplan.State);
                EntityIdListener.Listen(workplanEntity, workplan);
            }
            // If it was modified or the version increased we create a new one and reference the old one
            else if (workplan.Version > workplanEntity.Version)
            {
                var reference = referenceRepo.Create((int)WorkplanReferenceType.NewVersion);
                reference.Source = workplanEntity;
                reference.Target = workplanEntity = workplanRepo.Create(workplan.Name, workplan.Version, (int)workplan.State);
                EntityIdListener.Listen(workplanEntity, workplan);
            }

            // Set properties of the workplan entity
            workplanEntity.Name = workplan.Name;
            workplanEntity.State = (int)workplan.State;
            workplanEntity.MaxElementId = workplan.MaxElementId;

            // Save connectors and steps
            var steps = SaveSteps(uow, workplanEntity, workplan);
            var connectors = SaveConnectors(uow, workplanEntity, workplan);
            LinkSteps(uow, workplan, steps, connectors);
            RemoveUnusedConnectors(uow, workplanEntity, workplan);

            return workplanEntity;
        }

        /// <summary>
        /// Save or update connectors of the workplan and return a map of StepId => Entity
        /// </summary>
        private static IDictionary<long, ConnectorEntity> SaveConnectors(IUnitOfWork uow, WorkplanEntity workplanEntity, Workplan workplan)
        {
            var connectorRepo = uow.GetRepository<IConnectorEntityRepository>();

            var connectorEntities = new Dictionary<long, ConnectorEntity>();
            foreach (var connector in workplan.Connectors)
            {
                var connectorEntity = workplanEntity.Connectors.FirstOrDefault(ce => ce.ConnectorId == connector.Id);
                if (connectorEntity == null)
                {
                    connectorEntity = connectorRepo.Create(connector.Id, (int)connector.Classification);
                    workplanEntity.Connectors.Add(connectorEntity);
                }
                connectorEntity.Name = connector.Name;
                connectorEntities[connector.Id] = connectorEntity;
            }

            return connectorEntities;
        }

        /// <summary>
        /// Save or update steps of the workplan and return a map of StepId => Entity
        /// </summary>
        private static IDictionary<long, StepEntity> SaveSteps(IUnitOfWork uow, WorkplanEntity workplanEntity, Workplan workplan)
        {
            var stepRepo = uow.GetRepository<IStepEntityRepository>();
            var descriptionRepo = uow.GetRepository<IOutputDescriptionEntityRepository>();
            var referenceRepo = uow.GetRepository<IConnectorReferenceRepository>();

            // Remove connectors, that are now longer used. We only use Created/Updated columns
            // and do not want the entities flagged as deleted
            var removedSteps = workplanEntity.Steps.Where(se => workplan.Steps.All(s => s.Id != se.StepId));
            foreach (var removedStep in removedSteps.ToList())
            {
                descriptionRepo.RemoveRange(removedStep.OutputDescriptions);
                referenceRepo.RemoveRange(removedStep.Connectors);
                stepRepo.Remove(removedStep, true);
            }

            var stepEntities = new Dictionary<long, StepEntity>();
            foreach (var step in workplan.Steps)
            {
                // Get or create entity
                var stepEntity = workplanEntity.Steps.FirstOrDefault(se => se.StepId == step.Id);
                if (stepEntity == null)
                {
                    var stepType = step.GetType();
                    var assemblyName = stepType.Assembly.GetName().Name;
                    stepEntity = stepRepo.Create(step.Id, step.Name, assemblyName, stepType.Namespace, stepType.Name);
                    workplanEntity.Steps.Add(stepEntity);
                }
                else
                    stepEntity.Name = step.Name;

                // Update all output descriptions
                for (int index = 0; index < step.OutputDescriptions.Length; index++)
                {
                    var description = step.OutputDescriptions[index];
                    var descriptionEntity = stepEntity.OutputDescriptions.FirstOrDefault(ode => ode.Index == index);
                    if (descriptionEntity == null)
                    {
                        descriptionEntity = descriptionRepo.Create(index, (int)description.OutputType, description.MappingValue);
                        stepEntity.OutputDescriptions.Add(descriptionEntity);
                    }
                    else
                    {
                        descriptionEntity.OutputType = (int)description.OutputType;
                        descriptionEntity.MappingValue = description.MappingValue;
                    }
                    descriptionEntity.Name = description.Name;
                }

                // Task steps need parameters
                var taskStep = step as ITaskStep<IParameters>;
                if (taskStep != null)
                {
                    stepEntity.Parameters = JsonConvert.SerializeObject(taskStep.Parameters);
                }

                // Subworkplan steps and need a reference to the workplan
                var subworkPlanStep = step as ISubworkplanStep;
                if (subworkPlanStep != null)
                {
                    stepEntity.SubWorkplanId = subworkPlanStep.WorkplanId;
                }

                stepEntities[step.Id] = stepEntity;
            }

            return stepEntities;
        }

        /// <summary>
        /// Link steps and connectors in the database
        /// </summary>
        private static void LinkSteps(IUnitOfWork uow, Workplan workplan, IDictionary<long, StepEntity> steps, IDictionary<long, ConnectorEntity> connectors)
        {
            var referenceRepo = uow.GetRepository<IConnectorReferenceRepository>();

            foreach (var step in workplan.Steps)
            {
                var stepEntity = steps[step.Id];

                // Update inputs and output
                UpdateConnectors(referenceRepo, stepEntity, step, ConnectorRole.Input, connectors);
                UpdateConnectors(referenceRepo, stepEntity, step, ConnectorRole.Output, connectors);
            }
        }

        /// <summary>
        /// Update either inputs or outputs in the database
        /// </summary>
        private static void UpdateConnectors(IConnectorReferenceRepository referenceRepo, StepEntity stepEntity, IWorkplanStep step, ConnectorRole role, IDictionary<long, ConnectorEntity> connectors)
        {
            // Update inputs first
            var connectorArray = role == ConnectorRole.Input ? step.Inputs : step.Outputs;
            for (int index = 0; index < connectorArray.Length; index++)
            {
                var connector = connectorArray[index];
                var connectorReference = stepEntity.Connectors.FirstOrDefault(c => c.Role == role && c.Index == index);
                if (connectorReference == null && connector != null)
                {
                    // Reference not yet stored in database
                    connectorReference = referenceRepo.Create(index, role);
                    connectorReference.Step = stepEntity;
                    connectorReference.Connector = connectors[connector.Id];
                }
                else if (connectorReference != null && connector != null)
                {
                    // Reference possible modified
                    connectorReference.Connector = connectors[connector.Id];
                }
                else if (connectorReference != null)
                {
                    // Reference removed
                    connectorReference.Connector = null;
                }
                else
                {
                    // Connector null and no entity exists
                    connectorReference = referenceRepo.Create(index, role);
                    connectorReference.Step = stepEntity;
                }
            }
        }

        /// <summary>
        /// Remove all unused connectors AFTER the new linking was applied
        /// </summary>
        private static void RemoveUnusedConnectors(IUnitOfWork uow, WorkplanEntity workplanEntity, Workplan workplan)
        {
            var connectorRepo = uow.GetRepository<IConnectorEntityRepository>();

            // Remove connectors, that are now longer part of the workplan
            var removedConnectors = workplanEntity.Connectors.Where(ce => workplan.Connectors.All(c => c.Id != ce.ConnectorId));
            connectorRepo.RemoveRange(removedConnectors);
        }
    }
}