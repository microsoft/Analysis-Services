$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

# Set-FabricAuthToken -reset

$workspaceId = "552934ee-09ac-45c2-920f-d8da4e9e2764"

Export-FabricItems -workspaceId $workspaceId -path '.\export_TMDL_Jast' #-filter {$_.id -eq "74ab67cc-1cfc-4275-8a96-1d03ef4e186b"}
