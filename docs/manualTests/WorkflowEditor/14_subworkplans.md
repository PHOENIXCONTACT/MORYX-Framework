# Test Case 14 - Subworkplans

## Expected preconditions

There are two existing Workplans both of which are currently opened in a session (as is the result of [test case 3](03_open_an_existing_workplan.md) executed twice).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Drag and drop a subworkplan step from the list on the editor area. Take the workplan that you are not currently looking at in the editor | The step should appear on the canvas | The list on the left side should contain all workplans that can also be seen in the management page. When you try to add the workplan itself as a subworkplan, the ui will prompt an error. |
| Make sure the subworkplan step is selected | The panel on the right shows a button to navigate to the subworkplan ||
| Click on the button | The UI switches to the other opened session ||
| Close that workplan and return to the workplan you added the subworkplan step to. Repeat step 2. | The workplan is opened in a new session and the UI switches to the other new session. ||
