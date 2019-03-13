# Tool Versions
$MsBuildVersion = "14.0"; # valid versions are [12.0, 14.0, 15.0, latest] (latest only works >= 15.0)
$NunitVersion = "3.9.0";
$OpenCoverVersion = "4.7.922";
$DocFxVersion = "2.40.11";
$OpenCoverToCoberturaVersion = "0.3.4";
$ReportGeneratorVersion = "4.0.15";
$VswhereVersion = "2.6.7";
$GitLinkVersion = "3.1.0";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\packages";
$DotBuild = "$RootPath\.build";

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
$NugetConfig = "$dotBuild\NuGet.Config";
$NugetPackageArtifacts = "$ArtifactsDir\Packages";
$NugetCliSource = "http://packages-swtd.europe.phoenixcontact.com/source/nuget-4.7.1.exe";
$NugetPushCliSource = "http://packages-swtd.europe.phoenixcontact.com/source/nuget-3.4.4.exe";
$NugetPackageTarget = "http://packages-swtd.europe.phoenixcontact.com/nuget/MaRVIN-CI/";

# Load partial scripts
. "$DotBuild\Output.ps1";

# Define Tools
$global:GitCli = "";
$global:GitLink = "$BuildTools\GitLink.$GitLinkVersion\build\GitLink.exe";
$global:NugetCli = "$BuildTools\nuget.exe";
$global:NugetPushCli = "$BuildTools\nuget-push.exe";
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\net47\ReportGenerator.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";
$global:VswhereCli = "$BuildTools\vswhere.$VswhereVersion\tools\vswhere.exe";
$global:DocFxCli = "$BuildTools\docfx.console.$DocFxVersion\tools\docfx.exe";

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
        Invoke-ExitCodeCheck 1;
    }

    $global:GitCli = $gitCommand.Path;

    # Load Hash
    $global:GitCommitHash = (& $global:GitCli rev-parse --short HEAD);
    Invoke-ExitCodeCheck $LastExitCode;

    # Initialize Folders
    CreateFolderIfNotExists $BuildTools;
    CreateFolderIfNotExists $ArtifactsDir;

    # Assign nuget.exe
    if ((-not (Test-Path $global:NugetCli)) -or (-not (Test-Path $global:NugetPushCli))) {
        Write-Host "Downloading NuGet ..."
        try {
            Invoke-WebRequest $NugetCliSource -OutFile $global:NugetCli
            Invoke-WebRequest $NugetPushCliSource -OutFile $global:NugetPushCli
        }
        catch {
            Write-Host "Error while downloading NuGet: " + $_.Exception.Message
            Invoke-ExitCodeCheck 1;
        }
    }

    # Assign msbuild.exe
    if ($MsBuildVersion -eq "latest" -or $MsBuildVersion -eq "15.0") {
        if (-not (Test-Path $global:VswhereCli)) {
            Install-Tool "vswhere" $VswhereVersion $VswhereCli;
        }

        $installPath = [string] (& $global:VswhereCli -latest -prerelease -products * -requires "Microsoft.Component.MSBuild" -property "installationPath");
        if ($installPath) {
            $msbuildExe = Get-ChildItem -Path $installPath -Filter MSBuild.exe -Recurse -ErrorAction SilentlyContinue -Force | Select-Object -First 1
            $global:MSBuildCli = $msbuildExe.FullName;
        }
    }
    else {
        $global:MSBuildCli = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$MsBuildVersion")."MSBuildToolsPath" -childpath "msbuild.exe"
    }

    if ($null -eq $global:MSBuildCli  -or -not (Test-Path $global:MSBuildCli)) {
        Write-Host "Unable to find msbuild.exe.";
        Invoke-ExitCodeCheck 1;
    }

    # Environment Variable Defaults
    if (-not $env:MARVIN_BUILDNUMBER) {
        $env:MARVIN_BUILDNUMBER = 0;
    }

    if (-not $env:MARVIN_BUILD_CONFIG) {
        $env:MARVIN_BUILD_CONFIG = "Debug";
    }

    if (-not $env:MARVIN_OPTIMIZE_CODE) {
        $env:MARVIN_OPTIMIZE_CODE = $True;
    }
    else {
        if (-not [bool]::TryParse($env:MARVIN_OPTIMIZE_CODE,  [ref]$env:MARVIN_OPTIMIZE_CODE)) {
            $env:MARVIN_OPTIMIZE_CODE = $True;
        }
    }

    if (-not $env:MARVIN_BRANCH) {
        $env:MARVIN_BRANCH = "unknown";
    }

    Set-Version $Version;

    # Printing Variables
    Write-Step "Printing global variables"
    Write-Variable "RootPath" $RootPath;
    Write-Variable "Version" $Version;
    Write-Variable "DocumentationDir" $DocumentationDir;
    Write-Variable "NunitReportsDir" $NunitReportsDir;

    Write-Step "Printing global scope"
    Write-Variable "MSBuildCli" $global:MSBuildCli;
    Write-Variable "NugetCli" $global:NugetCli;
    Write-Variable "NugetPushCli" $global:NugetPushCli;
    Write-Variable "OpenCoverCli" $global:OpenCoverCli;
    Write-Variable "NUnitCli" $global:NUnitCli;
    Write-Variable "ReportGeneratorCli" $global:ReportGeneratorCli;
    Write-Variable "DocFxCli" $global:DocFxCli;
    Write-Variable "OpenCoverToCoberturaCli" $global:OpenCoverToCoberturaCli;
    Write-Variable "VswhereCli" $global:VswhereCli;
    Write-Variable "GitCli" $global:GitCli;
    Write-Variable "GitLink" $global:GitLink;
    Write-Variable "GitCommitHash" $global:GitCommitHash;
    Write-Variable "MARVIN_BRANCH" $env:MARVIN_BRANCH;
    Write-Variable "MARVIN_VERSION" $env:MARVIN_VERSION;
    Write-Variable "MARVIN_ASSEMBLY_VERSION" $env:MARVIN_ASSEMBLY_VERSION

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

        & $global:NugetCli restore $solution -Verbosity detailed -configfile $NugetConfig;
        Invoke-ExitCodeCheck $LastExitCode;
    }

    $additonalOptions = "";
    if (-not [string]::IsNullOrEmpty($Options)) {
        $additonalOptions = ",$Options";
    }

    $params = "Configuration=$env:MARVIN_BUILD_CONFIG,Optimize=" + (&{If($env:MARVIN_OPTIMIZE_CODE -eq $True) {"true"} Else {"false"}}) + ",DebugSymbols=true$additonalOptions";

    & $global:MSBuildCli $ProjectFile /p:$params /detailedsummary
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Nunit([string]$SearchPath = $RootPath, [string]$SearchFilter = "*.csproj") {
	$randomIncrement = Get-Random -Minimum 2000 -Maximum 2100
    Write-Step "Running $Name Tests: $SearchPath"

	$env:PORT_INCREMENT = $randomIncrement;
	
    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    CreateFolderIfNotExists $NunitReportsDir;

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter

	ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));
        $testAssembly = [System.IO.Path]::Combine($testProject.DirectoryName, "bin", $env:MARVIN_BUILD_CONFIG, "$projectName.dll");
		
		# If assembly does not exists, the project will be build
        if (-not (Test-Path $testAssembly)) {
            Invoke-Build $testProject 
        }

		& $global:NUnitCli $testProject /config:"$env:MARVIN_BUILD_CONFIG"
	}	
    
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-SmokeTest([string]$RuntimePath, [int]$ModulesCount, [int]$InterruptTime) {
    $randomIncrement = Get-Random -Minimum 2000 -Maximum 2100
    Write-Step "Invoking Runtime SmokeTest Modules: $ModulesCount, Interrupt Time: $InterruptTime, Port Increment: $randomIncrement."  

    & "$RuntimePath" @("-r=SmokeTest", "-e=$ModulesCount", "-i=$InterruptTime", "-pi=$randomIncrement")
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-CoverTests($SearchPath = $RootPath, $SearchFilter = "*.csproj", $FilterFile = "$RootPath\OpenCoverFilter.txt") {   
    Write-Step "Starting cover tests from $SearchPath with filter $FilterFile."
    
    if (-not (Test-Path $SearchPath)) {
        Write-Host "$SearchPath does not exists, ignoring!";
        return;
    }

    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    if (-not (Test-Path $global:OpenCoverCli)) {
        Install-Tool "OpenCover" $OpenCoverVersion $global:OpenCoverCli;
    }

    if (-not (Test-Path $global:OpenCoverToCoberturaCli)) {
        Install-Tool "OpenCoverToCoberturaConverter" $OpenCoverToCoberturaVersion $global:OpenCoverToCoberturaCli;
    }

    CreateFolderIfNotExists $OpenCoverReportsDir;
    CreateFolderIfNotExists $CoberturaReportsDir;
    CreateFolderIfNotExists $NunitReportsDir;

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter
    ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));
        $testAssembly = [System.IO.Path]::Combine($testProject.DirectoryName, "bin", $env:MARVIN_BUILD_CONFIG, "$projectName.dll");

        Write-Host "OpenCover Test: ${projectName}:";

        $nunitXml = ($NunitReportsDir + "\$projectName.TestResult.xml");
        $openCoverXml = ($OpenCoverReportsDir + "\$projectName.OpenCover.xml");
        $coberturaXml = ($CoberturaReportsDir + "\$projectName.Cobertura.xml");

        # If assembly does not exists, the project will be build
        if (-not (Test-Path $testAssembly)) {
            Invoke-Build $testProject 
        }

        $includeFilter = "+[Marvin*]*";
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
        } 

        Write-Host "Active Filter: `r`n Include: $includeFilter `r`n Exclude: $excludeFilter";

        $openCoverAgs = "-target:$global:NunitCli", "-targetargs:/config:$env:MARVIN_BUILD_CONFIG /result:$nunitXml $testAssembly"
        $openCoverAgs += "-log:Debug", "-register:user", "-output:$openCoverXml", "-hideskipped:all", "-skipautoprops", "-excludebyattribute:*OpenCoverIgnore*";
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

        & $global:OpenCoverToCoberturaCli -input:$openCoverXml -output:$coberturaXml -sources:$rootPath
        Invoke-ExitCodeCheck $LastExitCode;
    }
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

