# Tool Versions
$NunitVersion = "3.12.0";
$OpenCoverVersion = "4.7.1221";
$DocFxVersion = "2.58.4";
$ReportGeneratorVersion = "4.8.13";
$OpenCoverToCoberturaVersion = "0.3.4";

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
$CoberturaReportsDir = "$ArtifactsDir\Tests"

# Nuget
$NugetConfig = "$RootPath\NuGet.Config";
$NugetPackageArtifacts = "$ArtifactsDir\Packages";

# Load partial scripts
. "$PSScriptRoot\Output.ps1";

# Define Tools
$global:DotNetCli = "dotnet.exe";
$global:NugetCli = "nuget.exe";
$global:GitCli = "";
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\net47\ReportGenerator.exe";
$global:DocFxCli = "$BuildTools\docfx.console.$DocFxVersion\tools\docfx.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";

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

    if (-not $env:MORYX_PACKAGE_TARGET) {
        $env:MORYX_PACKAGE_TARGET = "";
    }

    if (-not $env:MORYX_PACKAGE_TARGET_V3) {
        $env:MORYX_PACKAGE_TARGET_V3 = "";
    }

    if (-not $env:MORYX_ASSEMBLY_VERSION) {
        $env:MORYX_ASSEMBLY_VERSION = $Version;
    }

    if (-not $env:MORYX_FILE_VERSION) {
        $env:MORYX_FILE_VERSION = $Version;
    }

    if (-not $env:MORYX_INFORMATIONAL_VERSION) {
        $env:MORYX_INFORMATIONAL_VERSION = $Version;
    }

    if (-not $env:MORYX_PACKAGE_VERSION) {
        $env:MORYX_PACKAGE_VERSION = $Version;
    }
    
    Set-Version $Version;

    # Printing Variables
    Write-Step "Printing global variables"
    Write-Variable "RootPath" $RootPath;
    Write-Variable "DocumentationDir" $DocumentationDir;
    Write-Variable "NunitReportsDir" $NunitReportsDir;

    Write-Step "Printing global scope"
    Write-Variable "OpenCoverCli" $global:OpenCoverCli;
    Write-Variable "NUnitCli" $global:NUnitCli;
    Write-Variable "ReportGeneratorCli" $global:ReportGeneratorCli;
    Write-Variable "DocFxCli" $global:DocFxCli;
    Write-Variable "OpenCoverToCoberturaCli" $global:OpenCoverToCoberturaCli;
    Write-Variable "GitCli" $global:GitCli;
    Write-Variable "GitCommitHash" $global:GitCommitHash;

    Write-Step "Printing environment variables"
    Write-Variable "MORYX_OPTIMIZE_CODE" $env:MORYX_OPTIMIZE_CODE;
    Write-Variable "MORYX_BUILDNUMBER" $env:MORYX_BUILDNUMBER;
    Write-Variable "MORYX_BUILD_CONFIG" $env:MORYX_BUILD_CONFIG;
    Write-Variable "MORYX_BUILD_VERBOSITY" $env:MORYX_BUILD_VERBOSITY;
    Write-Variable "MORYX_TEST_VERBOSITY" $env:MORYX_TEST_VERBOSITY;
    Write-Variable "MORYX_NUGET_VERBOSITY" $env:MORYX_NUGET_VERBOSITY;
    Write-Variable "MORYX_PACKAGE_TARGET" $env:MORYX_PACKAGE_TARGET;
    Write-Variable "MORYX_PACKAGE_TARGET_V3" $env:MORYX_PACKAGE_TARGET_V3;

    Write-Variable "MORYX_ASSEMBLY_VERSION" $env:MORYX_ASSEMBLY_VERSION;
    Write-Variable "MORYX_FILE_VERSION" $env:MORYX_FILE_VERSION;
    Write-Variable "MORYX_INFORMATIONAL_VERSION" $env:MORYX_INFORMATIONAL_VERSION;
    Write-Variable "MORYX_PACKAGE_VERSION" $env:MORYX_PACKAGE_VERSION;


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

        & $global:DotNetCli restore $solution --verbosity $env:MORYX_NUGET_VERBOSITY --configfile $NugetConfig;
        Invoke-ExitCodeCheck $LastExitCode;
    }

    $additonalOptions = "";
    if (-not [string]::IsNullOrEmpty($Options)) {
        $additonalOptions = ",$Options";
    }

    $msbuildParams = "Optimize=" + (&{If($env:MORYX_OPTIMIZE_CODE -eq $True) {"true"} Else {"false"}}) + ",DebugSymbols=true$additonalOptions";
    $buildArgs = "--configuration", "$env:MORYX_BUILD_CONFIG";
    $buildArgs += "--verbosity", $env:MORYX_BUILD_VERBOSITY;
    $buildArgs += "-p:$msbuildParams"

    & $global:DotNetCli build $ProjectFile @buildArgs
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
        Write-Host-Warning "$SearchPath does not exists, ignoring!";
        return;
    }

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter
    if ($testProjects.Length -eq 0) {
        Write-Host-Warning "No test projects found!"
        return;
    }

    CreateFolderIfNotExists $CoberturaReportsDir;

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
        Write-Host "Testing ${projectName}...";
        
        dotnet test ${testProject} --collect:"XPlat Code Coverage" `
            /p:CoverletOutputFormat="cobertura" `
            /p:Include="$includeFilter" `
            /p:Exclude="$excludeFilter"

        Invoke-ExitCodeCheck $LastExitCode;

        $testsDir = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($testProject), "TestResults");

        $testResults = Get-ChildItem -Path $testsDir -Recurse -File
        foreach ($resultFile in $testResults) {
            $destinationFile = $resultFile.FullName.Replace($testsDir, $CoberturaReportsDir);
            $destinationPath = [System.IO.Path]::GetDirectoryName($destinationFile);

            CreateFolderIfNotExists $destinationPath;

            Move-Item -Path $resultFile -Destination $destinationFile
        }
    }
}

function Get-CsprojIsNetCore($CsprojItem) {
    [xml]$csprojContent = Get-Content $CsprojItem.FullName
    $sdkProject = $csprojContent.Project.Sdk;
    if ($null -ne $sdkProject) {
        # Read Target Framework
        $targetFramework = $csprojContent.Project.PropertyGroup.TargetFramework;
        if ($targetFramework -Match "netcoreapp" -or $targetFramework -Match "net5.") {
            # NETCore
            return $true;
        }
    }
    return $false;
}

function Get-CsprojIsSdkProject($CsprojItem) {
    [xml]$csprojContent = Get-Content $CsprojItem.FullName
    $sdkProject = $csprojContent.Project.Sdk;
    if ($null -ne $sdkProject) {
        return $true;
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

function Invoke-PackSdkProject($CsprojItem, [bool]$IncludeSymbols = $False) {
    Write-Host "Try to pack .NET SDK project: $($CsprojItem.Name) ...";

    # Check if the project should be packed
    $csprojFullName = $CsprojItem.FullName;
    [xml]$csprojContent = Get-Content $csprojFullName
    $createPackage = $csprojContent.Project.PropertyGroup.CreatePackage;
;
    if ($null -eq $createPackage -or "false" -eq $createPackage) {
        Write-Host-Warning "... csproj not flagged with <CreatePackage>true</CreatePackage>: $($CsprojItem.Name)";
        return;
    }

    $packargs = "--output", "$NugetPackageArtifacts";
    $packargs += "--configuration", "$env:MORYX_BUILD_CONFIG";
    $packargs += "--verbosity", "$env:MORYX_NUGET_VERBOSITY";
    $packargs += "--no-build";

    if ($IncludeSymbols) {
        $packargs += "--include-symbols";
        $packargs += "--include-source";
    }

    & $global:DotNetCli pack "$csprojFullName" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-PackFrameworkProject($CsprojItem, [bool]$IsTool = $False, [bool]$IncludeSymbols = $False) {
    Write-Host "Try to pack .NET Framework project: $CsprojItem.Name ...";

    # Check if there is a matching nuspec for the proj
    $csprojFullName = $CsprojItem.FullName;
    $nuspecPath = [IO.Path]::ChangeExtension($csprojFullName, "nuspec")
    if(-not (Test-Path $nuspecPath)) {
        Write-Host-Warning "Nuspec for project not found: $CsprojItem.Name";
        return;
    }

    $packargs = "-outputdirectory", "$NugetPackageArtifacts";
    $packargs += "-includereferencedprojects";
    $packargs += "-Version", "$env:MORYX_PACKAGE_VERSION";
    $packargs += "-Prop", "Configuration=$env:MORYX_BUILD_CONFIG";
    $packargs += "-Verbosity", "$env:MORYX_NUGET_VERBOSITY";

    if ($IncludeSymbols) {
        $packargs += "-Symbols";
    }

    if ($IsTool) {
        $packargs += "-Tool";
    }

    # Call nuget with default arguments plus optional
    & $global:NugetCli pack "$csprojFullName" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Pack($ProjectItem, [bool]$IsTool = $False, [bool]$IncludeSymbols = $False) {
    CreateFolderIfNotExists $NugetPackageArtifacts;

    if (Get-CsprojIsSdkProject($ProjectItem)) {
        Invoke-PackSdkProject $ProjectItem $IncludeSymbols;
    }
    else {
        Invoke-PackFrameworkProject $ProjectItem $IsTool $IncludeSymbols;
    }
}

function Invoke-PackAll([switch]$Symbols = $False) {
    Write-Host "Looking for .csproj files..."
    # Look for csproj in this directory
    foreach ($csprojItem in Get-ChildItem $RootPath -Recurse -Filter *.csproj) {
        Invoke-Pack -ProjectItem $csprojItem -IncludeSymbols $Symbols
    }
}

function Invoke-Publish {
    Write-Host "Pushing packages from $NugetPackageArtifacts to $env:MORYX_PACKAGE_TARGET"
    
    $packages = Get-ChildItem $NugetPackageArtifacts -Recurse -Include *.nupkg
    if ($packages.Length -gt 0 -and [string]::IsNullOrEmpty($env:MORYX_PACKAGE_TARGET)) {
        Write-Host-Error "There is no package target given. Set the environment varialble MORYX_PACKAGE_TARGET to publish packages.";
        Invoke-ExitCodeCheck 1;
    }

    foreach ($package in $packages) {
        Write-Host "Pushing package $package"
        & $global:DotNetCli nuget push $package --api-key $env:MORYX_NUGET_APIKEY --no-symbols true --skip-duplicate --source $env:MORYX_PACKAGE_TARGET
        Invoke-ExitCodeCheck $LastExitCode;
    }

    $symbolPackages = Get-ChildItem $NugetPackageArtifacts -Recurse -Include *.snupkg
    if ($symbolPackages.Length -gt 0 -and [string]::IsNullOrEmpty($env:MORYX_PACKAGE_TARGET_V3)) {
        Write-Host-Error "There is no package (v3) target given. Set the environment varialble MORYX_PACKAGE_TARGET_V3 to publish snupkg symbol packages.";
        Invoke-ExitCodeCheck 1;
    }

    foreach ($symbolPackage in $symbolPackages) {
        Write-Host "Pushing symbol (snupkg) $symbolPackage"
        & $global:DotNetCli nuget push $symbolPackage --api-key $env:MORYX_NUGET_APIKEY --skip-duplicate --source $env:MORYX_PACKAGE_TARGET_V3
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Set-Version ([string]$MajorMinorPatch) {
    $semVer2Regex = "^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
    
    $version = Read-VersionFromRef($MajorMinorPatch);
    Write-Host "Setting environment version to $version";

    # Match semVer2 regex
    $regexMatch = [regex]::Match($version, $semVer2Regex);

    if (-not $regexMatch.Success) {
        Write-Host "Could not parse version: $version";
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
    $env:MORYX_ASSEMBLY_VERSION = $majorGroup.Value + ".0.0.0" # 3.0.0.0
    $env:MORYX_FILE_VERSION = $mmp + "." + $env:MORYX_BUILDNUMBER; # 3.1.2.42

    if ($preReleaseGroup.Success) {
        $env:MORYX_INFORMATIONAL_VERSION = $mmp + "-" + $preReleaseGroup.Value + "+" + $global:GitCommitHash; # 3.1.2-beta.1+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $env:MORYX_PACKAGE_VERSION = $mmp + "-" + $preReleaseGroup.Value;
    } else {
        $env:MORYX_INFORMATIONAL_VERSION = $mmp + "+" + $global:GitCommitHash; # 3.1.2+d95a996ed5ba14a1421dafeb844a56ab08211ead
        $env:MORYX_PACKAGE_VERSION = $mmp;
    }
}

function Read-VersionFromRef([string]$MajorMinorPatch) {
    function preReleaseVersion ([string] $name)
    {
        $name = $name.Replace("/","").ToLower();
        return "$MajorMinorPatch-$name.$env:MORYX_BUILDNUMBER";;
    }

    $ref = "";
    if ($env:GITHUB_WORKFLOW) { # GitHub Workflow
        Write-Host "Reading version from 'GitHub Workflow'";
        $ref = $env:GITHUB_REF;

        if ($ref.StartsWith("refs/tags/")) {
            if ($ref.StartsWith("refs/tags/v")) {
                # Its a version tag
                $version = $ref.Replace("refs/tags/v","")
            } 
            else {
                # Just a tag
                $name = $ref.Replace("refs/tags/","");
                $version = = preReleaseVersion($name);
            }
        }
        elseif ($ref.StartsWith("refs/heads/")) {
            # Its a branch
            $name = $ref.Replace("refs/heads/","");
            $version = preReleaseVersion($name);
        } 
        else {
            $version = preReleaseVersion($ref);
        }
    }
    else { # Local build
        Write-Host "Reading version from 'local'";
        $ref = (& $global:GitCli rev-parse --abbrev-ref HEAD);
        $version = preReleaseVersion($ref);
    }

    return $version;
}

function Set-AssemblyVersion([string]$InputFile) {
    $file = Get-Childitem -Path $inputFile;

    if (-Not $file) {
        Write-Host "AssemblyInfo: $inputFile was not found!";
        exit 1;
    }

    Write-Host "Applying assembly info of $($file.FullName) ->  $env:MORYX_ASSEMBLY_VERSION ";
   
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyVersion = 'AssemblyVersion("' +  $env:MORYX_ASSEMBLY_VERSION + '")';

    $assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyFileVersion = 'AssemblyFileVersion("' +  $env:MORYX_FILE_VERSION + '")';

    $assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyInformationalVersion = 'AssemblyInformationalVersion("' + $env:MORYX_INFORMATIONAL_VERSION + '")';

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

function CreateFolderIfNotExists([string]$Folder) {
    if (-not (Test-Path $Folder)) {
        Write-Host "Creating missing directory '$Folder'"
        New-Item $Folder -Type Directory | Out-Null
    }
}

function CopyAndReplaceFolder($SourceDir, $TargetDir) {
    Write-Host-Info "Copy $SourceDir to $TargetDir!"
    # Remove old folder if exists
    if (Test-Path $TargetDir) {
        Write-Host "Target path already exists, removing ..." -ForegroundColor Yellow
        Remove-Item -Recurse -Force $TargetDir
    }

    # Copy to target path
    Write-Host "Copy from $SourceDir to $TargetDir ..." -ForegroundColor Green
    Copy-Item -Path $SourceDir -Recurse -Destination $TargetDir -Container
}
