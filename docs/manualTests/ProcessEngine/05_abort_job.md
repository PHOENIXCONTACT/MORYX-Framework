# Test Case 5 - Abort Job

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Start the production of one operation in the orders module| The state of the operation switches to *Running*||
|Show the list of jobs by pressing the three dots in the top of the shell and then the *Processes* button in the context menu| The shell switches to the processes module and will show at least two jobs.||
|Press on the first job to expand it| There must be a list of some processes on the left side. The abort button should be visible and activated at the button right side of the job.||
|Press at the abort button to send the system the signal to abort the job.|The process controller will abort all containing processes which can lead to broken articles and the job will be removed afterwards| If there were processes in the job that had not been started a new job will be created with the remaining number of articles. |
