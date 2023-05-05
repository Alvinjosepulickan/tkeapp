CREATE Procedure [dbo].[usp_GetGroupStatusForFDA]
@groupId int
As
Begin
      Select UPPER(WorkflowStatus) as StatusKey from GroupConfiguration
	   where GroupId=@groupId and IsDeleted=0
End