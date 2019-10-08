# DeployAASAutoScale.ps1
# Installs or updates Azure Analysis Services QPU AutoScale.

# Do not manually change tier while AutoScale is configured or unexpected scaling will occur since the alerts need updating by the autoscale runbook.  
# Before manually changing tier after AutoScale is configured, remove AutoScale with the -Remove parameter.

# This script must be executed by an AAD authenticated user who is a server level administrator of the AAS instance and administrator/owner of the Automation Account.  
# NOTE: If Active Directory Connect is not setup in AAD to translate Windows accounts to AAD automatically, it is necessary to run Login-AzAccount in the PowerShell command window before running this script.

[CmdletBinding(DefaultParameterSetName="ConfigParameters")]
param(
    [Parameter (Mandatory = $true, HelpMessage = "Subscription containing the Resource Group, AAS instance and Automation Account for AutoScale configuration.", ParameterSetName = "ConfigParameters")]
    [Parameter (Mandatory = $true, ParameterSetName = "RemovalParameters")]
    [string] $SubscriptionId, 
    [Parameter (Mandatory = $true, HelpMessage = "The Resource Group of the AAS instance and the Automation Account that will be used to perform AutoScale.  These must be in the same resource group.", ParameterSetName = "ConfigParameters")]
    [Parameter (Mandatory = $true, ParameterSetName = "RemovalParameters")]
    [string] $ResourceGroupName, 

    [Parameter (Mandatory = $true, HelpMessage = "Automation Account must import modules Az.Accounts, Az.AnalysisServices, Az.Automation, Az.Monitor, and AzureRM.Insights, and be in the Resource Group specified in the ResourceGroupName parameter.", ParameterSetName = "ConfigParameters")] 
    [Parameter (Mandatory = $true, ParameterSetName = "RemovalParameters")]
    [string] $AutomationAccount, 
    [Parameter (Mandatory = $true, HelpMessage = "AAS instance to configure AutoScale.  The instance must be in the Resource Group provided in the ResourceGroupName paramater.", ParameterSetName = "ConfigParameters")]
    [Parameter (Mandatory = $true, ParameterSetName = "RemovalParameters")]
    [string] $ASServerName, 

    [Parameter (Mandatory = $true, HelpMessage = "The alert interval and window over which QPU usage is monitored for AutoScale alerts.", ParameterSetName = "ConfigParameters")]
    [int] $AlertWindowInMins,
    
    [Parameter (Mandatory = $true, HelpMessage = "Minimum tier for the instance's AutoScale configuration, for example: S0.", ParameterSetName = "ConfigParameters")]
    [string] $MinTier, 
    [Parameter (Mandatory = $true, HelpMessage = "Maximum tier for the instance's AutoScale configuration, for example: S4.", ParameterSetName = "ConfigParameters")]
    [string] $MaxTier, 
    
    # Scale-out to add replicas only starts after reaching $MaxTier.
    [Parameter (Mandatory = $true, HelpMessage = "Minimum number of read-only replicas to maintain for the AutoScale configuration.", ParameterSetName = "ConfigParameters")]
    [int] $MinReplicas,
    [Parameter (Mandatory = $true, HelpMessage = "Maximum number of read-only replicas to scale-out for the AutoScale configuration.`nNOTE: Scale-out of additional replicas only starts after reaching MaxTier.", ParameterSetName = "ConfigParameters")]
    [int] $MaxReplicas = 0, 
    
    # Even when $SeparateProcessingNodeFromQueryReplicas = $true, if $MinReplicas = 0, then processing cannot be isolated whenever we scale all the way in to 0 replicas.
    [Parameter (Mandatory = $false, ParameterSetName = "ConfigParameters")]
    [bool] $SeparateProcessingNodeFromQueryReplicas = $true,
    
    # Scale-In at X% below the lower tier's max, and Scale-Up/Out at X% below the current tier's max.
    [Parameter (Mandatory = $false, ParameterSetName = "ConfigParameters")]
    [int] $ScaleUpDownOutAtPctDistanceFromTierMax = 10, 
    # Scale-In at X% below the current tier's limit.  Since tier doesn't change with scale-in, the threshold to remove a replica can be higher.
    [Parameter (Mandatory = $false, ParameterSetName = "ConfigParameters")]
    [int] $ScaleInAtPctDistanceFromTierMax = 25, 

    # Uninstall AutoScale for the instance.
    [Parameter (Mandatory = $false, ParameterSetName = "RemovalParameters")]
    [switch] $Remove = $false,

    # Forces even if instance is not within configured AutoScale limits currently, or an existing alert is already in progress.  Could cause non-deterministic results.
    [Parameter (Mandatory = $false, ParameterSetName = "ConfigParameters")]
    [switch] $Force= $false
)

