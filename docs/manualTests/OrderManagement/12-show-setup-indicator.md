# Test Case 12 - Show setup indicator at an operation

It should be possible to type in an unknown materialnumber which will send to the backend with the creation context to have the possibility to test dynamic product data creation solutions.
The old ui allows to type in the material number directly in the combobox which will be used to filter but will use the number even if there is no product with this number.

## Preconditions

No machine has is setup for a material.

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Create two operations for both products | There should be two successfully created operations for two different products | |
| Successfull created operations are updated automatically to get information about a necessary setup | There should be an indicator at every operation | |
| Start the production of the first operation | You will be asked to confirm the setup and after that the setup indicator should disappear but the second operation should still have the indicator | |
| Create another operation with the same product as the started operation | The new operation should not have an setup indicator because the machine is already setup for the product | |
| Interrupt the first operation and start the second one with the different product | You will be asked again to confirm the setup and after that the setup indicator should also disappear | |