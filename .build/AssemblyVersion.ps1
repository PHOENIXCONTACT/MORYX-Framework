function CheckVersionParameter([string]$version) {
    $versionPattern = "[0-9]+(\.([0-9]+|\*)){1,3}";
    if ($version -notmatch $versionPattern) {
        Write-Host "Version does not match the pattern $versionPattern";
        return $False;
    }
    return $True;
}

function Set-AssemblyVersion {
    <#
        .Synopsis
            Sets the version on the given AssemblyInfo.cs.
        .Example
            Set-AssemblyVersion -InputFile "C:\MarvinPlatform\GlobalAssemblyInfo.cs" -Version "2.0.0.0" -InformationalVersion "2.0.0-beta1-23423"
            This call wil set the version 2.0.0.0 on the given AssemblyInfo.
    #>
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$InputFile,

        [Parameter(Mandatory=$true, Position=1)]
        [string]$Version,

        [Parameter(Mandatory=$true, Position=2)]
        [string]$InformationalVersion, 

        [Parameter(Mandatory=$false, Position=3)]
        [string]$Configuration = "Debug"
    )

    if (-not $(CheckVersionParameter $Version)) {
        exit 1;
    }

    $file = Get-Childitem -Path $inputFile;

    if (-Not $file) {
        Write-Host "AssemblyInfo: $inputFile was not found!";
        exit 1;
    }

    Write-Host "Applying assembly info of $($file.FullName) -> $Version ";
   
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyVersion = 'AssemblyVersion("' + $Version + '")';

    $assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyFileVersion = 'AssemblyFileVersion("' + $Version + '")';

    $assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyInformationalVersion = 'AssemblyInformationalVersion("' + $InformationalVersion + '")';

    $assemblyConfigurationPattern = 'AssemblyConfiguration\("\w+"\)';
    $assemblyConfiguration = 'AssemblyConfiguration("' + $Configuration + '")';
    
    $content = (Get-Content $file.FullName) | ForEach-Object  { 
        ForEach-Object {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
        ForEach-Object {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion } |
        ForEach-Object {$_ -replace $assemblyInformationalVersionPattern, $assemblyInformationalVersion } |
        ForEach-Object {$_ -replace $assemblyConfigurationPattern, $assemblyConfiguration } 
    }

    Out-File -InputObject $content -FilePath $file.FullName -Encoding utf8;
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
        [string]$Version,

        [Parameter(Mandatory=$true, Position=2)]
        [string]$InformationalVersion, 
        
        [Parameter(Mandatory=$false, Position=3)]
        [string]$Configuration = "Debug"
    )

    if ($Files)
    {
        Write-Host "Will apply Version '$Version' to $($Files.count) AssemblyInfos.";
        foreach ($file in $files) {
            Set-AssemblyVersion -InputFile $file -Version $Version -InformationalVersion $InformationalVersion -Configuration $Configuration;
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