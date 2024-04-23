$script:apiUrl = "https://api.fabric.microsoft.com/v1"
$script:resourceUrl = "https://api.fabric.microsoft.com" 
$script:fabricToken = $null

# Load TOM Assembly, required to manipulate the TMSL/TMDL of semantic models

$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

$nugets = @(
    @{
        name = "Microsoft.AnalysisServices.NetCore.retail.amd64"
        ;
        version = "19.77.0"
        ;
        path = @(
        "lib\netcoreapp3.0\Microsoft.AnalysisServices.Core.dll"
        , "lib\netcoreapp3.0\Microsoft.AnalysisServices.Tabular.dll"
        , "lib\netcoreapp3.0\Microsoft.AnalysisServices.Tabular.Json.dll"
        )
    }
)

foreach ($nuget in $nugets)
{
    if (!(Test-Path "$currentPath\.nuget\$($nuget.name).$($nuget.version)*" -PathType Container)) {
        
        Write-Host "Downloading and installing Nuget: $($nuget.name)"

        Install-Package -Name $nuget.name -ProviderName NuGet -Destination "$currentPath\.nuget" -RequiredVersion $nuget.Version -SkipDependencies -AllowPrereleaseVersions -Scope CurrentUser  -Force
    }
    
    foreach ($nugetPath in $nuget.path)
    {
        Write-Host "Loading assembly: '$nugetPath'"

        $path = Resolve-Path (Join-Path "$currentPath\.nuget\$($nuget.name).$($nuget.Version)" $nugetPath)
        
        Add-Type -Path $path -Verbose | Out-Null
    }
   
}

function Get-FabricAuthToken {
    <#
    .SYNOPSIS
        Get the Fabric API authentication token
    #>
    [CmdletBinding()]
    param
    (
    )

    if (!$script:fabricToken)
    {                
        Set-FabricAuthToken
    }
    
    Write-Output $script:fabricToken
}

function Set-FabricAuthToken {
    <#
    .SYNOPSIS
        Set authentication token for the Fabric service
    #>
    [CmdletBinding()]
    param
    (
        [string]$servicePrincipalId        
        ,
        [string]$servicePrincipalSecret
        ,
        [PSCredential]$credential
        ,
        [string]$tenantId 
        ,
        [switch]$reset
        ,
        [string]$apiUrl
    )

    if (!$reset)
    {
        $azContext = Get-AzContext
    }
    
    if ($apiUrl)
    {
        $script:apiUrl = $apiUrl
    }

    if (!$azContext) {
        
        Write-Host "Getting authentication token"
        
        if ($servicePrincipalId) {
            $credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $servicePrincipalId, ($servicePrincipalSecret | ConvertTo-SecureString -AsPlainText -Force)

            Connect-AzAccount -ServicePrincipal -TenantId $tenantId -Credential $credential | Out-Null

            Set-AzContext -Tenant $tenantId | Out-Null
        }
        elseif ($credential -ne $null)
        {
            Connect-AzAccount -Credential $credential -Tenant $tenantId | Out-Null
        }
        else {
            Connect-AzAccount | Out-Null
        }

        $azContext = Get-AzContext        
    }

    Write-Host "Connnected: $($azContext.Account)"

    $script:fabricToken = (Get-AzAccessToken -ResourceUrl $script:resourceUrl).Token
}

