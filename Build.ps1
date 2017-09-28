param (
    [switch]$Build,
    [switch]$SmokeTests,
    [switch]$CICoverTests,
    [switch]$DailyCoverTests,
    [switch]$GenerateDocs,
    [switch]$Pack,
    [switch]$Publish,
    [string]$Version = "3.0.0.0",
    [string]$Configuration = "Debug",
    [int]$PortIncrement = 0
)

# Load Toolkit
. ".build\BuildToolkit.ps1"

# Definition of local variables 
$openCoverFilter = "$RootPath\OpenCoverFilter.txt";

# Modify all assembly infos except of templates, .build, .buildtools
Write-Step "Modifing AssemblyInfos to Version '$Version'"
$assemblyInfos = Get-ChildItem -Path $Path -include "*AssemblyInfo.cs" -Recurse | Where-Object { 
    ($_.FullName -notmatch "\\Templates\\" -and $_.FullName -notmatch "\\.build\\" -and $_.FullName -notmatch "\\.buildtools\\")
}
Set-AssemblyVersions -Files $assemblyInfos -Version $Version

# Modify version of templates
Set-VsixManifestVersion -VsixManifest "$RootPath\Runtime\Templates\DataModelWizard\source.extension.vsixmanifest" -Version $Version
Set-VsTemplateVersion -VsTemplate "$PSScriptRoot\Runtime\Templates\DataModelTemplate\MyTemplate.vstemplate" -Version $Version
Set-AssemblyVersion -InputFile "$RootPath\Runtime\Templates\DataModelWizard\Properties\AssemblyInfo.cs" -Version $Version

if ($Build) {
    Invoke-Build ".\Toolkit.sln" $Configuration
    Invoke-Build ".\RuntimeCore.sln" $Configuration
}

if ($SmokeTests) {
    $runtimePath = "$RootPath\Build\ServiceRuntime\HeartOfGold.exe";
    Invoke-SmokeTest $runtimePath (&{If($Configuration -eq "Debug") {6} Else {4}}) 6000 $PortIncrement
}

if ($CICoverTests -or $DailyCoverTests) {
    Invoke-CoverTests "$RootPath\Toolkit\Tests" $openCoverFilter
    Invoke-CoverTests "$RootPath\Toolkit\IntegrationTests" $openCoverFilter

    Invoke-CoverTests "$RootPath\Runtime\Tests" $openCoverFilter
    Invoke-CoverTests "$RootPath\Runtime\IntegrationTests" $openCoverFilter
}

if ($DailyCoverTests) {
    Invoke-CoverTests "$RootPath\Toolkit\SystemTests" $openCoverFilter
    Invoke-CoverTests "$RootPath\Runtime\SystemTests" $openCoverFilter
}

if ($CICoverTests -or $DailyCoverTests) {
    Invoke-CoverReport "$RootPath\" "MarvinPlatform"
}

if ($GenerateDocs) {
    Invoke-DoxyGen
}

if ($Pack) {
    Invoke-PackAll $RootPath $NupkgTarget
}

if ($Publish) {
    Invoke-Publish
}

Write-Host "Success!"