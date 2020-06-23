// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.Runtime.Kernel
{
    internal abstract class LogFilter
    {
        public abstract bool Match(ILogMessage message);

        internal class Full : LogFilter
        {
            public override bool Match(ILogMessage message) => true;
        }

        internal class Level : LogFilter
        {
            private readonly LogLevel _level;

            public Level(LogLevel level)
            {
                _level = level;
            }

            public override bool Match(ILogMessage message)
            {
                return message.Level >= _level;
            }
        }

        internal class Name : LogFilter
        {
            private readonly string _name;

            public Name(string name)
            {
                _name = name;
            }

            public override bool Match(ILogMessage message)
            {
                return message.Logger.Name.StartsWith(_name);
            }
        }

        internal class NameAndLevel : LogFilter
        {
            private readonly string _name;
            private readonly LogLevel _level;

            public NameAndLevel(LogLevel level, string name)
            {
                _name = name;
                _level = level;
            }

            public override bool Match(ILogMessage message)
            {
                return message.Level >= _level && message.Logger.Name.StartsWith(_name);
            }
        }
    }
}
