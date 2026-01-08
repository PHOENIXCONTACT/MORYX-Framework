// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Documents;

namespace Moryx.Orders.Management
{
    internal class InternalOperation : Operation
    {
        private InternalOrder _order;
        private readonly List<Document> _documents;
        private readonly List<ProductPart> _parts;
        private readonly List<IProductRecipe> _recipes;

        public InternalOperation()
        {
            _recipes = new List<IProductRecipe>();
            base.Recipes = _recipes;

            _parts = new List<ProductPart>();
            base.Parts = _parts;

            _documents = new List<Document>();
            base.Documents = _documents;

            Progress = new InternalOperationProgress();
            base.Progress = Progress;

            base.Reports = Array.Empty<OperationReport>();
            base.Advices = Array.Empty<OperationAdvice>();
            base.Jobs = Array.Empty<Job>();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new InternalOrder Order
        {
            get => _order;
            set => base.Order = _order = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new OperationCreationContext CreationContext
        {
            get => base.CreationContext;
            set => base.CreationContext = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new Guid Identifier
        {
            get => base.Identifier;
            set => base.Identifier = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new int TotalAmount
        {
            get => base.TotalAmount;
            set => base.TotalAmount = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new int TargetAmount
        {
            get => base.TargetAmount;
            set => base.TargetAmount = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Number
        {
            get => base.Number;
            set => base.Number = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new int SortOrder
        {
            get => base.SortOrder;
            set => base.SortOrder = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new int OverDeliveryAmount
        {
            get => base.OverDeliveryAmount;
            set => base.OverDeliveryAmount = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new int UnderDeliveryAmount
        {
            get => base.UnderDeliveryAmount;
            set => base.UnderDeliveryAmount = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new DateTime PlannedStart
        {
            get => base.PlannedStart;
            set => base.PlannedStart = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new DateTime PlannedEnd
        {
            get => base.PlannedEnd;
            set => base.PlannedEnd = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new DateTime? Start
        {
            get => base.Start;
            set => base.Start = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new DateTime? End
        {
            get => base.End;
            set => base.End = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new double TargetCycleTime
        {
            get => base.TargetCycleTime;
            set => base.TargetCycleTime = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string TargetStock
        {
            get => base.TargetStock;
            set => base.TargetStock = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string Unit
        {
            get => base.Unit;
            set => base.Unit = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new ProductType Product
        {
            get => base.Product;
            set => base.Product = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new InternalOperationProgress Progress { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new OperationStateClassification State
        {
            get => base.State;
            set => base.State = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new string StateDisplayName
        {
            get => base.StateDisplayName;
            set => base.StateDisplayName = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IOperationSource Source
        {
            get => base.Source;
            set => base.Source = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IList<IProductRecipe> Recipes
        {
            get => _recipes;
            set
            {
                _recipes.Clear();
                _recipes.AddRange(value);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IList<ProductPart> Parts
        {
            get => _parts;
            set
            {
                _parts.Clear();
                _parts.AddRange(value);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IList<Document> Documents
        {
            get => _documents;
            set
            {
                _documents.Clear();
                _documents.AddRange(value);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IReadOnlyList<OperationReport> Reports
        {
            get => base.Reports;
            set => base.Reports = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IReadOnlyList<OperationAdvice> Advices
        {
            get => base.Advices;
            set => base.Advices = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new IReadOnlyList<Job> Jobs
        {
            get => base.Jobs;
            set => base.Jobs = value;
        }
    }
}
