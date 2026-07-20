// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.Benchmarking;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Notifications;
using Moryx.Serialization;
using Moryx.VisualInstructions;
using Activity = Moryx.AbstractionLayer.Activities.Activity;

namespace Moryx.Resources.Benchmarking;

[ResourceRegistration]
[DisplayName("Benchmark Resource")]
[Description("Resource to benchmark the MORYX framework. It can be used to test the process engine, the visual instructions and the notification system.")]
public class BenchmarkResource : Cell, IBenchmarkResource, INotificationSender
{
    #region Config

    /// <summary>
    /// Flag that processes shall be confirmed manually
    /// </summary>
    [DataMember, EntrySerialize]
    [Description("If set, the resource will wait for a manual confirmation of the process. If not set, the process will be completed automatically.")]
    public bool ManualProcessConfirmation { get; set; }

    /// <summary>
    /// Additional configured visual instructions for this resource
    /// </summary>
    [DataMember, EntrySerialize]
    [Description("Additional visual instructions to display")]
    public VisualInstruction[] Instructions { get; set; }

    #endregion

    #region Dependencies

    public INotificationAdapter NotificationAdapter { get; set; }

    [ResourceReference(ResourceRelationType.Extension, IsRequired = true)]
    [DisplayName("Visual Instructor")]
    [Description("Target to show instructions while benchmarking")]
    public IVisualInstructor VisualInstructor { get; set; }

    /// <summary>
    /// Additional configured possible results for the visual instructions for this resource
    /// </summary>
    [DataMember, EntrySerialize]
    [Description("Additional possible results for the visual instructions")]
    public InstructionResult[] PossibleResults { get; set; }

    #endregion

    private long _instructionId;

    private int _activityCount;
    private readonly Stopwatch _rtwWait = new();
    private readonly Stopwatch _acWait = new();
    private readonly Stopwatch _runtimeWait = new();

    /// <summary>
    /// Step id of this resource
    /// </summary>
    [DataMember, EntrySerialize]
    public int StepId { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    {
        await base.OnInitializeAsync(cancellationToken);

        Capabilities = new BenchmarkCapabilities(StepId);
    }

    /// <inheritdoc />
    protected override IEnumerable<Session> ProcessEngineAttached()
    {
        yield return Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
    }

    /// <inheritdoc />
    protected override IEnumerable<Session> ProcessEngineDetached()
    {
        yield break;
    }

    public override void StartActivity(ActivityStart activityStart)
    {
        _rtwWait.Stop();
        _runtimeWait.Start();

        if (ManualProcessConfirmation)
        {
            var parameters = (BenchmarkParameters)activityStart.Activity.Parameters;

            var preparedInstructions = parameters.Instructions.Concat(Instructions);

            var instructionResolver = new VisualInstructionBinder(preparedInstructions, new ProcessBindingResolverFactory());
            var resolved = instructionResolver.ResolveInstructions(activityStart.Process);

            _instructionId = VisualInstructor.Execute(Name, activityStart, CompleteActivity, resolved);
        }
        else
        {
            CompleteActivity(0, activityStart);
        }
    }

    /// <inheritdoc />
    public override void ProcessAborting(Activity affectedActivity)
    {
        VisualInstructor.Clear(_instructionId);
    }

    private void CompleteActivity(int result, ActivityStart activityStart)
    {
        _runtimeWait.Stop();

        var activity = activityStart.Activity;
        var tracing = (BenchmarkTracing)activity.Tracing;
        tracing.RuntimeMs = _runtimeWait.ElapsedMilliseconds;

        _runtimeWait.Reset();

        activity.Complete(result);

        var resultMsg = activityStart.CreateResult();

        _acWait.Start();

        // ReSharper disable once PossibleNullReferenceException
        PublishActivityCompleted(resultMsg);
    }

    /// <inheritdoc />
    public override void SequenceCompleted(SequenceCompleted completed)
    {
        _acWait.Stop();
        _activityCount++;

        _rtwWait.Start();
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
        PublishReadyToWork(rtw);
    }

    /// <inheritdoc />
    public BenchmarkReport GetReport()
    {
        var report = new BenchmarkReport
        {
            ActivityCount = _activityCount,
            ReadyToWorkWait = _rtwWait.ElapsedMilliseconds,
            ActivityCompletionWait = _acWait.ElapsedMilliseconds
        };
        _activityCount = 0;
        _rtwWait.Reset();
        _acWait.Reset();
        return report;
    }

    [DisplayName("Change Capabilities")]
    [EntrySerialize, Description("Change capabilities of the cell")]
    public void ChangeCapabilities([Description("New step value for the capabilities. '0' resets to LocalIdentifier")] int stepId = 0)
    {
        Capabilities = new BenchmarkCapabilities(stepId == 0 ? StepId : stepId);
    }

    [EntrySerialize]
    [Description("Publish a notification with the given title, message and severity")]
    public void PublishNotification(string title, string message, Severity severity, bool isAcknowledgable)
    {
        var notification = new Notification(title, message, severity, isAcknowledgable);
        NotificationAdapter.Publish(this, notification);
    }

    private const string MarkdownMessage = "# 1 Heading\n" +
                                           "This is a **markdown** notification with a [link](https://www.moryx-industry.net/) and an image:\n" +
                                           "![MORYX Logo](https://www.moryx-industry.net/assets/images/MORYX_logo.svg)\n" +
                                           "## 2 Heading\n" +
                                           "Table:\n\n" +
                                           "| Name | Value | Unit |\n" +
                                           "| --- | --- | --- |\n" +
                                           "| Temperature | 23.5 | °C |\n" +
                                           "| Pressure | 1013 | hPa |\n" +
                                           "| Humidity | 60 | % |\n";
    [EntrySerialize]
    [Description("Publish a notification with a markdown message")]
    public void PublishMarkdownNotification()
    {
        var notification = new Notification("Markdown Notification", MarkdownMessage, Severity.Info, false);
        NotificationAdapter.Publish(this, notification);
    }

    [EntrySerialize]
    [Description("Display a visual instruction with a markdown message")]
    public void PublishMarkdownInstruction()
    {
        VisualInstructor.Display(new ActiveInstruction
        {
            Title = "Markdown Instruction",
            Instructions =
            [
                new VisualInstruction { Content = MarkdownMessage, Type = InstructionContentType.Text }
            ]
        });
    }

    [EntrySerialize]
    [Description("Execute a visual instruction with configured instructions and possible results")]
    public void ExecuteVisualInstruction()
    {
        _instructionId = VisualInstructor.Execute(new ActiveInstruction { Title = Name, Instructions = Instructions, Results = PossibleResults, },
            response => { });
    }

    [EntrySerialize]
    [Description("Clear the visual instruction with the last executed instruction id")]
    public void ClearVisualInstruction()
    {
        VisualInstructor.Clear(_instructionId);
    }

    [EntrySerialize]
    [DisplayName("Acknowledge last notification")]
    public void AcknowledgeLastNotification()
    {
        var notifications = NotificationAdapter.GetPublished(this);
        if (notifications.Any())
            NotificationAdapter.Acknowledge(this, notifications.Last());
    }

    [EntrySerialize]
    [DisplayName("Acknowledge all notifications")]
    public void AcknowledgeAllNotifications()
    {
        NotificationAdapter.AcknowledgeAll(this);
    }

    void INotificationSender.Acknowledge(Notification notification, object tag)
    {
        NotificationAdapter.Acknowledge(this, notification);
    }

    string INotificationSender.Identifier => Id.ToString(CultureInfo.InvariantCulture);
}
