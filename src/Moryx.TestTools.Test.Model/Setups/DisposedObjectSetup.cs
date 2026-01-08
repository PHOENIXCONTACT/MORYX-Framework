// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    [ModelSetup(typeof(TestModelContext))]
    public class DisposedObjectSetup : IModelSetup
    {
        public int SortOrder => 1;

        public string Name => "Raw SQL Setup";

        public string Description => "";

        public string SupportedFileRegex => string.Empty;

        public async Task ExecuteAsync(IUnitOfWork openContext, string setupData, CancellationToken cancellationToken)
        {
            // If the caller wouldn't await this method,
            // the DbContext might be disposed before it's
            // being used in here.
            // Adding a delay to ensure that the is actually
            // gone then.
            await Task.Delay(50, cancellationToken);
            await openContext.DbContext.Database.ExecuteSqlRawAsync("SELECT * FROM \"Cars\"", cancellationToken: cancellationToken);
        }
    }
}
