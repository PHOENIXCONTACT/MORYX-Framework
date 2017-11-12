param (
    [switch]$SetAssemblyVersion,
    [int]$BuildNumber = 0,
    [string]$Preview = "",
    [ValidateSet('Debug','Release')]
    [string]$Configuration = "Debug",

    [switch]$Build,
    [switch]$OptimizeCode,

    [switch]$SmokeTests,
    [switch]$CITests,
    [switch]$DailyTests,
    [int]$PortIncrement = 0,

    [switch]$GenerateDocs,

    [switch]$Pack,
    [switch]$Publish,

    [switch]$PublishSymbols
)


# Set Version
$Version = "3.0.0" + "." + $BuildNumber;

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Check execution of BuilToolkit
if ($lastexitcode -ne 0) { 
    exit $lastexitcode
} 

# Definition of local variables 
$openCoverFilter = "$RootPath\OpenCoverFilter.txt";

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
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\Runtime\Marvin.Runtime.Console\bin\$Configuration\HeartOfGold.exe";
    Invoke-SmokeTest $runtimePath (&{If($Configuration -eq "Debug") {6} Else {4}}) 6000 $PortIncrement
}

if ($CITests -or $DailyTests) {
    Invoke-CoverTests "$RootPath\Toolkit\Tests" $openCoverFilter $Configuration
    Invoke-CoverTests "$RootPath\Toolkit\IntegrationTests" -FilterFile $openCoverFilter -Configuration $Configuration

    Invoke-CoverTests "$RootPath\Runtime\Tests" $openCoverFilter $Configuration
    Invoke-CoverTests "$RootPath\Runtime\IntegrationTests" $openCoverFilter $Configuration
}

if ($DailyTests) {
    Invoke-Nunit "$RootPath\Toolkit\SystemTests" $Configuration
    Invoke-Nunit "$RootPath\Runtime\SystemTests" $Configuration
}

if ($CITests -or $DailyTests) {
    Invoke-CoverReport "$RootPath\" "MarvinPlatform"
}

if ($GenerateDocs) {
    Invoke-DoxyGen
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