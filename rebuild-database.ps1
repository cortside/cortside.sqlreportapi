<#
.SYNOPSIS
    rebuild database by deleting and running update
#>
[cmdletBinding()]
Param()

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

# this script is not safe to run on any machine other than a local dev machine, because of db deletion
$serverInstance = "localhost"
$triggergenDbName = "SqlReportApi"

Write-Output @("
##########
# 1. Refresh database named $triggergenDbName so that all migrations are applied for pristine state
# 2. Generate new trigger sql scripts
# 3. Convert encoding on updated scripts to UTF-8
##########
")
start-sleep -Seconds 3

if ($PSScriptRoot.Contains(' ')) { 
    throw "Your working directory has a space in the path, which is not supported.  Wise up and move to C:\work\Cortside.Template.api! And have a wonderful work day!"
    exit
}

try {
    Import-Module SqlServer -ErrorAction Stop
}
catch {
    Write-Output "Installing SqlServer module for powershell"
    Install-PackageProvider -Name NuGet -Force
    Install-Module -Name SqlServer -AllowClobber -Force
    Import-Module SqlServer
}

try {
    Write-Output "Verifying SqlServer accessible at $serverInstance"
    invoke-sqlcmd -ServerInstance $serverInstance -Query "select 'invoke-sqlcmd successful' AS SqlServerStatus" -QueryTimeout 5 -ConnectionTimeout 5 -ErrorAction Stop
}
catch {
    throw "Problem connecting to SqlServer at $serverInstance. Please confirm up and running and try again."
}



$deleteLocalDbQuery = @"
use [master]
go
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'$triggergenDbName')
Begin
alter database [$triggergenDbName] set single_user with rollback immediate;
DROP DATABASE [$triggergenDbName];
End
"@

Write-Host "deleting $triggergenDbName on $serverInstance, if it exists"
invoke-sqlcmd -ServerInstance $serverInstance -Query $deleteLocalDbQuery -ErrorAction Stop

Write-Host "creating $triggergenDbName and applying all migrations"
& "$PSScriptRoot/update-database.ps1" -CreateDatabase -database $triggergenDbName