function Invoke-SourceIndex([string]$RawUrl, [string]$SearchPath = [System.IO.Path]::Combine($PSScriptRoot, "..\")) {
    Write-Step "Indexing SourceCode and patching PDBs to $RawUrl"

    if (-not (Test-Path $global:GitLink)) {
        Install-Tool "GitLink" $GitLinkVersion $global:GitLink;
    }

    $sourceLink = "$RawUrl/{revision}/{filename}";

    Write-Host "SearchPath for Projects: $SearchPath";
    $csprojs = Get-Childitem $SearchPath -recurse | Where-Object {$_.extension -eq ".csproj"}

    foreach ($csporj in $csprojs) {
        Write-Host;
        Write-Host "Reading csproj: $($csporj.Name)"; 

        $csprojXml = [xml](Get-Content $csporj.FullName);

        $outputGroup = $csprojXml.Project.PropertyGroup | Where-Object Condition -Like "*$env:MARVIN_BUILD_CONFIG|AnyCPU*";
        $outputPath = $outputGroup.OutputPath;

        $assemblyGroup = $csprojXml.Project.PropertyGroup | Where-Object {-not ([string]::IsNullOrEmpty($_.AssemblyName)) }
        $assemblyName = $assemblyGroup.AssemblyName;

        $pdbFileName = $($assemblyName + ".pdb");
        $projectPdbPath = [System.IO.Path]::Combine($outputPath, $pdbFileName);
        $pdbPath = [System.IO.Path]::Combine($csporj.DirectoryName, $projectPdbPath);

        Write-Host "PDB path of assembly for $($csporj.Name) is: $projectPdbPath"

        if (-not (Test-Path $pdbPath)) {
            Write-Host "PDB was not found. Project will be ignored!"
            continue;
        }

        $args = "-u", "$sourceLink";
        $args += $pdbPath

        & $global:GitLink $args
    }
}

function Invoke-Pack($FilePath, [bool]$IsTool = $False, [bool]$IncludeSymbols = $False) {
    CreateFolderIfNotExists $NugetPackageArtifacts;

    $packargs = "-outputdirectory", "$NugetPackageArtifacts";
    $packargs += "-includereferencedprojects";
    $packargs += "-Version", "$env:MARVIN_VERSION";
    $packargs += "-Prop", "Configuration=$env:MARVIN_BUILD_CONFIG";
    $packargs += "-Verbosity", "detailed";

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
    Write-Host "Pushing packages from $NugetPackageArtifacts to $NugetPackageTarget"
    $packages = Get-ChildItem $NugetPackageArtifacts -Recurse -Include '*.nupkg'

    foreach ($package in $packages) {
        & $global:NugetPushCli push $package $env:MARVIN_NUGET_APIKEY -Source $NugetPackageTarget -Verbosity detail
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Set-Version ([string]$MajorMinorPatch) {
    Write-Host "Setting environment version to $MajorMinorPatch";

    $version = $MajorMinorPatch;
    $tagRegex = '^v?(\d+\.\d+\.\d+)(-([a-zA-Z]+)\.?(\d*))?$'; # v1.0.0-beta1

    $isVersionTag = $env:MARVIN_BRANCH -match $tagRegex
    if ($isVersionTag) {
        Write-Debug "Building commit tagged with a compatable version number"
        
        $version = $matches[1]
        $postTag = $matches[3]
        $count = $matches[4]

        Write-Debug "version number: ${version} post tag: ${postTag} count: ${count}"
        
        if ("$postTag" -ne "") {
            $version = "${version}-${postTag}"
        }
        if("$count" -ne ""){
            $padded = $count.Trim().PadLeft(6,"0");
            $version = "${version}${padded}"
        }
    } else {
        Write-Debug "Untagged"

        # Build number replacement is padded to 6 places
        $buildNumber = "$env:MARVIN_BUILDNUMBER".Trim().PadLeft(6,"0");

        # This is a general branch commit
        $branch = $env:MARVIN_BRANCH
        $branch = $branch.Replace("/","").ToLower()
       
        $version = "${version}-${branch}${buildNumber}";
    }

    $env:MARVIN_VERSION = $version;
    $env:MARVIN_ASSEMBLY_VERSION = $MajorMinorPatch + "." + $env:MARVIN_BUILDNUMBER;
}

function Set-AssemblyVersion([string]$InputFile) {
    $file = Get-Childitem -Path $inputFile;

    if (-Not $file) {
        Write-Host "AssemblyInfo: $inputFile was not found!";
        exit 1;
    }

    Write-Host "Applying assembly info of $($file.FullName) -> $env:MARVIN_ASSEMBLY_VERSION ";
   
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyVersion = 'AssemblyVersion("' + $env:MARVIN_ASSEMBLY_VERSION + '")';

    $assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyFileVersion = 'AssemblyFileVersion("' + $env:MARVIN_ASSEMBLY_VERSION + '")';

    $assemblyInformationalVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+)){3}"\)';
    $assemblyInformationalVersion = 'AssemblyInformationalVersion("' + $env:MARVIN_VERSION + '")';

    $assemblyConfigurationPattern = 'AssemblyConfiguration\("\w+"\)';
    $assemblyConfiguration = 'AssemblyConfiguration("' + $env:MARVIN_BUILD_CONFIG + '")';
    
    $content = (Get-Content $file.FullName) | ForEach-Object  { 
        ForEach-Object {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
        ForEach-Object {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion } |
        ForEach-Object {$_ -replace $assemblyInformationalVersionPattern, $assemblyInformationalVersion } |
        ForEach-Object {$_ -replace $assemblyConfigurationPattern, $assemblyConfiguration } 
    }

    Out-File -InputObject $content -FilePath $file.FullName -Encoding utf8;
}

function Set-AssemblyVersions([string[]]$Ignored = $(), [string]$SearchPath = $RootPath) {
    $Ignored = $Ignored + "\\.build\\" + "\\.buildtools\\" + "\\Tests\\" + "\\IntegrationTests\\" + "\\SystemTests\\";

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
    $manifestContent.PackageManifest.Metadata.Identity.Version = $env:MARVIN_ASSEMBLY_VERSION
    $manifestContent.Save($VsixManifest) 

    Write-Host "Version $env:MARVIN_ASSEMBLY_VERSION applied to $VsixManifest!"
}

function Set-VsTemplateVersion([string]$VsTemplate) {
    $file = Get-Childitem -Path $VsTemplate
    if (-Not $file) {
        Write-Host "VsTemplate: $VsTemplate was not found!"
        exit 1;
    }

    [xml]$templateContent = Get-Content $VsTemplate

    $versionRegex = "(\d+)\.(\d+)\.(\d+)\.(\d+)"

    $wizardAssemblyStrongName = $templateContent.VSTemplate.WizardExtension.Assembly -replace $versionRegex, $env:MARVIN_ASSEMBLY_VERSION 
    $templateContent.VSTemplate.WizardExtension.Assembly = $wizardAssemblyStrongName
    $templateContent.Save($vsTemplate)

    Write-Host "Version $env:MARVIN_ASSEMBLY_VERSION applied to $VsTemplate!"
}

function Install-EddieLight([string]$Version, [string]$TargetPath) {
    Write-Step "Installing EddieLight"

    $eddieLightPackage = "Marvin.Runtime.EddieLight";
    $eddieLightSource = [System.IO.Path]::Combine($BuildTools, "$eddieLightPackage.$Version\EddieLight\");
    $heartOfSilver = [System.IO.Path]::Combine($eddieLightSource, "SilverlightApp\HeartOfSilver.xap");
    $eddieLightPackage = "Marvin.Runtime.EddieLight";

    Install-Tool $eddieLightPackage $Version $heartOfSilver;

    CopyAndReplaceFolder $eddieLightSource $TargetPath;
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