function CheckVersionParameter([string]$version) {
    $versionPattern = "[0-9]+(\.([0-9]+|\*)){1,3}";
    if ($version -notmatch $versionPattern) {
        Write-Host "Version does not match the pattern $versionPattern";
        return $False;
    }
    return $True;
}