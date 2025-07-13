﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Media.Previews;
using Moryx.Threading;

namespace Moryx.Media.Server.Previews
{
    [Plugin(LifeCycle.Singleton, typeof(IPreviewService))]
    internal class PreviewService : IPreviewService
    {
        #region Dependency Injection

        public IModuleLogger Logger { get; set; }

        public IPreviewCreator[] Creators { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        #endregion

        #region Fields and Properties

        private readonly Queue<PreviewJob> _jobs = new();
        private readonly object _jobLock = new();

        #endregion

        public void QueuePreviewJob(PreviewJob job)
        {
            lock (_jobLock)
            {
                _jobs.Enqueue(job);
            }

            ParallelOperations.ExecuteParallel(PreviewCreationThread);
        }

        public void Start()
        {
        }

        public void Stop()
        {
            lock(_jobLock)
                _jobs.Clear();
        }

        private void PreviewCreationThread()
        {
            try
            {
                PreviewJob job;
                lock (_jobLock)
                {
                    if (_jobs.Any())
                        job = _jobs.Dequeue();
                    else
                        return;
                }

                var creator = PickCreator(job);
                if (creator != null)
                {
                    var result = creator.CreatePreview(job);
                    PreviewJobCompleted?.Invoke(job, result);
                }
                else
                {
                    PreviewJobCompleted?.Invoke(job, new PreviewJobResult
                    {
                        PreviewState = PreviewState.Unsupported,
                        Preview = null
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e, "Error in preview service thread.");
            }

            lock (_jobLock)
            {
                if (_jobs.Any())
                    ParallelOperations.ExecuteParallel(PreviewCreationThread);
            }
        }

        private IPreviewCreator PickCreator(PreviewJob job)
        {
            return Creators.FirstOrDefault(c => c.CanCreate(job.Variant.MimeType));
        }

        public event PreviewCreationResultHandler PreviewJobCompleted;
    }
}

