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

If you encounter the error 'Install-Package: No match was found...', make sure NuGet is registered as your package source by running the following command:

``` 
Register-PackageSource -Name "NuGet.org" -Location "https://api.nuget.org/v3/index.json" -ProviderName "NuGet"
```

Or manually download the [Microsoft.AnalysisServices.NetCore.retail.amd64](https://www.nuget.org/packages/Microsoft.AnalysisServices.NetCore.retail.amd64) NuGet package and unzip it to the folder *.nuget\\* in the same folder as *FabricPS-PBIP.psm1* file

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

# Sample - Export item from workspace

```powershell

Export-FabricItem -workspaceId "[Workspace Id]" -itemId "[Item Id]" -path '[Export folder file path]'

```

# Sample - Import PBIP to workspace - item by item

```powershell

$semanticModelImport = Import-FabricItem -workspaceId "[Workspace Id]" -path "[PBIP Path]\[Name].SemanticModel"

# Import the report and ensure its binded to the previous imported report

$reportImport = Import-FabricItem -workspaceId $workspaceId -path "[PBIP Path]\[Name].Report" -itemProperties @{"semanticModelId"=$semanticModelImport.Id}

```

# Sample - Import PBIP item with different display name

```powershell

Import-FabricItem -workspaceId "[Workspace Id]" -path "[PBIP Path]\[Name].SemanticModel" -itemProperties @{"displayName"="[Semantic Model Name]"}

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

# Sample - Deploy semantic model and bind to Shared Cloud Connection (SCC)

Learn more about Sharec Cloud Connections [here](https://learn.microsoft.com/en-us/power-bi/connect-data/service-create-share-cloud-data-sources).

```powershell

$workspaceName = "[Workspace Name]"
$pbipPath = "[PBIP Path]\[Name].SemanticModel"
$connectionsToBind = @("[SCC Connection Id]")

# Authenticate

Set-FabricAuthToken -reset

# Ensure workspace exists

$workspaceId = New-FabricWorkspace  -name $workspaceName -skipErrorIfExists

# Import the semantic model and save the item id

$semanticModelImport = Import-FabricItem -workspaceId $workspaceId -path $pbipPath

# Bind to connections using PowerBI API - no need to specify the datasource, the service automatically maps the datasource to the connection

Write-Host "Binding semantic model to connections"

$authToken = Get-FabricAuthToken

# 'gatewayObjectId' as '00000000-0000-0000-0000-000000000000 indicate the connection is a sharable cloud.

$body = @{
    "gatewayObjectId"= "00000000-0000-0000-0000-000000000000";
    "datasourceObjectIds" = $connectionsToBind
} | ConvertTo-Json

$headers = @{'Content-Type'="application/json"; 'Authorization' = "Bearer $authToken"}

Invoke-RestMethod -Headers $headers -Uri "https://api.powerbi.com/v1.0/myorg/groups/$workspaceId/datasets/$($semanticModelImport.Id)/Default.BindToGateway" -Method Post -Body $body

```

# Sample - Invoke any Fabric API

```powershell

Import-Module ".\FabricPS-PBIP" -Force

Set-FabricAuthToken -reset

Invoke-FabricAPIRequest -uri "workspaces"

```