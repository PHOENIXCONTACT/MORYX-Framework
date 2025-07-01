# Test Case 6 - Complete Job

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Start the production of one operation in the orders module| The state of the operation switches to *Running*||
|Show the list of jobs by pressing the three dots in the top of the shell and then the *Processes* button in the context menu| The shell switches to the processes module and will show at least two jobs.||
|Press on the first job to expand it| There must be a list of some processes on the left side. The complete button should be visible and activated at the button right side of the job.||
|Press at the complete button to send the system the signal to complete the job.|The process controller will complete all containing processes but the order management will create a new job to reach the requested amount of articles.||
|Press at the complete button at the new created job to complete it.|The job will be completed and the order management will create a new job again.||