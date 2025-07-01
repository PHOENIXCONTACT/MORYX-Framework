# Test Case 3 - Execute and Instruction

## Preconditions

Set the first assembly cell to 'Manual Mode' in the ResourceManagement. Set the failure rate in the first Test resource in the Resource Management to 100%. **Create** and **Start** an Order in the Ordermanagement with two parts. Switch to the Worker Support Module. 

## Steps

| Step/Instruction | Expected Result |
|------------------|-----------------|
| Wait for the first instruction to pop up (max 5 sec.) | A visual instruction with 2 Media contents and a text instruction is shown.   |
| Use the buttons to switch between the media contents | All contents should be displayed and rendered correctly without changing the layout. |
| Click on the action button of the instruction and wait until 2 different instructions appeared. | Another assembly instruction and a manual soldering instruction should appear one after the other. |
| Use your mouse two sipe horizontally. | The displayed instruction should change between the two already mentioned instructions. |
| Click one of the action buttons of one of the instructions. | Check if the respective process continued, e.g. in the Factory Monitor. (Either the assembly process should be done and continue to soldering, testing and manual soldering or the soldering process should be done and continue to packaging) |