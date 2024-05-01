# Setup

The FabricPS-PBIP module has a dependency to Az.Accounts module for authentication into Fabric.

Before running the sample scripts below, run the following script to download and install 'fabricps-pbip' module including it's dependencies:

> **Warning**
> This module was updated to work only with PBIP format produced by Power BI Desktop March 2024 update.

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
Set-FabricAuthToken -servicePrincipalId "[AppId]" -servicePrincipalSecret "[AppSecret]" -tenantId "[TenantId]" -reset
```


# Sample - Export items from workspace

```powershell

Export-FabricItems -workspaceId "[Workspace Id]" -path '[Export folder file path]'

```

# Sample - Import PBIP to workspace - all items

```powershell

Import-FabricItems -workspaceId "[Workspace Id]" -path "[PBIP file path]"

```

# Sample - Import PBIP to workspace - item by item

```powershell

$semanticModelImport = Import-FabricItem -workspaceId "[Workspace Id]" -path "[PBIP Path]\[Name].SemanticModel"

# Import the report and ensure its binded to the previous imported report

$reportImport = Import-FabricItem -workspaceId $workspaceId -path "[PBIP Path]\[Name].Report" -itemProperties @{"semanticModelId"=$semanticModelImport.Id}

```

# Sample - Import PBIP item with different display name

```powershell

$semanticModelImport = Import-FabricItem -workspaceId "[Workspace Id]" -path "[PBIP Path]\[Name].SemanticModel" -itemProperties @{"displayName"="[Semantic Model Name]"}

```

# Sample - Import PBIP overriding semantic model parameters

```powershell

$pbipPath = "[PBIP Path]"
$workspaceId = "[Workspace Id]"

Set-SemanticModelParameters -path "$pbipPath\[Name].SemanticModel" -parameters @{"Parameter1"= "Parameter1Value"}

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