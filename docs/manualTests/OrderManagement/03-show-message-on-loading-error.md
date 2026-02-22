# Test Case 3 - When an error occurs while loading messages from the backend, a proper message will be displayed

## Preconditions

In your browsers network tab, set the throttling to `offline`.

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the *Messages* button of an operation | Drawer should show an error message |  |
| Click on the *Messages* button of an operation **multiple times** | The number of log messages must stay the same |  |
