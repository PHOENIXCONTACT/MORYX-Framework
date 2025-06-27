# Factory 6.x to 8.x

## Merged `ISetupTrigger` and `IMultiSetupTrigger` 
We already offered the possibility to create a list of setup tasks in a setup trigger. The API of the setup trigger was now merged and cleaned up.
- `ISetupTrigger.CreateStep(IProductionRecipe recipe` now returns `IReadOnlyList<IWorkplanStep>`. Issues could for example be resolved as follows:
```c#
/// MORYX 6
public IWorkplanStep CreateSteps(IProductRecipe recipe) {
...
return step;
}

/// MORYX 8
public IReadOnlyList<IWorkplanStep> CreateSteps(IProductRecipe recipe) {
...
return [...step];
}
```

## VisualInstruction API improvements
### IInstructionResults was removed
- The interface just made the instructions more complex and had no practical use  
*Note: `ActiveInstruction.PossibleResults` has before and is still providing the possible results of an instruction as strings*
  
### IVisualInstructor API was changed to improve extendability
- Interface methods now take an `ActiveInstruction` parameter instead of multiple seperate parameters
- You can create an `ActiveInstruction` using the information previously given to the instructor or keep using the extension methods
- Most previous method signature are still available via `VisualInstructorExtensions`
- Overloads taking `IInstructionResults` were removed. If you used them with `EnumInstructionResults` read along.
- Overloads taking `Action<int, ActivityStart>` callbacks were removed. If you used them read along.

### EnumInstructionResult was replaced with a static type providing conversion methods
- The type itself is not used anymore due to the changes mentioned above
- If you used a method on an `IVisualInstructor` providing an `EnumInstructionResult` you can use the new static methods on this type to get the possible results as strings
```c#
// MORYX 6
VisualInstructor.Execute("Some Title", someInstructions, new EnumInstructionResult(typeof(AskRotationPermissionResult), SomeAction));

// MORYX 8
VisualInstructor.Execute("Some Title", someInstructions, EnumInstructionResult.PossibleResults(typeof(AskRotationPermissionResult)),
  param => SomeAction(EnumInstructionResult.ResultToEnumValue(typeof(ArticleMountingStrategy), param.Result)));
```

### EnumInstructionAttribute.Title and the corresponding constructor were removed
- To set the display name of an enum value please use the default data annotation `[Display(Name = "Value's name")]`
