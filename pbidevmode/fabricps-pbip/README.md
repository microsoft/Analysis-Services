# Setup

The FabricPS-PBIP module has a dependency to Az.Accounts module for authentication into Fabric.

Before running the sample scripts below, run the following script to download and install 'fabricps-pbip' module including it's dependencies:

```powershell

New-Item -ItemType Directory -Path ".\modules" -ErrorAction SilentlyContinue | Out-Null

@("https://raw.githubusercontent.com/microsoft/Analysis-Services/master/pbidevmode/fabricps-pbip/FabricPS-PBIP.psm1"
, "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/pbidevmode/fabricps-pbip/FabricPS-PBIP.psd1") |% {

    Invoke-WebRequest -Uri $_ -OutFile ".\modules\$(Split-Path $_ -Leaf)"
}

if(-not (Get-Module Az.Accounts -ListAvailable)) { 
    Install-Module Az.Accounts -Scope CurrentUser -Force
}

Import-Module ".\modules\FabricPS-PBIP" -Force

```

# Authentication

To call the Fabric API you must authenticate with a user account or Service Principal. Learn more about service principals and how to enable them [here](https://learn.microsoft.com/en-us/power-bi/enterprise/service-premium-service-principal).

## With user account

```powershell
Set-FabricAuthToken -reset
```

## With service principal (spn)

```powershell
Set-FabricAuthToken -servicePrincipalId $appId -servicePrincipalSecret $appSecret -tenantId $tenantId -reset
```

# Sample - Import PBIP to workspace

```powershell

Import-FabricItems -workspaceId "[Workspace Id]" -path "[PBIP file path]"

```

# Sample - Export items from workspace

```powershell

Export-FabricItems -workspaceId "[Workspace Id]" -path '[Export folder file path]'

```

# Sample - Import PBIP content to multiple Workspaces with file override


```powershell

$pbipPath = "[PBIP Path]"
$workspaceName = "[Workspace Name]"
$workspaceDatasets = "$workspaceName-Models"
$workspaceReports = "$workspaceName-Reports"

# Deploy Dataset

$workspaceId = New-FabricWorkspace -name $workspaceDatasets -skipErrorIfExists

$deployInfo = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\*.dataset"

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
        "displayName" = "Report NewName"
    } | ConvertTo-Json
}

$deployInfo = Import-FabricItems -workspaceId $workspaceId -path $pbipPath -filter "*\*.report" -fileOverrides $fileOverrides

```

# Sample - Import PBIP overriding semantic model parameters

```powershell

$pbipPath = "[PBIP Path]"
$workspaceId = "[Workspace Id]"

Set-SemanticModelParameters -path "$pbipPath\[Name].Dataset" -parameters @{"Parameter1"= "Parameter1Value"}

Import-FabricItems -workspaceId $workspaceId -path $pbipPath

```

# Sample - Create Workspace and set permissions

```powershell

$workspaceName = "[Workspace Name]"

$workspaceId = New-FabricWorkspace  -name $workspaceName -skipErrorIfExists

$workspacePermissions = @(
    @{
    "principal" = @{
        "id" = "[User Principal Id1]"
        "type" = "user"
    }
    "role"= "Admin"
    }
    ,
    @{
    "principal" = @{
        "id" = "[User Principal Id2]"
        "type" = "user"
    }
    "role"= "Member"
    } 
)

Set-FabricWorkspacePermissions -workspaceId $workspaceId -permissions $workspacePermissions

```

# Sample - Invoke any Fabric API

```powershell

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

Invoke-FabricAPIRequest -uri "workspaces"

```