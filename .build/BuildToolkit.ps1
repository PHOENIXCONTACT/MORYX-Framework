# First check the powershell version
if ($PSVersionTable.PSVersion.Major -lt 5) {
    Write-Host ("The needed major powershell version for this script is 5. Your version: " + ($PSVersionTable.PSVersion.ToString()))
    exit 1;
}

# Tool Versions
$MsBuildVersion = "14.0"; # valid versions are [12.0, 14.0, 15.0, latest] (latest only works >= 15.0)
$NunitVersion = "3.7.0";
$OpenCoverVersion = "4.6.519";
$DoxyGenVersion = "1.8.9.2";
$OpenCoverToCoberturaVersion = "0.2.6.0";
$ReportGeneratorVersion = "3.0.2";
$VswhereVersion = "2.2.11";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\.buildtools";
$DotBuild = "$RootPath\.build";
$DocumentationDir = "$RootPath\Documentation";
$NunitReportsDir = "$RootPath\NUnitReportsDir";
$NupkgTarget = "$RootPath\Artefacts";

# Nuget
$NugetConfig = "$dotBuild\NuGet.Config";
$NugetPackageTarget = "http://127.0.0.1:5588/nuget/MaRVIN-CI/";
$NugetPackageTargetApiKey = "Admin:Admin";

# Load partial scripts
. "$DotBuild\Output.ps1";
. "$DotBuild\AssemblyVersion.ps1";
. "$DotBuild\SymbolStore.ps1";

# Define Tools
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\ReportGenerator.exe";
$global:DoxyGenCli = "$BuildTools\Doxygen.$DoxyGenVersion\tools\doxygen.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";
$global:VswhereCli = "$BuildTools\vswhere.$VswhereVersion\tools\vswhere.exe";

# Assign nuget.exe
function Install-Tool([string]$packageName, [string]$version, [string]$targetExecutable) {
    if (-not (Test-Path $targetExecutable)) {
        & $global:NugetCli install $packageName -version $version -outputdirectory $BuildTools -configfile $NugetConfig
        Invoke-ExitCodeCheck $LastExitCode;
    }
    else {
        Write-Host "$packageName ($version) already exists. Do not need to install."
    }
}

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

# Assign git.exe
$gitCommand = (Get-Command "git.exe" -ErrorAction SilentlyContinue);
if ($gitCommand -eq $null)  { 
    Write-Host "Unable to find git.exe in your PATH. Download from https://git-scm.com";
    Invoke-ExitCodeCheck 1;
}

$global:GitCli = $gitCommand.Path;

$GitCommitHash = (& $global:GitCli rev-parse --short HEAD);
$GitBranch = (& $global:GitCli rev-parse --abbrev-ref HEAD);

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
Write-Variable "DoxyGenCli" $global:DoxyGenCli;
Write-Variable "OpenCoverToCoberturaCli" $global:OpenCoverToCoberturaCli;
Write-Variable "VswhereCli" $global:VswhereCli;
Write-Variable "GitCli" $global:GitCli;
Write-Variable "GitCommitHash" $GitCommitHash;
Write-Variable "GitBranch" $GitBranch;

