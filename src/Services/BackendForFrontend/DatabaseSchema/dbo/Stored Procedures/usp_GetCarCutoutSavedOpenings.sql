




CREATE Procedure [dbo].[usp_GetCarCutoutSavedOpenings] 
-- Add the parameters for the stored procedure here
 @setId nvarchar(500)
as
Begin
    BEGIN TRY
		DECLARE @count int

		--Getting openings assigned for CarcallCutout Keyswitch--

		  SET @count=(SELECT COUNT(*) FROM CarCallCutoutLocations
		  WHERE SetId=@setId)

		IF(@count >=1)
		Begin	 
	
		   SELECT (SELECT COUNT(*)  FROM CarCallCutoutLocations  where Front = 1  and SetId = @setId) + (SELECT COUNT(*)  FROM CarCallCutoutLocations  where Rear = 1  and SetId = @setId) AS Quantity

		End
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



 

