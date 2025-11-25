// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Orders.Documents;
using Moryx.Serialization;
using Moryx.Users;

namespace Moryx.Orders.Endpoints
{
    internal static class Converter
    {
        internal static OperationAdvice FromModel(AdviceModel model, Operation operation)
        {
            if (!model.PartId.HasValue)
                return new OrderAdvice(model.ToteBoxNumber, model.Amount);
            else
                return new PickPartAdvice(operation.Parts.FirstOrDefault(p => p.Id == model.PartId), model.ToteBoxNumber);
        }

        internal static OperationReport FromModel(ReportModel model, IUserManagement userManagement)
        {
            var user = userManagement?.GetUser(model.UserIdentifier);
            return new OperationReport(model.ConfirmationType, model.SuccessCount, model.FailureCount, user)
            {
                Comment = model.Comment
            };
        }

        internal static ReportModel ToModel(OperationReport document)
        {
            return new ReportModel()
            {
                ConfirmationType = document.ConfirmationType,
                SuccessCount = document.SuccessCount,
                FailureCount = document.FailureCount,
                Comment = document.Comment,
                UserIdentifier = document.User.Identifier
            };
        }

        internal static AdviceModel ToModel(OperationAdvice advice)
        {
            return new AdviceModel()
            {
                Amount = (advice as OrderAdvice)?.Amount ?? 0,
                PartId = (advice as PickPartAdvice)?.Part.Id ?? 0,
                ToteBoxNumber = advice.ToteBoxNumber
            };
        }

        internal static DocumentModel ToModel(Document document)
        {
            return new DocumentModel
            {
                Number = document.Number,
                Revision = document.Revision,
                Type = document.Type,
                Description = document.Description,
                ContentType = document.ContentType,
                Identifier = document.Identifier,
                Source = document.Source
            };
        }

        internal static ProductPartModel ToModel(ProductPart productPart)
        {
            return new ProductPartModel
            {
                Id = productPart.Id,
                Name = productPart.Name,
                Identifier = productPart.Identity.Identifier,
                Quantity = productPart.Quantity,
                Unit = productPart.Unit,
                StagingIndicator = productPart.StagingIndicator,
                Classification = productPart.Classification
            };
        }

        internal static OperationLogMessageModel ToModel(OperationLogMessage message)
        {
            return new OperationLogMessageModel
            {
                LogLevel = message.LogLevel,
                Message = message.Message,
                Exception = message.Exception?.ToString() ?? string.Empty,
                TimeStamp = message.Timestamp
            };
        }

        internal static OperationModel ToModel(Operation operation)
        {
            var jobs = operation.Jobs.ToArray();
            var progress = operation.Progress;

            var operationModel = new OperationModel
            {
                Identifier = operation.Identifier,
                Name = operation.Name,
                Number = operation.Number,
                Unit = operation.Unit,
                TotalAmount = operation.TotalAmount,
                OverDeliveryAmount = operation.OverDeliveryAmount,
                UnderDeliveryAmount = operation.UnderDeliveryAmount,
                PlannedStart = operation.PlannedStart,
                PlannedEnd = operation.PlannedEnd,
                Start = operation.Start,
                End = operation.End,
                TargetCycleTime = operation.TargetCycleTime,
                ReportedSuccessCount = operation.ReportedSuccessCount(),
                ReportedFailureCount = operation.ReportedFailureCount(),

                SortOrder = operation.SortOrder,
                Classification = operation.State,
                StateDisplayName = operation.StateDisplayName,
                HasDocuments = operation.Documents.Any(),
                HasPartList = operation.Parts.Any(),
                JobIds = jobs.Select(j => j.Id).ToArray(),

                RunningCount = progress.RunningCount,
                SuccessCount = progress.SuccessCount,
                ScrapCount = progress.ScrapCount,
                PendingCount = progress.PendingCount,
                ProgressRunning = progress.ProgressRunning,
                ProgressSuccess = progress.ProgressSuccess,
                ProgressScrap = progress.ProgressScrap,
                ProgressPending = progress.ProgressPending,

                Order = operation.Order.Number,

                CanAssign = operation.FullState.HasFlag(OperationClassification.CanReload),
                CanBegin = operation.FullState.HasFlag(OperationClassification.CanBegin),
                CanInterrupt = operation.FullState.HasFlag(OperationClassification.CanInterrupt),
                CanReport = operation.FullState.HasFlag(OperationClassification.CanReport),
                CanAdvice = operation.FullState.HasFlag(OperationClassification.CanAdvice),
                IsFailed = operation.FullState.HasFlag(OperationClassification.Failed),
                IsAssigning = operation.FullState.HasFlag(OperationClassification.Assigning),
                IsCreated = operation.State > OperationClassification.Initial &&
                    !operation.FullState.HasFlag(OperationClassification.Failed),
                IsAborted = operation.FullState.HasFlag(OperationClassification.Aborted),
                IsAmountReached = operation.FullState.HasFlag(OperationClassification.IsAmountReached),

                RecipeIds = operation.Recipes.Select(r => r.Id).ToArray()
            };

            if (operation.Product?.Identity != null)
            {
                operationModel.ProductId = operation.Product.Id;
                var productIdentity = (ProductIdentity)operation.Product.Identity;
                operationModel.ProductIdentifier = productIdentity.Identifier;
                operationModel.ProductRevision = productIdentity.Revision;
                operationModel.ProductName = operation.Product.Name;
            }

            operationModel.OperationSource = EntryConvert.EncodeObject(operation.Source);

            var recipe = operation.Recipes.FirstOrDefault(r => r.Classification == RecipeClassification.Default)
                         ?? operation.Recipes.FirstOrDefault();

            operationModel.RecipeName = recipe != null ? recipe.Name : string.Empty;

            return operationModel;
        }

        internal static OperationRecipeModel ToModel(IProductRecipe recipe)
        {
            return new OperationRecipeModel
            {
                Id = recipe.Id,
                Name = recipe.Name
            };
        }
    }
}

