using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Runtime.Modules;
using System.Runtime.InteropServices;

namespace Moryx.Runtime.Kernel;


/// <summary>
/// A background service that handles starting and stopping the moryx modules.
/// On windows, it also provides gracefull shutdown when the console window is closed.
/// </summary>
public class MoryxHost : BackgroundService
{
    private readonly IModuleManager _moduleManager;
    private readonly IHost _lifeTime;
    private readonly ILogger<MoryxHost> _logger;

    /// <summary>
    /// Console shutdown handling according to https://stackoverflow.com/questions/21751545/how-to-make-a-console-app-exit-gracefully-when-it-is-closed
    /// </summary>
    /// <param name="Handler"></param>
    /// <param name="Add"></param>
    /// <returns></returns>
    [DllImport("Kernel32")]
    public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

    /// <summary>
    /// A delegate type to be used as the handler routine
    /// for SetConsoleCtrlHandler.
    /// </summary>
    /// <param name="CtrlType"></param>
    /// <returns></returns>
    public delegate bool HandlerRoutine(CtrlTypes CtrlType);

    /// <summary>
    /// An enumerated type for the control messages
    /// sent to the handler routine.
    /// </summary>
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
        _lifeTime.StopAsync(TimeSpan.FromSeconds(100)).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                _logger.LogError(task.Exception, "Received failure on shutdown.");
            }
        }).Wait();
        return true;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="moduleManager">The moryx module manager. Used to start the moryx system</param>
    /// <param name="lifeTime">The application host. Used to stop the application when a console controle event is received</param>
    /// <param name="logger">Logger for diagnostic messages</param>
    public MoryxHost(IModuleManager moduleManager, IHost lifeTime, ILogger<MoryxHost> logger)
    {
        this._moduleManager = moduleManager;
        this._lifeTime = lifeTime;
        this._logger = logger;
    }

    /// <summary>
    /// State of the MoryxHost
    /// </summary>
    public MoryxHostState State { get; private set; }

    /// <summary>
    /// Inform subscribers about state changes.
    /// Probably most usefull to receive a notification before MORYX shuts down
    /// </summary>
    public event EventHandler<MoryxHostState> StateChanged;

    /// <inheritdoc/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        State = MoryxHostState.Starting;
        StateChanged?.Invoke(this, State);
        // Only register on windows, because the behavior is os specific
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);
        }
        _moduleManager.StartModules();

        await base.StartAsync(cancellationToken);

        State = MoryxHostState.Started;
        StateChanged?.Invoke(this, State);
    }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // we don't have a long running task to perform
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        State = MoryxHostState.Stopping;
        StateChanged?.Invoke(this, State);

        _moduleManager.StopModules();

        await base.StopAsync(cancellationToken);

        State = MoryxHostState.Stopped;
        StateChanged?.Invoke(this, State);
    }
}
