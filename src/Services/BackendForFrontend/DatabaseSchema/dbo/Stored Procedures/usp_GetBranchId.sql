CREATE PROCEDURE [dbo].[usp_GetBranchId] --'ALBERQUE'

@branchName nvarchar(50)

AS


Begin
	BEGIN TRY
		Select BranchNumber from Branch where Branch=@branchName
		if(@@ROWCOUNT=0)
		begin
			RAISERROR('Invalid BranchName',16,1)
		END
	END TRY
	BEGIN CATCH
  --             );  
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end   
