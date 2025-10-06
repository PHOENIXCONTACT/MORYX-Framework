// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Orders.Management.Model;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Users;
using Newtonsoft.Json;

namespace Moryx.Orders.Management
{
    internal static class OperationStorage
    {
        public static OrderData LoadOrder(OrderEntity entity)
        {
            var orderData = new OrderData();
            ((IPersistentObject)orderData).Id = entity.Id;

            var order = orderData.Order;
            order.Number = entity.Number;
            order.Type = entity.Type;

            return orderData;
        }

        public static OrderEntity SaveOrder(IUnitOfWork uow, IOrderData orderData)
        {
            var orderEntity = uow.GetEntity<OrderEntity>((IPersistentObject)orderData);
            orderEntity.Number = orderData.Order.Number;
            orderEntity.Type = orderData.Order.Type;

            return orderEntity;
        }

        public static OperationEntity SaveOperation(IUnitOfWork uow, IOperationData operationData)
        {
            var operationRepo = uow.GetRepository<IOperationEntityRepository>();
            var entity = operationRepo.Linq.FirstOrDefault(o => o.Identifier.Equals(operationData.Identifier)) ??
                         operationRepo.Create();

            entity.AssignState = (int)operationData.AssignState;
            entity.State = operationData.State.Key;

            var operation = operationData.Operation;
            entity.Identifier = operation.Identifier;
            entity.TotalAmount = operation.TotalAmount;
            entity.Number = operation.Number;
            entity.Name = operation.Name;
            entity.OverDeliveryAmount = operation.OverDeliveryAmount;
            entity.UnderDeliveryAmount = operation.UnderDeliveryAmount;
            entity.TargetAmount = operation.TargetAmount;
            entity.PlannedStart = ConvertToUtc(operation.PlannedStart);
            entity.PlannedEnd = ConvertToUtc(operation.PlannedEnd);
            if (operation.Start != null)
                entity.ActualStart = ConvertToUtc((DateTime)operation.Start);
            if (operation.End != null)
                entity.ActualEnd = ConvertToUtc((DateTime)operation.End);
            entity.TargetCycleTime = operation.TargetCycleTime;
            entity.TargetStock = operation.TargetStock;
            entity.Unit = operation.Unit;

            // Source object
            var sourceJson = JsonConvert.SerializeObject(operation.Source, typeof(IOperationSource), JsonSettings.Minimal);
            entity.Source = sourceJson;

            // Product information
            entity.ProductId = operation.Product.Id;

            // Recipes
            var recipeReferenceRepo = uow.GetRepository<IOperationRecipeReferenceEntityRepository>();
            var dbRecipeReferences = entity.RecipeReferences.Select(r => r.RecipeId).ToArray();
            var currentRecipeReferences = operation.Recipes.Select(j => j.Id).ToArray();
            var missingRecipeReferences = currentRecipeReferences.Except(dbRecipeReferences);
            foreach (var missingRecipe in missingRecipeReferences)
            {
                var recipeReferenceEntity = recipeReferenceRepo.Create();
                recipeReferenceEntity.RecipeId = missingRecipe;
                recipeReferenceEntity.Operation = entity;
            }
            var recipeReferencesToRemove = dbRecipeReferences.Except(currentRecipeReferences);
            foreach (var recipeToRemove in recipeReferencesToRemove)
            {
                var recipeReferenceEntity = entity.RecipeReferences.FirstOrDefault(r => r.RecipeId == recipeToRemove);
                recipeReferenceRepo.Remove(recipeReferenceEntity);
            }

            // JobReferences
            var jobReferenceRepo = uow.GetRepository<IOperationJobReferenceEntityRepository>();
            var dbJobReferences = entity.JobReferences.Select(r => r.JobId).ToArray();
            var currentJobReferences = operation.Jobs.Select(j => j.Id).ToArray();
            var missingJobReferences = currentJobReferences.Except(dbJobReferences);
            foreach (var missingJob in missingJobReferences)
            {
                var jobReferenceEntity = jobReferenceRepo.Create();
                jobReferenceEntity.JobId = missingJob;
                jobReferenceEntity.Operation = entity;
            }

            // Reports
            foreach (var report in operation.Reports.Where(r => ((IPersistentObject)r).Id == 0))
            {
                var reportEntity = uow.CreateEntity<OperationReportEntity>(report);

                reportEntity.SuccessCount = report.SuccessCount;
                reportEntity.FailureCount = report.FailureCount;
                reportEntity.Comment = report.Comment;
                reportEntity.ReportedDate = report.ReportedDate.Kind == DateTimeKind.Utc ? 
                    report.ReportedDate : TimeZoneInfo.ConvertTimeToUtc(report.ReportedDate, TimeZoneInfo.Local);
                reportEntity.ConfirmationType = (int)report.ConfirmationType;
                reportEntity.UserIdentifier = report.User.Identifier;

                reportEntity.Operation = entity;
            }

            // Advices
            foreach (var advice in operation.Advices.Where(a => ((IPersistentObject)a).Id == 0))
            {
                var adviceEntity = uow.CreateEntity<OperationAdviceEntity>(advice);
                adviceEntity.ToteBoxNumber = advice.ToteBoxNumber;
                adviceEntity.Operation = entity;

                switch (advice)
                {
                    case OrderAdvice producedPartAdvice:
                        adviceEntity.Amount = producedPartAdvice.Amount;
                        break;
                    case PickPartAdvice pickPartAdvice:
                        adviceEntity.PartId = pickPartAdvice.Part.Id;
                        break;
                }
            }

            // Part list
            var partRepo = uow.GetRepository<IProductPartEntityRepository>();
            foreach (var part in operation.Parts.Where(p => ((IPersistentObject)p).Id == 0))
            {
                var partEntity = uow.CreateEntity<ProductPartEntity>(part);

                partEntity.Name = part.Name;
                partEntity.Number = part.Identity.Identifier;
                partEntity.Quantity = part.Quantity;
                partEntity.Unit = part.Unit;
                partEntity.StagingIndicator = (int)part.StagingIndicator;
                partEntity.Classification = (int)part.Classification;

                partEntity.Operation = entity;
            }

            return entity;
        }

