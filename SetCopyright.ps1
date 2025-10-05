# PowerShell script to insert a copyright header into .cs files containing "namespace Moryx.*"
# Ignores files in bin, obj, node_modules, and all *.Designer.cs files

$header = @"
// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
"@

$commercialPattern = '#if\s+COMMERCIAL.*?#endif\r?\n?'

# Recursively find all .cs files, excluding bin, obj, node_modules, and *.Designer.cs
Get-ChildItem -Path . -Recurse -Filter *.cs -File | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|node_modules)\\' -and
    $_.Name -notlike '*.Designer.cs'
} | ForEach-Object {
    $filePath = $_.FullName
    $originalContent = Get-Content $filePath -Raw
    $modifiedContent = [System.Text.RegularExpressions.Regex]::Replace($originalContent, $commercialPattern, '', 'Singleline')

    $needsHeader = $modifiedContent -match 'namespace\s+Moryx\..*' -and $modifiedContent -notmatch "//\s*Copyright"
    if ($needsHeader) {
        $modifiedContent = "$header`r`n`r`n$modifiedContent"
    }

    if ($modifiedContent -ne $originalContent) {
        Set-Content -Path $filePath -Value $modifiedContent -Encoding UTF8
        Write-Host "Modified: $filePath"
    }
}



