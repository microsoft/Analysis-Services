$currentPath = (Split-Path $MyInvocation.MyCommand.Definition -Parent)

Set-Location $currentPath

Import-Module ".\FabricPS-PBIP" -Force

$workspaceName = "RR - FabricAPIs - Deploy 11"
$datasetName = "Dataset XX"
$reportName = "Report YY"
$pbipPath = "$currentPath\SamplePBIP"

# Ensure workspace exists

$workspaceId = New-FabricWorkspace  -name $workspaceName -skipErrorIfExists

$fileOverrides = @{
    
    # Dataset Name

    "*.dataset\item.metadata.json" = @{
        "type" = "dataset"
        "displayName" = $datasetName
    } | ConvertTo-Json

    # Report Name

    "*.report\item.metadata.json" = @{
        "type" = "report"
        "displayName" = $reportName
    } | ConvertTo-Json

    # Change logo

    "*_7abfc6c7-1a23-4b5f-bd8b-8dc472366284171093267.jpg" = "$currentPath\sample-resources\logo2.jpg"

    # Change theme
    
    "*Light4437032645752863.json" = [System.IO.File]::ReadAllBytes("$currentPath\sample-resources\theme_dark.json")
}

Import-FabricItems -workspaceId $workspaceId -path $pbipPath -fileOverrides $fileOverrides






