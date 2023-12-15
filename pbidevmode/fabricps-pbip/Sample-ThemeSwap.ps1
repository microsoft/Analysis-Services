$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

$workspaceId = "d020f53d-eb41-421d-af50-8279882524f3"
$newTheme = "$currentPath\sample-resources\Theme_dark.json"
$exportFolder = "$currentPath\exportThemeSwap"

# Exports all reports from workspace

Export-FabricItems -workspaceId $workspaceId -path $exportFolder -itemTypes @("report")

# Only change reports with theme files

$themeFiles = Get-ChildItem  -Path $exportFolder -recurse |? {
    
    if ($_.FullName -like "*StaticResources\RegisteredResources\*.json")
    {
        $jsonContent = Get-Content $_.FullName | ConvertFrom-Json

        if ($jsonContent.'$schema' -ilike "*reportThemeSchema*")
        {
            return $true
        }
    }
}

#Swap theme file and import reports

foreach($themeFile in $themeFiles)
{
    Write-Host "Changing theme: '$themeFile'"

    $newThemeContent = Get-Content $newTheme

    $newThemeContent | Out-File $themeFile -Force

    $reportFolder = "$($themeFile.DirectoryName)\..\.."

    Import-FabricItems -workspaceId $workspaceId -path $reportFolder
}

