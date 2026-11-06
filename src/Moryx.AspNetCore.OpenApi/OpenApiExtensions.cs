// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace Moryx.AspNetCore.OpenApi;

public static class OpenApiExtensions
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
    extension(IHostEnvironment env)
    {
        /// <summary>
        /// Extension to make it fit in with IsDevelopment, IsProduction that are common in asp.net core
        /// </summary>
        /// <returns></returns>
        public bool IsOpenApiBuildTime()
        {
            if (string.Compare(env.EnvironmentName, "OpenApiBuildTime", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            // If the environment name is not set, fallback to detecting the entry point
            // Taken from https://github.com/dotnet/AspNetCore.Docs/blob/2c37585a425dd64e91015e60146031859ff52865/aspnetcore/fundamentals/openapi/samples/9.x/AspireApp1/AspireApp1.Web/Program.cs#L5
            // according to https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/fundamentals/openapi/aspnetcore-openapi.md#customize-runtime-behavior-during-build-time-document-generation
            return Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider";

        }
    }
    extension(IServiceProvider services)
    {
        /// <summary>
        /// Writes an openapi yaml spec for the specified document key
        /// to the output location identified by path and file name.
        /// </summary>
        /// <param name="fileName">Filename of the generated spec with extension i.e. 'framework-api.yaml'</param>
        /// <param name="path">Folder in which to place the spec. Can be infered from the command line args if it's running as part of the Microsoft.Extensions.ApiDescription.Server</param>
        /// <param name="documentKey">The document to generate the spec of. Defaults to the standard 'v1' document of the OpenApi integration</param>
        public async Task WriteYamlOpenApiSpec(string fileName, string path = null, string documentKey = "v1")
        {
            if (path == null)
            {
                var args = Environment.GetCommandLineArgs();
                var outputKey = args.IndexOf("--output");
                if (outputKey >= 0 && args.Length > outputKey + 1)
                {
                    path = args[outputKey + 1];
                }
                else
                {
                    path = Directory.GetCurrentDirectory();
                }

            }
            var provider = services.GetRequiredKeyedService<IOpenApiDocumentProvider>(documentKey);
            var document = await provider.GetOpenApiDocumentAsync();

            Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, fileName);
            using var textWriter = File.CreateText(filePath);
            var writer = new OpenApiYamlWriter(textWriter);

            document.SerializeAsV31(writer);
            await writer.FlushAsync();
            await textWriter.FlushAsync();
        }
    }
}
