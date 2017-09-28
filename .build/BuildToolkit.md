Build Script Toolkit
=======================

The script tookit was created to reduce build server configuration to a minimum and add the speciallities to the project. The build toolkit is a powershell script which contains several functions to automate the build process.

Another main feature is to reduce the commited executable to 0. Every executable has a high load on the VCS system and should never be commited or pushed. 

# Main Concept
Do never commit or push libraries or tools! All tools should be downloaded or loaded while the build process. All tools or libraries are existent as Nuget-Packages. 

Currently used tools: 

| Tool | Description |
|------|-------------|
| MSBuild | Build tool set for managed code (C#) |
| Nuget | NuGet is an open-source package manager designed for the Microsoft development platform  |
| Nunit | NUnit is an open source unit testing framework for Microsoft .NET |
| OpenCover | OpenCover is a code coverage tool for .NET 2 and above |
| Doxygen | Doxygen is a tool for generating documentation from C# sources |
| OpenCoverToCoberturaConverter | Converts OpenCover reports to Cobertura reports |
| ReportGenerator | Converts the OpenCover results to a human readable HTML page. |

For downloading Nuget-Packages, we need Nuget itself. So the first Step is to download Nuget from a reliable source. With nuget all build dependencies can be resolved.

# Available Functions

| Function | Description |
|----------|-------------|
| Install-Nuget | Will download nuget.exe from a specified source. |
| Write-Variables  | Will print the current build variables.  |
| Install-Tool | Will install a tool with Nuget. |
| Install-BuildTools | Will download all dependencies which are needed for the build process |
| Update-AssemblyInfo | Will set the version to the given AssemblyInfo.cs  |
| Invoke-Build | Restores Nuget packages for the given solution and starts the build with MSBuild |
| Invoke-Nunit | Will run Nunit tests with the Nunit console |
| Invoke-SmokeTest | Will start the runtime core smoke test  |
| Invoke-CoverTests | Starts the open cover tests with nunit |
| Invoke-DoxyGen | Generates whole documentation |
| Invoke-PackAll | Searches for .nuspec files and creates the packages |
| Invoke-Publish | Will publish packed Nuget packages to the configured source |

# Usage
You have to create an own Powershell-Script in the root folder of your projekt. 
Add the following parameter definition to the head of the script:

````ps1
param (
    [switch]$Build,
    [switch]$UnitTests,
    [switch]$IntegrationTests,
    [switch]$SystemTests,
    [switch]$SmokeTests,
    [switch]$CoverTests,
    [switch]$GenerateDocs,
    [switch]$Pack,
    [switch]$Publish,
    [string]$Version = "0.0.0.0",
    [string]$Configuration = "Debug",
    [int]$PortIncrement = 0
)
````

Now include the toolkit in you script:

````ps1
. ".build\BuildToolkit.ps1"
````

Now you should be able to build your project. Sample: 

````ps1
Install-BuildTools
Write-Variables
Update-AssemblyInfo "$RootPath\GlobalAssemblyInfo.cs"

if ($Build) {
    Invoke-Build ".\Tools.sln"
    Invoke-Build ".\Toolkit.sln"
    Invoke-Build ".\RuntimeCore.sln"
    Invoke-Build ".\ClientFramework.sln"
}
````