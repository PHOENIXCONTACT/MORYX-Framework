# Test Case 3 - Filter Processes

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Start the production of one operation in the orders module| The state of the operation switches to *Running*||
|Show the list of jobs by pressing the three dots in the top of the shell and then the *Processes* button in the context menu| The shell switches to the processes module and will show at least two jobs.||
|Press on the first job to expand it| There must be a list of some processes on the left side. Some could be completed and some running. The switch at the bottom is turned on by default.||
|Wait a little bit until a processes is completed|The completed process must be still visible because the switch in the bottom is turned on by default.||
|Turn the switch at the bottom of the job off|The completed processes are not visible||
|Turn the switch at the bottom of the job on|The completed processes are available again||