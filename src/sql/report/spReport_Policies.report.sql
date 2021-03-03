IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spReport_Policies]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spReport_Policies
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].spReport_Policies 
	@policy nvarchar(50)=null,
	@role nvarchar(50)=null,
	@permission nvarchar(100)=null
AS
BEGIN
	SET NOCOUNT ON;

	select pol.Id PolicyId, pol.Name PolicyName, r.Name Role, p.Name Permission
	from PolicyServer.dbo.Policies pol
	left join PolicyServer.dbo.Roles r on r.PolicyId=pol.Id
	left join PolicyServer.dbo.PermissionAssignments pa on pa.RoleId = r.Id
	left join PolicyServer.dbo.Permissions p on pa.PermissionId=p.Id
	where (pol.Name=@policy or @policy is null)
		and (r.Name=@role or @role is null)
		and (p.Name=@permission or @permission is null)
	order by pol.Name, r.Name, p.Name

END


GO
