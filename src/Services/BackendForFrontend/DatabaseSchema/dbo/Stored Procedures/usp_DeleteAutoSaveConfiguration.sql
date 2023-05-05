CREATE PROCEDURE [dbo].[usp_DeleteAutoSaveConfiguration] 
	 
	@userName nvarchar(250),
	@Result int OUTPUT
	AS 
BEGIN 
	BEGIN TRY
		IF EXISTS(
			SELECT * 
				FROM AutoSaveConfiguration 
					WHERE createdBy=@userName and isDeleted=0)
		BEGIN
			UPDATE AutoSaveConfiguration 
				SET isDeleted=1 
					WHERE createdBy=@userName
			SET @Result=1
		END
		ELSE
		BEGIN
			SET @Result=0
		END
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
