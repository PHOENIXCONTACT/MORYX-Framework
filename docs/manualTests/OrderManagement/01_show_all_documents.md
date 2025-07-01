# Test Case 1 - Show all documents and open one

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|Create an order with an operation and choose the product for which documents have been prepared |The new operation is successfully created and available at the order ui. The new created operation must contain a *Documents* button||
|Open the document overview by pressing at the *Documents* button at the created operation|The UI must switch to an overview with all available documents at the left side||
|Open an image by pressing at an entry which depends to an image|The UI must open the image||
|Open a pdf by pressing at an entry which depends to a pdf file|The UI must open a pdf viewer to show the selected pdf||
|Open a word document by pressing at an entry which depends to a word file|The UI must download the file to open it locally||
|Open an excel document by pressing at an entry which depends to an excel file|The UI must download the file to open it locally||