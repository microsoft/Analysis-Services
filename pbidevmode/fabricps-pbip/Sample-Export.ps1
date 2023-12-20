$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

$workspaceName = "RR-APIsDemo-DeployPBIP"

$workspace = Get-FabricWorkspace -workspaceName $workspaceName

Export-FabricItems -workspaceId $workspace.id -path '.\export' #-filter {$_.id -eq "74ab67cc-1cfc-4275-8a96-1d03ef4e186b"}
