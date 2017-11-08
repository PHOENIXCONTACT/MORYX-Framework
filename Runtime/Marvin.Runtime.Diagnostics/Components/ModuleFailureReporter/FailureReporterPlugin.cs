using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Marvin.Container;
using Marvin.Model;
using Marvin.Modules;
using Marvin.Runtime.ModuleManagement;
using Marvin.Threading;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    /// <summary>
    /// Plugin to report failures. Sends e-mails to the configured targets.
    /// </summary>
    [DependencyRegistration(typeof(IMailClient))]
    [ExpectedConfig(typeof(FailureReporterConfig))]
    [Plugin(LifeCycle.Singleton, typeof(IDiagnosticsPlugin), Name = PluginName)]
    public class FailureReporterPlugin : DiagnosticsPluginBase<FailureReporterConfig>
    {
        /// <summary>
        /// Const name of the plugin.
        /// </summary>
        public const string PluginName = "PluginFailureReporter";
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string Name => PluginName;

        /// <summary>
        /// Behavior to listen allways to config changes or not.
        /// </summary>
        protected override bool AllwaysListenToConfigChanged => false;

        public IMailClient MailClient { get; set; }

        public IModuleManager ModuleManager { get; set; }

        public IModuleErrorReporting ErrorReporting { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Additional behavior for when called from [Start].
        /// </summary>
        protected override void OnStart()
        {
            // Register event to be notified in case of plugin failure
            ModuleManager.ModuleChangedState += ServiceChangedState;
            MailClient.Initialize(Config.MailClient);
            _timerId =  ParallelOperations.ScheduleExecution(TryResendMail, Config.RetryIntervalMs, Config.RetryIntervalMs);
        }

        private void TryResendMail()
        {
            if(!_undeliveredMail.Any())
                return;

            var undeliveredMailCopy = _undeliveredMail;
            _undeliveredMail = new List<Mail>();

            foreach (var mail in undeliveredMailCopy)
            {
                try
                {
                    MailClient.SendMail(mail, null);
                }
                catch
                {
                    lock (_undeliveredMail)
                        _undeliveredMail.Add(mail);
                }
            }
        }

        /// <summary>
        /// Additional code which will run when [Dispose].
        /// </summary>
        protected override void OnDispose()
        {
            // Register event to be notified in case of plugin failure
            ModuleManager.ModuleChangedState -= ServiceChangedState;
            ParallelOperations.StopExecution(_timerId);
        }

        private IList<Mail> _undeliveredMail = new List<Mail>();
        private int _timerId;

        private void ServiceChangedState(object sender, ModuleStateChangedEventArgs moduleStateChangedEventArgs)
        {
            var module = (IServerModule)sender;

            if (!moduleStateChangedEventArgs.NewState.HasFlag(ServerModuleState.Failure) || !Config.ReportTargets.Any())
                return;

            var dependends = ModuleManager.AllModules.Where(item => ModuleManager.StartDependencies(item).Contains(module));
            var mail = new Mail
            {
                Recipients = new List<string>(),
                Subject = string.Format("{0} failed on {1}", module.Name, Dns.GetHostName()),
            };
            foreach (var reportTarget in Config.ReportTargets)
            {
                // If module does not have database access, ignore this target
                if (reportTarget.PluginFailureFilter == PluginFailureFilter.DataAccess
                    && module.GetType().GetProperties().All(prop => prop.PropertyType != typeof (IUnitOfWorkFactory)))
                    continue;
                // If the name does not match, ignore this target
                if (reportTarget.PluginFailureFilter == PluginFailureFilter.Named
                    && !reportTarget.PluginNames.Contains(module.Name))
                    continue;

                mail.Recipients.Add(reportTarget.Address);
            }
            var ex = module.Notifications.OrderBy(n => n.Timestamp).Last(n => n.Type == NotificationType.Failure).Exception;
            mail.MessageBody = new MailBody(Config.ReportTargets.First().Name, module.Name, ex, Config.IncludeActiveButtons,
                                            dependends.Select(item => item.Name).ToList())
                              .TransformText();
            try
            {
                MailClient.SendMail(mail, null);
            }
            catch (Exception exc)
            {
                ErrorReporting.ReportWarning(this, exc);
                lock(_undeliveredMail)
                    _undeliveredMail.Add(mail);
            }
        }
    }
}
