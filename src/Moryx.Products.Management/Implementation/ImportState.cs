// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Products.Management
{
    internal class ImportState
    {
        private readonly ProductManager _manager;

        public ImportState(ProductManager manager)
        {
            _manager = manager;
        }

        public Guid Session { get; set; }

        public bool Completed { get; set; }

        public string ErrorMsg { get; set; }

        public void TaskCompleted(Task<ProductImporterResult> task)
        {
            if (task.IsCompleted)
                _manager.HandleResult(task.Result);

            Completed = task.IsCompleted | task.IsFaulted;
            if (task.IsFaulted)
                ErrorMsg = task.Exception.Message;
        }
    }
}