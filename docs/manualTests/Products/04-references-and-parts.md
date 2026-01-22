# Test Case 4 - References and Parts

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Select a 'Housing' or a 'CircuitBoard' type and open **References** | An 'Article' type is listed there ||
| Select the 'Article' type. | The type has 4 tabs on the right side (Properties, Parts, Recipes, References). The **Properties** tab is selected | |
| Select the **References** tab | *This product is not used in other products* is written there | |
| Select the **Parts** tab | The expansion panels contain the 'Housing' and 'CircuitBoard' types |  |
| Click on the **edit** button on the left side (the pen) | Inputs will turn black instead of grey and can be edited. There should only be 3 icons on the left sidebar. | The input *Identifier* in the header stays grey. |
| Open the card for Housing and click **REMOVE** | The *REMOVE* button is grayed out and there is no Housing written there anymore. ||
| Click on **REPLACE** | The *Select Part Dialog* opens. The button *SELECT* is grayed out until a product is selected. The list contains all Housings. The style of the name is `<identifier>-<revision> <name>`. | |
| Type in something into the search bar | The list of possible HousingTypes will be filtered automatically without pressing enter | |
| Select a Housing and press **SELECT** | The Housing will appear in the card ||
| Select a ControlCabinet and navigate to the Parts tab. Select the card for the ProductLink | On the left side there is a list of all Articles contained in the ControlCabinet. The button *REMOVE* is grayed out as long there is no Article selected | |
| Click on **ADD** on the right bottom | The *Select Part Dialog* opens | The list contains also types, which were already added. This is only the case for parts containing a list of several elements |
| Select a type and press **SELECT** | The type appears in the list on the left side | |
| Select the first Article and press **REMOVE** | The Article vanishes from the list and the MORYX logo can be seen where the properties were originally || 
| Press **save** in the right side bar |All changes were saved correctly ||
| Open the Article, which was removed from the parts of the watch, in the tree and open the **References** tab | There is written *This product is not used in other products*||
