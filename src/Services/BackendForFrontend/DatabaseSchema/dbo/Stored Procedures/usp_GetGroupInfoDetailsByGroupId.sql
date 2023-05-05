CREATE Procedure [dbo].[usp_GetGroupInfoDetailsByGroupId]
@groupId int
as
Begin
   Select GroupConfigurationType as VariableId
          ,GroupConfigurationValue as [Value] 
		        from GroupConfigurationDetails
     Where GroupId=@groupId
End