declare @reportGroupId int
declare @reportId int
declare @stateQueryId int
declare @name nvarchar(50)

set @name = 'spReport_Contractors'
exec @stateQueryId = spAddReportArgumentQuery 'select distinct a.St ID, a.St State from eboa.dbo.Contractor c join eboa.dbo.Address a on c.MailingAddressId=a.AddressID'

set @reportGroupId = 1
exec @reportId = spAddReport @name, 'List Contractors', @reportGroupId
	exec spAddReportArgument @reportId, 'Start Date', '@startdate', 'DateTime', null, 1
	exec spAddReportArgument @reportId, 'End Date', '@enddate', 'DateTime', null, 2
	exec spAddReportArgument @reportId, 'State', '@state', 'varchar(2)', @stateQueryId, 3

declare @policyQueryId int
declare @roleQueryId int
declare @permissionQueryId int

set @name = 'spReport_Policies'
exec @policyQueryId = spAddReportArgumentQuery 'select distinct NormalizedName ID, Name from policyserver.dbo.Policies'
exec @roleQueryId = spAddReportArgumentQuery 'select distinct NormalizedName ID, Name from policyserver.dbo.Roles'
exec @permissionQueryId = spAddReportArgumentQuery 'select distinct NormalizedName ID, Name from policyserver.dbo.Permissions'

set @reportGroupId = 1
exec @reportId = spAddReport @name, 'List Policies', @reportGroupId
	exec spAddReportArgument @reportId, 'Policy', '@policy', 'nvarchar(50)', @policyQueryId, 1
	exec spAddReportArgument @reportId, 'Role', '@role', 'nvarchar(50)', @roleQueryId, 2
	exec spAddReportArgument @reportId, 'Permission', '@permission', 'nvarchar(50)', @permissionQueryId, 3
