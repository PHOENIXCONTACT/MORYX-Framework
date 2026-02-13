# Test Case 1 - Read the configuration of a Measurand

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the **Processs Data** button in the shell| The UI switches to the first step of the stepper `Select the Measurand`. |  |
| Click on any of the 3 cards | The title of the first step of the stepper changes. It refers to the card you selected. The page changes to the second step the of the stepper `Select the Data`. It shows a list of already configured data bindings on the Measurand. All switches are switched on. | While hovering over the card, the mouse symbol should change. When returning from one of the next steps the card should be highlighted. |
| Click on one of the expandable list entries. | The list entry should expand and display the "Data Binding" value and a "Data Type" dropdown. | The "Data Binding" field cannot be edited.  |
| Click on **NEXT** | The stepper shows the third step `Select the Target`. It shows 2 Listeners as possible targets. One of which is already selected. | |
| Click on **SAVE** | The stepper jumps back two the first step. The other steps in the stepper navigation are grayed out again. A snackbar pops up notifying you that the configuration was saved. |
