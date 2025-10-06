// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Orders.Assignment;
using Moryx.Orders.Documents;
using Moryx.Orders.Management.Properties;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Orders.Management.Assignment
{
    [Component(LifeCycle.Singleton, typeof(IOperationAssignStep), Name = nameof(DocumentAssignStep))]
    internal class DocumentAssignStep : IOperationAssignStep
    {
        #region Dependencies

        [UseChild(nameof(DocumentAssignStep))]
        public IModuleLogger Logger { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        public IDocumentLoader DocumentLoader { get; set; }

        #endregion

        #region Fields and Properties

        private const string MetadataExtension = ".json";
        private JsonSerializerSettings _jsonSettings;

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            _jsonSettings = JsonSettings.Minimal.Overwrite(s =>
            {
                s.TypeNameHandling = TypeNameHandling.All;
                return s;
            });
        }

        public void Stop()
        {
        }

        public async Task<bool> AssignStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;
            var identity = (ProductIdentity)operationData.Product.Identity;

            IReadOnlyList<Document> availableDocuments;
            try
            {
                availableDocuments = await DocumentLoader.Load(operation);
            }
            catch (Exception e)
            {
                operationLogger.LogException(LogLevel.Error, e, Strings.DocumentAssignStep_Documents_Loading_Failed);
                return false;
            }

            if (availableDocuments.Count == 0)
                return true;

            var fullPath = GetFullFolderPath(identity);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            var localDocuments = new List<Document>();
            foreach (var document in availableDocuments)
            {
                try
                {
                    var fileName = Path.Combine(fullPath, document.Identifier);
                    var metadata = Path.Combine(fullPath, document.Identifier + MetadataExtension);

                    // Store document to local path
                    var fsDocument = await StoreDocument(fileName, metadata, document);
                    localDocuments.Add(fsDocument);
                }
                catch (Exception e)
                {
                    operationLogger.LogException(LogLevel.Error, e, Strings.DocumentAssignStep_Document_Loading_Failed,
                        document.Identifier);
                    return false;
                }
            }

            operation.Documents = localDocuments.ToArray();
            return true;
        }

        private async Task<Document> StoreDocument(string fileName, string metadata, Document document)
        {
            using var stream = document.GetStream();

            var ext = GetDefaultExtension(document.ContentType);
            fileName += ext;

            // use FileMode.Create to not corrupt file caused by remaining bytes of a existing file (that was longer)
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            var localDocument = new FileSystemDocument(document.Number, document.Revision, fileName)
            {
                Description = document.Description,
                Type = document.Type,
                ContentType = document.ContentType
            };

            File.WriteAllText(metadata, JsonConvert.SerializeObject(localDocument, _jsonSettings));
            return localDocument;
        }

        /// <inheritdoc />
        public async Task<bool> RestoreStep(IOperationData operationData, IOperationLogger operationLogger)
        {
            var operation = operationData.Operation;
            var identity = (ProductIdentity)operationData.Product.Identity;
            var fullPath = GetFullFolderPath(identity);

            if (!Directory.Exists(fullPath))
            {
                operation.Documents = Array.Empty<Document>();
                return true;
            }

            Document[] documents;
            try
            {
                var jsonTasks = Directory.EnumerateFiles(fullPath, "*" + MetadataExtension).Select(f =>
                    Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Document>(File.ReadAllText(f), _jsonSettings)));
                documents = await Task.WhenAll(jsonTasks);
            }
            catch (Exception e)
            {
                operationLogger.LogException(LogLevel.Error, e, Strings.DocumentAssignStep_Restore_Documents_Failed);
                return false;
            }

            operation.Documents = documents;
            return true;
        }

        private string GetFullFolderPath(ProductIdentity identity)
        {
            return Path.Combine(ModuleConfig.Documents.DocumentsPath, identity.ToString().Replace('/', '_'));
        }

        private static string GetDefaultExtension(string mimeType)
        {
            var result = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
                var value = key?.GetValue("Extension", null);
                result = value?.ToString() ?? string.Empty;
            }
            return result;
        }
    }
}
