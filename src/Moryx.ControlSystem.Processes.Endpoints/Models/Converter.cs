using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    /// <summary>
    /// JobProcessModel,ProcessActivityModel,TracingModel Models Converter
    /// </summary>
    public static class Converter
    {
        internal static JobProcessModel ConvertProcess(IProcess process, IProcessControl processControl, IResourceManagement resourceManagement)
        {
            var activities = process.GetActivities().ToList();
            return new()
            {
                Id = process.Id,
                RecipeId = process.Recipe.Id,
                // information can be found using the job
                Rework = false,
                State = InferProcessState(activities),
                Started = activities.Where(a => a.Tracing?.Started != null).Select(a => a.Tracing.Started).Min() ?? DateTime.MinValue,
                IsRunning = processControl.RunningProcesses.Any(p => p.Id == process.Id),
                Completed = activities.Where(a => a.Tracing?.Completed != null).Select(a => a.Tracing.Completed).Max() ?? DateTime.MinValue,
                Activities = activities.Select(a => ConvertActivity(a, processControl, resourceManagement)).ToArray()
            };
        }

        private static ProcessProgress InferProcessState(List<IActivity> activities)
        {
            if (activities == null || activities.Count == 0)
                return ProcessProgress.Ready;
            else if (!activities.Any(a => a.Tracing.Completed == null))
                return ProcessProgress.Completed;
            else if (activities.Any(a => a.Tracing.Started != null))
                return ProcessProgress.Running;
            else
                return ProcessProgress.Ready;
        }

        /// <summary>
        /// Convert activity to a Process-Activity Model by providing more information about the process and the activity
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="processControl"></param>
        /// <param name="resourceManagement"></param>
        /// <returns></returns>
        public static ProcessActivityModel ConvertActivity(IActivity activity, IProcessControl processControl, IResourceManagement resourceManagement) => new()
        {
            Id = activity.Id,
            Type = activity.GetType().Name,
            RequiredCapabilities = activity.RequiredCapabilities.GetType().Name,
            Result = activity.Result?.Numeric,
            ResultName = InferResultName(activity, activity.Result?.Numeric),
            Tracing = ConvertTracing(activity.Tracing),
            IsCompleted = activity.Result is not null,
            State = InferActivityState(activity),
            Classification = InferActivityClassification(activity),
            InstanceId = (activity.Process as ProductionProcess)?.ProductInstance?.Id,
            Resource = ConvertResource(activity, resourceManagement),
            PossibleResources = processControl.Targets(activity)?.Select(r => new ActivityResourceModel
            {
                Id = r.Id,
                Name = r.Name,
            }).ToArray()
        };

        private static ActivityResourceModel ConvertResource(IActivity activity, IResourceManagement resourceManagement)
        {
            var resourceId = activity.Tracing?.ResourceId ?? -1;
            var resource = resourceManagement.GetResource<IResource>(resourceId);
            return (resource is null) ? null : new() { Name = resource.Name, Id = resourceId };
        }

        private static string InferResultName(IActivity activity, long? resultNumber)
        {
            var attr = activity.GetType().GetCustomAttribute<ActivityResultsAttribute>();
            return (resultNumber is null || attr is null) ? null : Enum.GetName(attr.ResultEnum, resultNumber);
        }

        private static ActivityClassification InferActivityClassification(IActivity activity)
        {
            if (activity.Process is ProductionProcess)
                return ActivityClassification.Production;
            else if (activity is IControlSystemActivity controlActivity)
                return controlActivity.Classification;
            return ActivityClassification.Unknown;
        }

        private static string InferActivityState(IActivity activity)
        {
            if (activity.Result != null)
                return ActivityProgress.Completed.ToString();
            else if (activity.Tracing?.Started != null)
                return ActivityProgress.Running.ToString();
            else
                return ActivityProgress.Ready.ToString();
        }

        internal static TracingModel ConvertTracing(Tracing tracing) => new()
        {
            Type = tracing.GetType().Name,
            Text = tracing.Text,
            Started = tracing.Started,
            Completed = tracing.Completed,
            ErrorCode = tracing.ErrorCode,
            Properties = EntryConvert.EncodeObject(tracing, new TracingSerialization())
        };
    }
}
