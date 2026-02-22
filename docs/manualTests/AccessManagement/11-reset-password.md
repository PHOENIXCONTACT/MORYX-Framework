# Test Case 11 - Reset the Password of a User

## Expected preconditions

Make sure you are logged in using an administrator account. There should be an existing user in addition to the default admin user (as is the result of [test case 10](10-register.md)).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the Action dropdown menu and select the Edit button of the user in the table | You should be navigated to a new page to edit the user | |
| Click on the Generate button next to the Password Reset Token field | The field should be filled with a reset token ||
| Click on the eye icon to make the reset token readable | You should see a readable token in the field ||
| Copy the token and click on logout | You should be navigated back to the Welcome page ||
| Click on Login and then on the Forgot Password link | You should be navigated to a new page to reset your password |
| Fill in a User Name that does not exist | An error message should be prompted notifying you that the User does not exist | Similar error messages should be prompted, if the token is not correct or the password has less then 6 characters, no upper case letter or no number |
| Fill in a valid set of inputs and click on the Save button | You should be navigated back to the Login page ||
| Login using the new credentials | You should be able to login ||