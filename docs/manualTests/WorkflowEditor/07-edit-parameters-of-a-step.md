# Test Case 7 - Edit Parameters of a step

## Expected preconditions

There is an existing Workplan that is currently opened in the Workplan Editor with a **Test** step in it (as is the result of [test case 5](05-add-a-new-step-to-the-workplan.md)).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on a node on the canvas | The node becomes visibly selected and a query parameter is added to the route including the id of the step. An area opens on the right side of the editor showing the step parameters. ||
| Change the values in the fields and click anywhere on the canvas or on another step | The area closes ||
| Repeat step 1 and 2 | The area opens and displays the previously changed values ||
