# Tool Versions
$MsBuildVersion = "14.0"; # valid versions are [12.0, 14.0, 15.0, latest] (latest only works >= 15.0)
$NunitVersion = "3.7.0";
$OpenCoverVersion = "4.6.519";
$DocFxVersion = "2.27.0";
$OpenCoverToCoberturaVersion = "0.2.6.0";
$ReportGeneratorVersion = "3.0.2";
$VswhereVersion = "2.2.11";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\.buildtools";
$DotBuild = "$RootPath\.build";

# Artifacts
$ArtifactsDir = "$RootPath\Artifacts";

# Documentation
$DocumentationDir = "$RootPath\Documentation";

# Nunit
$NunitReportsDir = "$ArtifactsDir\NUnitReportsDir";

# Nuget
$NugetConfig = "$dotBuild\NuGet.Config";
$NugetPackageArtifacts = "$ArtifactsDir\Packages";
$NugetPackageTarget = "http://127.0.0.1:5588/nuget/MaRVIN-CI/";
$NugetPackageTargetApiKey = "Admin:Admin";

# Load partial scripts
. "$DotBuild\Output.ps1";
. "$DotBuild\AssemblyVersion.ps1";
. "$DotBuild\SymbolStore.ps1";

# Define Tools
$global:NugetCli = "";
$global:GitCli = "";
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\ReportGenerator.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";
$global:VswhereCli = "$BuildTools\vswhere.$VswhereVersion\tools\vswhere.exe";
$global:DocFxCli = "$BuildTools\docfx.console.$DocFxVersion\tools\docfx.exe";

# Git
$global:GitCommitHash = "";
$global:GitBranch = "";

