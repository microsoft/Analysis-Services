$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

Invoke-FabricAPIRequest -uri "workspaces"