Function Invoke-FabricAPIRequest {
    <#
    .SYNOPSIS
        Sends an HTTP request to a Fabric API endpoint and retrieves the response.
        Takes care of: authentication, 429 throttling, Long-Running-Operation (LRO) response
    #>
    [CmdletBinding()]		
    param(									
        [Parameter(Mandatory = $false)] [string] $authToken,
        [Parameter(Mandatory = $true)] [string] $uri,
        [Parameter(Mandatory = $false)] [ValidateSet('Get', 'Post', 'Delete', 'Put', 'Patch')] [string] $method = "Get",
        [Parameter(Mandatory = $false)] $body,        
        [Parameter(Mandatory = $false)] [string] $contentType = "application/json; charset=utf-8",
        [Parameter(Mandatory = $false)] [int] $timeoutSec = 240,        
        [Parameter(Mandatory = $false)] [int] $retryCount = 0
            
    )

    if ([string]::IsNullOrEmpty($authToken)) {
        $authToken = Get-FabricAuthToken
    }	

    $fabricHeaders = @{
        'Content-Type'  = $contentType
        'Authorization' = "Bearer {0}" -f $authToken
    }

    try {
        
        $requestUrl = "$($script:apiUrl)/$uri"

        Write-Verbose "Calling $requestUrl"
        
        # If need to use -OutFile beware of the following breaking change: https://github.com/PowerShell/PowerShell/issues/20744
        $response = Invoke-WebRequest -Headers $fabricHeaders -Method $method -Uri $requestUrl -Body $body  -TimeoutSec $timeoutSec        

        $lroFailOrNoResultFlag = $false

        if ($response.StatusCode -eq 202)
        {
            do
            {                
                $asyncUrl = [string]$response.Headers.Location

                Write-Host "Waiting for request to complete. Sleeping..."

                Start-Sleep -Seconds 5

                $response = Invoke-WebRequest -Headers $fabricHeaders -Method Get -Uri $asyncUrl

                $lroStatusContent = $response.Content | ConvertFrom-Json

            }
            while($lroStatusContent.status -ine "succeeded" -and $lroStatusContent.status -ine "failed")

            if ($lroStatusContent.status -ieq "succeeded")
            {
                # Only calls /result if there is a location header, otherwise  'OperationHasNoResult' error is thrown

                $resultUrl = [string]$response.Headers.Location

                if ($resultUrl)
                {
                    $response = Invoke-WebRequest -Headers $fabricHeaders -Method Get -Uri $resultUrl    
                }
                else
                {
                    $lroFailOrNoResultFlag = $true
                }
            }
            else
            {
                $lroFailOrNoResultFlag = $true
                
                if ($lroStatusContent.error)
                {
                    throw "LRO API Error: '$($lroStatusContent.error.errorCode)' - $($lroStatusContent.error.message)"
                }
            }
            
        }

        #if ($response.StatusCode -in @(200,201) -and $response.Content)        
        if (!$lroFailOrNoResultFlag -and $response.Content)
        {            
            $contentBytes = $response.RawContentStream.ToArray()

            # Test for BOM

            if ($contentBytes[0] -eq 0xef -and $contentBytes[1] -eq 0xbb -and $contentBytes[2] -eq 0xbf)
            {
                $contentText = [System.Text.Encoding]::UTF8.GetString($contentBytes[3..$contentBytes.Length])                
            }
            else
            {
                $contentText = $response.Content
            }

            $jsonResult = $contentText | ConvertFrom-Json

            if ($jsonResult.value)
            {
                $jsonResult = $jsonResult.value
            }

            Write-Output $jsonResult -NoEnumerate
        }        
    }
    catch {
          
        $ex = $_.Exception
        
        $message = $null

        if ($ex.Response -ne $null) {

            $responseStatusCode = [int]$ex.Response.StatusCode

            if ($responseStatusCode -in @(429))
            {
                if ($ex.Response.Headers.RetryAfter)
                {
                    $retryAfterSeconds = $ex.Response.Headers.RetryAfter.Delta.TotalSeconds + 5
                }

                if (!$retryAfterSeconds)
                {
                    $retryAfterSeconds = 60
                }

                Write-Host "Exceeded the amount of calls (TooManyRequests - 429), sleeping for $retryAfterSeconds seconds."

                Start-Sleep -Seconds $retryAfterSeconds

                $maxRetries = 3
                
                if ($retryCount -le $maxRetries)
                {
                    Invoke-FabricAPIRequest -authToken $authToken -uri $uri -method $method -body $body -contentType $contentType -timeoutSec $timeoutSec -retryCount ($retryCount + 1)
                }
                else {
                    throw "Exceeded the amount of retries ($maxRetries) after 429 error."
                }

            }
            else
            {
                $apiErrorObj = $ex.Response.Headers |? {$_.key -ieq "x-ms-public-api-error-code"} | Select -First 1

                if ($apiErrorObj)
                {
                    $apiError = $apiErrorObj.Value[0]
                }

                if ($apiError -ieq "ItemHasProtectedLabel")
                {
                    Write-Warning "Item has a protected label."
                }
                else
                {
                    throw
                }

                # TODO: Investigate why response.Content is empty but powershell can read it on throw

                #$errorContent = $ex.Response.Content.ReadAsStringAsync().Result;
        
                #$message = "$($ex.Message) - StatusCode: '$($ex.Response.StatusCode)'; Content: '$errorContent'"
            }
        }
        else {
            $message = "$($ex.Message)"
        }
                
        if ($message)
        {
            throw $message
        }
    		
    }

}