# Functions
function Invoke-Initialize([bool]$Cleanup = $false) {
    Write-Step "Initializing BuildToolkit"

    # First check the powershell version
    if ($PSVersionTable.PSVersion.Major -lt 5) {
        Write-Host ("The needed major powershell version for this script is 5. Your version: " + ($PSVersionTable.PSVersion.ToString()))
        exit 1;
    }

    # Assign git.exe
    $gitCommand = (Get-Command "git.exe" -ErrorAction SilentlyContinue);
    if ($gitCommand -eq $null)  { 
        Write-Host "Unable to find git.exe in your PATH. Download from https://git-scm.com";
        Invoke-ExitCodeCheck 1;
    }
    
    $global:GitCli = $gitCommand.Path;

    # Load Hash
    $global:GitCommitHash = (& $global:GitCli rev-parse --short HEAD);
    Invoke-ExitCodeCheck $LastExitCode;

    # Current branch
    $global:GitBranch = (& $global:GitCli rev-parse --abbrev-ref HEAD);
    Invoke-ExitCodeCheck $LastExitCode;

    # Assign nuget.exe
    $nugetCommand = (Get-Command "nuget.exe" -ErrorAction SilentlyContinue);
    if ($nugetCommand -eq $null)  { 
        Write-Host "Unable to find nuget.exe in your PATH. Download from https://www.nuget.org/downloads";
        Invoke-ExitCodeCheck 1;
    }
    
    if ($nugetCommand.Version.Major -lt 4) {
        Write-Host "The minimum nuget.exe version should be 4.0.0.0. Currently installed: $($nugetCommand.Version)";
        Invoke-ExitCodeCheck 1;
    }
    
    $global:NugetCli = $nugetCommand.Path;
    
    # Assign msbuild.exe
    if ($MsBuildVersion -eq "latest" -or $MsBuildVersion -eq "15.0") {
        if (-not (Test-Path $global:VswhereCli)) {
            Install-Tool "vswhere" $VswhereVersion $VswhereCli;
        }
        $installPath = [string] (& $global:VswhereCli -latest -prerelease -products * -requires "Microsoft.Component.MSBuild" -property "installationPath");
        if ($installPath) {
            $global:MSBuildCli = Join-Path $installPath 'MSBuild\15.0\Bin\MSBuild.exe';
        }
    }
    else {
        $global:MSBuildCli = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$MsBuildVersion")."MSBuildToolsPath" -childpath "msbuild.exe"
    }
    
    if ($global:MSBuildCli -eq $null -or -not (Test-Path $global:MSBuildCli)) {
        Write-Host "Unable to find msbuild.exe.";
        Invoke-ExitCodeCheck 1;
    }
    
    # Printing Variables
    Write-Step "Printing global variables"
    Write-Variable "RootPath" $RootPath;
    Write-Variable "Version" $Version;
    Write-Variable "DocumentationDir" $DocumentationDir;
    Write-Variable "NunitReportsDir" $NunitReportsDir;

    Write-Step "Printing global scope"
    Write-Variable "MSBuildCli" $global:MSBuildCli;
    Write-Variable "NugetCli" $global:NugetCli;
    Write-Variable "OpenCoverCli" $global:OpenCoverCli;
    Write-Variable "NUnitCli" $global:NUnitCli;
    Write-Variable "ReportGeneratorCli" $global:ReportGeneratorCli;
    Write-Variable "DocFxCli" $global:DocFxCli;
    Write-Variable "OpenCoverToCoberturaCli" $global:OpenCoverToCoberturaCli;
    Write-Variable "VswhereCli" $global:VswhereCli;
    Write-Variable "GitCli" $global:GitCli;
    Write-Variable "GitCommitHash" $global:GitCommitHash;
    Write-Variable "GitBranch" $global:GitBranch;

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

function Install-Tool([string]$PackageName, [string]$Version, [string]$TargetExecutable, [string]$OutputDirectory = $BuildTools) {
    if (-not (Test-Path $TargetExecutable)) {
        & $global:NugetCli install $PackageName -version $Version -outputdirectory $OutputDirectory -configfile $NugetConfig
        Invoke-ExitCodeCheck $LastExitCode;
    }
    else {
        Write-Host "$PackageName ($Version) already exists. Do not need to install."
    }
}
function Invoke-Build([string]$SolutionFile, [string]$Configuration, [bool]$Optimize = $True, [string]$Options = "") {
    Write-Step "Building $SolutionFile"

    Write-Host "Restoring Nuget packages of $SolutionFile"
    & $global:NugetCli restore $SolutionFile -Verbosity detailed -configfile $NugetConfig
    Invoke-ExitCodeCheck $LastExitCode;

    $additonalOptions = "";
    if (-not [string]::IsNullOrEmpty($Options)) {
        $additonalOptions = ",$Options";
    }

    $params = "Configuration=$Configuration,Optimize=" + (&{If($Optimize) {"true"} Else {"false"}}) + ",DebugSymbols=true$additonalOptions";

    & $global:MSBuildCli $SolutionFile /p:$params /detailedsummary
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Nunit() {
    Param
    (
        [Parameter(Mandatory=$false, Position=0)]
        [string]$SearchPath = $RootPath,

        [Parameter(Mandatory=$false, Position=1)]
        [string]$SearchFilter = "*.csproj",

        [Parameter(Mandatory=$false, Position=3)]
        [string]$Configuration = "Debug"
    )

    Write-Step "Running $Name Tests: $SearchPath"

    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter

    & $global:NUnitCli $testProjects /config:"$Configuration"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-SmokeTest([string]$runtimePath, [int]$modulesCount, [int]$interruptTime, [int]$portIncrement) {
    Write-Step "Invoking Runtime SmokeTest Modules: $modulesCount, Interrupt Time: $interruptTime, Port Increment: $portIncrement."

    & $runtimePath "-r=SmokeTest -e=$modulesCount -i=$interruptTime -pi=$portIncrement"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-CoverTests {
    Param
    (
        [Parameter(Mandatory=$false, Position=0)]
        [string]$SearchPath = $RootPath,

        [Parameter(Mandatory=$false, Position=1)]
        [string]$SearchFilter = "*.csproj",

        [Parameter(Mandatory=$false, Position=2)]
        [string]$FilterFile = "$RootPath\OpenCoverFilter.txt", 

        [Parameter(Mandatory=$false, Position=3)]
        [string]$Configuration = "Debug"
    )
    
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

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include $SearchFilter
    ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));

        Write-Host "OpenCover Test: ${projectName}:";

        $projectDir = $testProject.DirectoryName;
        $resultsXml = ($projectDir + "\$projectName.TestResult.xml");
        $openCoverXml = ($projectDir + "\$projectName.OpenCover.xml");
        $coberturaXml = ($projectDir + "\$projectName.Cobertura.xml");

        $includeFilter = "+[Marvin*]*";
        $excludeFilter = "-[*nunit*]* -[*Tests]* -[*Model*]*";

        if (Test-Path $FilterFile) {
            # TODO: Load filter file 
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

        $openCoverAgs = "-target:$global:NunitCli", "-targetargs:/config:$Configuration /result:$resultsXml $testProject"
        $openCoverAgs += "-log:Debug", "-register:user", "-output:$openCoverXml", "-hideskipped:all", "-skipautoprops", "-excludebyattribute:*OpenCoverIgnore*";
        $openCoverAgs += "-filter:$includeFilter $excludeFilter"
        
        & $global:OpenCoverCli $openCoverAgs
        Invoke-ExitCodeCheck $LastExitCode;

        & $global:OpenCoverToCoberturaCli -input:$openCoverXml -output:$coberturaXml -sources:$rootPath
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Invoke-CoverReport {
    Param
    (
        [Parameter(Mandatory=$false, Position=0)]
        [string]$SearchPath = $RootPath
    )

    Write-Step "Creating cover report. Searching for OpenCover.xml files in $SearchPath."

    if (-not (Test-Path $global:ReportGeneratorCli)) {
        Install-Tool "ReportGenerator" $ReportGeneratorVersion $global:ReportGeneratorCli;
    }
    
    $reports = (Get-ChildItem $SearchPath -Recurse -Include '*.OpenCover.xml');
    $asArgument = [string]::Join(";",$reports);

    & $global:ReportGeneratorCli -reports:"$asArgument" -targetDir:"$ArtifactsDir\OpenCover\"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-DocFx {
    Param
    (
        [Parameter(Mandatory=$false, Position=0)]
        [string]$Metadata = [System.IO.Path]::Combine($DocumentationDir, "docfx.json")
    )

    Write-Step "Generating documentation using DocFx"

    if (-not (Test-Path $global:DocFxCli)) {
        Install-Tool "docfx.console" $DocFxVersion $global:DocFxCli;
    }

    & $global:DocFxCli $Metadata;
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Pack($FilePath, $Version, $Configuration) {
    if(!(Test-Path $NugetPackageArtifacts)) {
        Write-Host "Creating missing directory '$NugetPackageArtifacts'"
        New-Item $NugetPackageArtifacts -Type Directory | Out-Null
    }

    $packargs = @("-outputdirectory", "$NugetPackageArtifacts", "-includereferencedprojects", "-Version", $Version, "-Prop", "Configuration=$Configuration");

    # Call nuget with default arguments plus optional
    & $global:NugetCli pack "$FilePath" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-PackAll($Directory, $Version, $Configuration) {
    Write-Host "Looking for .nuspec files..."
    # Look for nuspec in this directory
    foreach ($nuspecFile in Get-ChildItem $Directory -Recurse -Filter *.nuspec) {
        $nuspecPath = $nuspecFile.FullName
        Write-Host "Packing $nuspecPath" -ForegroundColor Green
        
        # Check if there is a matching proj for the nuspec
        $projectPath = [IO.Path]::ChangeExtension($nuspecPath, "csproj")
        if(Test-Path $projectPath) {
            Invoke-Pack $projectPath $Version $Configuration
        } else {
            Invoke-Pack $nuspecPath $Version $Configuration
        }
    }
}

function Invoke-Publish {
    Write-Host "Pushing packages from $NugetPackageArtifacts to $NugetPackageTarget"
    $packages = Get-ChildItem $NugetPackageArtifacts -Recurse -Include '*.nupkg'

    foreach ($package in $packages) {
        & $global:NugetCli push $package $NugetPackageTargetApiKey -Source $NugetPackageTarget
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Get-MajorMinorPatchVersion($Version) {
    $versionSplit = $Version.Split(".");
    return "$($versionSplit[0]).$($versionSplit[1]).$($versionSplit[2])";
}

function Get-InformationalVersion($Version, $Preview) {
    $versionSplit = $Version.Split(".");
    $majorMinorPatch = "$($versionSplit[0]).$($versionSplit[1]).$($versionSplit[2])";
    $buildNumber = $versionSplit[3];

    # SemVer 2.0.0 will be used: "3.0.0+3ff33243" or "3.0.0-beta15432+3ff33243"
    $versionExt = (&{If([string]::IsNullOrEmpty($Preview)) {"+$global:GitCommitHash"} Else {"-" + $Preview + "$buildNumber+$global:GitCommitHash"}});
    $informationalVersion = $majorMinorPatch + $versionExt;
    return $informationalVersion;
}

function Get-NugetPackageVersion($Version, $Preview) {
    $versionSplit = $Version.Split(".");
    $majorMinorPatch = "$($versionSplit[0]).$($versionSplit[1]).$($versionSplit[2])";
    $buildNumber = $versionSplit[3];

    # SemVer 1.0.0 will be used: "3.0.0" or "3.0.0-beta15432"
    $packageVersion = $majorMinorPatch + (&{If([string]::IsNullOrEmpty($Preview)) {""} Else {"-" + $Preview + "$buildNumber"}})
    return $packageVersion;
}

function Install-EddieLight([string]$Version, [string]$TargetPath) {
    $eddieLightPackage = "Marvin.Runtime.EddieLight";
    $eddieLightSource = [System.IO.Path]::Combine($BuildTools, "$eddieLightPackage.$Version\EddieLight\");
    $heartOfSilver = [System.IO.Path]::Combine($TargetPath, "SilverlightApp\HeartOfSilver.xap");
    $eddieLightPackage = "Marvin.Runtime.EddieLight";

    Install-Tool $eddieLightPackage $Version $heartOfSilver;

    # Copy to target path
    Write-Host "Copy EddieLight from $eddieLightSource to $TargetPath ..."
    Copy-Item -Path $eddieLightSource -Recurse -Destination $TargetPath -Container
}