// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using MimeTypes;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Media.Previews;
using Moryx.Media.Server.Previews;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Media.Server
{
    [Plugin(LifeCycle.Singleton, typeof(IContentManager))]
    internal class ContentManager : IContentManager
    {
        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// Collection for all iterating and index based accesses
        /// </summary>
        private readonly List<ContentDescriptor> _descriptors = new List<ContentDescriptor>();
        /// <summary>
        /// Dictionary for all guid based accesses
        /// </summary>
        private readonly Dictionary<Guid, ContentDescriptor> _descriptorMap = new Dictionary<Guid, ContentDescriptor>();

        private string _descriptorDirectory;

        #region Dependency Injection

        public ModuleConfig Config { get; set; }

        public IPreviewService PreviewService { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

        public ContentManager()
        {
            _jsonSettings = JsonSettings.Readable;
        }

        public void Start()
        {
            PreviewService.PreviewJobCompleted += OnPreviewJobCompleted;

            if (!Directory.Exists(Config.StoragePath))
                Directory.CreateDirectory(Config.StoragePath);

            _descriptorDirectory = Path.Combine(Config.StoragePath, "descriptors");
            if (!Directory.Exists(_descriptorDirectory))
                Directory.CreateDirectory(_descriptorDirectory);

            LoadDescriptors();

            EnqueuePreviews();
        }

        public void Stop()
        {
            PreviewService.PreviewJobCompleted -= OnPreviewJobCompleted;
            _descriptorMap.Clear();
            _descriptors.Clear();
        }

        public ContentDescriptor GetDescriptor(Guid contentId)
        {
            _descriptorMap.TryGetValue(contentId, out var descriptor);
            return descriptor;
        }

        public VariantDescriptor GetVariant(Guid contentId, string variantName)
        {
            var descriptor = GetDescriptor(contentId);
            return descriptor?.GetVariant(variantName);
        }

        public IReadOnlyList<ContentDescriptor> GetDescriptors()
        {
            return _descriptors;
        }

        public IReadOnlyList<ContentDescriptor> GetDescriptors(uint start, uint offset)
        {
            var descriptors = new List<ContentDescriptor>((int)offset);
            if (start > _descriptors.Count)
                throw new ArgumentOutOfRangeException(nameof(start), start,
                    $"Start value is greater than list count {_descriptors.Count}");

            for (var idx = start; idx < start + offset && idx < _descriptors.Count; idx++)
            {
                var descriptor = _descriptors[(int)idx];
                descriptors.Add(descriptor);
            }

            return descriptors.ToArray();
        }

        public Stream GetStream(FileDescriptor fileDescriptor)
        {
            if (string.IsNullOrEmpty(fileDescriptor.FileHash))
                return null;

            var variantFilePath = HashPath.FromHash(fileDescriptor.FileHash).FilePath(Config.StoragePath);
            return File.Exists(variantFilePath) ? new FileStream(variantFilePath, FileMode.Open, FileAccess.Read) : null;
        }

        public async Task<ContentAddingInfo> AddMaster(string fileName, Stream contentStream)
        {
            if (!contentStream.CanSeek)
                throw new NotSupportedException("Stream needs to be seekable");

            var hashPath = HashPath.FromStream(contentStream);

            // Check if master is existent
            var existing = GetVariantByHash(hashPath.Hash);

            // If exists, return here
            if (existing != null)
            {
                return new ContentAddingInfo
                {
                    Descriptor = existing.Item1,
                    Variant = existing.Item2,
                    Result = ContentAddingResult.AlreadyExists
                };
            }

            // Else create new content descriptor
            var descriptor = new ContentDescriptor(Guid.NewGuid())
            {
                Name = Path.GetFileNameWithoutExtension(fileName)
            };

            // Add master variant
            var variant = await AddContent(descriptor, MediaConstants.MasterName, fileName, hashPath, contentStream);

            lock (_descriptors)
            {
                _descriptors.Add(descriptor);
                _descriptorMap.Add(descriptor.Id, descriptor);
            }

            return new ContentAddingInfo
            {
                Descriptor = descriptor,
                Variant = variant,
                Result = ContentAddingResult.Ok
            };
        }

        public async Task<ContentAddingInfo> AddVariant(Guid contentId, string variantName, string fileName, Stream contentStream)
        {
            if (variantName == MediaConstants.MasterName)
                return new ContentAddingInfo { Descriptor = null, Result = ContentAddingResult.VariantNameReserved };

            var descriptor = GetDescriptor(contentId);
            var variant = descriptor?.GetVariant(variantName);
            if (variant != null)
                return new ContentAddingInfo { Descriptor = descriptor, Result = ContentAddingResult.AlreadyExists };

            var hash = HashPath.FromStream(contentStream);
            if (File.Exists(hash.FilePath(Config.StoragePath)))
            {
                var existing = GetVariantByHash(hash.Hash);
                if (existing != null)
                {
                    return new ContentAddingInfo
                    {
                        Descriptor = existing.Item1,
                        Variant = existing.Item2,
                        Result = ContentAddingResult.AlreadyExists
                    };
                }
            }

            variant = await AddContent(descriptor, variantName, fileName, hash, contentStream);
            return new ContentAddingInfo
            {
                Descriptor = descriptor,
                Variant = variant,
                Result = ContentAddingResult.Ok
            };
        }

        private async Task<VariantDescriptor> AddContent(ContentDescriptor descriptor, string variantName, string fileName, HashPath hashPath, Stream contentStream)
        {
            var fileExtension = Path.GetExtension(fileName);
            var variant = new VariantDescriptor
            {
                Name = variantName,
                CreationDate = DateTime.UtcNow,
                MimeType = MimeTypeMap.GetMimeType(fileExtension),
                FileHash = hashPath.Hash,
                Size = contentStream.Length,
                Extension = fileExtension
            };

            await hashPath.SaveStream(contentStream, Config.StoragePath, Logger);
            descriptor.Variants.Add(variant);

            await SaveDescriptorToFile(descriptor);

            PreviewService.QueuePreviewJob(new PreviewJob
            {
                ContentDescriptor = descriptor,
                Variant = variant,
                SourcePath = hashPath.FilePath(Config.StoragePath)
            });

            return variant;
        }

        /// <summary>
        /// Use nested for loops to be enumeration change tolerant
        /// </summary>
        private Tuple<ContentDescriptor, VariantDescriptor> GetVariantByHash(string hash)
        {
            VariantDescriptor existingVariant = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            // Find variant, use for loop to avoid enumeration changes
            for (int i = 0; i < _descriptors.Count; i++)
            {
                var currentDescriptor = _descriptors[i];
                // ReSharper disable once ForCanBeConvertedToForeach
                for (int j = 0; j < currentDescriptor.Variants.Count; j++)
                {
                    var currentVariant = currentDescriptor.Variants[j];
                    if (currentVariant.FileHash == hash)
                    {
                        existingVariant = currentVariant;
                        break;
                    }
                }

                if (existingVariant != null)
                    return new Tuple<ContentDescriptor, VariantDescriptor>(currentDescriptor, existingVariant);
            }

            return null;
        }

        private bool Remove(ContentDescriptor contentDescriptor)
        {
            var variants = contentDescriptor.Variants.ToArray();
            foreach (var variantDescriptor in variants)
                Remove(contentDescriptor, variantDescriptor);

            var descriptorFile = Path.Combine(_descriptorDirectory, contentDescriptor.Id.ToString());
            if (File.Exists(descriptorFile))
                File.Delete(descriptorFile);
            var hashPath = HashPath.FromHash(contentDescriptor.FileHash);
            hashPath.DeleteFile(Config.StoragePath, Logger);

            return _descriptors.Remove(contentDescriptor) & _descriptorMap.Remove(contentDescriptor.Id);
        }

        private bool Remove(ContentDescriptor contentDescriptor, VariantDescriptor variantDescriptor)
        {
            var hashPath = HashPath.FromHash(variantDescriptor.FileHash);
            hashPath.DeleteFile(Config.StoragePath, Logger);

            if (variantDescriptor.Preview.State == PreviewState.Created)
                RemovePreview(variantDescriptor);

            contentDescriptor.Variants.Remove(variantDescriptor);
            if (contentDescriptor.Variants.Count > 0)
                SaveDescriptorToFile(contentDescriptor);
            return true;
        }

        public bool RemoveContent(Guid contentId, string variantName)
        {
            var isMaster = variantName == MediaConstants.MasterName;
            var descriptor = GetDescriptor(contentId);
            var variant = descriptor?.GetVariant(variantName);

            if (variant == null)
                return false;

            return isMaster ? Remove(descriptor) : Remove(descriptor, variant);
        }

        private async Task AddPreview(VariantDescriptor variantDescriptor, Stream previewStream)
        {
            if (!string.IsNullOrWhiteSpace(variantDescriptor.Preview.FileHash))
                RemovePreview(variantDescriptor);

            var hashPath = HashPath.FromStream(previewStream);
            await hashPath.SaveStream(previewStream, Config.StoragePath, Logger);

            variantDescriptor.Preview.State = PreviewState.Created;
            variantDescriptor.Preview.FileHash = hashPath.Hash;
        }

        private void RemovePreview(VariantDescriptor variantDescriptor)
        {
            var hashPath = HashPath.FromHash(variantDescriptor.Preview.FileHash);
            hashPath.DeleteFile(Config.StoragePath, Logger);

            variantDescriptor.Preview = new PreviewDescriptor
            {
                FileHash = string.Empty,
                State = PreviewState.Removed
            };
        }

        private void OnPreviewJobCompleted(PreviewJob job, PreviewJobResult result) =>
            Task.Run(() => HandleCompletedPreview(job, result));

        private async Task HandleCompletedPreview(PreviewJob job, PreviewJobResult result)
        {
            var variant = job.Variant;
            var preview = variant.Preview;

            try
            {
                if (result.PreviewState == PreviewState.Created)
                    await AddPreview(variant, result.Preview);

                result.Dispose();

                // Set state AFTER disposing the stream to avoid race conditions
                preview.State = result.PreviewState;
                preview.MimeType = result.MimeType;
                preview.Size = result.Size;

                await SaveDescriptorToFile(job.ContentDescriptor);
            }
            catch (Exception e)
            {
                // The possible duplicate disposal is allowed
                result.Dispose();

                Logger.Log(LogLevel.Error, e,
                    $"Cannot apply preview for {job.SourcePath} (Content: {job.ContentDescriptor.Id})");
            }
        }

        private async Task SaveDescriptorToFile(ContentDescriptor descriptor)
        {
            using (var stream = new MemoryStream())
            {
                // Create json for descriptor and hash
                var json = JsonConvert.SerializeObject(descriptor, _jsonSettings);
                // Write json to stream
                var writer = new StreamWriter(stream);
                writer.Write(json);
                await writer.FlushAsync();

                // Create new file hash and save the file
                var file = HashPath.FromStream(stream);

                // Initiate disk write
                await file.SaveStream(stream, Config.StoragePath, Logger);

                // Update the guid file
                var guidPath = Path.Combine(_descriptorDirectory, descriptor.Id.ToString());
                File.WriteAllText(guidPath, file.Hash);

                // Remove old file
                if (!string.IsNullOrEmpty(descriptor.FileHash) && descriptor.FileHash != file.Hash)
                    HashPath.FromHash(descriptor.FileHash).DeleteFile(Config.StoragePath, Logger);

                // Overwrite hash on object
                descriptor.FileHash = file.Hash;
            }
        }

        private void LoadDescriptors()
        {
            foreach (var file in Directory.EnumerateFiles(_descriptorDirectory))
            {
                try
                {
                    var guid = Path.GetFileName(file);
                    var hash = File.ReadAllText(file);

                    var contentPath = HashPath.FromHash(hash).FilePath(Config.StoragePath);
                    var content = File.ReadAllText(contentPath);

                    var descriptor = new ContentDescriptor(Guid.Parse(guid))
                    {
                        FileHash = hash
                    };
                    JsonConvert.PopulateObject(content, descriptor, _jsonSettings);

                    _descriptors.Add(descriptor);
                    _descriptorMap.Add(descriptor.Id, descriptor);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Warning, e, $"Cannot read descriptor: {file}");
                }
            }
        }

        private void EnqueuePreviews()
        {
            // Collect all descriptors which previews have not been created yet
            foreach (var descriptor in _descriptors)
            {
                var missingPreviews = descriptor.Variants.Where(v => v.Preview.State == PreviewState.Creating);
                foreach (var variant in missingPreviews)
                {
                    var sourcePath = HashPath.FromHash(variant.FileHash).FilePath(Config.StoragePath);
                    PreviewService.QueuePreviewJob(new PreviewJob
                    {
                        ContentDescriptor = descriptor,
                        SourcePath = sourcePath,
                        Variant = variant
                    });
                }
            }
        }

        public string[] GetSupportedFileTypes()
            => Config.SupportedFileTypes;

        public int FileSizeLimitInMb()
            => Config.MaxFileSizeInMb;
    }
}