function Get-AzAccessToken {
    [CmdletBinding()]
    param ()

    $ErrorActionPreference = 'Stop'
  
    if(-not (Get-Module Az.Accounts)) {
        Import-Module Az.Accounts
    }

    $azProfile = [Microsoft.Azure.Commands.Common.Authentication.Abstractions.AzureRmProfileProvider]::Instance.Profile
    if (-not $azProfile.Accounts.Count) {
        Write-Error 'Could not find a valid AzProfile, please run Connect-AzAccount'
        return
    }

    $currentAzureContext = Get-AzContext
    $profileClient = New-Object Microsoft.Azure.Commands.ResourceManager.Common.RMProfileClient($azProfile)
    $token = $profileClient.AcquireAccessToken($currentAzureContext.Tenant.TenantId)
    $token.AccessToken
}

$ErrorActionPreference = "stop"

cls
Write-Host "`n`n`n`n`n`nConfiguring Azure Analysis Services QPU AutoScale for instance:" -ForegroundColor Green
$PSBoundParameters | Format-Table -HideTableHeaders

$Token = Get-AzAccessToken
$SKUSObj = Invoke-RestMethod -Uri "https://management.azure.com/subscriptions/$($SubscriptionId)/resourceGroups/$($ResourceGroupName)/providers/Microsoft.AnalysisServices/servers/$($ASServerName)/skus?api-version=2017-08-01" -Headers @{"Authorization" = "Bearer " + $Token }
$SKUS = $SKUSObj.value | Foreach {"$($_.sku.name)"}

# Get the current SKU to appropriately set initial conditions
$srv = Get-AzAnalysisServicesServer -ResourceGroupName $ResourceGroupName -Name $ASServerName -WarningAction silentlyContinue -ErrorAction Stop
$CurrentSKU = $srv.Sku.Name 
$CurrentCapacity = $srv.Sku.Capacity
if ($srv.State -ne "Succeeded")
{
    Write-Host "Server must be active to setup AutoScale.  AutoScale will not be configured.  Please start the server first." -ForegroundColor Red
    exit
}

if ($Remove)
{
    # Remove everything
    Write-Host "-Remove parameter specified.  Removing AutoScale." -ForegroundColor Green
    Write-Host "Removing metric alerts..." -NoNewline
    Remove-AzMetricAlertRuleV2 -Name "AutoScaleDownAlert" -ResourceGroupName $ResourceGroupName -ErrorAction SilentlyContinue -WarningAction Ignore | Out-Null 
    Remove-AzMetricAlertRuleV2 -Name "AutoScaleUpAlert" -ResourceGroupName $ResourceGroupName -ErrorAction SilentlyContinue -WarningAction Ignore | Out-Null 
    Write-Host " Success" -ForegroundColor Green
    Write-Host "Removing action groups..." -NoNewline
    Remove-AzActionGroup -Name "AutoScaleUpActionGroup" -ResourceGroupName $ResourceGroupName -ErrorAction SilentlyContinue -WarningAction Ignore | Out-Null
    Remove-AzActionGroup -Name "AutoScaleDownActionGroup" -ResourceGroupName $ResourceGroupName -ErrorAction SilentlyContinue -WarningAction Ignore | Out-Null
    Write-Host " Success" -ForegroundColor Green
    Write-Host "Removing runbook..." -NoNewline
    Remove-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -ErrorAction SilentlyContinue -WarningAction Ignore
    Remove-AzAutomationRunbook -Name "AASAutoScale-$($ASServerName)" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Force -ErrorAction SilentlyContinue -WarningAction Ignore | Out-Null
    Write-Host " Success" -ForegroundColor Green
    Write-Host "`nAutoScale removed." -ForegroundColor Green
    exit
}

# This gets resused in the runbook script below as well as in the install script so reuse and store as an expression string
$TierMaxesExpression = @'
    $TierMaxes = [ordered]@{
        'S0' = 40; 
        'S1' = 100;
        'S2' = 200;
        'S4' = 400;
        'S8' = 320; 
        'S9' = 640;
        'S8v2' = 640; 
        'S9v2' = 1280;
    }
