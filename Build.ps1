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
    [switch]$Publish,

    [switch]$PublishSymbols
)

# Extend version number
$env:MARVIN_BUILDNUMBER = [int]::Parse($env:MARVIN_BUILDNUMBER) + 364;

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Initialize Toolkit
Invoke-Initialize -Version (Get-Content "VERSION");

if ($SetAssemblyVersion) {
    Set-AssemblyVersions @("\\Templates\\");
}

if ($Build) {
    Invoke-Build ".\MarvinPlatform.sln"
    Install-EddieLight "3.0.5" "Runtime\Marvin.Runtime.Console\bin\$env:MARVIN_BUILD_CONFIG\EddieLight\";
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\Runtime\Marvin.Runtime.Console\bin\$env:MARVIN_BUILD_CONFIG\HeartOfGold.exe";
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
    Invoke-PackAll
}

if ($Publish) {
    Invoke-Publish
}

if ($PublishSymbols) {
    # This is temporary until the real symbol storage and nuget packages are active
    $storage = "$RootPath\Artefacts\Symbols";
    if (-not (Test-Path $storage)) {
        try {
            New-Item $storage -ItemType Directory
        }
        catch {
            Write-Host "Storage $storage cannot be created.";
            exit 1;
        }
    }
    Publish-PDBs -Project "MarvinPlatform3" -Storage $storage;
}

Write-Host "Success!"