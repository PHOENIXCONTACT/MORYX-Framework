################################
# Functions for Console Output #
################################

function Write-Step([string]$step) {
    Write-Host "########################################################################################################" -foreground Magenta;
    Write-Host "#### $step" -foreground Magenta;
    Write-Host "########################################################################################################" -foreground Magenta
}

function Write-Variable ([string]$variableName, [string]$variableValue) {
    Write-Host ($variableName + " = " + $variableValue)
}

function Invoke-ExitCodeCheck([string]$exitCode) {
    if ([int]::Parse($exitCode) -gt 0) {
        Write-Host "This is the end, you know (ExitCode: $exitCode) - Lady, the plans we had went all wrong - We ain't nothing but fight and shout and tears." -ForegroundColor Red
        exit $exitCode;
    }
}