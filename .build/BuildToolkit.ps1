# Tool Versions
$NunitVersion = "3.11.1";
$OpenCoverVersion = "4.7.922";
$DocFxVersion = "2.56.2";
$ReportGeneratorVersion = "4.6.7";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\packages";

# Artifacts
$ArtifactsDir = "$RootPath\artifacts";

# Documentation
$DocumentationDir = "$RootPath\docs";
$DocumentationArtifcacts = "$ArtifactsDir\Documentation";

# Tests
$NunitReportsDir = "$ArtifactsDir\Tests";
$OpenCoverReportsDir = "$ArtifactsDir\Tests"

# Nuget
$NugetConfig = "$RootPath\NuGet.Config";
$NugetPackageArtifacts = "$ArtifactsDir\Packages";

# Load partial scripts
. "$PSScriptRoot\Output.ps1";

# Define Tools
$global:MSBuildCli = "msbuild.exe";
$global:DotNetCli = "dotnet.exe";
$global:NugetCli = "nuget.exe";
$global:GitCli = "";
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\net47\ReportGenerator.exe";
$global:DocFxCli = "$BuildTools\docfx.console.$DocFxVersion\tools\docfx.exe";

# Global Variables
$global:AssemblyVersion = "";
$global:AssemblyFileVersion = "";
$global:AssemblyInformationalVersion = ""
$global:PackageVersion = "";

# Git
$global:GitCommitHash = "";

