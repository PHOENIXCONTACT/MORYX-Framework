# Visual Instructions

Defines the types and API for resources, that display visual instructions to users. For more information on how to use visual instructions in your own activities, take a look [here](VisualInstructions.md).

## Content-Types

- Text: The Content consists of text that may be displayed without further modifications.
- Media: For images or other files the content and preview consists of a URL and the content-type specifies "Media". The URL could be from the internal media store or some other external source.

## Display Instructions

Display instructions are instructions without a result. The instruction will be completed if the instruction was displayed by the client.

### Display Clear

Every displayed instruction have to be cleared manually!

### Sample

````cs
[ResourceReference(ResourceRelationType.Extension)]
public IVisualInstructor Instructor { get; set; }

public void DisplaySomething()
{
    var instructions = new []
    {
        new VisualInstruction {Content = "Hello World", Type = InstructionContentType.Text},
        new VisualInstruction 
        {
            Type = InstructionContentType.Media
            Content = "https://moryx-server/files/da1512-1241/master",
            Preview = "https://moryx-server/files/da1512-1241/master/preview",
        }
    };

    var instructionId = Instructor.Display("Some Name", instructions)
    // Work => Instruction no longer necessary => Clear
    Instructor.Clear(instructionId);
}
````

## Execute Instructions

Execute instructions are instructions with possible results. The client will display several buttons for each results. There are multiple ways to define the instruction results.

### Results

An implementation of `IInstructionResults` must be used. An sample implementation is the `EnumInstructionResult`. It will convert an enum type to possible results. I does the conversion with respect of the `DisplayAttribute` which will give the possibility to hide a result from the enum or add special texts for the buttons.

### Execute Clear

An executed instruction have not to be cleared. It will be cleared automatically after calling the given callback and if an button was pressed on the client. It can however be cleared if an event renders the instruction obsolete.