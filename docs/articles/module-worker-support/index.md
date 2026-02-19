# Visual Instructions

Defines the types and API for resources, that display visual instructions to users. For more information on how to use visual instructions in your own activities, take a look [here](visual-instructions.md).

## Content-Types

- Text: The Content consists of text that may be displayed without further modifications.
- Media: For images or other files the content and preview consists of a URL and the content-type specifies "Media". The URL could be from the internal media store or some other external source.

## Display Instructions

Display instructions are instructions without a result. The instruction will be completed if the instruction was displayed by the client.

### Display Clear

Every displayed instruction has either to be cleared manually or an auto clear timeout has to be provided, when `IVisualInstructor.Display()` is called.

### Sample

````cs
[ResourceReference(ResourceRelationType.Extension)]
public IVisualInstructor Instructor { get; set; }

public void DisplaySomething()
{
    var activeInstruction = new ActiveInstruction
    {
        Title = "Some Name",
        Instructions = new []
        {
            new VisualInstruction {Content = "Hello World", Type = InstructionContentType.Text},
            new VisualInstruction 
            {
                Type = InstructionContentType.Media
                Content = "https://moryx-server/files/da1512-1241/master",
                Preview = "https://moryx-server/files/da1512-1241/master/preview",
            }
        }
    };

    var instructionId = Instructor.Display(activeInstruction)
    // Work => Instruction no longer necessary => Clear
    Instructor.Clear(instructionId);
}
````

## Execute Instructions

Execute instructions are instructions with possible results. The client will display several buttons for each results. There are multiple ways to define the instruction results.

### Results

Possible results can be delcared by using instances of the `InstructionResult` class, which can be passed to the `ActiveInstruction.Results` property. Enums can be converted automatically to a collection of `InstructionResult` instances by using the `EnumInstructionResult` helper class. It does the conversion with respect to the `DisplayAttribute`, which allows to add special texts to the buttons. Additionally `EnumInstructionAttribute.Hide` can be used to skip an enum value during `InstructionResult` construction.

### Execute Clear

An executed instruction has not to be cleared. It will be cleared automatically after calling the given callback and if a button was pressed on the client. It can however be cleared if an event renders the instruction obsolete.
