# How to create a MORYX Web UI

In this tutorial, you'll be guided towards creating your own UI in the MORYX Launcher.
First step: Create a *Razor Class Library (RCL)* and reference it from your application project.
The second step highly depends on your own preferences, [here](HowToAddAnAngularApplicationToYourUI.md) you can get an example on how to continue adding a single page application as the content of your UI with Angular.

## Creating a Razor class Library

First of all, you need an RCL project **MyModule**.
For that, take a look into the [Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries?view=aspnetcore-8.0&tabs=visual-studio#create-an-rcl).
During creation set a check mark in order to support adding traditional Razor pages and views to this library.  
Within your RCL the only thing you need is a Razor page `MyModule.cshtml` containing the html, css and javascript that is your UI.
Move the **Pages/** directory containing it to the root directory of your project and remove the other directories.
Also the code behind file `Page.cshtml.cs` can be removed.  

Now, add a package reference to `Moryx` in the MyModule.csproj file.

```xml
// MyModule.csproj
<ItemGroup>
  ...
  <PackageReference Include="Moryx" />
  ...
</ItemGroup>
```

Lastly to appear in the navigation bar in the Launcher, replace the content of the component with this snippet

```csharp
// MyModule.cshtml
@page
@using Moryx.Asp.Integration
@using System.ComponentModel.DataAnnotations
@{
    ViewData["Title"] = "MyModule";
    ViewData["BaseRef"] = "MyModule/";

    Layout = "_Layout";
}

@* This lines defines the text which appears in the navigation bar for your module as well as its icon and description *@
@attribute [WebModule("MyModule", "check_circle"), Display(Name = "My Module", Description = "Description of My Module")]

<p>This could be your content</p>
```

>Note: The name of the file determines its path, make sure to have the baseref and the path given to the `WebModule` attribute in line with your page's name

Add all content you want on your page below the code.
This could for example mean serving an Agnular application.
Follow along in [How to add an Angular application to your UI](./02_HowToAddAnAngularApplicationToYourUI.md) for instructions how to achieve that.
