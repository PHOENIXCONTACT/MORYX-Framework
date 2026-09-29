using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Extensions.Properties;
using Moryx.AbstractionLayer.Resources;
using Moryx.Notifications;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Extensions;

/// <summary>
/// Extension resource that observes an <see cref="IDriver"/> and publishes notifications based on its current
/// <see cref="StateClassification"/>.
/// </summary>
[Display(Name = nameof(Strings.DriverStateNotifier_Name), Description = nameof(Strings.DriverStateNotifier_Description), ResourceType = typeof(Strings))]
[ResourceRegistration]
public class DriverStateNotifier : Resource, INotificationSender
{
    #region Dependency Injection

    public required INotificationAdapter NotificationAdapter { get; set; }

    #endregion

    #region Resource Reference

    [ReferenceOverride(nameof(Parent))]
    [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Source, IsRequired = true)]
    public required IDriver Driver { get; set; }

    #endregion

    #region Config

    /// <summary>
    /// Flags that control which driver state transitions produce a notification.
    /// </summary>
    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_EnabledNotifications_Name), Description = nameof(Strings.DriverStateNotifier_EnabledNotifications_Description), ResourceType = typeof(Strings))]
    private DriverStateNotificationFlags EnabledNotifications { get; set; } = DriverStateNotificationFlags.Error
        | DriverStateNotificationFlags.Offline | DriverStateNotificationFlags.Initializing | DriverStateNotificationFlags.Maintenance;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_NotificationTitle_Name), Description = nameof(Strings.DriverStateNotifier_NotificationTitle_Description), ResourceType = typeof(Strings))]
    public string NotificationTitle { get; set; } = Strings.DriverStateNotifier_NotificationTitle_Default;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_OfflineMessage_Name), Description = nameof(Strings.DriverStateNotifier_OfflineMessage_Description), ResourceType = typeof(Strings))]
    public string OfflineMessage { get; set; } = Strings.DriverStateNotifier_OfflineMessage_Default;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_InitializingMessage_Name), Description = nameof(Strings.DriverStateNotifier_InitializingMessage_Description), ResourceType = typeof(Strings))]
    public string InitializingMessage { get; set; } = Strings.DriverStateNotifier_InitializingMessage_Default;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_BusyMessage_Name), Description = nameof(Strings.DriverStateNotifier_BusyMessage_Description), ResourceType = typeof(Strings))]
    public string BusyMessage { get; set; } = Strings.DriverStateNotifier_BusyMessage_Default;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_MaintenanceMessage_Name), Description = nameof(Strings.DriverStateNotifier_MaintenanceMessage_Description), ResourceType = typeof(Strings))]
    public string MaintenanceMessage { get; set; } = Strings.DriverStateNotifier_MaintenanceMessage_Default;

    [DataMember, EntrySerialize]
    [Display(Name = nameof(Strings.DriverStateNotifier_ErrorMessage_Name), Description = nameof(Strings.DriverStateNotifier_ErrorMessage_Description), ResourceType = typeof(Strings))]
    public string ErrorMessage { get; set; } = Strings.DriverStateNotifier_ErrorMessage_Default;

    #endregion

    #region INotificationSender

    /// <inheritdoc/>
    string INotificationSender.Identifier => Id.ToString(CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    void INotificationSender.Acknowledge(Notification notification, object tag) =>
        NotificationAdapter.Acknowledge(this, notification);

    #endregion

    #region LifeCycle

    /// <inheritdoc/>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Name ??= Driver.Name + " Notifier";
        Driver.StateChanged += OnDriverStateChanged;
        if (Driver.CurrentState != null)
        {
            UpdateNotification(Driver.CurrentState);
        }

        return base.OnStartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Driver.StateChanged -= OnDriverStateChanged;
        NotificationAdapter.AcknowledgeAll(this);
        return base.OnStopAsync(cancellationToken);
    }

    /// <inheritdoc/>
    protected override void OnDispose()
    {
        NotificationAdapter.AcknowledgeAll(this);
        base.OnDispose();
    }

    #endregion

    private void OnDriverStateChanged(object? sender, IDriverState state) => UpdateNotification(state);

    private void UpdateNotification(IDriverState state)
    {
        NotificationAdapter.AcknowledgeAll(this);

        var classification = state.Classification;

        // We only show the most important notification.
        // Priority: Error > Offline > Maintenance > Initializing > Busy
        if (classification.HasFlag(StateClassification.Error) &&
            EnabledNotifications.HasFlag(DriverStateNotificationFlags.Error))
        {
            Publish(ErrorMessage, Severity.Error);
        }
        else if (classification == StateClassification.Offline &&
                 EnabledNotifications.HasFlag(DriverStateNotificationFlags.Offline))
        {
            Publish(OfflineMessage, Severity.Error);
        }
        else if (classification.HasFlag(StateClassification.Maintenance) &&
                 EnabledNotifications.HasFlag(DriverStateNotificationFlags.Maintenance))
        {
            Publish(MaintenanceMessage, Severity.Warning);
        }
        else if (classification.HasFlag(StateClassification.Initializing) &&
                 EnabledNotifications.HasFlag(DriverStateNotificationFlags.Initializing))
        {
            Publish(InitializingMessage, Severity.Warning);
        }
        else if (classification.HasFlag(StateClassification.Busy) &&
                 EnabledNotifications.HasFlag(DriverStateNotificationFlags.Busy))
        {
            Publish(BusyMessage, Severity.Info);
        }
    }

    private void Publish(string message, Severity severity)
    {
        var driverResource = Driver as Resource ?? throw new InvalidOperationException(
            $"{Id}-{Name}: cannot publish notification — {nameof(IDriver)} reference could not be cast to {nameof(Resource)}.");
        var title = $"{driverResource.Id.ToString(CultureInfo.InvariantCulture)} - {driverResource.Name} – {NotificationTitle}";
        NotificationAdapter.Publish(this, new Notification(title, message, severity, isAcknowledgable: true));
    }

    /// <summary>
    /// Flags that control which <see cref="StateClassification"/> values produce a notification in
    /// <see cref="DriverStateNotifier"/>.
    /// </summary>
    [Flags]
    private enum DriverStateNotificationFlags
    {
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_None), ResourceType = typeof(Strings))]
        None = 0,

        /// <summary>
        /// Publish an error notification when the driver is offline (no connection).
        /// </summary>
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_Offline), ResourceType = typeof(Strings))]
        Offline = 1 << 0,

        /// <summary>
        /// Publish a warning notification while the driver is initializing or connecting.
        /// </summary>
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_Initializing), ResourceType = typeof(Strings))]
        Initializing = 1 << 1,

        /// <summary>
        /// Publish an info notification while the driver is busy.
        /// </summary>
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_Busy), ResourceType = typeof(Strings))]
        Busy = 1 << 2,

        /// <summary>
        /// Publish a warning notification when the driver is in maintenance state.
        /// </summary>
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_Maintenance), ResourceType = typeof(Strings))]
        Maintenance = 1 << 3,

        /// <summary>
        /// Publish an error notification when the driver is in an error state.
        /// </summary>
        [Display(Name = nameof(Strings.DriverStateNotificationFlags_Error), ResourceType = typeof(Strings))]
        Error = 1 << 4,

        [Display(Name = nameof(Strings.DriverStateNotificationFlags_All), ResourceType = typeof(Strings))]
        All = Offline | Initializing | Busy | Maintenance | Error,
    }
}
