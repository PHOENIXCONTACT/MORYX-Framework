# First check the powershell version
if ($PSVersionTable.PSVersion.Major -lt 4) {
    Write-Host ("The needed major powershell version for this script is 4. Your version: " + ($PSVersionTable.PSVersion.ToString()))
    exit 1;
}

# Tool Versions
$MsBuildVersion = "14.0"; # valid versions are [2.0, 3.5, 4.0, 12.0, 14.0]
$NunitVersion = "3.7.0";
$OpenCoverVersion = "4.6.519";
$DoxyGenVersion = "1.8.9.2";
$OpenCoverToCoberturaVersion = "0.2.6.0";
$ReportGeneratorVersion = "3.0.2";

# Folder Pathes
$RootPath = $MyInvocation.PSScriptRoot;
$BuildTools = "$RootPath\.buildtools";
$DotBuild = "$RootPath\.build";
$DocumentationDir = "$RootPath\Documentation";
$NunitReportsDir = "$RootPath\NUnitReportsDir";
$NupkgTarget = "$RootPath\Artefacts";

# Nuget
$NugetConfig = "$dotBuild\NuGet.Config";
$NugetCliSource = "\\nts-eu-jenk02.europe.phoenixcontact.com\Sources\nuget\latest\nuget.exe";

$NugetPackageTarget = "http://127.0.0.1:5588/nuget/MaRVIN-CI/";
$NugetPackageTargetApiKey = "Admin:Admin";

# Load partial scripts
. "$DotBuild\Output.ps1";
. "$DotBuild\Version.ps1";
. "$DotBuild\AssemblyVersion.ps1";

# Define Tools
$global:MSBuildCli = join-path -path (Get-ItemProperty "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$MsBuildVersion")."MSBuildToolsPath" -childpath "msbuild.exe"
$global:NugetCli = "$BuildTools\nuget\nuget.exe"
$global:OpenCoverCli = "$BuildTools\OpenCover.$OpenCoverVersion\tools\OpenCover.Console.exe";
$global:NunitCli = "$BuildTools\NUnit.ConsoleRunner.$NunitVersion\tools\nunit3-console.exe";
$global:ReportGeneratorCli = "$BuildTools\ReportGenerator.$ReportGeneratorVersion\tools\ReportGenerator.exe";
$global:DoxyGenCli = "$BuildTools\Doxygen.$DoxyGenVersion\tools\doxygen.exe";
$global:OpenCoverToCoberturaCli = "$BuildTools\OpenCoverToCoberturaConverter.$OpenCoverToCoberturaVersion\tools\OpenCoverToCoberturaConverter.exe";

function Write-Variables {
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
}

function Install-Nuget {
    Write-Step "Installing nuget"

    $nugetFolderPath = Split-Path -Path $global:NugetCli -Parent

	if (-not (Test-Path $nugetFolderPath)) {
		New-Item $nugetFolderPath -Type Directory | Out-Null
	}
	
    # Check if nuget already exists
    if (Test-Path $global:NugetCli) {
        Write-Host "Nuget already exists. Do not need to install.";
        return;
    }

	# If not download it!
    Write-Host "Nuget does not exist. Downloading from $NugetCliSource" -foreground Green;
    
    try { 
        Invoke-WebRequest -Uri $NugetCliSource -OutFile $global:NugetCli
    } catch {
        Invoke-ExitCodeCheck 1;
    }
}

function Install-Tool([string]$packageName, [string]$version, [string]$targetExecutable) {
    if (-not (Test-Path $targetExecutable)) {
        & $global:NugetCli install $packageName -version $version -outputdirectory $BuildTools -configfile $NugetConfig
        Invoke-ExitCodeCheck $LastExitCode;
    }
    else {
        Write-Host "$packageName ($version) already exists. Do not need to install."
    }
}

