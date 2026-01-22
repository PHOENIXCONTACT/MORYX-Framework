# Test Case 2 - Add a Binding and a Listener

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the **Processs Data** button in the shell| The UI switches to the first step of the stepper `Select the Measurand`. |  |
| Click on any of the 3 cards | The page changes to the second step the of the stepper `Select the Data`. | |
| Click in the Input field that promps "Add a new Data Binding" | The field should expand horizontaly. A list of suggestions should appear below the field |  |
| Type in a set of letters | The list of suggestions should shrink only containing entries which include the provided letters. |  |
| Click on the add binding button to the right of the input field, while the input of the field differs from all suggestions | The suggestions section collapses. No new entry is added to the list above. The input field indicates an error. | Make sure you didn't type in a valid binding. |
| Remove the letters and select one of the suggestions. | The input field should contain the value of the selected suggestion. |  |
| Click on the add binding button to the right of the input field | The suggestions section collapses. A new entry is added to the list above. | The value of the selected suggestion can be found in the "Name" and "Data Binding" field of the new entry. The "Data Type" is set to the *Field* value. |
| Change the values of the "Name" and "Data Type" fields and click on **NEXT** | The stepper shows the third step `Select the Target`. | |
| Click on **PREVIOUS** | The stepper shows the second step `Select the Data`. The previously added entry with changed "Name" and "Data Type" is in the list. | |
| Click on **NEXT** | The stepper shows the third step `Select the Target`. | |
| Select the second entry in the list of targets | Both targets are selected. | |
| Click on **SAVE** | The stepper jumps back two the first step. The other steps in the stepper navigation are grayed out again. A snackbar pops up notifying you that the configuration was saved. | |
| Go through the steps of the stepper again and check that the added binding and target are now included | | |

