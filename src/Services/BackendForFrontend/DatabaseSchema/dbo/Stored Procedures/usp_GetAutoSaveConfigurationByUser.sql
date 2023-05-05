CREATE Procedure [dbo].[usp_GetAutoSaveConfigurationByUser] 
@userName nvarchar(250)
as
Begin
	BEGIN TRY
	   Select [RequestMessage],[CreatedBy],[CreatedOn]
			   from [dbo].[AutoSaveConfiguration]
				   where CreatedBy = @userName and isDeleted=0
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
	   
END 
