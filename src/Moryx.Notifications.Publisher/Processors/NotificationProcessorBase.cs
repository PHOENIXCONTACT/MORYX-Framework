// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Notifications.Model;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Notifications.Publisher
{
    /// <summary>
    /// Base class for notifications with no special type information
    /// </summary>
    public abstract class NotificationProcessorBase<TNotification> : NotificationProcessorBase<TNotification, NotificationType>
        where TNotification : Notification, new()
    {
    }

    /// <summary>
    /// Base class for notification processors.
    /// The base class will handle all necessary processes to store, load and handle notifications of a specific type
    /// </summary>
    public abstract class NotificationProcessorBase<TNotification, TNotificationType> : INotificationProcessor
        where TNotification : Notification, new()
        where TNotificationType : class, INotificationType, new()
    {
        /// <summary>
        /// Private collection of types. Will be filled by start of the processor
        /// </summary>
        private List<TNotificationType> _types;

        /// <summary>
        /// Lock for the type collection
        /// </summary>
        private readonly ReaderWriterLockSlim _typeLock = new(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// List of available notification types within this processor
        /// </summary>
        protected IReadOnlyList<TNotificationType> Types { get; private set; }

        /// <summary>
        /// Base database model for the notifications
        /// </summary>
        public IUnitOfWorkFactory<NotificationsContext> UnitOfWorkFactory { get; set; }

        /// <inheritdoc />
        public virtual void Start()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var loadedTypes = LoadTypes(uow);
                _types = new List<TNotificationType>(loadedTypes);
            }

            Types = new NotificationTypeList<TNotificationType>(_types, _typeLock);
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            // Clear loaded types if initialized
            if (_types == null)
                return;

            _typeLock.EnterWriteLock();

            _types.Clear();

            _typeLock.ExitWriteLock();

            _types = null;
        }

        /// <inheritdoc />
        public virtual bool CanProcess(Notification notification)
        {
            return CanProcess(notification.GetType());
        }

        /// <inheritdoc />
        public virtual bool CanProcess(Type type)
        {
            return type == typeof(TNotification);
        }

        /// <inheritdoc />
        public virtual bool CanProcess(string type)
        {
            _typeLock.EnterReadLock();

            var isExistingType = _types.Any(t => t.Type.Equals(type));

            _typeLock.ExitReadLock();

            return isExistingType;
        }

        /// <inheritdoc />
        public NotificationProcessorResult Process(Notification notification)
        {
            var typedNotification = (TNotification) notification;
            var type = GetOrCreateType(typedNotification);

            var processResult = OnProcess(typedNotification, type);

            // If ignored, we do not have to save
            if (processResult == NotificationProcessorResult.Ignored)
                return processResult;

            using var uow = UnitOfWorkFactory.Create();
            var entity = GetNotificationByIdentifier(uow, notification.Identifier);
            if (entity == null)
            {
                SaveNotification(uow, typedNotification, type);
            }
            else
            {
                UpdateNotification(entity, typedNotification);
            }

            uow.SaveChanges();

            return NotificationProcessorResult.Processed;
        }

        /// <summary>
        /// Processes the notification.
        /// This method can be used to write type information to the notification instance
        /// </summary>
        protected virtual NotificationProcessorResult OnProcess(TNotification notification, TNotificationType type)
        {
            CopyTypeInformationToNotification(type, notification);

            return type.IsDisabled ? NotificationProcessorResult.Ignored : NotificationProcessorResult.Processed;
        }

        /// <summary>
        /// Selects or creates a notification type for the given notification
        /// The method is split into <see cref="SelectType"/> and <see cref="PopulateType"/> which
        /// are protected virtual to override in specializations of the processor
        /// </summary>
        private TNotificationType GetOrCreateType(TNotification notification)
        {
            _typeLock.EnterUpgradeableReadLock();

            var selectedType = SelectType(notification);
            if (selectedType != null)
            {
                _typeLock.ExitUpgradeableReadLock();
                return selectedType;
            }

            _typeLock.EnterWriteLock();

            using (var uow = UnitOfWorkFactory.Create())
            {
                selectedType = CreateType(uow, notification);               
            }

            _types.Add(selectedType);

            _typeLock.ExitWriteLock();
            _typeLock.ExitUpgradeableReadLock();

            return selectedType;
        }

        /// <summary>
        /// Selects a notification type by the given notification
        /// </summary>
        protected virtual TNotificationType SelectType(TNotification notification)
        {
            return _types.FirstOrDefault(t => t.Type.Equals(notification.GetType().Name) && t.Severity == notification.Severity);
        }

        /// <summary>
        /// Will create a new notification type with the given notification
        /// This method is split to override the saving of the type after creating the entity
        /// <see cref="PopulateType"/>
        /// </summary>
        private TNotificationType CreateType(IUnitOfWork uow, TNotification notification)
        {
            var type = new TNotificationType
            {
                Type = typeof(TNotification).Name,
                Severity = notification.Severity
            };

            var entity = uow.GetEntity<NotificationTypeEntity>(type);
            entity.Type = type.Type;
            entity.Severity = (int)type.Severity;
            entity.IsDisabled = type.IsDisabled;

            PopulateType(notification, type);

            entity.Identifier = type.Identifier;
            entity.ExtensionData = JsonConvert.SerializeObject(type, typeof(TNotificationType), JsonSettings.Minimal);

            uow.SaveChanges();
            type.Id = entity.Id;
            return type;
        }

        /// <summary>
        /// Will populate type information from specialized notification processors
        /// </summary>
        protected virtual void PopulateType(TNotification notification, TNotificationType type)
        {
        }

        /// <inheritdoc />
        public virtual NotificationProcessorResult Acknowledge(Notification notification)
        {
            var typedNotification = (TNotification)notification;
            var type = GetOrCreateType(typedNotification);

            var ackResult = OnAcknowledge(notification, type);
            if (ackResult == NotificationProcessorResult.Ignored)
                return ackResult;

            using var uow = UnitOfWorkFactory.Create();
            var entity = GetNotificationByIdentifier(uow, notification.Identifier);
            UpdateNotification(entity, typedNotification);

            uow.SaveChanges();

            return NotificationProcessorResult.Processed;
        }

        /// <summary>
        /// Execute acknowledge preprocessing
        /// This method can be used to handle the acknowledgement
        /// </summary>
        protected virtual NotificationProcessorResult OnAcknowledge(Notification notification, TNotificationType type)
        {
            return type.IsDisabled ? NotificationProcessorResult.Ignored : NotificationProcessorResult.Processed;
        }

        /// <summary>
        /// Loads all notification types from the database.
        /// </summary>
        private static IEnumerable<TNotificationType> LoadTypes(IUnitOfWork uow)
        {
            var typeRepo = uow.GetRepository<INotificationTypeEntityRepository>();
            var typeEntities = typeRepo.Linq.Where(t => t.Type.Equals(typeof(TNotification).Name)).ToArray();
            var types = new List<TNotificationType>(typeEntities.Length);

            foreach (var typeEntity in typeEntities)
            {
                var type = string.IsNullOrEmpty(typeEntity.ExtensionData)
                    ? new TNotificationType()
                    : JsonConvert.DeserializeObject<TNotificationType>(typeEntity.ExtensionData, JsonSettings.Minimal);

                type.Id = typeEntity.Id;
                type.Type = typeEntity.Type;
                type.Identifier = typeEntity.Identifier;
                type.Severity = (Severity)typeEntity.Severity;
                type.IsDisabled = typeEntity.IsDisabled;

                types.Add(type);
            }

            return types;
        }

        /// <summary>
        /// Loads an existing notification entity by the identifier
        /// </summary>
        private static NotificationEntity GetNotificationByIdentifier(IUnitOfWork uow, Guid identifier)
        {
            var repo = uow.GetRepository<INotificationEntityRepository>();
            return repo.Linq.SingleOrDefault(n => n.Identifier == identifier);
        }

        /// <summary>
        /// Will create a new notification database entity and saves the given notification
        /// </summary>
        private static void SaveNotification(IUnitOfWork uow, TNotification notification, TNotificationType type)
        {
            var notificationRepo = uow.GetRepository<INotificationEntityRepository>();
            var entity = notificationRepo.Create();
            entity.TypeId = type.Id;

            CopyFromNotificationToEntity(entity, notification);
        }

        /// <summary>
        /// Updates an existing notification entity
        /// </summary>
        private static void UpdateNotification(NotificationEntity entity, TNotification notification)
        {
            CopyFromNotificationToEntity(entity, notification);
        }

        /// <summary>
        /// Copies the basic properties of the notification base type to an existing database entity
        /// </summary>
        private static void CopyFromNotificationToEntity(NotificationEntity entity, TNotification notification)
        {
            entity.Identifier = notification.Identifier;
            entity.Title = notification.Title;
            entity.Message = notification.Message;
            if(notification.Created!=null)
                entity.Created = ConvertToUtc((DateTime)notification.Created);
            if (notification.Acknowledged != null)
                entity.Acknowledged = ConvertToUtc((DateTime)notification.Acknowledged);
            entity.Sender = notification.Sender;
            entity.Source = notification.Source;

            entity.ExtensionData = JsonConvert.SerializeObject(notification, typeof(TNotification), JsonSettings.Minimal);
        }

        private static DateTime ConvertToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            if (dateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
            throw new ArgumentException($"Provided {nameof(DateTime)} is neither UTC nor Local Time");
        }

        /// <summary>
        /// Creates a notification based of the database entity
        /// </summary>
        private TNotification CopyFromEntityToNotification(NotificationEntity entity)
        {
            TNotification notification = new TNotification();        
            notification.Identifier = entity.Identifier;
            notification.Title = entity.Title;
            notification.Message = entity.Message;
            notification.Created = entity.Created;
            notification.Acknowledged = entity.Acknowledged;
            notification.Sender = entity.Sender;
            notification.Source = entity.Source;
            notification.Severity = (Severity)entity.Type.Severity;

            var type = Types.FirstOrDefault(t => t.Id == entity.TypeId);
            CopyTypeInformationToNotification(type, notification);

            if (!string.IsNullOrEmpty(entity.ExtensionData))
                JsonConvert.PopulateObject(entity.ExtensionData, notification, JsonSettings.Minimal);

            return notification;
        }

        /// <summary>
        /// Converts given notification entity to a notification
        /// </summary>
        protected virtual void CopyTypeInformationToNotification(TNotificationType type, TNotification notification)
        {
        }

        /// <inheritdoc />
        public IReadOnlyList<Notification> GetHistory(DateTime start, DateTime end, Severity[] severity)
        {
            var typeName = typeof(TNotification).Name;
            using var uow = UnitOfWorkFactory.Create();
            var notificationRepo = uow.GetRepository<INotificationEntityRepository>();
            var timedFilteredNotifications =
                notificationRepo.Linq.Where(n => n.Created >= start && n.Created <= end &&
                                                 severity.Contains((Severity) n.Type.Severity) && n.Type.Type.Equals(typeName)).ToList();

            var result = new List<Notification>();
            foreach (var notificationEntity in timedFilteredNotifications)
            {
                var notification = CopyFromEntityToNotification(notificationEntity);
                result.Add(notification);
            }
            return result;
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<Notification> GetSelectedHistory(DateTime start, DateTime end, string title, Severity severity)
        {
            using var uow = UnitOfWorkFactory.Create();
            var notificationRepo = uow.GetRepository<INotificationEntityRepository>();

            var filter = from n in notificationRepo.Linq
                where n.Created >= start && n.Created <= end &&
                      n.Type.Severity == (int)severity &&
                      n.Type.Type == typeof(TNotification).Name
                select n;

            if (!string.IsNullOrEmpty(title))
                filter = filter.Where(n => n.Title.Equals(title));

            var results = filter.ToArray();

            return results.Select(n => (Notification)CopyFromEntityToNotification(n)).ToArray();
        }

        internal List<TNotificationType> GetTypes()
        {
            return _types;
        }
    }
}
