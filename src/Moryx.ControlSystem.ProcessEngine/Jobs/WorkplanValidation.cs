// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.Tools;
using Moryx.Workplans;
using Moryx.Workplans.Validation;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    internal class WorkplanValidation
    {
        public static IReadOnlyList<string> Validate(IWorkplan workplan)
        {
            // Validate the recipes workplan
            var errors = new List<string>();
            var validation = WorkplanInstance.Validate(workplan, ValidationAspect.DeadEnd | ValidationAspect.LoneWolf);
            if (!validation.Success)
            {
                errors.AddRange(validation.Errors.Select(e => e.Print(workplan)));
            }

            // Find any step, that has an empty output
            foreach (var step in workplan.Steps)
            {
                if(step.Outputs.Any(o => o is null))
                    errors.Add(string.Format(Strings.WorkplanValidation_OutputUnset, step.Name));
            }

            // Validate step parameters
            foreach (var taskStep in workplan.Steps.OfType<ITaskStep<IParameters>>())
            {
                var properties = taskStep.Parameters.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttribute<ValidationAttribute>();
                    var result = attribute?.GetValidationResult(property.GetValue(taskStep.Parameters), new ValidationContext(taskStep.Parameters)
                    {
                        MemberName = property.Name,
                        DisplayName = property.GetDisplayName() ?? property.Name
                    });
                    if (!string.IsNullOrEmpty(result?.ErrorMessage))
                        errors.Add(result.ErrorMessage);
                }
            }

            return errors;
        }
    }
}