Function New-FabricWorkspace {
    <#
    .SYNOPSIS
        Creates a new Fabric workspace.
    #>
    [CmdletBinding()]
    param
    (
        [string]$name
        ,
        [switch]$skipErrorIfExists        
    )

    $itemRequest = @{ 
        displayName = $name
    } | ConvertTo-Json

    try {        
        $createResult = Invoke-FabricAPIRequest -Uri "workspaces" -Method Post -Body $itemRequest

        Write-Host "Workspace created: '$name'"

        Write-Output $createResult.id
    }
    catch {
        $ex = $_.Exception

        if ($skipErrorIfExists) {
            if ($ex.Message -ilike "*409*") {
                Write-Host "Workspace '$name' already exists"

                $listWorkspaces = Invoke-FabricAPIRequest -Uri "workspaces" -Method Get

                $workspace = $listWorkspaces | ? { $_.displayName -ieq $name }

                if (!$workspace) {
                    throw "Cannot find workspace '$name'"
                }
                
                Write-Output $workspace.id
            }
            else {
                throw
            }
        }        
    }
    
}

Function Remove-FabricWorkspace {
    <#
    .SYNOPSIS
        Deletes a  Fabric workspace.
    #>
    [CmdletBinding()]
    param
    (
        [string]$workspaceId     
    )

    try {        

        Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId" -Method Delete
    }
    catch {
        throw
    }
}


Function Get-FabricWorkspace {
    <#
    .SYNOPSIS
        Get Fabric workspaces
    #>
    [CmdletBinding()]
    param
    (
        [string]$workspaceName
    )
      
    $result = Invoke-FabricAPIRequest -Uri "workspaces" -Method Get

    if ($workspaceName)
    {
        $workspace = $result |? {$_.displayName -ieq $workspaceName}

        if (!$workspace)
        {
            throw "Cannot find workspace '$workspaceName'"
        }

        Write-Output $workspace
    }
    else
    {
        Write-Output $result
    }
    
}

Function Set-FabricWorkspacePermissions {
    <#
    .SYNOPSIS
        Sets workspace role permissions
    #>
    [CmdletBinding()]
    param
    (
        [string]$workspaceId
        ,
        $permissions
    )

    try {        

        
        $existingRoles = Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/roleAssignments" -Method Get
        
        foreach ($permission in $permissions)
        {
            $matchRole = $existingRoles |? {$_.principal.id -ieq $permission.principal.id} | select -First 1
            
            if (!$matchRole)
            {
                Write-Host "Adding role '$($permission.role)' to '$($permission.principal.id)'"

                $request = $permission | ConvertTo-Json

                Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/roleAssignments" -Method Post -Body $request                
            }
            else {
                # If role already exists for principal, check the role

                if ($permission.role -ine $matchRole.role)
                {
                    Write-Host "Updating principal '$($permission.principal.id)' role to '$($permission.role)'"

                    $request = @{"role" = $permission.role} | ConvertTo-Json

                    Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/roleAssignments/$($permission.principal.id)" -Method Patch -Body $request
                }
            }
        }        
    }
    catch {
        throw
    }
}

