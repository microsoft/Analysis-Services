$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

$workspaceName = "RR-APIsDemo-DeployPBIP"
$pbipPath = "$currentPath\SamplePBIP"

# Ensure workspace exists

$workspaceId = New-FabricWorkspace  -name $workspaceName -skipErrorIfExists

# Import the PBIP to service

Import-FabricItems -workspaceId $workspaceId -path $pbipPath
