CREATE Procedure [dbo].[usp_GetOutputTypesForXMLGeneration]
@groupId int
as 
Begin
	BEGIN TRY
	declare @fieldDrawingId int;
	  SET @fieldDrawingId = (Select top 1 Id from FieldDrawingMaster 
		Where GroupId=@groupId
		   order by CreatedOn desc)

		   Select * from FieldDrawingAutomation
			  where FDAValue=1 and FieldDrawingId=@fieldDrawingId
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