'@
# We trim down the TierMaxes to only reflect those SKUs that are available.
$TierMaxesExpression = $TierMaxesExpression.Split("`n") | Foreach-Object {if ($SKUS -ccontains $_.Substring($_.IndexOf("'") + 1, 2) -or $_.IndexOf("'") -eq -1) {$_}}
# If S8v2 supported, then remove S8 and S9 from our scaling path unless the instance is currently already on S8 or S9, or Min or Max tiers are explicitly S8 or S9, and then remove v2 versions.
if ($SKUS.Contains("S8v2") -and $CurrentSKU -ne "S8" -and $CurrentSKU -ne "S9" -and ("S8", "S9") -cnotcontains $MinTier -and ("S8", "S9") -cnotcontains $MaxTier) 
{ $TierMaxesExpression = $TierMaxesExpression.Split("`n") | Foreach-Object {if ($_.IndexOf("'S8'") -eq -1 -and $_.IndexOf("'S9'") -eq -1) {$_}} | Out-String } 
else
{ $TierMaxesExpression = $TierMaxesExpression.Split("`n") | Foreach-Object {if ($_.IndexOf("'S8v2'") -eq -1 -and $_.IndexOf("'S9v2'") -eq -1) {$_}} | Out-String }

# Invoke the expression here where it is used in this install script too...
Invoke-Expression $TierMaxesExpression

$MaxReplicasForRegion = 0
try { Set-AzAnalysisServicesServer -Name quicktest -ResourceGroupName jburchel-AS -ReadonlyReplicaCount 99 -ErrorAction SilentlyContinue }
catch
{ 
    $err = $Error[0].Exception.Message
    $err = $err.Substring($err.IndexOf("The 99 argument is greater than the maximum allowed range of ") + "The 99 argument is greater than the maximum allowed range of ".Length)
    $MaxReplicasForRegion = [int]$err.Substring(0, $err.IndexOf("."))
}

if ($MaxReplicasForRegion -lt $MaxReplicas)
{
    Write-Host ("Maximum replica count specified would exceed region capacity.  AutoScale will not be configured.  Specify a -MaxReplicas value less than or equal to the region maximum of " + $MaxReplicasForRegion + ".") -ForegroundColor Red
    exit
}

if (-not $TierMaxes.Contains($MinTier) -or -not $TierMaxes.Contains($MaxTier)) 
{
    Write-Host "MinTier and MaxTier parameters must fall within avaible tiers for the instance.  AutoScale will not be configured.  Available tiers in the current region are:" -ForegroundColor Red
    $TierMaxes.Keys | Out-String | Write-Host
    exit
}

$ExistingAlert = Get-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -ErrorAction SilentlyContinue
if ($ExistingAlert -ne "Idle" -and $ExistingAlert -ne $null -and $Force -ne $True)
{
    Write-Host "An existing AutoScale alert for this instance is still being processed.  AutoScale will not be configured.  Please wait until all alerts resolve before updating AutoScale." -ForegroundColor Red
    exit
}

Write-Host "Configuration details:" -ForegroundColor Green
[ordered]@{
    "Minimum tier:" = $MinTier
    "Maximum tier:" = $MaxTier
    "Minimum replica count:" = $MinReplicas
    "Maximum replica count:" = "" + $MaxReplicas + " (adding replicas only after reaching max tier)"
    "Isolate processing from replicas:" = $SeparateProcessingNodeFromQueryReplicas
    "Alert window/frequency:" = "Checks QPU every " + $AlertWindowInMins + " minute(s), over the prior " + $AlertWindowInMins + " minute(s)."
    "Scale-Up threshold:" = "Increase tier when avg QPU exceeds " + $ScaleUpDownOutAtPctDistanceFromTierMax + "% below current tier QPU max."
    "Scale-Down threshold:" = "Reduce tier when max QPU remains more than " + $ScaleUpDownOutAtPctDistanceFromTierMax + "% below QPU max of lower tier."
    "Scale-Out threshold:" = "Add replica (after max tier) when avg QPU exceeds " + $ScaleUpDownOutAtPctDistanceFromTierMax + "% below current tier QPU max."
    "Scale-In threshold:" = "Remove replica when QPU remains more than " + $ScaleInAtPctDistanceFromTierMax + "% below QPU max for current tier."
} | Format-Table -HideTableHeaders -AutoSize

