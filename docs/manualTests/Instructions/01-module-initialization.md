# Test Case 1 - Module Initialization

## Preconditions

If a cookie `moryx-client-identifier` exists, delete it.

## Steps

| Step/Instruction | Expected Result |
|------------------|-----------------|
| Open/refresh the instructions UI | The configuration dialog pops up |
| Submit the dialog by clicking on an instructor name | The instructors name is stored within a  cookie called `moryx-client-identifier` |
