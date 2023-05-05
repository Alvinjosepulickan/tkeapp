CREATE PROCEDURE [dbo].[usp_InsertUnitDetails]
@GroupConfigurationId int 

as

BEGIN
	BEGIN TRY
		DECLARE @Start int
		set @start=1
		while @Start<= 8
		begin
			insert into TempUnitTable (GroupConfigurationId,unitName) values(@GroupConfigurationId,'U'+cast(@Start as nvarchar(max)) )
			set @Start=@Start+1
		end
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