        public static void RestoreOperationData(OperationData operationData, OperationEntity entity)
        {
            var operation = operationData.Operation;

            operationData.AssignState = (OperationAssignState)entity.AssignState;

            operation.Identifier = entity.Identifier;
            operation.TotalAmount = entity.TotalAmount;
            operation.Number = entity.Number;
            operation.Name = entity.Name;
            operation.OverDeliveryAmount = entity.OverDeliveryAmount;
            operation.UnderDeliveryAmount = entity.UnderDeliveryAmount;
            operation.TargetAmount = entity.TargetAmount;
            operation.PlannedStart = entity.PlannedStart;
            operation.PlannedEnd = entity.PlannedEnd;
            operation.Start = entity.ActualStart;
            operation.End = entity.ActualEnd;
            operation.TargetCycleTime = entity.TargetCycleTime;
            operation.TargetStock = entity.TargetStock;
            operation.Unit = entity.Unit;

            // Reload source
            operation.Source = JsonConvert.DeserializeObject<IOperationSource>(entity.Source, JsonSettings.Minimal) ??
                                             new NullOperationSource();

            // Product reference
            operation.Product = new ProductReference(entity.ProductId);

            // Add recipe references
            operation.Recipes.AddRange(entity.RecipeReferences.Select(r => new ProductRecipeReference(r.RecipeId)).ToList());

            // Add temporary job objects
            var jobs = entity.JobReferences.Select(j => new Job(null, 0) { Id = j.JobId }).ToArray();
            operationData.RestoreJobsUnsynchronized(jobs);

            // Load reports
            var reports = entity.Reports.Select(delegate (OperationReportEntity r)
            {
                var report = new OperationReport((ConfirmationType)r.ConfirmationType, r.SuccessCount, r.FailureCount, new UserReference(r.UserIdentifier))
                {
                    Comment = r.Comment,
                };
                ((IPersistentObject)report).Id = r.Id;
                return report;
            }).ToArray();

            operationData.RestoreReportsUnsynchronized(reports);

            // Load product part list
            var productParts = entity.ProductParts.Select(delegate (ProductPartEntity p)
            {
                var part = new ProductPart
                {
                    Name = p.Name,
                    Identity = new ProductIdentity(p.Number, 0),
                    Quantity = p.Quantity,
                    Unit = p.Unit,
                    StagingIndicator = (StagingIndicator)p.StagingIndicator,
                    Classification = (PartClassification)p.Classification
                };
                ((IPersistentObject)part).Id = p.Id;
                return part;
            });

            operation.Parts = productParts.ToArray();

            // load advices
            var adivces = entity.Advices.Select(delegate (OperationAdviceEntity a)
            {
                OperationAdvice advice;
                if (a.PartId.HasValue)
                {
                    var part = operation.Parts.Single(p => p.Id == a.PartId);
                    advice = new PickPartAdvice(part, a.ToteBoxNumber);
                }
                else
                {
                    advice = new OrderAdvice(a.ToteBoxNumber, a.Amount);
                }

                ((IPersistentObject)advice).Id = a.Id;
                return advice;
            }).ToArray();

            operationData.RestoreAdvicesUnsynchronized(adivces);
        }

        public static void RemoveOperation(IUnitOfWork uow, IOperationData operationData)
        {
            var operationRepo = uow.GetRepository<IOperationEntityRepository>();
            var entity = operationRepo.Linq.FirstOrDefault(o => o.Identifier.Equals(operationData.Identifier));

            if (entity == null) // This operation was not saved before
                return;

            operationRepo.Remove(entity);
        }

        private static DateTime ConvertToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
            throw new ArgumentException($"Provided {nameof(DateTime)} is neither UTC nor Local Time");
        }
    }
}
