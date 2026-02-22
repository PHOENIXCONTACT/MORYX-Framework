# Test Case 10 - Register a new User

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on Register | You should be navigated to the Register page ||
| Fill in the requested information using a password consisting only of letters and click register | You should see an error message notifying you that the password must contain at least one special character. The other two fields should keep their content. | Similar error messages should be prompted, if the password has less then 6 characters, no upper case letter or no number |
| Fill in a valid password and click register | You should be navigated back to the welcome screen and should not be logged in. |  |