Function Export-FabricItems {
    <#
    .SYNOPSIS
        Exports items from a Fabric workspace to a specified local file system destination.
    #>
    [CmdletBinding()]
    param
    (
        [string]$path = '.\pbipOutput'
        ,
        [string]$workspaceId = ''    
        ,
        [scriptblock]$filter = {$_.type -in @("report", "SemanticModel")}
        ,
        [string]$format
    )    

    $items = Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items" -Method Get

    if ($filter) {
        #$items = $items | ? { $_.type -in  $itemTypes }
        $items = $items | Where-Object $filter
    }

    Write-Host "Existing items: $($items.Count)"

    foreach ($item in $items) {
        $itemId = $item.id
        $itemName = $item.displayName
        $itemType = $item.type
        $itemOutputPath = "$path\$workspaceId\$($itemName).$($itemType)"

        if ($itemType -in @("report", "semanticmodel")) {
            Write-Host "Getting definition of: $itemId / $itemName / $itemType"

            #POST https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items/{itemId}/getDefinition

            $response = $null

            $getDefinitionUrl = "workspaces/$workspaceId/items/$itemId/getDefinition"

            if ($format)
            {
                $getDefinitionUrl += "?format=$format"
            }

            $response = Invoke-FabricAPIRequest -Uri $getDefinitionUrl -Method Post

            $partCount = $response.definition.parts.Count

            Write-Host "Parts: $partCount"
            
            if ($partCount -gt 0)
            {
                foreach ($part in $response.definition.parts) {
                    Write-Host "Saving part: $($part.path)"
                    
                    $outputFilePath = "$itemOutputPath\$($part.path.Replace("/", "\"))"

                    $parentFolderPath = Split-Path $outputFilePath -Parent

                    New-Item -ItemType Directory -Path $parentFolderPath -ErrorAction SilentlyContinue | Out-Null

                    $parentFolderPath = Resolve-Path $parentFolderPath

                    $bytes = [Convert]::FromBase64String($part.payload)

                    Set-Content -LiteralPath $outputFilePath $bytes -AsByteStream
                }

                @{
                    "type"        = $itemType
                    "displayName" = $itemName

                } | ConvertTo-Json | Out-File "$itemOutputPath\item.metadata.json"
            }
        }
        else {
            Write-Host "Type '$itemType' not available for export."
        }
    }
}

