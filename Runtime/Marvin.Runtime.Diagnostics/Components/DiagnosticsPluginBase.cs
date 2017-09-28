using System;
using Marvin.Configuration;

namespace Marvin.Runtime.Diagnostics
{
    /// <summary>
    /// Basic for all module plugins that require some diagnostic basics.
    /// </summary>
    /// <typeparam name="TConf"></typeparam>
    public abstract class DiagnosticsPluginBase<TConf> : IDiagnosticsPlugin
        where TConf : DiagnosticsPluginConfigBase
    {
        private bool _running;
        /// <summary>
        /// Configuiration of type TConf
        /// </summary>
        protected TConf Config { get; private set;  }

        /// <summary>
        /// Inititalize the diagnostic plugin
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(DiagnosticsPluginConfigBase config)
        {
            var castedConf = config as TConf;
            if (castedConf == null)
                throw new ArgumentException("Given configuration is of wrong type");

            Config = castedConf;
            OnInitialize();
            config.ConfigChanged += ConfigChanged;
        }

        /// <summary>
        /// Additional code which will be called when the module is initialized.
        /// </summary>
        protected virtual void OnInitialize()
        {
            
        }

        private void ConfigChanged(object sender, ConfigChangedEventArgs configChangedEventArgs)
        {
            if(!_running && !AllwaysListenToConfigChanged)
                return;

            OnConfigChanged(sender, configChangedEventArgs);
        }

        /// <summary>
        /// Will be called when a configuration changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="configChangedEventArgs">change arguments. See <see cref="ConfigChangedEventArgs"/> for details.</param>
        protected virtual void OnConfigChanged(object sender, ConfigChangedEventArgs configChangedEventArgs)
        {
            
        }

        /// <summary>
        /// Behavior to listen allways to config changes or not.
        /// </summary>
        protected abstract bool AllwaysListenToConfigChanged { get; }

        /// <summary>
        /// Starts the module.
        /// </summary>
        public void Start()
        {
            OnStart();
            _running = true;
        }

        /// <summary>
        /// Called in Start to enable additional code to be executed.
        /// </summary>
        protected virtual void OnStart()
        {
            
        }

        /// <summary>
        /// Disposes all.
        /// </summary>
        public void Dispose()
        {
            if(!_running)
                return;

            OnDispose();
            _running = false;
        }

        /// <summary>
        /// Will be called in Dispose to enable additional code to be executed.
        /// </summary>
        protected virtual void OnDispose()
        {
            
        }

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string Name { get; }
    }
}
