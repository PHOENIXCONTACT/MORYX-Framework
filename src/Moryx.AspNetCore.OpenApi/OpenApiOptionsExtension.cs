// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

public static class OpenApiOptionsExtension
{
    extension(OpenApiOptions options)
    {
        public OpenApiOptions ConfigureMoryxTransformers()
        {
            options.AddDocumentTransformer<MoryxOpenApiDocumentTransformer>();
            options.AddOperationTransformer<MoryxOperationIdOperationTransformer>();
            options.AddOperationTransformer<MoryxSseOperationTransformer>();
            return options;
        }
    }
}