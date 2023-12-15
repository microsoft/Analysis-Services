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

# Import PBIP content to workspace with overrides

```powershell

Import-Module ".\FabricPS-PBIP" -Force

$workspaceName = "[Workspace Name]"
$datasetName = "[Dataset Name]"
$reportName = "[Report Name]"
$pbipDatasetPath = "[Path to Dataset PBIP folder]"
$pbipReportPath = "[Path to Report PBIP folder]"

# Ensure workspace exists

$workspaceId = New-FabricWorkspace  -name $workspaceName -skipErrorIfExists

if (!$workspaceId) { throw "WorkspaceId cannot be null"}

# Deploy Dataset

$fileDatasetOverrides = @{    
    "*item.metadata.json" = @{
        "type" = "dataset"
        "displayName" = $datasetName
    } | ConvertTo-Json
}

$datasetId = Import-FabricItems -workspaceId $workspaceId -path $pbipDatasetPath -fileOverrides $fileDatasetOverrides

# Deploy Report

$fileReportOverrides = @{
    
    # Change the connected dataset

    "*definition.pbir" = @{
        "version" = "1.0"
        "datasetReference" = @{          
            "byConnection" =  @{
            "connectionString" = $null
            "pbiServiceModelId" = $null
            "pbiModelVirtualServerName" = "sobe_wowvirtualserver"
            "pbiModelDatabaseName" = "$datasetId"                
            "name" = "EntityDataSource"
            "connectionType" = "pbiServiceXmlaStyleLive"
            }
        }
    } | ConvertTo-Json

    # Change logo

    "*_7abfc6c7-1a23-4b5f-bd8b-8dc472366284171093267.jpg" = [System.IO.File]::ReadAllBytes("$currentPath\sample-resources\logo2.jpg")

    # Change theme
    
    "*Light4437032645752863.json" = [System.IO.File]::ReadAllBytes("$currentPath\sample-resources\theme_dark.json")

    # Report Name

    "*item.metadata.json" = @{
            "type" = "report"
            "displayName" = $reportName
        } | ConvertTo-Json
}

$reportId = Import-FabricItems -workspaceId $workspaceId -path $pbipReportPath -fileOverrides $fileReportOverrides

```