function Invoke-Build([string]$solutionFile, [string]$configuration) {
    Write-Step "Building $solutionFile"

    Write-Host "Restoring Nuget packages of $solutionFile"
    & $global:NugetCli restore $solutionFile -Verbosity detailed -configfile $NugetConfig
    Invoke-ExitCodeCheck $LastExitCode;

    & $global:MSBuildCli $solutionFile /p:Configuration=$configuration /detailedsummary
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-Nunit([string]$testsPath, [string]$name) {
    Write-Step "Running $name Tests: $testsPath"

    if (-not (Test-Path $global:NUnitCli)) {
        Install-Tool "NUnit.Console" $NunitVersion $global:NunitCli;
    }

    $resultFileName = "$name.TestResult.xml";
    if (Test-Path $resultFileName) {
        Remove-Item $resultFileName
    }

    $testProjects = Get-ChildItem $testsPath -Recurse -Include '*.csproj'

    & $global:NUnitCli $testProjects /framework:"net-4.5" /config:"$Configuration"
    Invoke-ExitCodeCheck $LastExitCode;

    Rename-Item -Path "TestResult.xml" -NewName "$resultFileName"
}

function Invoke-SmokeTest([string]$runtimePath, [int]$modulesCount, [int]$interruptTime, [int]$portIncrement) {
    Write-Step "Invoking Runtime SmokeTest Modules: $modulesCount, Interrupt Time: $interruptTime, Port Increment: $portIncrement."

    & $runtimePath "-r=SmokeTest -e=$modulesCount -i=$interruptTime -pi=$portIncrement"
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-CoverTests([string]$searchPath, [string]$filterFile) {
    Write-Step "Starting cover tests from $searchPath with filter $filterFile."
    
    if (-not (Test-Path $searchPath)) {
        Write-Host "$searchPath does not exists, ignoring!";
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

    $testProjects = Get-ChildItem $searchPath -Recurse -Include '*.csproj'
    ForEach($testProject in $testProjects ) { 
        $projectName = ([System.IO.Path]::GetFileNameWithoutExtension($testProject.Name));

        Write-Host "OpenCover Test: ${projectName}:";

        $projectDir = $testProject.DirectoryName;
        $resultsXml = ($projectDir + "\$projectName.TestResult.xml");
        $openCoverXml = ($projectDir + "\$projectName.OpenCover.xml");
        $coberturaXml = ($projectDir + "\$projectName.Cobertura.xml");

        $includeFilter = "+[Marvin*]*";
        $excludeFilter = "-[*nunit*]* -[*Tests]* -[*Model*]*";

        if (Test-Path $filterFile) {
            # TODO: Load filter file 
            $ignoreContent = Get-Content $filterFile;

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
        } else {
            Write-Host "Filter file $filterFile does not exists, ignoring!"
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

function Invoke-CoverReport([string]$searchPath, [string]$name) {
    Write-Step "Creating cover report for $name. Searching for OpenCover.xml files in $searchPath."

    if (-not (Test-Path $global:ReportGeneratorCli)) {
        Install-Tool "ReportGenerator" $ReportGeneratorVersion $global:ReportGeneratorCli;
    }
    
    $reports = (Get-ChildItem $searchPath -Recurse -Include '*.OpenCover.xml');
    $asArgument = [string]::Join(";",$reports);

    & $global:ReportGeneratorCli -reports:"$asArgument" -targetDir:"$DocumentationDir\$name.OpenCover\"
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

function Invoke-Pack($filePath, $outdir) {
    $packargs = @("-outputdirectory", "$outdir", "-includereferencedprojects");

    #$packargs += "-msbuildversion"
    #$packargs += $MsBuildVersion

    $packargs += "-Version"
    $packargs += $Version

    # Call nuget with default arguments plus optional
    & $global:NugetCli pack "$filePath" @packargs
    Invoke-ExitCodeCheck $LastExitCode;
}

function Invoke-PackAll($directory, $outdir) {
    if(!(Test-Path $outdir)) {
        Write-Host "Creating missing directory '$outdir'"
        New-Item $outdir -Type Directory | Out-Null
    }

    Write-Host "Looking for .nuspec files..."
    # Look for nuspec in this directory
    foreach ($nuspecFile in Get-ChildItem $directory -Recurse -Filter *.nuspec) {
        $nuspecPath = $nuspecFile.FullName
        Write-Host "Packing $nuspecPath" -ForegroundColor Green
        
        # Check if there is a matching proj for the nuspec
        $projectPath = [IO.Path]::ChangeExtension($nuspecPath, "csproj")
        if(Test-Path $projectPath) {
            Invoke-Pack $projectPath $outdir
        } else {
            Invoke-Pack $nuspecPath $outdir
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

Install-Nuget
Write-Variables