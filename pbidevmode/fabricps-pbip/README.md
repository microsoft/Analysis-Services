# Requirements

The FabricPS-PBIP module has a dependency to Az.Accounts module for authentication into Fabric.

Run the following command before importing FabricPS-PBIP to ensure the "Az.Accounts" is installed in your machine:

```powershell
Install-Module Az.Accounts
```

# Export items from workspace

```powershell

Import-Module ".\FabricPS-PBIP" -Force

Export-FabricItems -workspaceId "[Workspace Id]" -path '[Export folder file path]'

```

# Import PBIP content to workspace

```powershell

Import-Module ".\FabricPS-PBIP" -Force

Import-FabricItems -workspaceId "[Workspace Id]" -path "[PBIP file path]"

```

# Import PBIP content to multiple Workspaces

```powershell

Import-Module ".\FabricPS-PBIP" -Force

$workspaceDatasets = "RR - FabricAPIs - Deploy [Datasets]"
$workspaceReports = "RR - FabricAPIs - Deploy [Reports]"

$pbipPath = "$currentPath\SamplePBIP"

# Deploy Dataset

$workspaceId = New-FabricWorkspace  -name $workspaceDatasets -skipErrorIfExists

$dataset = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\sales.dataset"

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

    # Change logo

    "*_7abfc6c7-1a23-4b5f-bd8b-8dc472366284171093267.jpg" = "$currentPath\sample-resources\logo2.jpg"
}

$reportId = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\*.report" -fileOverrides $fileOverrides

```