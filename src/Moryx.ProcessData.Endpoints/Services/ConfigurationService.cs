// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Notifications;
using Moryx.ProcessData.Configuration;
using Moryx.ProcessData.Endpoints.Models;
using Moryx.Runtime.Modules;
using Moryx.Tools;
using System.Reflection;

namespace Moryx.ProcessData.Endpoints.Services
{
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IModuleManager _moduleManager;
        private readonly IConfigManager _configManager;

        public ConfigurationService(IModuleManager moduleManager, IConfigManager configManager)
        {
            _moduleManager = moduleManager;
            _configManager = configManager;
        }

        public Dictionary<string, List<string>> GetBindings(IModule module)
        {
            return new Dictionary<string, List<string>>();
        }

        public MeasurandResponse GetMeasuarand(string name)
            => GetMeasuarands()
                .Where(m => m.Name == name)
                .FirstOrDefault();

        public ConfiguredBindings GetMeasuarandBindings(string name)
            => new()
            {
                Bindings = GetBindingsForMeasurandName(name)
            };

        public MeasurandBindings GetAvailableBindings(string adapterName)
            => new()
            {
                Bindings = GetAvailableBindings(GetMeasurandType(adapterName)),
            };


        private static Type GetMeasurandType(string measurandName)
            => measurandName.Contains("Activity")
                ? typeof(IActivity)
                : measurandName.Contains("Notification")
                    ? typeof(Notification)
                    : measurandName.Contains("Process")
                        ? typeof(IProcess)
                        : throw new ArgumentException("Measurand type not available");


        private List<MeasurementBinding> GetBindingsForMeasurandName(string name)
            => _moduleManager.AllModules
                .SelectMany(m => FindMeasurandBindings(GetConfig(m, false), name))
                .Distinct()
                .ToList();


        private static List<MeasurementBinding> FindMeasurandBindings(IConfig config, string measurandName)
        {
            var bindings = config.GetType().GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(List<MeasurementBinding>) && p.Name == measurandName);

            if (bindings == null)
                return new();

            return bindings.GetMeasurementBindings(config);
        }


        public List<string> GetAvailableBindings(Type typeInQuestion)
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => typeInQuestion.IsAssignableFrom(type) && !type.IsAbstract)
                .SelectMany(t => ListProperties(t, "", typeInQuestion))
                .Distinct()
                .ToList();


        public List<string> ListProperties(Type root, string name, Type baseType)
        {
            var properties = root.GetRuntimeProperties().ToArray();
            if (properties.Length == 0 || root.IsValueType || root == typeof(string))
            {
                return new List<string>() { name };
            }

            var filteredProperties = properties
                .Where(p => p.MemberType == MemberTypes.Property && p.DeclaringType == p.ReflectedType)
                .ToArray();
            var list = filteredProperties
                .SelectMany(p => ListProperties(p.PropertyType, name.AppendWithDot(p.Name), baseType));

            if (baseType.IsAssignableFrom(root.BaseType))
            {
                list = list.Concat(ListProperties(root.BaseType, name, baseType));
            }

            return list
                .Distinct()
                .ToList();
        }

        public List<MeasurandResponse> GetMeasuarands()
            => _moduleManager.AllModules
                .SelectMany(GetMeasurandsForModule)
                .ToList();

        public IEnumerable<MeasurandResponse> GetMeasurandsForModule(IServerModule module)
            => GetConfig(module, false)
                .GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(List<MeasurementBinding>))
                .Select(p => MeasurandFromModuleProperty(module, p));

        private static MeasurandResponse MeasurandFromModuleProperty(IServerModule module, PropertyInfo p)
            => new MeasurandResponse(p.Name)
            {
                GeneratedName = GetGeneratedName(p),
                Description = p.GetDescription(),
                Source = module.Name,
            };

        private static string GetGeneratedName(PropertyInfo p)
        {
            if (p.Name == "ActivityBindings")
                return "controlSystem_activities";
            if (p.Name == "ProcessBindings")
                return "controlSystem_processes";
            if (p.Name == "NotificationBindings")
                return "notifications_acknowledged";
            return p.Name;
        }

        public IEnumerable<MeasurandResponse> GetMeasurementBindings(string name)
            => _moduleManager.AllModules
                .SelectMany(m => GetConfig(m, false)
                    .GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(List<MeasurementBinding>) && p.Name == name)
                    .Select(p => MeasurandFromModuleProperty(m, p)));

        public IServerModule GetModule(string moduleName)
           => _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);


        private IConfig GetConfig(IModule module, bool copy)
        {
            var moduleType = module.GetType();
            var configType = moduleType.BaseType != null && moduleType.BaseType.IsGenericType
                ? moduleType.BaseType.GetGenericArguments()[0]
                : moduleType.Assembly.GetTypes().FirstOrDefault(type => typeof(IConfig).IsAssignableFrom(type));

            return _configManager.GetConfiguration(configType, copy);
        }


        public ConfiguredBindings UpdateMeasuarandBindings(string name, ConfiguredBindings measurandBindings)
        {
            var measurand = GetMeasuarand(name);

            var measurands = GetMeasuarands()
                .Where(m => m.Name == name)
                .ToList();

            var module = GetModule(measurand.Source);
            var config = GetConfig(module, false);

            var bindings = FindMeasurandBindings(config, name);

            bindings.Clear();
            bindings.AddRange(measurandBindings.Bindings.Select(b => new MeasurementBinding
            {
                Binding = b.Binding,
                Name = b.Name,
                ValueTarget = b.ValueTarget
            }));

            _moduleManager.ReincarnateModule(module);

            return new ConfiguredBindings{ Bindings = bindings };
        }
    }

    internal static class BindingPropertiesExtension
    {
        public static List<MeasurementBinding> GetMeasurementBindings(this PropertyInfo propertyInfo, IConfig config)
            => (List<MeasurementBinding>)propertyInfo.GetValue(config, null);

        public static string AppendWithDot(this string prefix, string str)
            => (prefix == ""
                ? ""
                : prefix + ".") + str;
    }
}

