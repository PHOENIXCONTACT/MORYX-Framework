# Test Case 6 - Create and start an operation

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the **Add** button in the bottom right corner | The **New Order** Dialog opens | The CREATE button, the add button and the Recipe field are disabled |
| Fill out all the blanks and press the **Add** button | The number behind *Added Operations* increases by one. The dialog dynamically scales and if it gets too big you can now scroll. The information of the added operation appear at the bottom | |
| Click **CREATE** to create the operation | The dialog closes and operation appears on the screen | The fields *Success*, *Running* and *Scrap* are still zero. The colour is petrol|
| Expand the operation | The state should be *Ready* and the button **BEGIN** must be available. Check if product and recipe match | |
| Click the button **BEGIN** | The *Begin the Production* dialog opens | It should contain the matching operation and product |
| Add an amount and click on **BEGIN** | The dialog closes and the production starts. | It is possible to click **BEGIN** again while the production is running. This would would just increase the amount to be produced |
