using Castle.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.Runtime.Kernel
{
    internal class MoryxService : BackgroundService
    {
        private readonly IModuleManager moduleManager;
        private readonly IHost lifeTime;
        private readonly ILogger<MoryxService> logger;

        // Console shutdown handling according to https://stackoverflow.com/questions/21751545/how-to-make-a-console-app-exit-gracefully-when-it-is-closed
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        /// <summary>
        /// Callback that is called when a console event is raised.
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        private bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Stop the host and await shutdown.
            lifeTime.StopAsync(TimeSpan.FromSeconds(100)).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    logger.LogError(task.Exception, "Received failure on shutdown.");
                }
            }).Wait();
            return true;
        }

        public MoryxService(IModuleManager moduleManager, IHost lifetTime, ILogger<MoryxService> logger)
        {
            this.moduleManager = moduleManager;
            this.lifeTime = lifetTime;
            this.logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);
            moduleManager.StartModules();
            await base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            moduleManager.StopModules();
            await base.StopAsync(cancellationToken);
        }
    }
}
