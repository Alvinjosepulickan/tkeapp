CREATE Procedure [dbo].[usp_GetCarPositionByGroupId]
@groupId int
as
Begin

	DECLARE @carPositionTable TABLE
		(
		VariableId NVARCHAR(50),
		[value] NVARCHAR(30)
		)

	INSERT @carPositionTable
	EXEC [dbo].[usp_GetCarPositionFromJson] @groupId

	Select * from @carPositionTable
					 
End