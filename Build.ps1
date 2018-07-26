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
$env:MARVIN_BUILDNUMBER = [int]::Parse($env:MARVIN_BUILDNUMBER) + 364;

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Initialize Toolkit
Invoke-Initialize -Version (Get-Content "VERSION");

if ($SetAssemblyVersion) {
    Set-AssemblyVersions;
}

if ($Build) {
    Invoke-Build ".\MarvinPlatform.sln"
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\src\Marvin.Runtime.Console\bin\$env:MARVIN_BUILD_CONFIG\HeartOfGold.exe";
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