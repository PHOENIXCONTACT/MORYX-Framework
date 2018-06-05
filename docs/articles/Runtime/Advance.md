---
uid: Advance
---
Runtime Advance
===============

This tutorial targets more advanced users who feel the need to extend or modify the RuntimeCore and/or its behavior.

# Creating a RuntimeEnvironment

The different runtime modes provided by the RuntimeCore like Console, Windows service or SmokeTest are realized by separating HeartOfGold infrastructure from the Runtimes kernel interaction. In order to create custom behavior and usage of the kernel you may implement your own RuntimeEnvironment to perfectly fit your needs.

Minimal requirements:
* Implement IRuntimeEnvironment from the HeartOfGold.exe
* Decorate with [RuntimeEnvironment("NameOfYourRuntime")] attribute
* Build to Core directory
* Run HeartOfGold.exe with "-r=NameOfYourRuntime" argument

Sample environment:

````cs
[RuntimeEnvironment("NameOfYourRuntime")]
public class MyRuntime : IRuntimeEnvironment
{
    /// <summary>
    /// Service manager instance
    /// </summary>
    public IModuleManager ModuleManager { get; set; }

    /// <summary>
    /// Setup the environment by passing the command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public void Setup(string[] args)
    {
        // Arguments passed to the runtime can be parsed here
    }

    /// <summary>
    /// Run environment
    /// </summary>
    /// <returns>0: All fine - 1: Warning - 2: Error</returns>
    public int RunEnvironment()
    {
        // Call to ModuleManager kernel to start modules
        ModuleManager.StartServices();
        
        // Keep modules running
        Console.ReadLine();
        ModuleManager.StopServices();
        
        return 0;
    }
}
````