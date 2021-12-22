// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;

namespace Moryx.Runtime.Kernel
{
    internal class MsLoggingLogEvent : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        public string Message { get; }

        public MsLoggingLogEvent(string message)
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public MsLoggingLogEvent WithProperty(string name, object value)
        {
            _properties.Add(name, value);
            return this;
        }

        public static Func<MsLoggingLogEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;
    }
}