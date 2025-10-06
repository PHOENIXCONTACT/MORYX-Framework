// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.ControlSystem.Capabilities;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Modules;
using Moryx.Tools;
using Moryx.Workplans;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Generic setup trigger for <see cref="T:Moryx.Resources.Assemble.AssembleCell" />
    /// </summary>
    [ExpectedConfig(typeof(AssembleSetupTriggerConfig))]
    [Plugin(LifeCycle.Transient, typeof(ISetupTrigger), Name = nameof(AssembleSetupTrigger))]
    public class AssembleSetupTrigger : SetupTriggerBase<AssembleSetupTriggerConfig>
    {
        private VisualInstructionBinder _instructionBinder;

        private Func<ICapabilities> _capabilitiesConstructor;

        private IBindingResolver _sourceResolver;
        private IPropertyAccessor<ICapabilities, object> _descriptorAccessor;


        /// <inheritdoc />
        public override SetupExecution Execution => Config.Execution;

        /// <inheritdoc />
        public override void Initialize(SetupTriggerConfig config)
        {
            base.Initialize(config);

            // Validate type and create constructor delegate
            if (string.IsNullOrWhiteSpace(Config.RequiredCapabilityType))
                throw new NullReferenceException($"{nameof(AssembleSetupTriggerConfig.RequiredCapabilityType)} is null.");

            var requiredAssembleCapabilitiesType = ReflectionTool
                .GetPublicClasses<AssembleCapabilities>(type => type.Name == Config.RequiredCapabilityType)
                .FirstOrDefault();

            if (requiredAssembleCapabilitiesType == null)
                throw new InvalidConfigException(Config.RequiredCapabilityType, $"{nameof(AssembleSetupTriggerConfig.RequiredCapabilityType)} was not found!");

            _capabilitiesConstructor = ReflectionTool.ConstructorDelegate<ICapabilities>(requiredAssembleCapabilitiesType); ;

            // Validate property and create accessor
            if (string.IsNullOrWhiteSpace(Config.ValueSource))
                throw new NullReferenceException($"{nameof(AssembleSetupTriggerConfig.ValueSource)} is null.");

            _sourceResolver = RecipeResolverFactory.Create(Config.ValueSource);

            if (string.IsNullOrWhiteSpace(Config.TargetPropertyName))
                throw new NullReferenceException($"{nameof(AssembleSetupTriggerConfig.TargetPropertyName)} is null.");

            var targetProperty = requiredAssembleCapabilitiesType.GetProperty(Config.TargetPropertyName);
            if (targetProperty == null)
                throw new ArgumentException($"{nameof(AssembleSetupTriggerConfig.TargetPropertyName)} is not available in {Config.RequiredCapabilityType}");

            // Make custom descriptor instance
            _descriptorAccessor = ReflectionTool.PropertyAccessor<ICapabilities>(targetProperty);

            // Only create resolve if config was valid
            if (!string.IsNullOrEmpty(Config.Instruction))
                _instructionBinder = new VisualInstructionBinder(new[] {new VisualInstruction
                {
                    Type = InstructionContentType.Text,
                    Content = Config.Instruction
                }}, RecipeResolverFactory);
        }

        /// <inheritdoc />
        public override SetupEvaluation Evaluate(IProductRecipe recipe)
        {
            var currentCapabilities = _capabilitiesConstructor();
            var targetCapabilities = _capabilitiesConstructor();

            var value = _sourceResolver.Resolve(recipe);
            _descriptorAccessor.WriteProperty(targetCapabilities, value);

            return new SetupEvaluation.Change(Config.SetupClassification, currentCapabilities, targetCapabilities);
        }

        /// <inheritdoc />
        public override IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe)
        {
            var requiredCapabilities = _capabilitiesConstructor();
            var targetCapabilities = _capabilitiesConstructor();

            var value = _sourceResolver.Resolve(recipe);
            _descriptorAccessor.WriteProperty(targetCapabilities, value);

            var setupTask = new AssembleSetupTask
            {
                Parameters = new AssembleDescriptorSetupParameters
                {
                    RequiredCapabilities = requiredCapabilities,
                    TargetCapabilities = targetCapabilities,

                    PropertyName = Config.TargetPropertyName,
                    Value = value,

                    Instructions = _instructionBinder.ResolveInstructions(recipe)
                }
            };

            return new[] { setupTask };
        }
    }
}

