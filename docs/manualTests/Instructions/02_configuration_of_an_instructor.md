# Test Case 2 - Configuration of an Instructor

## Preconditions

Have a list of multiple instructors and one of them configured (saved within the `moryx-client-identifier` cookie).

- Requires creation of a second instructor in the Demo

## Steps

| Step/Instruction | Expected Result |
|------------------|-----------------|
| Verify, that a configuration button is visible |  |
| Click the button | The configuration dialog pops up. The configured instructor should be selected. |
| Select a different instructor from the list | The cookie  `moryx-client-identifier` should change its content to match the selected instructor. |
