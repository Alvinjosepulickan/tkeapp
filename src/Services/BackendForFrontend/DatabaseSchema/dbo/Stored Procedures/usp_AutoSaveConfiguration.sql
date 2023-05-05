CREATE PROCEDURE [dbo].[usp_AutoSaveConfiguration] 
	 
    @requestMessage nvarchar(max),
	@userId nvarchar(250),
	@Result int OUTPUT
	AS 
BEGIN 
	BEGIN TRY 
	IF EXISTS(SELECT * FROM AutoSaveConfiguration WHERE createdBy=@userId and isDeleted=0)
	BEGIN
		UPDATE AutoSaveConfiguration SET requestMessage=@requestMessage,modifiedBy=@userId,modifiedOn=getDate() WHERE createdBy=@userId 
	END
	ELSE
	BEGIN
		INSERT INTO AutoSaveConfiguration (requestMessage,createdOn,createdBy) VALUES (@requestMessage,getDate(),@userId)
	END
	SET @Result=1
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
END
 
