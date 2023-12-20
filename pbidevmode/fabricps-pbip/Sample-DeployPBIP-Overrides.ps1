$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

$workspaceName = "RR-APIsDemo-DeployPBIP"
$workspaceDatasets = "$workspaceName-Models"
$workspaceReports = "$workspaceName-Reports"

$pbipPath = "$currentPath\SamplePBIP"

# Deploy Dataset

$workspaceId = New-FabricWorkspace  -name $workspaceDatasets -skipErrorIfExists

$deployInfo = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\sales.dataset"

# Deploy Report

$workspaceId = New-FabricWorkspace  -name $workspaceReports -skipErrorIfExists

$fileOverrides = @{
    
    # Change the connected dataset

    "*definition.pbir" = @{
        "version" = "1.0"
        "datasetReference" = @{          
            "byConnection" =  @{
            "connectionString" = $null
            "pbiServiceModelId" = $null
            "pbiModelVirtualServerName" = "sobe_wowvirtualserver"
            "pbiModelDatabaseName" = "$($deployInfo.id)"                
            "name" = "EntityDataSource"
            "connectionType" = "pbiServiceXmlaStyleLive"
            }
        }
    } | ConvertTo-Json

    # Change logo

    "*_7abfc6c7-1a23-4b5f-bd8b-8dc472366284171093267.jpg" = "$currentPath\sample-resources\logo2.jpg"

    # Change Report Name

    "*.report\item.metadata.json" = @{
        "type" = "report"
        "displayName" = "Sales-NewName"
    } | ConvertTo-Json
}

$deployInfo = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\*.report" -fileOverrides $fileOverrides