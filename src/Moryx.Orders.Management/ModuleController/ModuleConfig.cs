// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Orders.Advice;
using Moryx.Orders.Assignment;
using Moryx.Orders.Dispatcher;
using Moryx.Orders.Management.Advice;
using Moryx.Orders.Management.Assignment;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// The <see cref="ModuleController"/>'s module configuration.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ModuleConfig"/>
        /// </summary>
        public ModuleConfig()
        {
            CountStrategy = DoNotReplaceScrapStrategy.StrategyName;

            ProductAssignment = new ProductAssignmentConfig
            {
                PluginName = nameof(DefaultProductAssignment)
            };

            PartsAssignment = new PartsAssignmentConfig
            {
                PluginName = nameof(NullPartsAssignment)
            };

            RecipeAssignment = new RecipeAssignmentConfig
            {
                PluginName = nameof(DefaultRecipeAssignment)
            };

            OperationValidation = new RegexOperationValidationConfig();

            OperationDispatcher = new OperationDispatcherConfig
            {
                PluginName = nameof(SingleJobOperationDispatcher)
            };

            Documents = new DocumentsConfig
            {
                DocumentLoader = new DocumentLoaderConfig
                {
                    PluginName = nameof(NullDocumentLoader)
                }
            };

            Advice = new AdviceConfig
            {
                AdviceExecutor = new AdviceExecutorConfig
                {
                    PluginName = nameof(NullAdviceExecutor)
                }
            };

            Users = new UsersConfig();
        }

        /// <inheritdoc cref="ICountStrategy"/>
        [DataMember, Description("Strategy to decide how to count parts")]
        [ModuleStrategy(typeof(ICountStrategy))]
        [PluginNameSelector(typeof(ICountStrategy))]
        public string CountStrategy { get; set; }

        /// <inheritdoc cref="IProductAssignment"/>
        [DataMember]
        [Description("Will be used to assign a product to the operation")]
        [ModuleStrategy(typeof(IProductAssignment))]
        [PluginConfigs(typeof(IProductAssignment))]
        public ProductAssignmentConfig ProductAssignment { get; set; }

        /// <inheritdoc cref="IPartsAssignment"/>
        [DataMember]
        [Description("Will be used to assign the product parts to the operation")]
        [ModuleStrategy(typeof(IPartsAssignment))]
        [PluginConfigs(typeof(IPartsAssignment))]
        public PartsAssignmentConfig PartsAssignment { get; set; }

        /// <inheritdoc cref="IProductAssignment"/>
        [DataMember]
        [Description("Will be used to assign recipes to the operation")]
        [ModuleStrategy(typeof(IRecipeAssignment))]
        [PluginConfigs(typeof(IRecipeAssignment))]
        public RecipeAssignmentConfig RecipeAssignment { get; set; }

        /// <inheritdoc cref="IOperationValidation"/>
        [DataMember]
        [Description("Will be used to validate a created operation.")]
        [ModuleStrategy(typeof(IOperationValidation))]
        [PluginConfigs(typeof(IOperationValidation))]
        public OperationValidationConfig OperationValidation { get; set; }

        /// <inheritdoc cref="IJobHandler"/>
        [DataMember, Description("Dispatcher used by the operation to handle jobs")]
        [ModuleStrategy(typeof(IOperationDispatcher))]
        [PluginConfigs(typeof(IOperationDispatcher))]
        public OperationDispatcherConfig OperationDispatcher { get; set; }

        /// <summary>
        /// Will be used configure document handling
        /// </summary>
        [DataMember, Description("Will be used configure document handlig")]
        public DocumentsConfig Documents { get; set; }

        /// <summary>
        /// Will be used configure user handling
        /// </summary>
        [DataMember, Description("Will be used configure user handling")]
        public UsersConfig Users { get; set; }

        /// <summary>
        /// Configuration of the user assignment
        /// </summary>
        [DataMember, Description("Configuration of the advice manager")]
        public AdviceConfig Advice { get; set; }

        /// <summary>
        /// Limits the maximum number of running operations.
        /// </summary>
        [DataMember, Description("Limits the maximum number of running operations."), DefaultValue(1)]
        public int MaxRunningOperations { get; set; }

        /// <summary>
        /// Configuration of notifications
        /// </summary>
        [DataMember, Description("Configuration of notifications")]
        public OperationNotificationConfig Notifications { get; set; }
    }
}