Function Import-FabricItems {
    <#
    .SYNOPSIS
        Imports items using the Power BI Project format (PBIP) into a Fabric workspace from a specified file system source.

    .PARAMETER fileOverrides
        This parameter let's you override a PBIP file without altering the local file. 
    
    .PARAMETER itemProperties
        This parameter let's you override item properties like type, displayName. 
        E.g. -itemProperties @{"<Item Folder Name>" = @{"type" = "SemanticModel"; "displayName"="<Name of the model>"}}
    #>
    [CmdletBinding()]
    param
    (
        [string]$path = '.\pbipOutput'
        ,
        [string]$workspaceId
        ,
        [string[]]$filter = $null
        ,
        [hashtable]$fileOverrides
        ,
        [hashtable]$itemProperties
    )

    # Search for folders with .pbir and .pbism in it

    $itemsInFolder = Get-ChildItem  -Path $path -recurse -include *.pbir, *.pbism

    if ($filter) {
        $itemsInFolder = $itemsInFolder | ? { 
            $pathFolder = $_.Directory.FullName
            $filter |? { $pathFolder -ilike $_ }
        }
    }

    if ($itemsInFolder.Count -eq 0)
    {
        Write-Host "No items found in the path '$path' (*.pbir; *.pbism)"
        return
    }

    Write-Host "Items in the folder: $($itemsInFolder.Count)"

    # File Overrides processing, convert all to base64 - Its the final format of the parts for Fabric APIs

    $fileOverridesEncoded = @()
    
    if ($fileOverrides)
    {
        foreach ($fileOverride in $fileOverrides.GetEnumerator())
        {
            $fileContent = $fileOverride.Value

            # convert to byte array

            if ($fileContent -is [string]) {
                
                # If its a valid path, read it as byte[]
                
                if (Test-Path $fileContent)
                {
                    $fileContent = [System.IO.File]::ReadAllBytes($fileContent)                        
                }
                else
                {
                    $fileContent = [system.Text.Encoding]::UTF8.GetBytes($fileContent)
                }
            }
            elseif (!($fileContent -is [byte[]])) {
                throw "FileOverrides value type must be string or byte[]"
            }
            
            $fileOverridesEncoded += @{Name=$fileOverride.Name; Value = $fileContent}
        }
    }

    # Get existing items of the workspace

    $items = Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items" -Method Get

    Write-Host "Existing items in the workspace: $($items.Count)"

    # Datasets first 

    $itemsInFolder = $itemsInFolder | Select-Object  @{n="Order";e={ if ($_.Name -like "*.pbism") {1} else {2} }}, * | sort-object Order    

    $datasetReferences = @{}

    foreach ($itemInFolder in $itemsInFolder) {	
        
        # Get the parent folder

        $itemName = $itemInFolder.Directory.Name
        $itemPath = $itemInFolder.Directory.FullName

        write-host "Processing item: '$itemPath'"

        $files = Get-ChildItem -Path $itemPath -Recurse -Attributes !Directory

        # Remove files not required for the API: item.*.json; cache.abf; .pbi folder

        $files = $files | ? { $_.Name -notlike "item.*.json" -and $_.Name -notlike "*.abf" -and $_.Directory.Name -notlike ".pbi" }        


        # Prioritizes reading the displayName and type from itemProperties parameter
        $itemType = $null
        $displayName = $null

        if ($itemProperties -ne $null)
        {            
            $foundItemProperty = $itemProperties."$itemName"

            if ($foundItemProperty)
            {
                $itemType = $foundItemProperty.type
    
                $displayName = $foundItemProperty.displayName
            }            
        }
        
        # Try to read the item properties from the .platform file if not found in itemProperties

        if ((!$itemType -or !$displayName) -and (Test-Path "$itemPath\.platform"))
        {            
            $itemMetadataStr = Get-Content "$itemPath\.platform"

            $fileOverrideMatch = $null
            if ($fileOverridesEncoded)
            {
                $fileOverrideMatch = $fileOverridesEncoded |? { "$itemPath\.platform" -ilike $_.Name  } | select -First 1
                if ($fileOverrideMatch) {
                    Write-Host "File override '.platform'"
                    $itemMetadataStr = [System.Text.Encoding]::UTF8.GetString($fileOverrideMatch.Value)
                }
            }

            $itemMetadata = $itemMetadataStr | ConvertFrom-Json

            $itemType = $itemMetadata.metadata.type
    
            $displayName = $itemMetadata.metadata.displayName
        }

        if (!$itemType -or !$displayName)
        {
            throw "Cannot import item if any of the following properties is missing: itemType, displayName"
        }

        $itemPathAbs = Resolve-Path $itemPath

        $parts = $files | % {
            
            $fileName = $_.Name
            $filePath = $_.FullName   
            
            $fileOverrideMatch = $null

            if ($fileOverridesEncoded)
            {
                $fileOverrideMatch = $fileOverridesEncoded |? { $filePath -ilike $_.Name  } | select -First 1            
            }

            if ($fileOverrideMatch) {

                Write-Host "File override '$fileName'"

                $fileContent = $fileOverrideMatch.Value              
            }
            else {                
                if ($filePath -like "*.pbir") {                  
    
                    $fileContentText = Get-Content -Path $filePath
                    $pbirJson = $fileContentText | ConvertFrom-Json

                    if ($pbirJson.datasetReference.byPath -and $pbirJson.datasetReference.byPath.path) {

                        # try to swap byPath to byConnection

                        $reportDatasetPath = (Resolve-path (Join-Path $itemPath $pbirJson.datasetReference.byPath.path.Replace("/", "\"))).Path

                        $datasetReference = $datasetReferences[$reportDatasetPath]       
                        
                        if ($datasetReference)
                        {
                            $datasetName = $datasetReference.name
                            $datasetId = $datasetReference.id
                            
                            $newPBIR = @{
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
                            
                            $fileContent = [system.Text.Encoding]::UTF8.GetBytes($newPBIR)

                        }
                        else
                        {
                            throw "Item API dont support byPath connection, switch the connection in the *.pbir file to 'byConnection'."
                        }
                    }
                    # if its byConnection then just send original
                    else {
                        $fileContent = [system.Text.Encoding]::UTF8.GetBytes($fileContentText)
                    }
                }
                else
                {
                    $fileContent = Get-Content -Path $filePath -AsByteStream -Raw                
                }
            }

            $partPath = $filePath.Replace($itemPathAbs, "").TrimStart("\").Replace("\", "/")

            $fileEncodedContent = [Convert]::ToBase64String($fileContent)
            
            Write-Output @{
                Path        = $partPath
                Payload     = $fileEncodedContent
                PayloadType = "InlineBase64"
            }
        }

        Write-Host "Payload parts:"        

        $parts |% { Write-Host "part: $($_.Path)" }

        $itemId = $null

        # Check if there is already an item with same displayName and type
        
        $foundItem = $items | ? { $_.type -ieq $itemType -and $_.displayName -ieq $displayName }

        if ($foundItem) {
            if ($foundItem.Count -gt 1) {
                throw "Found more than one item for displayName '$displayName'"
            }

            Write-Host "Item '$displayName' of type '$itemType' already exists." -ForegroundColor Yellow

            $itemId = $foundItem.id
        }

        if ($itemId -eq $null) {
            write-host "Creating a new item"

            # Prepare the request                    

            $itemRequest = @{ 
                displayName = $displayName
                type        = $itemType    
                definition  = @{
                    Parts = $parts
                }
            } | ConvertTo-Json -Depth 3		

            $createItemResult = Invoke-FabricAPIRequest -uri "workspaces/$workspaceId/items"  -method Post -body $itemRequest

            $itemId = $createItemResult.id

            write-host "Created a new item with ID '$itemId' $([datetime]::Now.ToString("s"))" -ForegroundColor Green

            Write-Output @{
                "id" = $itemId
                "displayName" = $displayName
                "type" = $itemType 
            }
        }
        else {
            write-host "Updating item definition"

            $itemRequest = @{ 
                definition = @{
                    Parts = $parts
                }			
            } | ConvertTo-Json -Depth 3		
            
            Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items/$itemId/updateDefinition" -Method Post -Body $itemRequest

            write-host "Updated item with ID '$itemId' $([datetime]::Now.ToString("s"))" -ForegroundColor Green

            Write-Output @{
                "id" = $itemId
                "displayName" = $displayName
                "type" = $itemType 
            }
        }

        # Save dataset references to swap byPath to byConnection

        if ($itemType -ieq "semanticmodel")
        {
            $datasetReferences[$itemPath] = @{"id" = $itemId; "name" = $displayName}
        }
    }

}

Function Import-FabricItem {
    <#
    .SYNOPSIS
        Imports items using the Power BI Project format (PBIP) into a Fabric workspace from a specified file system source.
    
    .PARAMETER itemProperties
        This parameter let's you override item properties like type, displayName. 
        E.g. -itemProperties @{"type" = "SemanticModel"; "displayName"="<Name of the model>"}
    #>
    [CmdletBinding()]
    param
    (
        [string]$path = '.\pbipOutput'
        ,
        [string]$workspaceId
        ,
        [hashtable]$itemProperties
    )

    # Search for folders with .pbir and .pbism in it

    $itemsInFolder = Get-ChildItem -Path $path |? {@(".pbism", ".pbir")-contains $_.Extension }

    if ($itemsInFolder.Count -eq 0)
    {
        Write-Host "Cannot find valid item definitions (*.pbir; *.pbism) in the '$path'"
        return
    }    

    if ($itemsInFolder |? {$_.Extension -ieq ".pbir"})
    {
        $itemType = "Report"
    }elseif($itemsInFolder |? {$_.Extension -ieq ".pbism"})
    {
        $itemType = "SemanticModel"
    }
    else {
        throw "Cannot determine the itemType."
    }
    
    # Get existing items of the workspace

    $items = Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items" -Method Get

    Write-Host "Existing items in the workspace: $($items.Count)"

    $files = Get-ChildItem -Path $path -Recurse -Attributes !Directory

    # Remove files not required for the API: item.*.json; cache.abf; .pbi folder

    $files = $files | ? { $_.Name -notlike "item.*.json" -and $_.Name -notlike "*.abf" -and $_.Directory.Name -notlike ".pbi" }        

    # Prioritizes reading the displayName and type from itemProperties parameter    
    $displayName = $null
    
    if ($itemProperties -ne $null)
    {            
        $displayName = $itemProperties.displayName         
    }

    # Try to read the item properties from the .platform file if not found in itemProperties

    if ((!$itemType -or !$displayName) -and (Test-Path "$path\.platform"))
    {            
        $itemMetadataStr = Get-Content "$path\.platform"

        $fileOverrideMatch = $null
        if ($fileOverridesEncoded)
        {
            $fileOverrideMatch = $fileOverridesEncoded |? { "$path\.platform" -ilike $_.Name  } | select -First 1
            if ($fileOverrideMatch) {
                Write-Host "File override '.platform'"
                $itemMetadataStr = [System.Text.Encoding]::UTF8.GetString($fileOverrideMatch.Value)
            }
        }

        $itemMetadata = $itemMetadataStr | ConvertFrom-Json

        $itemType = $itemMetadata.metadata.type

        $displayName = $itemMetadata.metadata.displayName
    }

    if (!$itemType -or !$displayName)
    {
        throw "Cannot import item if any of the following properties is missing: itemType, displayName"
    }

    $itemPathAbs = Resolve-Path $path

    $parts = $files | % {

        $filePath = $_.FullName
        
        if ($filePath -like "*.pbir") {

            $fileContentText = Get-Content -Path $filePath
            $pbirJson = $fileContentText | ConvertFrom-Json

            if ($pbirJson.datasetReference.byPath -and $pbirJson.datasetReference.byPath.path) {

                $datasetId = $itemProperties.semanticModelId

                if (!$datasetId)
                {
                    throw "Cannot import directly a report using byPath connection. You must first resolve the semantic model id and pass it through the 'itemProperties' parameter."
                }
                
                $newPBIR = @{
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
                
                $fileContent = [system.Text.Encoding]::UTF8.GetBytes($newPBIR)
            }
            # if its byConnection then just send original
            else {
                $fileContent = [system.Text.Encoding]::UTF8.GetBytes($fileContentText)
            }
        }
        else
        {
            $fileContent = Get-Content -Path $filePath -AsByteStream -Raw
        }
        
        $partPath = $filePath.Replace($itemPathAbs, "").TrimStart("\").Replace("\", "/")

        $fileEncodedContent = [Convert]::ToBase64String($fileContent)
        
        Write-Output @{
            Path        = $partPath
            Payload     = $fileEncodedContent
            PayloadType = "InlineBase64"
        }
    }

    Write-Host "Payload parts:"        

    $parts |% { Write-Host "part: $($_.Path)" }

    $itemId = $null

    # Check if there is already an item with same displayName and type
    
    $foundItem = $items | ? { $_.type -ieq $itemType -and $_.displayName -ieq $displayName }

    if ($foundItem) {
        if ($foundItem.Count -gt 1) {
            throw "Found more than one item for displayName '$displayName'"
        }

        Write-Host "Item '$displayName' of type '$itemType' already exists." -ForegroundColor Yellow

        $itemId = $foundItem.id
    }

    if ($itemId -eq $null) {
        write-host "Creating a new item"

        # Prepare the request                    

        $itemRequest = @{ 
            displayName = $displayName
            type        = $itemType    
            definition  = @{
                Parts = $parts
            }
        } | ConvertTo-Json -Depth 3		

        $createItemResult = Invoke-FabricAPIRequest -uri "workspaces/$workspaceId/items"  -method Post -body $itemRequest

        $itemId = $createItemResult.id

        write-host "Created a new item with ID '$itemId' $([datetime]::Now.ToString("s"))" -ForegroundColor Green

        Write-Output @{
            "id" = $itemId
            "displayName" = $displayName
            "type" = $itemType 
        }
    }
    else {
        write-host "Updating item definition"

        $itemRequest = @{ 
            definition = @{
                Parts = $parts
            }			
        } | ConvertTo-Json -Depth 3		
        
        Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items/$itemId/updateDefinition" -Method Post -Body $itemRequest

        write-host "Updated item with ID '$itemId' $([datetime]::Now.ToString("s"))" -ForegroundColor Green

        Write-Output @{
            "id" = $itemId
            "displayName" = $displayName
            "type" = $itemType 
        }
    }
}

Function Remove-FabricItems {
    <#
    .SYNOPSIS
        Removes selected items from a Fabric workspace.
    #>
    [CmdletBinding()]
    param
    (
        [string]$workspaceId = $null
        ,
        [string]$filter = $null 
    )
   
    if (!$fabricHeaders) {
        $fabricHeaders = Get-FabricHeaders
    }

    $items = Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items" -Method Get

    Write-Host "Existing items: $($items.Count)"

    if ($filter) {
        $items = $items | ? { $_.DisplayName -like $filter }
    }

    foreach ($item in $items) {
        $itemId = $item.id
        $itemName = $item.displayName

        Write-Host "Removing item '$itemName' ($itemId)"
        
        Invoke-FabricAPIRequest -Uri "workspaces/$workspaceId/items/$itemId" -Method Delete
    }
    
}

Function Set-SemanticModelParameters {
    <#
    .SYNOPSIS
        TODO
    #>
    [CmdletBinding()]
    param
    (
        [string]$path = $null
        ,
        [hashtable]$parameters = $null
        ,
        [switch]$failIfNotFound
    )

    $modelPath = "$path\definition"

    $isTMSL = $false

    if (!(Test-Path $modelPath))
    {
        $modelPath = "$path\model.bim"
        $isTMSL = $true
    }

    if (!(Test-Path $modelPath))
    {
        throw "Cannot find semantic model definition: '$modelPath'"
    }

    $compatibilityMode = [Microsoft.AnalysisServices.CompatibilityMode]::PowerBI

    if ($isTMSL)
    {
        $modelText = Get-Content $modelPath
    
        $database = [Microsoft.AnalysisServices.Tabular.JsonSerializer]::DeserializeDatabase($modelText, $null, $compatibilityMode)
    }
    else {
        $database = [Microsoft.AnalysisServices.Tabular.TmdlSerializer]::DeserializeDatabaseFromFolder($modelPath)
    }

    $database.CompatibilityMode = $compatibilityMode

    # Set expression parameters

    $changedFlag = $false

    $parameters.GetEnumerator() |? {

        $parameterName = $_.Name
        $parameterValue = $_.Value

        $modelExpression = $database.Model.Expressions.Find($parameterName)

        if (!$modelExpression)
        {
            if ($failIfNotFound)
            {
                throw "Cannot find model expression '$parameterName'"
            }
            else {
                Write-Host "Cannot find model expression '$parameterName'"
            }
        }
        else
        {
            Write-Host "Changing model expression '$parameterName'"
            $modelExpression.Expression = $modelExpression.Expression -replace """?(.*)""? meta","""$parameterValue"" meta"
            $changedFlag = $true
        }
    }

    if ($changedFlag)
    {
        $serializeOptions = New-Object Microsoft.AnalysisServices.Tabular.SerializeOptions
        
        if ([string]::IsNullOrEmpty($database.Name))
        {
            # If serialized without name an error is thrown later on deserialize. TODO: Review

            $database.Name = "Unknown"
        }

        if ($isTMSL)
        {
            $modelText = [Microsoft.AnalysisServices.Tabular.JsonSerializer]::SerializeDatabase($database, $serializeOptions)

            $modelText | Out-File $modelPath -Force
        }
        else {
            [Microsoft.AnalysisServices.Tabular.TmdlSerializer]::SerializeDatabaseToFolder($database, $modelPath)
        }
    }
}