using System;
using System.Linq;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer.Identity;
using Marvin.Bindings;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Resolver factory that accepts recipes as input arguments
    /// </summary>
    public class RecipeBindingResolverFactory : BindingResolverFactory
    {
        /// <inheritdoc />
        protected override IBindingResolverChain CreateBaseResolver(string baseKey)
        {
            switch (baseKey)
            {
                case "Recipe":
                    return new NullResolver();
                case "Product":
                    return new DelegateResolver(source => (source as IProductRecipe)?.Product);
                case "Workplan":
                    return new WorkplanResolver();
                default:
                    return null;
            }
        }

        private readonly Regex _wpOperationRegex = new Regex(@"^Step\[(?<name>\w)\]");

        /// <inheritdoc />
        protected override IBindingResolverChain AddToChain(IBindingResolverChain resolver, string property)
        {
            resolver = resolver.Extend(new PartLinkShortCut());

            if (property == nameof(IIdentity.Identifier))
                return resolver.Extend(new IdentifierShortCut());

            var wpResolver = resolver as WorkplanResolver;
            if (wpResolver == null)
                return base.AddToChain(resolver, property);

            var match = _wpOperationRegex.Match(property);
            if (!match.Success)
                return base.AddToChain(resolver, property);

            var argument = match.Groups["name"].Value;
            var stepResolver = new StepResolver(argument);
            return resolver.Append(stepResolver);
        }

        private class WorkplanResolver : BindingResolverBase
        {
            protected override object Resolve(object source)
            {
                source = (source as IWorkplanRecipe)?.Workplan;
                return source;
            }

            protected override bool Update(object source, object value)
            {
                throw new InvalidOperationException("Workplan can not be updated!");
            }
        }

        private class StepResolver : BindingResolverBase
        {
            private readonly string _stepName;

            public StepResolver(string stepName)
            {
                _stepName = stepName;
            }

            protected override object Resolve(object source)
            {
                var workplan = ((IWorkplan)source);
                var step = workplan.Steps.FirstOrDefault(s => s.Name == _stepName);
                return step;
            }

            protected override bool Update(object source, object value)
            {
                throw new InvalidOperationException("Step can not be updated!");
            }
        }
    }
}