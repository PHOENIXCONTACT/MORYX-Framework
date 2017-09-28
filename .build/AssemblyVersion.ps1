. "$PSScriptRoot\Version.ps1"

function Set-AssemblyVersion {
    <#
        .Synopsis
            Sets the version on the given AssemblyInfo.cs.
        .Example
            Set-AssemblyVersion -InputFile "C:\MarvinPlatform\GlobalAssemblyInfo.cs" -Version "2.0.0.0"
            This call wil set the version 2.0.0.0 on the given AssemblyInfo.
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$InputFile,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$Version
    )

    if (-not $(CheckVersionParameter $Version)) {
        exit 1;
    }

    $file = Get-Childitem -Path $inputFile

    if (-Not $file) {
        Write-Host "AssemblyInfo: $inputFile was not found!"
        exit 1;
    }

    Write-Host "Applying assembly info of $($file.FullName) -> $Version -> $Configuration"
   
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $Version + '")';

    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersion = 'AssemblyFileVersion("' + $Version + '")';

    $assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyInformationalVersion = 'AssemblyInformationalVersion("' + $Version + '")';
    
    $content = (Get-Content $file.FullName) | ForEach-Object  { 
        ForEach-Object {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
        ForEach-Object {$_ -replace $assemblyInformationalVersionPattern, $assemblyInformationalVersion } |
        ForEach-Object {$_ -replace $fileVersionPattern, $fileVersion }
    }

    [System.IO.File]::WriteAllLines($file.FullName, $content, [System.Text.Encoding]::UTF8);
}

function Set-AssemblyVersions
{
    <#
        .Synopsis
            Sets the assembly versions on all AssemblyInfo.cs files in the given path.
        .Example
            Set-AssemblyVersions -Files "C:\Data\MarvinPlatform\" -Version "2.0.0.0"
            This call will scan the "C:\Data\MarvinPlatform\" folder and sets the version 2.0.0.0 with on every AssemblyInfo.cs file.
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string[]]$Files,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$Version
    )

    if ($Files)
    {
        Write-Host "Will apply Version '$Version' and Configuration '$Configuration' to $($Files.count) AssemblyInfos."
        foreach ($file in $files) {
            Set-AssemblyVersion -InputFile $file -Version $Version
        }
    }
}

function Set-VsixManifestVersion {
    <#
        .Synopsis
            Sets the version of a VSIX-Mmanifest file
        .Example
            Set-VsixManifestVersion -VsixManifest ".\DataModelWizard\source.extension.vsixmanifest" -Version "2.0.0.0"
            This call will set the version number of the VSIX-Manifest to 2.0.0.0.
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$VsixManifest,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$Version
    )

    $file = Get-Childitem -Path $VsixManifest
    if (-Not $file) {
        Write-Host "VSIX Manifest: $VsixManifest was not found!"
        exit 1;
    }

    if (-not $(CheckVersionParameter $Version)) {
        exit 1;
    }
    
    [xml]$manifestContent = Get-Content $file
    $manifestContent.PackageManifest.Metadata.Identity.Version = $version
    $manifestContent.Save($VsixManifest) 

    Write-Host "Version $Version applied to $VsixManifest!"
}

function Set-VsTemplateVersion {
    <#
        .Synopsis
            Sets the version of a visual studio template
        .Example
            Set-VsTemplateVersion -VsTemplate ".\DataModelTemplate\MyTemplate.vstemplate" -Version "4.0.0.0"
            This call will set the version number of the WizardExtension to 4.0.0.0 in the vstemplate file.
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$VsTemplate,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$Version
    )

    $file = Get-Childitem -Path $VsTemplate
    if (-Not $file) {
        Write-Host "VsTemplate: $VsTemplate was not found!"
        exit 1;
    }

    if (-not $(CheckVersionParameter $Version)) {
        exit 1;
    }

    [xml]$templateContent = Get-Content $VsTemplate

    $versionRegex = "(\d+)\.(\d+)\.(\d+)\.(\d+)"

    $wizardAssemblyStrongName = $templateContent.VSTemplate.WizardExtension.Assembly -replace $versionRegex, $Version 
    $templateContent.VSTemplate.WizardExtension.Assembly = $wizardAssemblyStrongName
    $templateContent.Save($vsTemplate)

    Write-Host "Version $Version applied to $VsTemplate!"
}