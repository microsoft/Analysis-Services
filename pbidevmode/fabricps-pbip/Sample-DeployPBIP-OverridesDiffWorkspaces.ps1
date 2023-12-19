$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

$workspaceDatasets = "RR - FabricAPIs - Deploy [Datasets]"
$workspaceReports = "RR - FabricAPIs - Deploy [Reports]"

$pbipPath = "$currentPath\SamplePBIP"

# Deploy Dataset

$workspaceId = New-FabricWorkspace  -name $workspaceDatasets -skipErrorIfExists

$dataset = Import-FabricItems -workspaceId $workspaceId -path "$pbipPath\Sales.Dataset"

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
            "pbiModelDatabaseName" = "$($dataset.id)"                
            "name" = "EntityDataSource"
            "connectionType" = "pbiServiceXmlaStyleLive"
            }
        }
    } | ConvertTo-Json
}

$reportId = Import-FabricItems -workspaceId $workspaceId -path "$pbipPath\Sales.Report" -fileOverrides $fileOverrides