if ($Force -ne $True -and
    (
        $($TierMaxes.Keys).IndexOf($CurrentSKU) -lt $($TierMaxes.Keys).IndexOf($MinTier) -or `
        $($TierMaxes.Keys).IndexOf($CurrentSKU) -gt $($TierMaxes.Keys).IndexOf($MaxTier) -or `
        $CurrentCapacity -gt ($MaxReplicas + 1) -or `
        $CurrentCapacity -lt ($MinReplicas + 1)
    )
   )
{
    Write-Host "Instance's current tier ($($CurrentSKU)) and replica count ($($CurrentCapacity - 1)) exceed the configured AutoScale limits.  AutoScale will not be configured.`nInstance must be running within AutoScale configuration's tier/capacity limits to complete this install." -ForegroundColor Red
    exit
}

Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 0

$MinCapacity = $MinReplicas + $SeparateProcessingNodeFromQueryReplicas
if ($MinCapacity -eq 0) { $MinCapacity = 1 }

# This large string variable that follows is the PS script used for the AutoScale runbook triggered when scaling alerts fire
$PSScript = @'
param(
    [Parameter (Mandatory = $true)]
    [boolean] $Up,
    [Parameter (Mandatory = $false)]
    [object] $WebhookData
)

function GetNextLowerQPUTier
{
    # find the highest QPU available in the tiers below (to avoid scaling down from S9 to S8, which would leave more memory but reduce QPU more than S4)
    $HighestLowerTierQPUAvailable = 0
    for ($i = $($TierMaxes.Keys).IndexOf($CurrentSKU) - 1; $i -ge 0; $i--)
    {
        if ($HighestLowerTierQPUAvailable -lt $TierMaxes[$i]) { $HighestLowerTierQPUAvailable = $TierMaxes[$i] }
    }
    return ($TierMaxes.GetEnumerator() | Where-Object {$_.Value -eq $HighestLowerTierQPUAvailable}).Key
}

$ErrorActionPreference = "Stop"

if ($WebhookData) { $body = ConvertFrom-Json -InputObject $WebhookData.RequestBody } 
else
{
    Write-Output "Missing information"
    exit
}

$AlertStage = $body.data.status

$ResourceGroupName = "
'@ + $ResourceGroupName + @'
"
$AutomationAccount = "
'@ + $AutomationAccount + @'
"
$ASServerName = "
'@ + $ASServerName + @'
"
$SubscriptionId = "
'@ + $SubscriptionId + @'
"
$AlertWindowInMins = 
'@ + $AlertWindowInMins + @'

$Window = New-TimeSpan -Minutes $AlertWindowInMins
$MaxTier = "
'@ + $MaxTier + @'
"
$MinTier = "
'@ + $MinTier + @'
"
$MaxReplicas = 
'@ + $MaxReplicas + @'

$MinReplicas = 
'@ + $MinReplicas + @'

$SeparateProcessingNodeFromQueryReplicas = $
'@ + $SeparateProcessingNodeFromQueryReplicas + @'

$MinCapacity = $MinReplicas + $SeparateProcessingNodeFromQueryReplicas
if ($MinCapacity -eq 0) { $MinCapacity = 1 }

$ScaleUpDownOutAtPctDistanceFromTierMax = 
'@ + $ScaleUpDownOutAtPctDistanceFromTierMax  + @'

$ScaleInAtPctDistanceFromTierMax = 
'@ + $ScaleInAtPctDistanceFromTierMax + @'


'@ + $TierMaxesExpression + @'


$servicePrincipalConnection=Get-AutomationConnection -Name "AzureRunAsConnection" 
Connect-AzAccount -ServicePrincipal -Tenant $servicePrincipalConnection.TenantId -ApplicationId $servicePrincipalConnection.ApplicationId -CertificateThumbprint $servicePrincipalConnection.CertificateThumbprint | Out-Null

$Status = Get-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount

if (($Status.Value -ne "Idle" -and $AlertStage -ne "Deactivated") -or ($Status.Value -ne "Active" -and $AlertStage -eq "Deactivated"))
{ 
    Write-Output "Another alert is already in progress so this one is being skipped."
    exit 
}
if ($AlertStage -eq "Activated") { Set-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Value "Active" -Encrypted $false | Out-Null }
if ($AlertStage -eq "Deactivated") { Set-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Value "Deactivating" -Encrypted $false | Out-Null }

# Get current SKU and determine new SKU based on that and the $Up parameter
$srv = Get-AzAnalysisServicesServer -ResourceGroupName $ResourceGroupName -Name $ASServerName -WarningAction silentlyContinue -ErrorAction Stop
$CurrentSKU = $NewSKU = $srv.Sku.Name 
$CurrentCapacity = $NewCapacity = $srv.Sku.Capacity

$TargetResourceId = "/subscriptions/" + $SubscriptionId + "/resourceGroups/" + $ResourceGroupName + "/providers/Microsoft.AnalysisServices/servers/" + $ASServerName

$AutoScaleUpActionGroupId = "/subscriptions/" + $SubscriptionId + "/resourceGroups/" + $ResourceGroupName +"/providers/microsoft.insights/actiongroups/AutoScaleUpActionGroup"
$AutoScaleDownActionGroupId = "/subscriptions/" + $SubscriptionId + "/resourceGroups/" + $ResourceGroupName +"/providers/microsoft.insights/actiongroups/AutoScaleDownActionGroup"
$AutoScaleUpActionGroup = New-AzActionGroup -ActionGroupId $AutoScaleUpActionGroupId -WarningAction silentlyContinue -ErrorAction Stop
$AutoScaleDownActionGroup = New-AzActionGroup -ActionGroupId $AutoScaleDownActionGroupId -WarningAction silentlyContinue -ErrorAction Stop

# It is necessary to update the alerts when first activated, so they get resolved in the portal.  Otherwise they cannot fire again.
if ($AlertStage -eq "Activated")
{
    if ($Up -eq $true)
    {
        Write-Output "Scale-up alert activated."
        if ($CurrentSKU -ne $MaxTier) 
        {
            # find the next higher SKU that _also_ has higher QPU (to avoid scaling up to S8 from S4, which would increase memory but reduce available QPU)
            for ($i = $($TierMaxes.Keys).IndexOf($CurrentSKU) + 1; $i -lt $TierMaxes.Count; $i++)
            {
                # checking the QPU of the next higher tier against current QPU
                if ($TierMaxes[$i] -gt $TierMaxes[$CurrentSKU])
                {
                    $NewSKU = $($TierMaxes.Keys)[$i]
                    break
                }
            }
        }

        if ($CurrentSKU -eq $MaxTier -and $CurrentCapacity -lt ($MaxReplicas + 1)) { $NewCapacity = $CurrentCapacity + 1 }
        
        $Dimensions = New-AzMetricAlertRuleV2DimensionSelection -DimensionName "ServerResourceType" -ValuesToInclude "*" -WarningAction silentlyContinue -ErrorAction Stop
        
        $UpperBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Average -Operator GreaterThan -Threshold 99999 -WarningAction silentlyContinue -ErrorAction Stop
        Add-AzMetricAlertRuleV2 -Condition $UpperBoundCriteria -Name "AutoScaleUpAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleUpActionGroup -WindowSize 00:01:00 -Frequency 00:01:00 -Severity 3 -WarningAction silentlyContinue -ErrorAction Stop | Out-Null
        Write-Output "New upper bound set to 99999 just to trigger deactivation of alert.  The runbook will be called again on deactivation and new alert values will be set then."
    }
    else
    {
        Write-Output "Scale-down alert activated."
        if ($CurrentCapacity -gt $MinCapacity) 
        { 
            $NewCapacity = $CurrentCapacity - 1 
        }
        else
        {
            $NewCapacity = $CurrentCapacity
            $NewSKU = GetNextLowerQPUTier
        }

        $Dimensions = New-AzMetricAlertRuleV2DimensionSelection -DimensionName "ServerResourceType" -ValuesToInclude "*" -WarningAction silentlyContinue -ErrorAction Stop

        $LowerBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Maximum -Operator LessThan -Threshold 0  -WarningAction silentlyContinue -ErrorAction Stop
        Add-AzMetricAlertRuleV2 -Condition $LowerBoundCriteria -Name "AutoScaleDownAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleDownActionGroup -WindowSize 00:01:00 -Frequency 00:01:00 -Severity 3  -WarningAction silentlyContinue -ErrorAction Stop | Out-Null
        Write-Output "New lower bound set to 0 just to trigger deactivation of alert.  The runbook will be called again on deactivation and new alert values will be set then."
    }
    
    Write-Output ("Current SKU: `t" + $CurrentSKU)
    Write-Output ("Current replica count: `t$($CurrentCapacity - 1)")
    Write-Output ("New SKU: `t" + $NewSKU)   
    Write-Output ("New replica count: `t" + ($NewCapacity - 1))
        
    if ($SeparateProcessingNodeFromQueryReplicas -eq $true -and $NewCapacity -gt 1) { $DefaultConnectionMode = "Readonly" } else { $DefaultConnectionMode = "All" }

    # Update tier
    Set-AzAnalysisServicesServer -ResourceGroupName $ResourceGroupName -Name $ASServerName -Sku $NewSKU -ReadonlyReplicaCount ($NewCapacity - 1) -DefaultConnectionMode $DefaultConnectionMode -WarningAction silentlyContinue -ErrorAction Stop | Out-Null
    Write-Output "Tier updated successfully."
}

# When the alert is resolved the status is Deactivated, so the alerts can be set to their correct new values.
if ($AlertStage -eq "Deactivated")
{
    if ($Up) { Write-Output "Scale-up alert deactivated.  Resetting alert values." }
    else { Write-Output "Scale-down alert deactivated.  Resetting alert values." }

    if ($CurrentSKU -eq $MaxTier -and $CurrentCapacity -eq ($MaxReplicas + 1)) { $NewQPUUpperBound = 99999 }
    else { $NewQPUUpperBound = $($($TierMaxes.GetEnumerator()) | Where-Object { $_.Name -eq $CurrentSKU }).Value * (1 - ($ScaleUpDownOutAtPctDistanceFromTierMax * .01)) }

    if ($CurrentCapacity -gt $MinCapacity) 
    { 
        $NewQPULowerBound = (1 - $ScaleInAtPctDistanceFromTierMax * .01) * $($TierMaxes.GetEnumerator() | Where-Object { $_.Name -eq $CurrentSKU }).Value
    }
    else
    {
        if ($CurrentSKU -eq $MinTier -and $CurrentCapacity -eq $MinCapacity) { $NewQPULowerBound = 0 }
        else 
        { 
            $NewSKU = GetNextLowerQPUTier
            $NewQPULowerBound = $($($TierMaxes.GetEnumerator()) | Where-Object { $_.Name -eq $NewSKU }).Value * (1 - ($ScaleUpDownOutAtPctDistanceFromTierMax * .01)) 
        }
    }
    
    Write-Output ("New upper bound: `t" + $NewQPUUpperBound)
    Write-Output ("New lower bound: `t" + $NewQPULowerBound)

    if ($SeparateProcessingNodeFromQueryReplicas -and $CurrentCapacity -gt 1) { $DimValues = "QueryPool" } else { $DimValues = "*" }
    $Dimensions = New-AzMetricAlertRuleV2DimensionSelection -DimensionName "ServerResourceType" -ValuesToInclude $DimValues -WarningAction silentlyContinue -ErrorAction Stop
    
    $UpperBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Average -Operator GreaterThan -Threshold $NewQPUUpperBound -WarningAction silentlyContinue -ErrorAction Stop
    Add-AzMetricAlertRuleV2 -Condition $UpperBoundCriteria -Name "AutoScaleUpAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleUpActionGroup -WindowSize $Window -Frequency $Window -Severity 3 -WarningAction silentlyContinue -ErrorAction Stop | Out-Null   
       
    $LowerBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Maximum -Operator LessThan -Threshold $NewQPULowerBound -WarningAction silentlyContinue -ErrorAction Stop
    Add-AzMetricAlertRuleV2 -Condition $LowerBoundCriteria -Name "AutoScaleDownAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleDownActionGroup -WindowSize $Window -Frequency $Window -Severity 3 -WarningAction silentlyContinue -ErrorAction Stop | Out-Null

    Write-Output "Alert values updated succesfully."

    # Update alert status synchronization variable finally so new alerts after this will continue processing.
    Set-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Value "Idle" -Encrypted $false | Out-Null
}
'@

# Write the Runbook script to file for importing to the runbook, then publish it
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 5 -Status "Publishing runbook AASAutoScale-$($ASServerName) to Automation Account $($AutomationAccount)..." 
Write-Host  "Publishing runbook " -NoNewline
Write-Host "AASAutoScale-$($ASServerName)" -NoNewline -ForegroundColor Green
Write-Host " to Automation Account $($AutomationAccount)..." -NoNewline
Set-Content -Path $env:temp\AutoScale.ps1 -Value $PSScript  -ErrorAction Stop 
$rb = Import-AzAutomationRunbook -Name "AASAutoScale-$($ASServerName)" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Path $env:temp\AutoScale.ps1 -Type PowerShell -Force -WarningAction silentlyContinue -ErrorAction Stop
Remove-Item -Path $env:temp\AutoScale.ps1
Publish-AzAutomationRunbook -Name "AASAutoScale-$($ASServerName)" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -WarningAction silentlyContinue -ErrorAction Stop | Out-Null
# Also setup an automation variable used to track when we are processing the alert, to prevent other alerts from starting.
Remove-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -ErrorAction SilentlyContinue
New-AzAutomationVariable -Name "AASAutoScaleSynchronizationStatus" -ResourceGroupName $ResourceGroupName -AutomationAccountName $AutomationAccount -Description "Inidicates when an AAS QPU AutoScale alert is being processed, preventing other alerts from scaling until complete." -Value "Idle" -Encrypted $false | Out-Null
Write-Host "  Success!" -ForegroundColor Green

# Create 2 webhooks on the Runbook, one for Up and one for Down
Write-Host  "Creating web hooks for scale up and scale down alerts to runbook..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 40 -Status "Creating web hooks for scale up and scale down alerts to runbook..."
$UpWebhook = New-AzAutomationWebhook -Name "AutoScaleUpWebhook$(Get-Random)" -RunbookName "AASAutoScale-$($ASServerName)" -IsEnabled $true -ExpiryTime "01/01/2029" -Parameters @{"Up"=$true} `
                        -AutomationAccountName $AutomationAccount -ResourceGroup $ResourceGroupName -Force -WarningAction silentlyContinue -ErrorAction Stop
$DownWebhook = New-AzAutomationWebhook -Name "AutoScaleDownWebhook$(Get-Random)" -RunbookName "AASAutoScale-$($ASServerName)" -IsEnabled $true -ExpiryTime "01/01/2029" -Parameters @{"Up"=$false} `
                        -AutomationAccountName $AutomationAccount -ResourceGroup $ResourceGroupName -Force -WarningAction silentlyContinue -ErrorAction Stop
Write-Host "  Success!" -ForegroundColor Green

# Create action receivers for both web hooks                       
Write-Host  "Creating action receivers to the webhooks..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 45 -Status "Creating action receivers to the webhooks..."
$UpRuleActionReceiver = New-AzActionGroupReceiver -Name "UpRuleActionReceiver" -WebhookReceiver -ServiceUri $UpWebhook.WebhookURI -WarningAction silentlyContinue -ErrorAction Stop
$DownRuleActionReceiver = New-AzActionGroupReceiver -Name "DownRuleActionReceiver" -WebhookReceiver -ServiceUri $DownWebhook.WebhookURI -WarningAction silentlyContinue -ErrorAction Stop
Write-Host "  Success!" -ForegroundColor Green

# Create action groups for each web hook
Write-Host "Creating action groups " -NoNewline
Write-Host "AutoScaleUpActionGroup" -NoNewline -ForegroundColor Green 
Write-Host " and " -NoNewline
Write-Host "AutoScaleDownActionGroup" -NoNewline -ForegroundColor Green 
Write-Host " for AS server $($ASServerName)..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 60 -Status "Creating action groups AutoScaleUpActionGroup and AutoScaleDownActionGroup for AS server $($ASServerName)..."
$AutoScaleUpActionGroup = Set-AzActionGroup -ResourceGroupName $ResourceGroupName -Name "AutoScaleUpActionGroup" -ShortName "AutoUp" -Receiver $UpRuleActionReceiver -WarningAction silentlyContinue -ErrorAction Stop
$AutoScaleDownActionGroup = Set-AzActionGroup -ResourceGroupName $ResourceGroupName -Name "AutoScaleDownActionGroup" -ShortName "AutoDown" -Receiver $DownRuleActionReceiver -WarningAction silentlyContinue -ErrorAction Stop
Write-Host "  Success!" -ForegroundColor Green

Write-Host  "Getting current server configuration and details required to create alerts..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 70 -Status "Getting current server configuration and details required to create alerts..."
# Obtain the action group from Id as necessary (the above ActionGroup returned when they are created is not of the correct type and we have to obtain after creation this way.
$AutoScaleUpActionGroup = New-AzActionGroup -ActionGroupId $AutoScaleUpActionGroup.Id -WarningAction silentlyContinue -ErrorAction Stop
$AutoScaleDownActionGroup = New-AzActionGroup -ActionGroupId $AutoScaleDownActionGroup.Id -WarningAction silentlyContinue -ErrorAction Stop

# Specify the initial conditions for the alerts
if ($SeparateProcessingNodeFromQueryReplicas -and $CurrentCapacity -gt 1) { $DimValues = "QueryPool" } else { $DimValues = "*" }
$Dimensions = New-AzMetricAlertRuleV2DimensionSelection -DimensionName "ServerResourceType" -ValuesToInclude $DimValues -WarningAction silentlyContinue -ErrorAction Stop
$TargetResourceId = "/subscriptions/" + $SubscriptionId + "/resourceGroups/" + $ResourceGroupName + "/providers/Microsoft.AnalysisServices/servers/" + $ASServerName

if ($CurrentSKU -eq $MaxTier -and $CurrentCapacity -eq ($MaxReplicas + 1)) { $NewQPUUpperBound = 99999 }
else { $NewQPUUpperBound = $($($TierMaxes.GetEnumerator()) | Where-Object { $_.Name -eq $CurrentSKU }).Value * (1 - ($ScaleUpDownOutAtPctDistanceFromTierMax * .01)) }

if ($CurrentCapacity -gt $MinCapacity) 
{ 
    $NewQPULowerBound = (1 - $ScaleInAtPctDistanceFromTierMax * .01) * $($TierMaxes.GetEnumerator() | Where-Object { $_.Name -eq $CurrentSKU }).Value
}
else
{
    if ($CurrentSKU -eq $MinTier -and $CurrentCapacity -eq $MinCapacity) { $NewQPULowerBound = 0 }
    else 
    {
        $HighestLowerTierQPUAvailable = 0
        for ($i = $($TierMaxes.Keys).IndexOf($CurrentSKU) - 1; $i -ge 0; $i--)
        {
            if ($HighestLowerTierQPUAvailable -lt $TierMaxes[$i]) { $HighestLowerTierQPUAvailable = $TierMaxes[$i] }
        }
        $NextLowerSKU = ($TierMaxes.GetEnumerator() | Where-Object {$_.Value -eq $HighestLowerTierQPUAvailable}).Key 
        $NewQPULowerBound = $($($TierMaxes.GetEnumerator()) | Where-Object { $_.Name -eq $NextLowerSKU }).Value * (1 - ($ScaleUpDownOutAtPctDistanceFromTierMax * .01)) 
    }
}
Write-Host "  Success!" -ForegroundColor Green

# Create the up/down alert criteria 
Write-Host  "Creating alert criteria..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 80 -Status "Creating alert criteria..."
$UpperBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Average -Operator GreaterThan -Threshold $NewQPUUpperBound -WarningAction silentlyContinue -ErrorAction Stop
$LowerBoundCriteria = New-AzMetricAlertRuleV2Criteria -MetricName "qpu_metric" -DimensionSelection $Dimensions -TimeAggregation Maximum -Operator LessThan -Threshold $NewQPULowerBound -WarningAction silentlyContinue -ErrorAction Stop
Write-Host "  Success!" -ForegroundColor Green

$Window = New-TimeSpan -Minutes $AlertWindowInMins

# And finally add the rules
Write-Host "Adding rules " -NoNewline
Write-Host "AutoScaleUpAlert" -NoNewline -ForegroundColor Green
Write-Host " and " -NoNewline
Write-Host "AutoScaleDownAlert" -NoNewline -ForegroundColor Green
Write-Host " for AS server $($ASServerName)..." -NoNewline
Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 95 -Status "Adding rules AutoScaleUpAlert and AutoScaleDownAlert for AS server $($ASServerName)..."
Add-AzMetricAlertRuleV2 -Condition $UpperBoundCriteria -Name "AutoScaleUpAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleUpActionGroup -WindowSize $Window -Frequency $Window -Severity 3 -WarningAction silentlyContinue -ErrorAction Stop `
                                -Description "Metric alert rule to automatically scale up Azure AS when average QPU exceeeds threshold for the configured time window." `
                                | Out-Null 
Add-AzMetricAlertRuleV2 -Condition $LowerBoundCriteria -Name "AutoScaleDownAlert" -ResourceGroupName $ResourceGroupName -TargetResourceId  $TargetResourceId `
                                -ActionGroup $AutoScaleDownActionGroup -WindowSize $Window -Frequency $Window -Severity 3 -WarningAction silentlyContinue -ErrorAction Stop `
                                -Description "Metric alert rule to automatically scale down Azure AS when max QPU is below threshold for the configured time window." `
                                | Out-Null 
Write-Host "  Success!`n" -ForegroundColor Green

Write-Host  ("`nAutoScale configured and active for server " + $ASServerName + "!") -ForegroundColor Green

if ($CurrentCapacity -gt $MinCapacity)
{
    if ($CurrentCapacity -lt ($MaxReplicas + 1)) { $NextUp = "Server will add a replica when average QPU exceeds " + $NewQPUUpperBound + " over " + $AlertWindowInMins + " minute(s)." }
    else { $NextUp = "Server will not scale up or out further from this tier/capacity." }
    $NextDown = "Server will remove a replica when maximum QPU does not exceed " + $NewQPULowerBound + " over " + $AlertWindowInMins + " minute(s)."
}
else
{
    if ($CurrentSKU -gt $MinTier)
    {
        if ($CurrentSKU -eq $MaxTier) 
        { 
            if ($CurrentCapacity -lt ($MaxReplicas + 1)) { $NextUp = "Server will add a replica when average QPU exceeds " + $NewQPUUpperBound + " over " + $AlertWindowInMins + " minute(s)." }
            else { $NextUp = "Server will not scale up or out further from this tier/capacity." }
        }
        else { $NextUp = "Server will scale up when average QPU exceeds " + $NewQPUUpperBound + " over " + $AlertWindowInMins + " minute(s)." }
        $NextDown = "Server will scale down SKU when maximum QPU does not exceed " + $NewQPULowerBound + " over " + $AlertWindowInMins + " minutes(s)."
    }
    else 
    { 
        $NextDown = "Server will not scale in or down further from this tier/capacity."
        if ($CurrentSKU -ne $MaxTier) { $NextUp = "Server will scale up SKU when average QPU exceeds " + $NewQPUUpperBound + " over " + $AlertWindowInMins + " minute(s)." }
        elseif ($CurrentCapacity -lt ($MaxReplicas + 1)) { $NextUp = "Server will add a replica when average QPU exceeds " + $NewQPUUpperBound + " over " + $AlertWindowInMins + " minute(s)." }
        else { $NextUp = "Server will not scale up or out further from this tier/capacity." }
    }
}

[ordered]@{
    "Current SKU:" = $CurrentSKU
    "Replica Count:" = $CurrentCapacity - 1
    "Next up action:" = $NextUp
    "Next down action:" = $NextDown
} | Format-Table -HideTableHeaders -AutoSize

Write-Progress -Activity "Installing Azure Analysis Services QPU AutoScale" -PercentComplete 100 -Completed