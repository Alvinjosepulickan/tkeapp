CREATE Procedure [dbo].[usp_UpdateFDARequestStatus]
@fieldDrawingId int
as
Begin
	BEGIN TRY
	  Update FieldDrawingMaster
		  set StatusKey = 'DWG_INI'
			Where Id=@fieldDrawingId
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