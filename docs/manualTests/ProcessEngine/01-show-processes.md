# Test Case 1 - Show Processes

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Start the production of one operation in the orders module| The state of the operation switches to *Running*||
|Show the list of jobs by pressing the three dots in the top of the shell and then the *Processes* button in the context menu| The shell switches to the processes module and will show at least two jobs. The *production job* must containt the depending order- and operation number and the amount from the begin dialog of the production start. At the end there must be a *setup job*.||