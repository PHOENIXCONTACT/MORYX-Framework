# Test Case 3 - Create, edit and delete recipes

## Expected preconditions
A HousingType or CircuitBoardType was created (see Test Case 1).

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the type created in Test Case 1. | The properties, recipes and references tabs can be seen on the right side. | |
| Click on the **Recipes** tab| Recipe tab opens and there are no recipes yet. |  |
| Click on the **edit** button in the sidebar on the left | The button *Add Recipe* can be clicked||
| Click on **ADD RECIPE** | Add Recipe dialog opens | As long as not all inputs are filled in, the button *create* is greyed out.  |
| Select **ProductionRecipe** as *Recipe Type* | The Input *Workplan* should appear | Possible *Recipe Types* are ProductionRecipe, ProductRecipe and DemoRecipe. Only ProductionRecipe and DemoRecipe have the Input *Workplan*|
| Enter a recipe name and a workplan and create the recipe. | The recipe appears in the recipe list and is selected directly. | The input *Workplan* should contain 2 possible workplans. If it does not contain any workplans execute the DemoImporter or create two workplans on your own. |
| Create another recipe | The recipe appears in the recipe list and is selected directly. | |
| Enter something in the properties, change between the recipes and click save | *Add Recipe* is greyed out again. The recipes are in the recipe list| The id of the recipes in the url may change|
| Select the first recipe and click on the **edit** button in the sidebar on the right | The button *Add Recipe* can be clicked and the properties of the recipe can be edited||
| Delete the first recipe by clicking on the trash bin next to the recipe name | The recipe disappears from the list and the next recipe appears in the recipe details view right next to recipe list | |
| Save the changes| The recipe stays gone and the trash bin next to the second recipe is grayed out || 
