﻿using Microsoft.Extensions.DependencyInjection;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.TestModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Products.Samples.Recipe
{
    public class FacacadeRecipeValueAttribute : PossibleValuesAttribute
    {
        public override bool OverridesConversion => false;

        public override bool UpdateFromPredecessor => false;

        public override IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
        {
            var module = serviceProvider.GetRequiredService<ITestModule>();
            return new[] { module.Bla.ToString("D") };
        }
    }
}
