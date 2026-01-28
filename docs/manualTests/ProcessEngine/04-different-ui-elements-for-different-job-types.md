# Test Case 4 - Different ui elements for different job types

## Preconditions

* Make shure there is no assembly resource which is already setup for a product

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Start the production of one operation in the orders module| The state of the operation switches to *Running*||
|Show the list of jobs by pressing the three dots in the top of the shell and then the *Processes* button in the context menu| The shell switches to the processes module and will show at least two jobs. There must be jobs which look different. The *production job* contains the order- and operation number. The *setup job* basically shows the progress||
|Show the current state of the setup steps by pressing on the first setup job.| The job must be expand and will show all containing steps with an icon which will switch the color depending if it is done, running or pending.||