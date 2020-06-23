param (
    [switch]$SetAssemblyVersion,
    [switch]$Build,

    [switch]$SmokeTests,
    [switch]$UnitTests,
    [switch]$IntegrationTests,
    [switch]$SystemTests,

    [switch]$CoverReport,
    [switch]$GenerateDocs,

    [switch]$Pack,
    [switch]$Publish
)

# Extend version number
$env:MORYX_BUILDNUMBER = [int]::Parse($env:MORYX_BUILDNUMBER) + 364;

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Set build version to 15 if build in CI 
# CI runner have problems with satelite assemblies in msbuild-14
$MsBuildVersion = "latest"

# Initialize Toolkit
Invoke-Initialize -Version (Get-Content "VERSION");

if ($SetAssemblyVersion) {
    Set-AssemblyVersions;
}

if ($Build) {
    Invoke-Build ".\MoryxPlatform.sln"
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\src\StartProject\bin\$env:MORYX_BUILD_CONFIG\StartProject.exe";
    Invoke-SmokeTest $runtimePath 3 6000
}

if ($UnitTests) {
    Invoke-CoverTests -SearchFilter "*.Tests.csproj"
}

if ($IntegrationTests) {
    Invoke-CoverTests -SearchFilter "*.IntegrationTests.csproj"
}

if ($SystemTests) {
    Invoke-Nunit -SearchFilter "*.SystemTests.csproj"
}

if ($CoverReport) {
    Invoke-CoverReport
}

if ($GenerateDocs) {
    Invoke-DocFx
}

if ($Pack) {
    Invoke-PackAll -Symbols
}

if ($Publish) {
    Invoke-Publish
}

Write-Host "Success!"