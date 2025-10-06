// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Notifications.Model;
using Moryx.Threading;
using Moryx.Tools;
using Microsoft.Extensions.Logging;

namespace Moryx.Notifications.Publisher
{
    [Plugin(LifeCycle.Singleton, typeof(INotificationManager))]
    internal class NotificationManager : INotificationManager
    {
        #region Dependencies

        /// <summary>
        /// Facade sources for notifications
        /// </summary>
        public INotificationSource[] Sources { get; set; }

        /// <summary>
        /// Array of <see cref="INotificationProcessor"/> to handle all published notifications
        /// </summary>
        public INotificationProcessor[] Processors { get; set; }

        /// <summary>
        /// Parallel Operations to detach restore from start
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Yes, this is a logger
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Model to acknowledge old notifications
        /// </summary>
        public IUnitOfWorkFactory<NotificationsContext> UnitOfWorkFactory { get; set; }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Collection of tasks which are running because of deactivation from notification sources
        /// </summary>
        private readonly ICollection<Task> _deactivationTasks = new List<Task>();

        /// <summary>
        /// Currently published notifications
        /// </summary>
        private readonly ICollection<NotificationMap> _current = new List<NotificationMap>();

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public void Start()
        {
            // Start processors before any NotificationSender was started
            Processors.ForEach(p => p.Start());

            // Restore notifications by processors
            var current = Sources.Where(s => s.IsActivated).SelectMany(s => s.GetPublished().Select(n => new NotificationMap
            {
                Notification = n,
                Source = s,
                State = NotificationState.Created
            })).ToArray();

            // Sync with database
            SyncWithDatabase(current.Select(c => c.Notification).ToArray());

            // Add to private list
            lock (_current)
                _current.AddRange(current);

            // Register to all sources
            foreach (var source in Sources)
            {
                source.Published += OnNotificationOccurred;
                source.Acknowledged += OnNotificationAcknowledged;

                if (source.IsActivated)
                    source.Sync();

                source.StateChanged += OnSourceStateChanged;
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            // Unregister from all sources
            foreach (var source in Sources)
            {
                source.Published -= OnNotificationOccurred;
                source.Acknowledged -= OnNotificationAcknowledged;
                source.StateChanged -= OnSourceStateChanged;
            }

            // Wait for all deactivation tasks
            Task.WaitAll(_deactivationTasks.ToArray());

            // Stop processors
            Processors.ForEach(p => p.Stop());
        }

        public void Dispose()
        {
        }

        private void OnSourceStateChanged(object sender, bool activated)
        {
            if (activated)
            {
                // only deactivated is implemented
                return;
            }

            var source = (INotificationSource)sender;

            // Run in new task and add
            var task = Task.Run(delegate
            {
                try
                {
                    AcknowledgeFromDeactivatedSource(source);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Warning, e, "AcknowledgeFromDeactivatedSource failed");
                }
            });

            // Add to running tasks
            lock (_deactivationTasks)
                _deactivationTasks.Add(task);

            // If task finishes, remove from running tasks
            task.ContinueWith(delegate (Task target)
            {
                lock (_deactivationTasks)
                    _deactivationTasks.Remove(target);
            });
        }

        private void AcknowledgeFromDeactivatedSource(INotificationSource source)
        {
            // Load published
            NotificationMap[] current;
            lock (_current)
                current = _current.Where(m => m.Source == source).ToArray();

            if (current.Length <= 0)
                return;

            // Acknowledge in database
            using (var uow = UnitOfWorkFactory.Create())
            {
                var notificationRepo = uow.GetRepository<INotificationEntityRepository>();

                var entities = (from entity in notificationRepo.Linq
                                where !entity.Acknowledged.HasValue && entity.Source == source.Name
                                select entity).ToArray();

                var timeStamp = DateTime.UtcNow;
                if (entities.Length > 0)
                {
                    entities.ForEach(entity => entity.Acknowledged = timeStamp);
                    uow.SaveChanges();
                }
            }

            // Remove from private list
            lock (_current)
                _current.RemoveRange(current);

            // Log and raise
            foreach (var notification in current.Select(c => c.Notification))
            {
                Logger.Log(LogLevel.Information, "{0} was acknowledged by {1}. Title: {2}", notification.GetType().Name,
                    nameof(NotificationManager), notification.Title);

                RaiseNotificationAcknowledged(notification);
            }
        }

        /// <inheritdoc />
        public Notification[] GetAll()
        {
            lock (_current)
                return _current.Select(m => m.Notification).ToArray();
        }

