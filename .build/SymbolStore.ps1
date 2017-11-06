function Publish-PDBs {
    Param(
        [parameter(Position=0, Mandatory=$true)]
        [string]$Project,

        [parameter(Position=1, Mandatory=$false)]
        [string]$Comment,

        [parameter(Position=2, Mandatory=$false)]
        [string]$SearchPath = [System.IO.Path]::Combine($PSScriptRoot, "..\"),

        [parameter(Position=3, Mandatory=$false)]
        [string]$Storage = "E:\SymbolStore"
    )

    if (-not (Test-Path $Storage)) {
        Write-Host "Storage $Storage does not exists. Error!";
        exit 1;
    }

    $symstore = "C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\symstore.exe";
    $symbolStore = [System.IO.Path]::Combine($Storage, $Project);

    if (-not (Test-Path -Path $symbolStore)) {
        Write-Host "Path to symbol store does not exists: $symbolStore, creating ...";
        try {
            New-Item $symbolStore -ItemType Directory
        }
        catch {
            Write-Host "Path to symbol store $symbolStore cannot be created.";
            exit 1;
        }
    }

    Write-Host "SearchPath for Projects: $SearchPath";

    $csprojs = Get-Childitem $SearchPath -recurse | Where-Object {$_.extension -eq ".csproj"}

    foreach ($csporj in $csprojs) {
        Write-Host;
        Write-Host "Reading csproj: $($csporj.Name)"; 

        $csprojXml = [xml](Get-Content $csporj.FullName);
        
        $outputGroup = $csprojXml.Project.PropertyGroup | Where-Object Condition -Like '*Release|AnyCPU*';
        $outputPath = $outputGroup.OutputPath;

        if (-not $outputPath.Contains("..\Build\")) {
            Write-Host "Project $($csporj.Name) will be ignored because it have not the output folder to \Build\";
            continue;
        }
       
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

        Write-Host "Querying SymStore and check for existence of: $pdbFileName"
        
        $symStoreQuery = [string] (& $symstore query /r /f $pdbPath /s $symbolStore 2>&1);
        if (-not $symStoreQuery.Contains("NOT FOUND")) {
           Write-Host "The $pdbFileName was already found in SymStore."
           continue;
        }

        Write-Host "The $pdbFileName will be added to SymStore.";

        $args = @("add", "/f", $pdbPath, "/s", $symbolStore, "/t", $Project, "/compress");
        if (-not [string]::IsNullOrEmpty($Comment)) {
            $args += "/c";
            $args += $Comment;
        }

        & $symstore $args
    }
}