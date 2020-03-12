// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marvin.Tools.Wcf.Tests.Maintenance {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServerModuleModel", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class ServerModuleModel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.AssemblyModel AssemblyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel> DependenciesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour FailureBehaviourField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleState HealthStateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.NotificationModel> NotificationsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour StartBehaviourField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.AssemblyModel Assembly {
            get {
                return this.AssemblyField;
            }
            set {
                if ((object.ReferenceEquals(this.AssemblyField, value) != true)) {
                    this.AssemblyField = value;
                    this.RaisePropertyChanged("Assembly");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel> Dependencies {
            get {
                return this.DependenciesField;
            }
            set {
                if ((object.ReferenceEquals(this.DependenciesField, value) != true)) {
                    this.DependenciesField = value;
                    this.RaisePropertyChanged("Dependencies");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour FailureBehaviour {
            get {
                return this.FailureBehaviourField;
            }
            set {
                if ((this.FailureBehaviourField.Equals(value) != true)) {
                    this.FailureBehaviourField = value;
                    this.RaisePropertyChanged("FailureBehaviour");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleState HealthState {
            get {
                return this.HealthStateField;
            }
            set {
                if ((this.HealthStateField.Equals(value) != true)) {
                    this.HealthStateField = value;
                    this.RaisePropertyChanged("HealthState");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.NotificationModel> Notifications {
            get {
                return this.NotificationsField;
            }
            set {
                if ((object.ReferenceEquals(this.NotificationsField, value) != true)) {
                    this.NotificationsField = value;
                    this.RaisePropertyChanged("Notifications");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour StartBehaviour {
            get {
                return this.StartBehaviourField;
            }
            set {
                if ((this.StartBehaviourField.Equals(value) != true)) {
                    this.StartBehaviourField = value;
                    this.RaisePropertyChanged("StartBehaviour");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AssemblyModel", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class AssemblyModel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BundleField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Bundle {
            get {
                return this.BundleField;
            }
            set {
                if ((object.ReferenceEquals(this.BundleField, value) != true)) {
                    this.BundleField = value;
                    this.RaisePropertyChanged("Bundle");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version {
            get {
                return this.VersionField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionField, value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.FlagsAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="FailureBehaviour", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Modules")]
    public enum FailureBehaviour : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Stop = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        StopAndNotify = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Reincarnate = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ReincarnateAndNotify = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.FlagsAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServerModuleState", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Modules")]
    public enum ServerModuleState : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Stopped = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Initializing = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Ready = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Starting = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Running = 8,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Stopping = 10,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        BootWarning = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Warning = 12,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Failure = 4,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="NotificationModel", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class NotificationModel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.SerializableException ExceptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ImportantField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime TimestampField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.SerializableException Exception {
            get {
                return this.ExceptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ExceptionField, value) != true)) {
                    this.ExceptionField = value;
                    this.RaisePropertyChanged("Exception");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Important {
            get {
                return this.ImportantField;
            }
            set {
                if ((this.ImportantField.Equals(value) != true)) {
                    this.ImportantField = value;
                    this.RaisePropertyChanged("Important");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Timestamp {
            get {
                return this.TimestampField;
            }
            set {
                if ((this.TimestampField.Equals(value) != true)) {
                    this.TimestampField = value;
                    this.RaisePropertyChanged("Timestamp");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ModuleStartBehaviour", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Modules")]
    public enum ModuleStartBehaviour : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Auto = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Manual = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OnDependency = 2,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SerializableException", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class SerializableException : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExceptionTypeNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Marvin.Tools.Wcf.Tests.Maintenance.SerializableException InnerExceptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StackTraceField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExceptionTypeName {
            get {
                return this.ExceptionTypeNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ExceptionTypeNameField, value) != true)) {
                    this.ExceptionTypeNameField = value;
                    this.RaisePropertyChanged("ExceptionTypeName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Marvin.Tools.Wcf.Tests.Maintenance.SerializableException InnerException {
            get {
                return this.InnerExceptionField;
            }
            set {
                if ((object.ReferenceEquals(this.InnerExceptionField, value) != true)) {
                    this.InnerExceptionField = value;
                    this.RaisePropertyChanged("InnerException");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StackTrace {
            get {
                return this.StackTraceField;
            }
            set {
                if ((object.ReferenceEquals(this.StackTraceField, value) != true)) {
                    this.StackTraceField = value;
                    this.RaisePropertyChanged("StackTrace");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DependencyEvaluation", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class DependencyEvaluation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int MaxDependenciesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int MaxDependendsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int MaxDepthField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RootModulesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxDependencies {
            get {
                return this.MaxDependenciesField;
            }
            set {
                if ((this.MaxDependenciesField.Equals(value) != true)) {
                    this.MaxDependenciesField = value;
                    this.RaisePropertyChanged("MaxDependencies");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxDependends {
            get {
                return this.MaxDependendsField;
            }
            set {
                if ((this.MaxDependendsField.Equals(value) != true)) {
                    this.MaxDependendsField = value;
                    this.RaisePropertyChanged("MaxDependends");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int MaxDepth {
            get {
                return this.MaxDepthField;
            }
            set {
                if ((this.MaxDepthField.Equals(value) != true)) {
                    this.MaxDepthField = value;
                    this.RaisePropertyChanged("MaxDepth");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RootModules {
            get {
                return this.RootModulesField;
            }
            set {
                if ((this.RootModulesField.Equals(value) != true)) {
                    this.RootModulesField = value;
                    this.RaisePropertyChanged("RootModules");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Config", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    [System.SerializableAttribute()]
    public partial class Config : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<Marvin.Serialization.Entry> EntriesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ModuleField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<Marvin.Serialization.Entry> Entries {
            get {
                return this.EntriesField;
            }
            set {
                if ((object.ReferenceEquals(this.EntriesField, value) != true)) {
                    this.EntriesField = value;
                    this.RaisePropertyChanged("Entries");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Module {
            get {
                return this.ModuleField;
            }
            set {
                if ((object.ReferenceEquals(this.ModuleField, value) != true)) {
                    this.ModuleField = value;
                    this.RaisePropertyChanged("Module");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ConfigUpdateMode", Namespace="http://schemas.datacontract.org/2004/07/Marvin.Runtime.Maintenance.Plugins.Module" +
        "Maintenance.Wcf")]
    public enum ConfigUpdateMode : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OnlySave = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SaveAndReincarnate = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UpdateLiveAndSave = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Maintenance.IModuleMaintenance")]
    public interface IModuleMaintenance {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetAll", ReplyAction="http://tempuri.org/IModuleMaintenance/GetAllResponse")]
        System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel> GetAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetAll", ReplyAction="http://tempuri.org/IModuleMaintenance/GetAllResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel>> GetAllAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetDependencyEvaluation", ReplyAction="http://tempuri.org/IModuleMaintenance/GetDependencyEvaluationResponse")]
        Marvin.Tools.Wcf.Tests.Maintenance.DependencyEvaluation GetDependencyEvaluation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetDependencyEvaluation", ReplyAction="http://tempuri.org/IModuleMaintenance/GetDependencyEvaluationResponse")]
        System.Threading.Tasks.Task<Marvin.Tools.Wcf.Tests.Maintenance.DependencyEvaluation> GetDependencyEvaluationAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Start", ReplyAction="http://tempuri.org/IModuleMaintenance/StartResponse")]
        void Start(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Start", ReplyAction="http://tempuri.org/IModuleMaintenance/StartResponse")]
        System.Threading.Tasks.Task StartAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Stop", ReplyAction="http://tempuri.org/IModuleMaintenance/StopResponse")]
        void Stop(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Stop", ReplyAction="http://tempuri.org/IModuleMaintenance/StopResponse")]
        System.Threading.Tasks.Task StopAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Reincarnate", ReplyAction="http://tempuri.org/IModuleMaintenance/ReincarnateResponse")]
        void Reincarnate(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/Reincarnate", ReplyAction="http://tempuri.org/IModuleMaintenance/ReincarnateResponse")]
        System.Threading.Tasks.Task ReincarnateAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/ConfirmWarning", ReplyAction="http://tempuri.org/IModuleMaintenance/ConfirmWarningResponse")]
        void ConfirmWarning(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/ConfirmWarning", ReplyAction="http://tempuri.org/IModuleMaintenance/ConfirmWarningResponse")]
        System.Threading.Tasks.Task ConfirmWarningAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetConfig", ReplyAction="http://tempuri.org/IModuleMaintenance/GetConfigResponse")]
        Marvin.Tools.Wcf.Tests.Maintenance.Config GetConfig(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/GetConfig", ReplyAction="http://tempuri.org/IModuleMaintenance/GetConfigResponse")]
        System.Threading.Tasks.Task<Marvin.Tools.Wcf.Tests.Maintenance.Config> GetConfigAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetConfig", ReplyAction="http://tempuri.org/IModuleMaintenance/SetConfigResponse")]
        void SetConfig(Marvin.Tools.Wcf.Tests.Maintenance.Config model, Marvin.Tools.Wcf.Tests.Maintenance.ConfigUpdateMode updateMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetConfig", ReplyAction="http://tempuri.org/IModuleMaintenance/SetConfigResponse")]
        System.Threading.Tasks.Task SetConfigAsync(Marvin.Tools.Wcf.Tests.Maintenance.Config model, Marvin.Tools.Wcf.Tests.Maintenance.ConfigUpdateMode updateMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetStartBehaviour", ReplyAction="http://tempuri.org/IModuleMaintenance/SetStartBehaviourResponse")]
        void SetStartBehaviour(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour startBehaviour);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetStartBehaviour", ReplyAction="http://tempuri.org/IModuleMaintenance/SetStartBehaviourResponse")]
        System.Threading.Tasks.Task SetStartBehaviourAsync(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour startBehaviour);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetFailureBehaviour", ReplyAction="http://tempuri.org/IModuleMaintenance/SetFailureBehaviourResponse")]
        void SetFailureBehaviour(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour failureBehaviour);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IModuleMaintenance/SetFailureBehaviour", ReplyAction="http://tempuri.org/IModuleMaintenance/SetFailureBehaviourResponse")]
        System.Threading.Tasks.Task SetFailureBehaviourAsync(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour failureBehaviour);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IModuleMaintenanceChannel : Marvin.Tools.Wcf.Tests.Maintenance.IModuleMaintenance, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ModuleMaintenanceClient : System.ServiceModel.ClientBase<Marvin.Tools.Wcf.Tests.Maintenance.IModuleMaintenance>, Marvin.Tools.Wcf.Tests.Maintenance.IModuleMaintenance {
        
        public ModuleMaintenanceClient() {
        }
        
        public ModuleMaintenanceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ModuleMaintenanceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ModuleMaintenanceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ModuleMaintenanceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel> GetAll() {
            return base.Channel.GetAll();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<Marvin.Tools.Wcf.Tests.Maintenance.ServerModuleModel>> GetAllAsync() {
            return base.Channel.GetAllAsync();
        }
        
        public Marvin.Tools.Wcf.Tests.Maintenance.DependencyEvaluation GetDependencyEvaluation() {
            return base.Channel.GetDependencyEvaluation();
        }
        
        public System.Threading.Tasks.Task<Marvin.Tools.Wcf.Tests.Maintenance.DependencyEvaluation> GetDependencyEvaluationAsync() {
            return base.Channel.GetDependencyEvaluationAsync();
        }
        
        public void Start(string moduleName) {
            base.Channel.Start(moduleName);
        }
        
        public System.Threading.Tasks.Task StartAsync(string moduleName) {
            return base.Channel.StartAsync(moduleName);
        }
        
        public void Stop(string moduleName) {
            base.Channel.Stop(moduleName);
        }
        
        public System.Threading.Tasks.Task StopAsync(string moduleName) {
            return base.Channel.StopAsync(moduleName);
        }
        
        public void Reincarnate(string moduleName) {
            base.Channel.Reincarnate(moduleName);
        }
        
        public System.Threading.Tasks.Task ReincarnateAsync(string moduleName) {
            return base.Channel.ReincarnateAsync(moduleName);
        }
        
        public void ConfirmWarning(string moduleName) {
            base.Channel.ConfirmWarning(moduleName);
        }
        
        public System.Threading.Tasks.Task ConfirmWarningAsync(string moduleName) {
            return base.Channel.ConfirmWarningAsync(moduleName);
        }
        
        public Marvin.Tools.Wcf.Tests.Maintenance.Config GetConfig(string moduleName) {
            return base.Channel.GetConfig(moduleName);
        }
        
        public System.Threading.Tasks.Task<Marvin.Tools.Wcf.Tests.Maintenance.Config> GetConfigAsync(string moduleName) {
            return base.Channel.GetConfigAsync(moduleName);
        }
        
        public void SetConfig(Marvin.Tools.Wcf.Tests.Maintenance.Config model, Marvin.Tools.Wcf.Tests.Maintenance.ConfigUpdateMode updateMode) {
            base.Channel.SetConfig(model, updateMode);
        }
        
        public System.Threading.Tasks.Task SetConfigAsync(Marvin.Tools.Wcf.Tests.Maintenance.Config model, Marvin.Tools.Wcf.Tests.Maintenance.ConfigUpdateMode updateMode) {
            return base.Channel.SetConfigAsync(model, updateMode);
        }
        
        public void SetStartBehaviour(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour startBehaviour) {
            base.Channel.SetStartBehaviour(moduleName, startBehaviour);
        }
        
        public System.Threading.Tasks.Task SetStartBehaviourAsync(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.ModuleStartBehaviour startBehaviour) {
            return base.Channel.SetStartBehaviourAsync(moduleName, startBehaviour);
        }
        
        public void SetFailureBehaviour(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour failureBehaviour) {
            base.Channel.SetFailureBehaviour(moduleName, failureBehaviour);
        }
        
        public System.Threading.Tasks.Task SetFailureBehaviourAsync(string moduleName, Marvin.Tools.Wcf.Tests.Maintenance.FailureBehaviour failureBehaviour) {
            return base.Channel.SetFailureBehaviourAsync(moduleName, failureBehaviour);
        }
    }
}