        /// <inheritdoc />
        public void Acknowledge(Guid identifier)
        {
            NotificationMap map;
            lock (_current)
                map = _current.SingleOrDefault(m => m.Notification.Identifier == identifier);

            // Acknowledge on Source
            map?.Source.Acknowledge(map.Notification);
        }

        private void OnNotificationOccurred(object sender, Notification notification)
        {
            var map = new NotificationMap
            {
                Source = (INotificationSource)sender,
                Notification = notification,
                State = NotificationState.Pending
            };

            lock (_current)
                _current.Add(map);

            // Change to a different thread so the caller doesn't have to wait, or breaks down if an exception occurs...
            ParallelOperations.ExecuteParallel(ProcessNotification, map);
        }

        private void ProcessNotification(NotificationMap map)
        {
            var processor = Processors.Single(p => p.CanProcess(map.Notification));
            var processResult = processor.Process(map.Notification);

            var notification = map.Notification;
            var source = map.Source;

            // Tell the sender that the notification was processed
            source.PublishProcessed(map.Notification);

            // Log and raise
            Logger.Log(LogLevel.Debug, "{0} was published by {1}. Title: {2}", notification.GetType().Name,
                source.Name, notification.Title);

            bool isPendingAck;
            lock (map)
            {
                isPendingAck = map.State == NotificationState.PendingAcknowledge;
                map.State = NotificationState.Created;
            }

            // Raise event for processed notifications
            if (processResult == NotificationProcessorResult.Processed)
                RaiseNotificationPublished(notification);

            // Check if notification was acknowledged
            if (isPendingAck)
                ProcessAcknowledgement(map);
        }

        private void OnNotificationAcknowledged(object sender, Notification notification)
        {
            // Change to a different thread so the caller doesn't have to wait, or breaks down if an exception occurs...
            ParallelOperations.ExecuteParallel(() => ProcessAcknowledgement((INotificationSource)sender, notification));
        }

        private void ProcessAcknowledgement(INotificationSource source, Notification notification)
        {
            NotificationMap map;
            lock (_current)
                map = _current.Single(m => m.Notification == notification);

            ProcessAcknowledgement(map);
        }

        private void ProcessAcknowledgement(NotificationMap map)
        {
            // Check state, if pending, switch to pending acknowledge if acknowledged or pending acknowledged, return
            lock (map)
            {
                switch (map.State)
                {
                    case NotificationState.Pending:
                        map.State = NotificationState.PendingAcknowledge;
                        return;
                    case NotificationState.Acknowledged:
                    case NotificationState.PendingAcknowledge:
                        return;
                }
            }

            var notification = map.Notification;
            var source = map.Source;

            var processor = Processors.Single(p => p.CanProcess(notification));
            var ackResult = processor.Acknowledge(notification);

            // Tell the sender that the notification was processed
            source.AcknowledgeProcessed(notification);

            // Remove from private list
            lock (_current)
                _current.Remove(map);

            // Log and raise
            Logger.Log(LogLevel.Information, "{0} was acknowledged by {1}. Title: {2}", notification.GetType().Name,
                source.Name, notification.Title);

            // Raise event for acknowledged notifications
            if (ackResult == NotificationProcessorResult.Processed)
                RaiseNotificationAcknowledged(notification);
        }

        private void SyncWithDatabase(IReadOnlyList<Notification> existing)
        {
            using var uow = UnitOfWorkFactory.Create();
            var notificationRepo = uow.GetRepository<INotificationEntityRepository>();
            var entities = (from entity in notificationRepo.Linq
                            where !entity.Acknowledged.HasValue
                            select entity).ToArray();

            // If entity exists which is not existing anymore, we acknowledge it
            var acknowledged = entities.Where(entity => existing.All(n => n.Identifier != entity.Identifier)).ToArray();
            if (acknowledged.Length <= 0)
                return;

            acknowledged.ForEach(a => a.Acknowledged = DateTime.UtcNow);
            uow.SaveChanges();
        }

        /// <inheritdoc />
        public event EventHandler<Notification> Published;

        private void RaiseNotificationPublished(Notification notification)
            => Published?.Invoke(this, notification);

        /// <inheritdoc />
        public event EventHandler<Notification> Acknowledged;

        private void RaiseNotificationAcknowledged(Notification notification)
            => Acknowledged?.Invoke(this, notification);

        internal class NotificationMap
        {
            public INotificationSource Source { get; set; }

            public Notification Notification { get; set; }

            public NotificationState State { get; set; }
        }

        internal enum NotificationState
        {
            Pending,
            Created,
            PendingAcknowledge,
            Acknowledged,
        }
    }
}
