// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;

namespace Moryx.Runtime.Logging
{
    internal class ModuleLoggerEvent : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        public string Message { get; }
        public object[] Parameters { get; }

        public ModuleLoggerEvent(string message, object[] parameters)
        {
            Message = message;
            Parameters = parameters;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public ModuleLoggerEvent WithProperty(string name, object value)
        {
            _properties.Add(name, value);
            return this;
        }

        public static Func<ModuleLoggerEvent, Exception, string> Formatter { get; } = (l, e) =>
        {
            try
            {
                return string.Format(l.Message, l.Parameters);
            }
            catch
            {
                return l.Message + "- Format failed!";
            }
        };
    }
}