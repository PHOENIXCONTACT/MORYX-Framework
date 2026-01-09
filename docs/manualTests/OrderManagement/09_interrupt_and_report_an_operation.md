# Test Case 9 - Interrupt and report an operation

## Preconditions

There is a running order, where the production is not finished

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Expand the operation | **INTERRUPT** and **REPORT** are not disabled | |
| Click on **INTERUPT** | The *Interrupt Operation* dialog opens. | While the machine is producing, it's not possible to send a final report. You can just do a partial report |
| Click on **CANCEL** and then click on **REPORT** | The *Report Operation* dialog opens. Except for the title it looks nearly the same as the other dialog | While the machine is producing, it's not possible to send a final report. You can just do a partial report |
| Write 2 into the field *Success* and click on **REPORT** | The number in *Reported Success* increases by 2. The state of the operation stays in *Running* | |
| Wait until the operation has run through. | It should now be possible to send either partial or final reports | |
| Send a final report | The operation changes it state to *Completed*, turns grey and should vanish | |