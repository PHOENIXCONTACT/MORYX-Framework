# Test Case 6 - Manage Permissions

## Expected preconditions

Make sure you are logged in using an administrator account and have navigated to the roles page. There should also be an existing role besides the SuperAdmin role (as is the result of [test case 4](04-add-role.md)) as well as an existing permission (as is the result of [test case 2](02-add-permission.md)).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the Action dropdown menu and select the Manage Permissions button of the role in the table | You should be navigated to a new page to manage the permissions | The SuperAdmin role should not have this option available |
| Select the permission in the list and click save | An indicator should be displayed to notify you of a successful update of the permissions. In the Permissions page you should also see that the modified role should be displayed in the respective column of the permission you selected. ||
| Deselect the permission in the list and click save | An indicator should be displayed to notify you of a successful update of the permissions. In the Permissions page you should also see that the modified role should not be displayed in the respective column of the permission you selected anymore. | You might need to refresh the page if you had it open in a second tab. |
