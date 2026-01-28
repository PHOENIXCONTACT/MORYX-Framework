# Test Case 3 - Create, Edit and Delete References

## Expected preconditions

There is an existing Resource (as is the result of [test case 1](01-add-a-resource.md)).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the existing resource  | The **Properties**, **References** and **Methods** tabs can be seen on the right side | |
| Create a new resource as described in [test case 1](01-add-a-resource.md) | The new resource is created as a child under the first resource. | |
| Click on the **References** tab, then click on **Parent** card| References tab opens and there is a parent to the new resource. |  |
| Click on the **edit** button in the sidebar on the left | The drop down menu gets enabled ||
| Select *No target selected* from the menu and click the **save** button on the left | The tree structure in the left sidenav flattens and the resource looses its parent entry, | |
| Click the **Children** entry in the list of references | An empty table of references and related information appears on the right side of the screen |  
| Click the **edit** button in the sidebar on the left | A dropdown menu appears below the table containing a single entry ||
| Select the other resource in the dropdown menu | A new entry for the selected resource appears in the table ||
| Click the **save** button in the left side navigation | The two resources form an inverted tree compared to the one created at the beginning and the *Children* references contains a single entry ||
