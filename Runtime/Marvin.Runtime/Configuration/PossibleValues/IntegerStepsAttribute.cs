using System;
using System.Collections.Generic;
using Marvin.Container;

namespace Marvin.Runtime.Configuration
{
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

    public class IntegerStepsAttribute : PossibleConfigValuesAttribute
    {
        private readonly int _min;
        private readonly int _max;
        private readonly int _step;
        private readonly StepMode _mode;

        public IntegerStepsAttribute(int min, int max, int step, StepMode mode)
        {
            _min = min;
            _max = max;
            _step = step;
            _mode = mode;
        }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        public override IEnumerable<string> ResolvePossibleValues(IContainer pluginContainer)
        {
            var modeCalculation = (Func<int, int>)(possibleValue =>  _mode == StepMode.Addition 
                                                                   ? possibleValue + _step 
                                                                   : possibleValue * _step);
            for (var possibleValue = _min; possibleValue <= _max; possibleValue = modeCalculation(possibleValue))
            {
                yield return possibleValue.ToString();
            }
        }

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public override bool OverridesConversion
        {
            get { return false; }
        }

        public override bool UpdateFromPredecessor
        {
            get { return false; }
        }
    }
}