# Functions
function Invoke-Initialize([string]$Version = "1.0.0", [bool]$Cleanup = $False) {
    Write-Step "Initializing BuildToolkit"

    # First check the powershell version
    if ($PSVersionTable.PSVersion.Major -lt 5) {
        Write-Host ("The needed major powershell version for this script is 5. Your version: " + ($PSVersionTable.PSVersion.ToString()))
        exit 1;
    }

    # Assign git.exe
    $gitCommand = (Get-Command "git.exe" -ErrorAction SilentlyContinue);
    if ($null -eq $gitCommand)  { 
        Write-Host "Unable to find git.exe in your PATH. Download from https://git-scm.com";
        exit 1;
    }

    $global:GitCli = $gitCommand.Path;

    # Load Hash
    $global:GitCommitHash = (& $global:GitCli rev-parse HEAD);
    Invoke-ExitCodeCheck $LastExitCode;

    # Initialize Folders
    CreateFolderIfNotExists $BuildTools;
    CreateFolderIfNotExists $ArtifactsDir;

    # Environment Variable Defaults
    if (-not $env:MORYX_BUILDNUMBER) {
        $env:MORYX_BUILDNUMBER = 0;
    }

    if (-not $env:MORYX_BUILD_CONFIG) {
        $env:MORYX_BUILD_CONFIG = "Debug";
    }

    if (-not $env:MORYX_BUILD_VERBOSITY) {
        $env:MORYX_BUILD_VERBOSITY = "minimal"
    }

    if (-not $env:MORYX_TEST_VERBOSITY) {
        $env:MORYX_TEST_VERBOSITY = "normal"
    }

    if (-not $env:MORYX_NUGET_VERBOSITY) {
        $env:MORYX_NUGET_VERBOSITY = "normal"
    }

    if (-not $env:MORYX_OPTIMIZE_CODE) {
        $env:MORYX_OPTIMIZE_CODE = $True;
    }
    else {
        if (-not [bool]::TryParse($env:MORYX_OPTIMIZE_CODE,  [ref]$env:MORYX_OPTIMIZE_CODE)) {
            $env:MORYX_OPTIMIZE_CODE = $True;
        }
    }

    if (-not $env:MORYX_GIT_REF) {
        $env:MORYX_GIT_REF = "unknown";
    }

    if (-not $env:MORYX_PACKAGE_TARGET) {
        $env:MORYX_PACKAGE_TARGET = "";
    }

    $global:AssemblyVersion = $Version;
    $global:AssemblyFileVersion = $Version;
    $global:AssemblyInformationalVersion = $Version;
    $global:PackageVersion = $Version;

    Set-Version $Version;

    # Printing Variables
    Write-Step "Printing global variables"
    Write-Variable "RootPath" $RootPath;
    Write-Variable "DocumentationDir" $DocumentationDir;
    Write-Variable "NunitReportsDir" $NunitReportsDir;

    Write-Step "Printing global scope"
    Write-Variable "AssemblyVersion" $global:AssemblyVersion;
    Write-Variable "AssemblyFileVersion" $global:AssemblyFileVersion;
    Write-Variable "AssemblyInformationalVersion" $global:AssemblyInformationalVersion;
    Write-Variable "PackageVersion" $global:PackageVersion;
    Write-Variable "OpenCoverCli" $global:OpenCoverCli;
    Write-Variable "NUnitCli" $global:NUnitCli;
    Write-Variable "ReportGeneratorCli" $global:ReportGeneratorCli;
    Write-Variable "DocFxCli" $global:DocFxCli;
    Write-Variable "GitCli" $global:GitCli;
    Write-Variable "GitCommitHash" $global:GitCommitHash;

    Write-Step "Printing environment variables"
    Write-Variable "MORYX_GIT_REF" $env:MORYX_GIT_REF;
    Write-Variable "MORYX_OPTIMIZE_CODE" $env:MORYX_OPTIMIZE_CODE;
    Write-Variable "MORYX_BUILDNUMBER" $env:MORYX_BUILDNUMBER;
    Write-Variable "MORYX_BUILD_CONFIG" $env:MORYX_BUILD_CONFIG;
    Write-Variable "MORYX_BUILD_VERBOSITY" $env:MORYX_BUILD_VERBOSITY;
    Write-Variable "MORYX_TEST_VERBOSITY" $env:MORYX_TEST_VERBOSITY;
    Write-Variable "MORYX_NUGET_VERBOSITY" $env:MORYX_NUGET_VERBOSITY;
    Write-Variable "MORYX_PACKAGE_TARGET" $env:MORYX_PACKAGE_TARGET;

    # Cleanp
    if ($Cleanup) {
        Write-Step "Cleanup"

        Write-Host "Cleaning up repository ..." -ForegroundColor Red;
        & $global:GitCli clean -f -d -x
        Invoke-ExitCodeCheck $LastExitCode;

        & $global:GitCli checkout .
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Invoke-Cleanup {
    # Clean up
    Write-Step "Cleaning up repository ...";
    & $global:GitCli clean -f -d -x
    Invoke-ExitCodeCheck $LastExitCode;
}

function Install-Tool([string]$PackageName, [string]$Version, [string]$TargetExecutable, [string]$OutputDirectory = $BuildTools) {
    if (-not (Test-Path $TargetExecutable)) {
        & $global:NugetCli install $PackageName -version $Version -outputdirectory $OutputDirectory -configfile $NugetConfig
        Invoke-ExitCodeCheck $LastExitCode;
    }
    else {
        Write-Host "$PackageName ($Version) already exists. Do not need to install."
    }
}
function Invoke-Build([string]$ProjectFile, [string]$Options = "") {
    Write-Step "Building $ProjectFile"

    # TODO: maybe we find a better way: currently all packages of all solutions are restored.
    ForEach ($solution in (Get-ChildItem $RootPath -Filter "*.sln")) {
        Write-Host "Restoring Nuget packages of $solution";

        & $global:NugetCli restore $solution -Verbosity $env:MORYX_NUGET_VERBOSITY -configfile $NugetConfig;
        Invoke-ExitCodeCheck $LastExitCode;
    }

    $additonalOptions = "";
    if (-not [string]::IsNullOrEmpty($Options)) {
        $additonalOptions = ",$Options";
    }

    $params = "Configuration=$env:MORYX_BUILD_CONFIG,Optimize=" + (&{If($env:MORYX_OPTIMIZE_CODE -eq $True) {"true"} Else {"false"}}) + ",DebugSymbols=true$additonalOptions";

    & $global:MSBuildCli $ProjectFile /p:$params /verbosity:$env:MORYX_BUILD_VERBOSITY
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Nunit([string]$SearchPath = $RootPath, [string]$SearchFilter = "*.csproj") {
	$randomIncrement = Get-Random -Minimum 2000 -Maximum 2100
    Write-Step "Running $Name Tests: $SearchPath"

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter
    if ($testProjects.Length -eq 0) {
        Write-Host-Warning "No test projects found!"
        return;
    }

	$env:PORT_INCREMENT = $randomIncrement;
	
    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    CreateFolderIfNotExists $NunitReportsDir;

	ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));
        $testAssembly = [System.IO.Path]::Combine($testProject.DirectoryName, "bin", $env:MORYX_BUILD_CONFIG, "$projectName.dll");
		
		# If assembly does not exists, the project will be build
        if (-not (Test-Path $testAssembly)) {
            Invoke-Build $testProject 
        }

		& $global:NUnitCli $testProject /config:"$env:MORYX_BUILD_CONFIG"
	}	
    
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-SmokeTest([string]$RuntimePath, [int]$ModulesCount, [int]$InterruptTime) {
    $randomIncrement = Get-Random -Minimum 2000 -Maximum 2100
    Write-Step "Invoking Runtime SmokeTest Modules: $ModulesCount, Interrupt Time: $InterruptTime, Port Increment: $randomIncrement."  

    & "$RuntimePath" @("smokeTest", "-e $ModulesCount", "-i $InterruptTime", "-p $randomIncrement")
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-CoverTests($SearchPath = $RootPath, $SearchFilter = "*.csproj", $FilterFile = "$RootPath\OpenCoverFilter.txt") {   
    Write-Step "Starting cover tests from $SearchPath with filter $FilterFile."
    
    if (-not (Test-Path $SearchPath)) {
        Write-Host "$SearchPath does not exists, ignoring!";
        return;
    }

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter
    if ($testProjects.Length -eq 0) {
        Write-Host-Warning "No test projects found!"
        return;
    }

    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    if (-not (Test-Path $global:OpenCoverCli)) {
        Install-Tool "OpenCover" $OpenCoverVersion $global:OpenCoverCli;
    }

    CreateFolderIfNotExists $OpenCoverReportsDir;
    CreateFolderIfNotExists $NunitReportsDir;

    $includeFilter = "+[Moryx*]*";
    $excludeFilter = "-[*nunit*]* -[*Tests]* -[*Model*]*";

    if (Test-Path $FilterFile) {
        $ignoreContent = Get-Content $FilterFile;

        foreach ($line in $ignoreContent) {
            $parts = $line.Split(":");
            if ($parts.Count -lt 2) {
                continue
            }

            $filterType = $parts[0];
            $filterValue = $parts[1];

            if ($filterType.StartsWith("INCLUDE")) {
                $includeFilter += " $filterValue";
            }
            
            if ($filterType.StartsWith("EXCLUDE")) {
                $excludeFilter += " $filterValue";
            }
        }

        Write-Host "Active Filter: `r`n Include: $includeFilter `r`n Exclude: $excludeFilter";
    } 

    ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));
        $testAssembly = [System.IO.Path]::Combine($testProject.DirectoryName, "bin", $env:MORYX_BUILD_CONFIG, "$projectName.dll");
        $isNetCore = Get-CsprojIsNetCore($testProject);

        Write-Host "OpenCover Test: ${projectName}:";

        $nunitXml = ($NunitReportsDir + "\$projectName.TestResult.xml");
        $openCoverXml = ($OpenCoverReportsDir + "\$projectName.OpenCover.xml");

        if ($isNetCore) {
            $targetArgs = '"test -v ' + $env:MORYX_TEST_VERBOSITY + ' -c ' + $env:MORYX_BUILD_CONFIG + ' ' + $testProject + '"';
            $openCoverAgs = "-target:$global:DotNetCli", "-targetargs:$targetArgs"
        }
        else {
            # If assembly does not exists, the project will be build
            if (-not (Test-Path $testAssembly)) {
                Invoke-Build $testProject 
            }

            $openCoverAgs = "-target:$global:NunitCli", "-targetargs:/config:$env:MORYX_BUILD_CONFIG /result:$nunitXml $testAssembly"
        }

        $openCoverAgs += "-log:Debug", "-register:user", "-output:$openCoverXml", "-hideskipped:all", "-skipautoprops";
        $openCoverAgs += "-returntargetcode" # We need the nunit return code
        $openCoverAgs += "-filter:$includeFilter $excludeFilter"

        & $global:OpenCoverCli $openCoverAgs
        
        $exitCode = [int]::Parse($LastExitCode);
        if ($exitCode -ne 0) {
            $errorText = "";
            switch ($exitCode) {
                -1 { $errorText = "INVALID_ARG"; }
                -2 { $errorText = "INVALID_ASSEMBLY"; }
                -4 { $errorText = "INVALID_TEST_FIXTURE"; }
                -5 { $errorText = "UNLOAD_ERROR"; }
                Default { $errorText = "UNEXPECTED_ERROR"; }
            }

            if ($exitCode -gt 0) {
                $errorText = "FAILED_TESTS ($exitCode)";
            }

            Write-Host "Nunit exited with $errorText for $projectName";
            Invoke-ExitCodeCheck $exitCode;
        }

        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Get-CsprojIsNetCore($csprojFile) {
    [xml]$csprojContent = Get-Content $csprojFile.FullName
    $sdkProject = $csprojContent.Project.Sdk;
    if ($null -ne $sdkProject) {
        # Read Target Framework
        $targetFramework = $csprojContent.Project.PropertyGroup.TargetFramework;
        if ($targetFramework -Match "netcoreapp") {
            # NETCore
            return $true;
        }
    }
    return $false;
}

function Invoke-CoverReport {
    Write-Step "Creating cover report. Searching for OpenCover.xml files in $OpenCoverReportsDir."

    if (-not (Test-Path $OpenCoverReportsDir)) {
        Write-Host-Error "$OpenCoverReportsDir was not found!";
        Invoke-ExitCodeCheck 1;
    }

    if (-not (Test-Path $global:ReportGeneratorCli)) {
        Install-Tool "ReportGenerator" $ReportGeneratorVersion $global:ReportGeneratorCli;
    }
    
    $reports = (Get-ChildItem $OpenCoverReportsDir -Recurse -Include '*.OpenCover.xml');
    $asArgument = [string]::Join(";",$reports);

    CreateFolderIfNotExists $DocumentationArtifcacts;

    & $global:ReportGeneratorCli -reports:"$asArgument" -targetDir:"$DocumentationArtifcacts/OpenCover"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-DocFx($Metadata = [System.IO.Path]::Combine($DocumentationDir, "docfx.json")) {
    Write-Step "Generating documentation using DocFx"

    if (-not (Test-Path $Metadata)) {
        Write-Host-Error "Metadata was not found at: $Metadata!"
        Invoke-ExitCodeCheck 1;
    }

    if (-not (Test-Path $global:DocFxCli)) {
        Install-Tool "docfx.console" $DocFxVersion $global:DocFxCli;
    }
    
    $docFxObj = (Get-Content $Metadata) | ConvertFrom-Json;
    $metadataFolder = [System.IO.Path]::GetDirectoryName($Metadata);
    $docFxDest = [System.IO.Path]::Combine($metadataFolder, $docFxObj.build.dest);

    & $global:DocFxCli $Metadata;
    Invoke-ExitCodeCheck $LastExitCode;

    CreateFolderIfNotExists $DocumentationArtifcacts;
    CopyAndReplaceFolder $docFxDest "$DocumentationArtifcacts\DocFx";
}

function Invoke-Pack($FilePath, [bool]$IsTool = $False, [bool]$IncludeSymbols = $False) {
    CreateFolderIfNotExists $NugetPackageArtifacts;

    $packargs = "-outputdirectory", "$NugetPackageArtifacts";
    $packargs += "-includereferencedprojects";
    $packargs += "-Version", "$global:PackageVersion";
    $packargs += "-Prop", "Configuration=$env:MORYX_BUILD_CONFIG";
    $packargs += "-Verbosity", "$env:MORYX_NUGET_VERBOSITY";

    if ($IncludeSymbols) {
        $packargs += "-Symbols";
    }

    if ($IsTool) {
        $packargs += "-Tool";
    }

    # Call nuget with default arguments plus optional
    & $global:NugetCli pack "$FilePath" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-PackAll([switch]$Symbols = $False) {
    Write-Host "Looking for .nuspec files..."
    # Look for nuspec in this directory
    foreach ($nuspecFile in Get-ChildItem $RootPath -Recurse -Filter *.nuspec) {
        $nuspecPath = $nuspecFile.FullName
        Write-Host "Packing $nuspecPath" -ForegroundColor Green

        # Check if there is a matching proj for the nuspec
        $projectPath = [IO.Path]::ChangeExtension($nuspecPath, "csproj")
        if(Test-Path $projectPath) {
            Invoke-Pack -FilePath $projectPath -IncludeSymbols $Symbols
        } else {
            Invoke-Pack -FilePath $nuspecPath -IncludeSymbols $Symbols
        }
    }
}

function Invoke-Publish {
    Write-Host "Pushing packages from $NugetPackageArtifacts to $env:MORYX_PACKAGE_TARGET"
    
    if ([string]::IsNullOrEmpty($env:MORYX_PACKAGE_TARGET)) {
        Write-Host "There is no package target given. Set the environment varialble MORYX_PACKAGE_TARGET to publish packages.";
        Invoke-ExitCodeCheck 1;
    }

    $packages = Get-ChildItem $NugetPackageArtifacts -Recurse -Include '*.nupkg'

    foreach ($package in $packages) {
        & $global:NugetCli push $package $env:MORYX_NUGET_APIKEY -Source $env:MORYX_PACKAGE_TARGET -Verbosity $env:MORYX_NUGET_VERBOSITY
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Set-Version ([string]$MajorMinorPatch) {
    Write-Host "Setting environment version to $MajorMinorPatch";

    $semVer2Regex = "^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

    $ref = $env:MORYX_GIT_REF;
    if ($ref -like "refs/tags/v*") {
        # Its a version tag
        $version = $ref.Replace("refs/tags/v","")
    }

    if ($ref -like "refs/heads/*") {
        # Its a branch
        $refName = $ref.Replace("refs/heads/","");
        $refName = $refName.Replace("/","").ToLower();

        $version = "$MajorMinorPatch-$refName.$env:MORYX_BUILDNUMBER";
    }

    # Match semVer2 regex
    $regexMatch = [regex]::Match($version, $semVer2Regex);

    if (-not $regexMatch.Success) {
        Write-Host "Could not parse version: $ref";
        Invoke-ExitCodeCheck 1;
    }

    # Extract groups
    $matchgroups = $regexMatch.captures.groups;
    $majorGroup = $matchgroups[1];
    $minorGroup = $matchgroups[2];
    $patchGroup = $matchgroups[3];
    $preReleaseGroup = $matchgroups[4];

    # Compose Major.Minor.Patch
    $mmp = $majorGroup.Value + "." + $minorGroup.Value + "." + $patchGroup.Value;

    # Check if it is a pre release
    $global:AssemblyVersion = $majorGroup.Value + ".0.0.0" # 3.0.0.0
    $global:AssemblyFileVersion = $mmp + "." + $env:MORYX_BUILDNUMBER; # 3.1.2.42

    if ($preReleaseGroup.Success) {
        $global:AssemblyInformationalVersion = $mmp + "-" + $preReleaseGroup.Value + "+" + $global:GitCommitHash; # 3.1.2-beta.1+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $global:PackageVersion = $mmp + "-" + $preReleaseGroup.Value;
    } else {
        $global:AssemblyInformationalVersion = $mmp + "+" + $global:GitCommitHash; # 3.1.2+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $global:PackageVersion = $mmp;
    }
}

function Set-AssemblyVersion([string]$InputFile) {
    $file = Get-Childitem -Path $inputFile;

    if (-Not $file) {
        Write-Host "AssemblyInfo: $inputFile was not found!";
        exit 1;
    }

    Write-Host "Applying assembly info of $($file.FullName) -> $global:AssemblyVersion ";
   
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyVersion = 'AssemblyVersion("' + $global:AssemblyVersion + '")';

    $assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyFileVersion = 'AssemblyFileVersion("' + $global:AssemblyFileVersion + '")';

    $assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyInformationalVersion = 'AssemblyInformationalVersion("' + $global:AssemblyInformationalVersion + '")';

    $assemblyConfigurationPattern = 'AssemblyConfiguration\("\w+"\)';
    $assemblyConfiguration = 'AssemblyConfiguration("' + $env:MORYX_BUILD_CONFIG + '")';
    
    $content = (Get-Content $file.FullName) | ForEach-Object  { 
        ForEach-Object {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
        ForEach-Object {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion } |
        ForEach-Object {$_ -replace $assemblyInformationalVersionPattern, $assemblyInformationalVersion } |
        ForEach-Object {$_ -replace $assemblyConfigurationPattern, $assemblyConfiguration } 
    }

    Out-File -InputObject $content -FilePath $file.FullName -Encoding utf8;
}

function Set-AssemblyVersions([string[]]$Ignored = $(), [string]$SearchPath = $RootPath) {
    $Ignored = $Ignored + "\\.build\\" + "\\Tests\\" + "\\IntegrationTests\\" + "\\SystemTests\\";

    $assemblyInfos = Get-ChildItem -Path $RootPath -include "*AssemblyInfo.cs" -Recurse | Where-Object { 
        $fullName = $_.FullName;
        return -not ($Ignored.Where({ $fullName -match $_ }).Count -gt 0);
    }

    if ($assemblyInfos)
    {
        Write-Host "Will apply version to $($assemblyInfos.Count) AssemblyInfos.";
        foreach ($file in $assemblyInfos) {
            Set-AssemblyVersion -InputFile $file;
        }
    }
}

function Set-VsixManifestVersion([string]$VsixManifest) {
    $file = Get-Childitem -Path $VsixManifest
    if (-Not $file) {
        Write-Host "VSIX Manifest: $VsixManifest was not found!"
        exit 1;
    }
    
    [xml]$manifestContent = Get-Content $file
    $manifestContent.PackageManifest.Metadata.Identity.Version = $global:AssemblyVersion
    $manifestContent.Save($VsixManifest) 

    Write-Host "Version $global:AssemblyVersion applied to $VsixManifest!"
}

function Set-VsTemplateVersion([string]$VsTemplate) {
    $file = Get-Childitem -Path $VsTemplate
    if (-Not $file) {
        Write-Host "VsTemplate: $VsTemplate was not found!"
        exit 1;
    }

    [xml]$templateContent = Get-Content $VsTemplate

    $versionRegex = "(\d+)\.(\d+)\.(\d+)\.(\d+)"

    $wizardAssemblyStrongName = $templateContent.VSTemplate.WizardExtension.Assembly -replace $versionRegex, $global:AssemblyVersion 
    $templateContent.VSTemplate.WizardExtension.Assembly = $wizardAssemblyStrongName
    $templateContent.Save($vsTemplate)

    Write-Host "Version $global:AssemblyVersion applied to $VsTemplate!"
}

function CreateFolderIfNotExists([string]$Folder) {
    if (-not (Test-Path $Folder)) {
        Write-Host "Creating missing directory '$Folder'"
        New-Item $Folder -Type Directory | Out-Null
    }
}

function CopyAndReplaceFolder($SourceDir, $TargetDir) {
    Write-Host-Info "Copy $TargetDir to $SourceDir!"
    # Remove old folder if exists
    if (Test-Path $TargetDir) {
        Write-Host "Target path already exists, removing ..." -ForegroundColor Yellow
        Remove-Item -Recurse -Force $TargetDir
    }

    # Copy to target path
    Write-Host "Copy from $SourceDir to $TargetDir ..." -ForegroundColor Green
    Copy-Item -Path $SourceDir -Recurse -Destination $TargetDir -Container
}