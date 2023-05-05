
CREATE PROCEDURE [dbo].[usp_GetRequestQueueDetails]  
@groupId int
As
Begin
 BEGIN TRY
	   Select 
	          fdm.id 
			 ,fds.StatusName as statusName
			 ,fds.StatusKey as statusKey
			 ,(case when bg.id is null then 0 else bg.id end) as buildingId
	         ,bg.BldName as buildingName
			  ,(case when gc.GroupId is null then 0 else gc.GroupId end) as groupId
			 ,gc.GroupName as groupName
			 ,(case when ut.unitId is null then 0 else ut.unitId end) as unitId
			 ,ut.Name as unitName
			 ,fdm.CreatedOn as lastModified 
			 ,0 as [version]
			 ,fdm.CreatedBy as modifiedBy
			 ,fda.FDAType
			  from FieldDrawingMaster fdm  
			  Left Join Status fds on fdm.StatusKey=fds.StatusKey
			   Left Join FieldDrawingAutomation fda on fdm.Id=fda.FieldDrawingId
			 Left Join	GroupConfiguration gc on fdm.GroupId=gc.GroupId
			 Left Join Building bg on bg.Id = gc.BuildingId
			 Left Join units ut on gc.GroupId=ut.GroupConfigurationId
			    Where gc.GroupId= @groupId and fda.FDAValue <> 0
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
End
