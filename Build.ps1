param (
    [switch]$Cleanup,
    [switch]$SetAssemblyVersion,
    [int]$BuildNumber = 0,
    [ValidateSet('Debug','Release')]
    [string]$Configuration = "Debug",

    [switch]$Build,
    [switch]$OptimizeCode,

    [switch]$SmokeTests,
    [int]$PortIncrement = 0,
    [switch]$UnitTests,
    [switch]$SystemTests,
    [switch]$IntegrationTests,
    
    [switch]$GenerateDocs,

    [switch]$Pack,
    [switch]$Publish,

    [switch]$PublishSymbols
)

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Set Version
$versionInfo = (Get-Content "VERSION").Split("-");
$Version = $versionInfo[0] + "." + $BuildNumber;
$Preview = (&{If($versionInfo.Length -gt 1) {$versionInfo[1]} Else {""}})

# Initialize Toolkit
Invoke-Initialize -Cleanup $Cleanup;

if ($SetAssemblyVersion) {
    $assemblyVersion = Get-MajorMinorPatchVersion $Version;
    $informationalVersion = Get-InformationalVersion $Version $Preview;

    # Modify all assembly infos except some pathes
    Write-Step "Modifing AssemblyInfos to Version '$Version' and InformationalVersion '$informationalVersion'"
    $assemblyInfos = Get-ChildItem -Path $RootPath -include "*AssemblyInfo.cs" -Recurse | Where-Object { 
        ($_.FullName -notmatch "\\Templates\\" `
        -and $_.FullName -notmatch "\\.build\\" `
        -and $_.FullName -notmatch "\\.buildtools\\" `
        -and $_.FullName -notmatch "\\Tests\\" `
        -and $_.FullName -notmatch "\\IntegrationTests\\" `
        -and $_.FullName -notmatch "\\SystemTests\\")
    }
    
    Set-AssemblyVersions $assemblyInfos $assemblyVersion $informationalVersion $Configuration

    # Modify version of templates
    Set-VsixManifestVersion -VsixManifest "$RootPath\Runtime\Templates\DataModelWizard\source.extension.vsixmanifest" -Version $Version
    Set-VsTemplateVersion -VsTemplate "$PSScriptRoot\Runtime\Templates\DataModelTemplate\MyTemplate.vstemplate" -Version $Version
    Set-AssemblyVersion "$RootPath\Runtime\Templates\DataModelWizard\Properties\AssemblyInfo.cs" $Version $informationalVersion $Configuration
}

if ($Build) {
    Invoke-Build ".\MarvinPlatform.sln" $Configuration $OptimizeCode
    Install-EddieLight "3.0.3" "Runtime\Marvin.Runtime.Console\bin\$Configuration\EddieLight\";
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\Runtime\Marvin.Runtime.Console\bin\$Configuration\HeartOfGold.exe";
    Invoke-SmokeTest $runtimePath (&{If($Configuration -eq "Debug") {6} Else {4}}) 6000 $PortIncrement
}

if ($UnitTests) {
    Invoke-CoverTests -SearchFilter "*.Tests.csproj" -Configuration $Configuration
}

if ($IntegrationTests) {
    Invoke-CoverTests -SearchFilter "*.IntegrationTests.csproj" -Configuration $Configuration
}

if ($SystemTests) {
    Invoke-Nunit -SearchFilter "*.SystemTests.csproj" -Configuration $Configuration
}

if ($UnitTests -or $IntegrationTests) {
    Invoke-CoverReport
}

if ($GenerateDocs) {
    Invoke-DocFx
}

if ($Pack) {
    $NugetPackageVersion = Get-NugetPackageVersion -Version $Version -Preview $Preview;
    Invoke-PackAll $RootPath $NupkgTarget $NugetPackageVersion $Configuration
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