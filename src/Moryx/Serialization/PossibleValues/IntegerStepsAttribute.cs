// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Container;

namespace Moryx.Serialization
{
    /// <summary>
    /// Mode of the integer steps
    /// </summary>
    public enum StepMode
    {
        /// <summary>
        /// Increase value by adding step value
        /// </summary>
        Addition,

        /// <summary>
        /// Increase value by multiplying with step value
        /// </summary>
        Multiplication
    }

    /// <summary>
    /// <see cref="PossibleValuesAttribute"/> which provides integer steps
    /// </summary>
    public class IntegerStepsAttribute : PossibleValuesAttribute
    {
        private readonly int _min;
        private readonly int _max;
        private readonly int _step;
        private readonly StepMode _mode;

        /// <summary>
        /// Creates a new instance of <see cref="IntegerStepsAttribute"/>
        /// </summary>
        public IntegerStepsAttribute(int min, int max, int step, StepMode mode)
        {
            _min = min;
            _max = max;
            _step = step;
            _mode = mode;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValues(IContainer container)
        {
            var modeCalculation = (Func<int, int>)(possibleValue =>  _mode == StepMode.Addition 
                                                                   ? possibleValue + _step 
                                                                   : possibleValue * _step);
            for (var possibleValue = _min; possibleValue <= _max; possibleValue = modeCalculation(possibleValue))
            {
                yield return possibleValue.ToString();
            }
        }

        /// <inheritdoc />
        public override bool OverridesConversion => false;

        /// <inheritdoc />
        public override bool UpdateFromPredecessor => false;
    }
}
