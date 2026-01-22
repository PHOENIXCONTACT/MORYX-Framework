# Test Case 3 - Remove a Binding and a Listener

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the **Processs Data** button in the shell| The UI switches to the first step of the stepper `Select the Measurand`. |  |
| Click on any of the 3 cards | The page changes to the second step the of the stepper `Select the Data`. | |
| Click on one of the switches in the list to remove the binding. | The switch is toggled. |  |
| Click on **NEXT** | The stepper shows the third step `Select the Target`. | |
| Click on **PREVIOUS** | The stepper shows the second step `Select the Data`. The entry with the toggled switch is no longer in the list | |
| Click on **NEXT** | The stepper shows the third step `Select the Target`. | |
| Deselect an entry in the list of targets | The target is deselected. | |
| Click on **SAVE** | The stepper jumps back two the first step. The other steps in the stepper navigation are grayed out again. A snackbar pops up notifying you that the configuration was saved. | |
| Go through the steps of the stepper again and check that the added binding and target are now included | | |
