# Test Case 5 - Duplicate a product

## Expected preconditions
A CircuitBoardType, HoudingType and a ArticleType containing those two were imported (see Test Case 1 or use the *DemoImport*).
   
## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Select an 'Article' type and click on **duplicate** in the left side bar  | The Duplicate Product Dialog opens | The title contains the name of the type to be duplicated |
| Fill in the blanks and click **duplicate** | The user will be automatically routed to the newly created product, which has the same properties, parts and recipes as the original one. The *References* are empty. | The identifier and revision combination has to be unique. The button **duplicate** is grayed out until all blanks are filled |