# Functions
function Invoke-Build([string]$solutionFile, [string]$configuration, [bool]$optimize = $True) {
    Write-Step "Building $solutionFile"

    Write-Host "Restoring Nuget packages of $solutionFile"
    & $global:NugetCli restore $solutionFile -Verbosity detailed -configfile $NugetConfig
    Invoke-ExitCodeCheck $LastExitCode;

    $params = "Configuration=$configuration,Optimize=" + (&{If($optimize) {"true"} Else {"false"}}) + ",DebugSymbols=true";

    & $global:MSBuildCli $solutionFile /p:$params /detailedsummary
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Nunit([string]$SearchPath, [string]$Name, [string]$Configuration) {
    Write-Step "Running $Name Tests: $SearchPath"

    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    $resultFileName = "$Name.TestResult.xml";
    if (Test-Path $resultFileName) {
        Remove-Item $resultFileName
    }

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include '*.csproj'

    & $global:NUnitCli $testProjects /config:"$Configuration" #/framework:"net-4.5" 
    Invoke-ExitCodeCheck $LastExitCode;

    Rename-Item -Path "TestResult.xml" -NewName "$resultFileName"
}

function Invoke-SmokeTest([string]$runtimePath, [int]$modulesCount, [int]$interruptTime, [int]$portIncrement) {
    Write-Step "Invoking Runtime SmokeTest Modules: $modulesCount, Interrupt Time: $interruptTime, Port Increment: $portIncrement."

    & $runtimePath "-r=SmokeTest -e=$modulesCount -i=$interruptTime -pi=$portIncrement"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-CoverTests([string]$SearchPath, [string]$FilterFile, $Configuration) {
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

    $testProjects = Get-ChildItem $SearchPath -Recurse -Include '*.csproj'
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

        $openCoverAgs = "-target:$global:NunitCli", "-targetargs:/config:$Configuration /result:$resultsXml /framework:net-4.5 $testProject"
        $openCoverAgs += "-log:Debug", "-register:user", "-output:$openCoverXml", "-hideskipped:all", "-skipautoprops", "-excludebyattribute:*OpenCoverIgnore*";
        $openCoverAgs += "-filter:$includeFilter $excludeFilter"

        & $global:OpenCoverCli $openCoverAgs
        Invoke-ExitCodeCheck $LastExitCode;

        & $global:OpenCoverToCoberturaCli -input:$openCoverXml -output:$coberturaXml -sources:$rootPath
        Invoke-ExitCodeCheck $LastExitCode;
    }
}

function Invoke-CoverReport([string]$SearchPath, [string]$Name) {
    Write-Step "Creating cover report for $Name. Searching for OpenCover.xml files in $SearchPath."

    if (-not (Test-Path $global:ReportGeneratorCli)) {
        Install-Tool "ReportGenerator" $ReportGeneratorVersion $global:ReportGeneratorCli;
    }
    
    $reports = (Get-ChildItem $SearchPath -Recurse -Include '*.OpenCover.xml');
    $asArgument = [string]::Join(";",$reports);

    & $global:ReportGeneratorCli -reports:"$asArgument" -targetDir:"$DocumentationDir\$Name.OpenCover\"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-DoxyGen {
    Write-Step "Generating documentation using doxygen"

    if (-not (Test-Path $global:DoxyGenCli)) {
        Install-Tool "Doxygen" $DoxyGenVersion $global:DoxyGenCli;
    }

    & $global:DoxyGenCli "$DotBuild\DefaultDoxyFile"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Pack($FilePath, $OutDir, $Version, $Configuration) {
    $packargs = @("-outputdirectory", "$OutDir", "-includereferencedprojects", "-Version", $Version, "-Prop", "Configuration=$Configuration");

    # Call nuget with default arguments plus optional
    & $global:NugetCli pack "$FilePath" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-PackAll($Directory, $OutDir, $Version, $Configuration) {
    if(!(Test-Path $OutDir)) {
        Write-Host "Creating missing directory '$OutDir'"
        New-Item $OutDir -Type Directory | Out-Null
    }

    Write-Host "Looking for .nuspec files..."
    # Look for nuspec in this directory
    foreach ($nuspecFile in Get-ChildItem $Directory -Recurse -Filter *.nuspec) {
        $nuspecPath = $nuspecFile.FullName
        Write-Host "Packing $nuspecPath" -ForegroundColor Green
        
        # Check if there is a matching proj for the nuspec
        $projectPath = [IO.Path]::ChangeExtension($nuspecPath, "csproj")
        if(Test-Path $projectPath) {
            Invoke-Pack $projectPath $OutDir $Version $Configuration
        } else {
            Invoke-Pack $nuspecPath $OutDir $Version $Configuration
        }
    }
}

function Invoke-Publish {
    Write-Host "Pushing packages from $NupkgTarget to $NugetPackageTarget"
    $packages = Get-ChildItem $NupkgTarget -Recurse -Include '*.nupkg'

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
    $informationalVersion = $majorMinorPatch + (&{If([string]::IsNullOrEmpty($Preview)) {"+$GitCommitHash"} Else {"-" + $Preview + "$buildNumber+$GitCommitHash"}});
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