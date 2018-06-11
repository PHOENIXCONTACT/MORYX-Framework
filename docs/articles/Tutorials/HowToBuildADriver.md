How to build a driver {#howToBuildADriver}
=====================
[TOC]


A driver is the part which will communicate with the outer world, the hardware or other software.

# How to implement a driver #

A driver has this basic solution structure which can be extended with your needs:

~~~~
-Marvin.Driver.ExampleDriver
|-ExampleConfig.cs
|-ExampleDriver.cs
|-IExampleDriver.cs
~~~~

This means that every driver has its main driver class (ExampleDriver.cs) and the corresponding config class (ExampleConfig.cs). 

## Create a ExampleConfig.cs ##

The basic structure of the config file should look like this:
~~~~{.cs}
/// Configuration for the YourDriver driver
[DataContract]
public class ExampleConfig : DriverConfigBase
{
    /// <summary>
    /// Name of the <see cref="IDriver"/> implemenation
    /// </summary>
    public override string PluginName
    {
        get { return ExampleDriver.PluginName; }
    }
}

~~~~

The config file will be derived from [DriverConfigBase](xref:Marvin.Drivers.DriverConfigBase) and will override the PluginName. The simple way is to use the internal constant defined in ExampleDriver.
Don't forget the DataContract above the class and the DataMember above the properties which should be configurable. Otherwhise is it not possible to configure them.

An example of an already implemented driver config is the [CognexScannerConfig](xref:Marvin.Drivers.Scanner.Cognex.CognexScannerConfig):

~~~~{.cs}

namespace Marvin.Drivers.Scanner.Cognex
{
    /// <summary>
    /// Config for the cognex barcode scanner
    /// </summary>
    public class CognexScannerConfig : DriverConfigBase
    {
        /// <summary>
        /// Name of the <see cref="IDriver"/> implemenation
        /// </summary>
        public override string PluginName
        {
            get { return CognexScannerDriver.PluginName; }
        }

        /// 
        [Description("Type of connection used (USB_Serial or TCP_IP)")]
        [DataMember, DefaultValue(ConnectionTypes.UsbSerial)]
        public ConnectionTypes ConnectionType { get; set; }

        /// 
        [Description("The port of the scanner in USB mode")]
        [DataMember,AvailableComPorts]
        public string Port { get; set; }

        ///
        [Description("IP address of the scanner in tcp/ip mode")]
        [DataMember, DefaultValue("0.0.0.0")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Method used to connect scanner with host PC
        /// </summary>
        public enum ConnectionTypes
        {
            /// <summary>
            /// Scanner is connected to the host via a dedicated usb cable
            /// </summary>
            UsbSerial,
            /// <summary>
            /// Scanner is connected to the same network as the host and accessible over TCP
            /// </summary>
            TcpIp
        }
    }
}

~~~~

## Available attribute ##
The port of the config has an attribute named [AvailablecomPorts](xref:Marvin.Drivers.Scanner.AvailablecomPorts) which will be checked and fill the possible com port list with the available com ports at runtime.

~~~~{.cs}

namespace Marvin.Drivers.Scanner
{
    /// <summary>
    /// Attribute listing COM ports on maintenance
    /// </summary>
    public class AvailableComPortsAttribute : PossibleConfigValuesAttribute
    {
        /// <summary>
        /// Resolves the registered COM ports.
        /// </summary>
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            var registryKey = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");

            if (registryKey == null)
                return Enumerable.Range(1, 9).Select(p => string.Format("COM{0}", p));

            string[] names = registryKey.GetValueNames();
            return names.Select(name => (string)registryKey.GetValue(name)).ToArray();
        }

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// 
        /// </summary>
        public override bool OverridesConversion
        {
            get { return false; }
        }

        /// <summary>
        /// Flag if new values shall be updated from the old value
        /// 
        /// </summary>
        public override bool UpdateFromPredecessor
        {
            get { return false; }
        }
    }
}

~~~~

- The attribute is derived from the class [PossibleConfigValuesAttribute](xref:Marvin.Runtime.Configuration.PossibleConfigValuesAttribute)
- Override the *ResolvePossibleValues* to determine the possible values for the enumeration.

## Create an IExampleDriver.cs ##

~~~~{.cs}

/// Interface for YourDriver drivers
public interface IExampleDriver : IDriver
{
}

~~~~
Interface for your driver. This must be existend so that resources can search with the interface for the type of the driver. 

An example for an already implemented interface is the [ICognexScannerDriver](xref:Marvin.Drivers.Scanner.Cognex.ICognexScannerDriver):

~~~~{.cs}

namespace Marvin.Drivers.Scanner.Cognex
{
    /// <summary>
    /// Interface for the cognex scanner
    /// </summary>
    public interface ICognexScannerDriver : IScannerDriver
    {
        /// <summary>
        /// Determines whether the button of the scanner can be used to initiate a scan or whether it is triggered by the system
        /// </summary>
        bool UseButton { get; set; }

        /// <summary>
        /// When set to scan automatically, then it can continue with the scan when one value was read in.
        /// </summary>
        bool ContinousScan { get; set; }

        /// <summary>
        /// Read a single bar code
        /// </summary>
        void SingleRead(DriverResponse<SingleReadResult> resultCallback);
    }

    /// <summary>
    /// Result of a single read operation
    /// </summary>
    public class SingleReadResult : TransmissionResult
    {
        /// <summary>
        /// Initialize a result object with a given barcode
        /// </summary>
        /// <param name="barCode"></param>
        public SingleReadResult(string barCode)
        {
            BarCode = barCode;
        }

        /// <summary>
        /// Barcode that was read from the device
        /// </summary>
        public string BarCode { get; private set; }
    }
}
~~~~

- If there are more than one implementation of that category (like scanner) then it is a good idea to add an additional base interface like [IScannerDriver](xref:Marvin.Drivers.IScannerDriver):

~~~~{.cs}
namespace Marvin.Drivers
{
    /// <summary>
    /// Common interface for barcode / QR-Code scanners
    /// </summary>
    public interface IScannerDriver : IDriver
    {
        /// <summary>
        /// Event raised when a code was read
        /// </summary>
        event EventHandler<string> CodeRead;
    }
}
~~~~


## Create an ExampleDriver.cs ##

The basic structure of the driver should look like this:

~~~~{.cs}
/// <summary>
/// Description of your driver.
/// </summary>
[DriverRegistration(PluginName)]
[ExpectedConfig(typeof (ExampleConfig))]
[DependencyRegistration(InstallerMode.All)]
public class ExampleDriver : DriverBase<ExampleConfig>, IExampleDriver
{
    /// <summary>
    /// The name of the Plugin.
    /// </summary>
    internal const string PluginName = "ExampleDriver";

    #region Injected properties
    #endregion

    #region Fields
    #endregion

    /// <seealso cref="IDriver"/>
    public override void Initialize(DriverConfigBase config)
    {
        base.Initialize(config);
    }

    /// <seealso cref="IDriver"/>
    public override void Start()
    {       
    }

    /// <seealso cref="IDriver"/>
    public override void Dispose()
    {          
    }
}
~~~~
- The [DriverRegistrationAttribute](xref:Marvin.Drivers.DriverRegistrationAttribute) will be registerd with the *PluginName* so try to not use the same name over and over.
- The [ExpectedConfigAttribute](xref:Marvin.Modules.ModulePlugins.ExpectedConfigAttribute) is *ExampleConfig*. 
- For the [DependencyRegistrationAttribute](xref:Marvin.Container.DependencyRegistrationAttribute) will the [InstallerMode](xref:Marvin.Container.InstallerMode) all used when you are not shure what you need.
- The class derives from the [DriverBase](xref:Marvin.Drivers.DriverBase) with the *ExampleConfig* type and uses the interface *IExampleDriver*

An example of an implementation is the [CognexScannerDriver](xref:Marvin.Drivers.Scanner.Cognex.CognexScannerDriver). This is only a part of the class:
~~~~{.cs}
namespace Marvin.Drivers.Scanner.Cognex
{
    /// <summary>
    /// Driver implementation for a cognex scanner
    /// </summary>
    [DriverRegistration(PluginName)]
    [ExpectedConfig(typeof(CognexScannerConfig))]
    public class CognexScannerDriver : ScannerDriverBase<CognexScannerConfig>, ICognexScannerDriver
    {
        internal const string PluginName = "CognexScanner";

        /// <summary>
        /// System for the Cognex Device.
        /// </summary>
        private DataManSystem _dataManSystem;

        /// <summary>
        /// Wrapper of the ThreadPool for executing methods on new threads
        /// [Injected]
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Typed reference to the state machine
        /// </summary>
        internal CognexScannerState State
        {
            get { return (CognexScannerState)CurrentState; }
            set { CurrentState = value; }
        }

        #region IModulePlugin

        /// <seealso cref="IDriver"/> 
        public override void Initialize(DriverConfigBase config)
        {
            base.Initialize(config);

            if (string.IsNullOrWhiteSpace(Config.Port))
            {
                State = FailureState.Create(this);
            }
            else
            {
                var systemConnector = new SerSystemConnector(Config.Port);
                _dataManSystem = new DataManSystem(systemConnector);

                State = DisconnectedState.Create(this);
            }
        }

        /// <seealso cref="IDriver"/> 
        public override void Start()
        {
            base.Start();

            State.Connect();
        }

        /// <seealso cref="IDriver"/> 
        public override void Dispose()
        {
            State.Disconnect();

            base.Dispose();
        }

        #endregion

        /// <summary>
        /// Connects to the device.
        /// </summary>
        /// <returns>true: connected, false: an error occured.</returns>
        internal bool Connect()
        {
            _dataManSystem.XmlResultArrived += DataManSystemXmlResultArrived;
            try
            {
                _dataManSystem.Connect();
                _dataManSystem.SetResultTypes(ResultTypes.ReadXml);
                return true;
            }
            catch (Exception)
            {
                _dataManSystem.XmlResultArrived -= DataManSystemXmlResultArrived;
                return false;
            }
        }

        /// <summary>
        /// All things that must be done when disconnected.
        /// </summary>
        internal void Disconnect()
        {
            // Send the trigger that the button will be used to scan so that the scanner will not be triggerd all the time when not connected.
            _dataManSystem.XmlResultArrived -= DataManSystemXmlResultArrived;
            _dataManSystem.Dispose();
        }

       ...
    }
}
~~~~

