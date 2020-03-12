// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Marvin.Workflows;
using Marvin.Workflows.Compiler;

namespace Marvin.Tests.Workflows
{
    public class DummyStepCompiler : ICompiler<CompiledDummyTransition>
    {
        /// <summary>
        /// Compile a workplanstep
        /// </summary>
        public CompiledDummyTransition CompileTransition(ITransition transition)
        {
            return new CompiledDummyTransition { Name = ((DummyTransition)transition).Name };
        }

        /// <summary>
        /// Compile a result connector to an outfeed step
        /// </summary>
        public CompiledDummyTransition CompileResult(IConnector output)
        {
            return new CompiledDummyTransition { Name = output.Name, IsOutfeed = true };
        }
    }

    public class StationMapCompiler : ICompiler<CompiledDummyTransition>
    {
        private readonly IDictionary<long, int> _stationMap;

        public StationMapCompiler(IDictionary<long, int> stationMap)
        {
            _stationMap = stationMap;
        }

        /// <summary>
        /// Compile a workplanstep
        /// </summary>
        public CompiledDummyTransition CompileTransition(ITransition transition)
        {
            return new CompiledDummyTransition { Name = ((DummyTransition)transition).Name, Station = _stationMap[transition.Id] };
        }

        /// <summary>
        /// Compile a result connector to an outfeed step
        /// </summary>
        public CompiledDummyTransition CompileResult(IConnector output)
        {
            return new CompiledDummyTransition { Name = output.Name, IsOutfeed = true, Station = _stationMap[0] };
        }
    }

    public class CompiledDummyTransition : CompiledTransition
    {
        public string Name { get; set; }

        public bool IsOutfeed { get; set; }

        public int Station { get; set; }
    }
}
