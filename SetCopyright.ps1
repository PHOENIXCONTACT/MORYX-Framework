# PowerShell-Skript zum Einfügen eines Copyright-Headers in .cs-Dateien mit "namespace Moryx.*"
# Ignoriert Dateien in bin, obj, node_modules und alle *.Designer.cs-Dateien

$header = @"
// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
"@

# Rekursiv alle .cs-Dateien im aktuellen Verzeichnis und Unterverzeichnissen finden
Get-ChildItem -Path . -Recurse -Filter *.cs -File | Where-Object {
    # Ignoriere Dateien in bin, obj, node_modules und alle *.Designer.cs-Dateien
    $_.FullName -notmatch '\\(bin|obj|node_modules)\\' -and
    $_.Name -notlike '*.Designer.cs'
} | ForEach-Object {
    $filePath = $_.FullName
    $content = Get-Content $filePath -Raw

    # Nur Dateien mit "namespace Moryx." berücksichtigen
    if ($content -match 'namespace\s+Moryx\..*') {
        # Prüfen, ob bereits ein Copyright-Hinweis vorhanden ist
        if ($content -notmatch "//\s*Copyright") {
            # Header einfügen und Datei überschreiben
            $newContent = "$header`r`n`r`n$content"
            Set-Content -Path $filePath -Value $newContent -Encoding UTF8
            Write-Host "Header hinzugefügt: $filePath"
        } else {
            Write-Host "Bereits vorhanden: $filePath"
        }
    } else {
        Write-Host "Übersprungen (kein Moryx-Namespace): $filePath"
    }
}
