# Test Case 5 - Remove a Resource

## Expected preconditions

There are two existing resources, one the child of the other (as is the result of [test case 3](03_create_edit_delete_references.md)).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Select the child resource, go to **References** and select the **Parent** | You can see the name of the parent resource | |
| Right click on the parent resource | A context menu appears at the cursor location ||
| Select the **Delete** menu entry | Resource Disappears from the tree and the details view of the child. The tree flattens ||