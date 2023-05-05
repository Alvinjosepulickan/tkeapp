CREATE Procedure [dbo].[usp_UpdateFDADrawingMethodByGroupId]
 @drawingMethod int
,@groupId int
as
Begin
	BEGIN TRY
	 declare @fieldDrawingId int
	 declare @statusId nvarchar(200)

	   SET @fieldDrawingId = (Select top 1 Id from FieldDrawingMaster 
		  where GroupId=@groupId 
			order by CreatedOn desc)
 

 
		IF(@drawingMethod = 1)
		Begin
		  SET @statusId = 'DWG_NA'
		End
		Else
		Begin
		 SET @statusId = 'DWG_REQD'
		End



	   Update FieldDrawingMaster
		 set DrawingMethod=@drawingMethod 
			,StatusKey= @statusId
		   Where GroupId=@groupId and Id=@fieldDrawingId
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