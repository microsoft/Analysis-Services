$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

$workspaceName = "RR-APIsDemo-DeployPBIP"
$newTheme = "$currentPath\sample-resources\Theme_dark.json"
$exportFolder = "$currentPath\exportThemeSwap"

$workspace = Get-FabricWorkspace -workspaceName $workspaceName

# Exports all reports from workspace

Export-FabricItems -workspaceId $workspace.id -path $exportFolder -filter {$_.type -in @("report")}

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

# Swap theme file and import reports

foreach($themeFile in $themeFiles)
{
    Write-Host "Changing theme: '$themeFile'"

    $newThemeContent = Get-Content $newTheme

    $newThemeContent | Out-File $themeFile -Force

    $reportFolder = "$($themeFile.DirectoryName)\..\.."

    Import-FabricItems -workspaceId $workspace.id -path $reportFolder
}

