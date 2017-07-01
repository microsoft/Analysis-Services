Function script:deletedb($databasename)
{
    $dbstring=$server.Databases |select-object name| select-string -simplematch $databasename

    if ($dbstring)
    {
        $db=$server.databases.item($databasename)
        $db.drop()
        Write-host "Deleted " $databasename
    }
    else
    {
        Write-host "Database " $databasename " DOES NOT exist"
    }
}

[Reflection.Assembly]::LoadWithPartialName("Microsoft.AnalysisServices") >$NULL

$server = New-Object Microsoft.AnalysisServices.Server
$server.connect("localhost") 

deletedb "Test1103_Source"
deletedb "Test1103_Target"
deletedb "Test1200_Source"
deletedb "Test1200_Target"

if (!($args.Length -eq 1 -and $args[0] -eq "-DeleteOnly"))
{
    $scriptcontent = Get-Content .\Test1103_Source.xmla
    $server.Execute($scriptcontent)
    Write-host "Created Test1103_Source"
    $scriptcontent = Get-Content .\Test1103_Target.xmla
    $server.Execute($scriptcontent)
    Write-host "Created Test1103_Target"
    $scriptcontent = Get-Content .\Test1200_Source.xmla
    $server.Execute($scriptcontent)
    Write-host "Created Test1200_Source"
    $scriptcontent = Get-Content .\Test1200_Target.xmla
    $server.Execute($scriptcontent)
    Write-host "Created Test1200_Target"
}

# Clean up  
$server.Disconnect() 
Write-host ""
Write-host "Disconnected"
