$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

$workspaceId = "cdfc383c-5eaa-4f39-91de-0eb26fdd2401"

Import-FabricItems -workspaceId $workspaceId -path '.\SamplePBIP' -filter "*Sales.Report*"