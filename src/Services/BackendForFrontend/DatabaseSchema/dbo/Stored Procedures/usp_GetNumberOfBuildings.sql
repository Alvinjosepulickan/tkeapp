
CREATE PROCEDURE [dbo].[usp_GetNumberOfBuildings]

       -- Add the parameters for the stored procedure here

@projectid nvarchar(200),
@QuoteId nvarchar(25)
--@OpportunityId nvarchar(25),--@QuoteId nvarchar(25),
--@VersionId nvarchar(25)
AS
Begin
	BEGIN TRY
		--declare @QuoteId nvarchar(20)
		--Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)

		declare @BuildingNameCheck int
		declare @BuildingCount int
		declare @BuildingName nvarchar(10)
		set @BuildingCount=(SELECT count(*)  from [dbo].[Building]  where QuoteId= @QuoteId)
		set @BuildingNameCheck=1
		while(@BuildingNameCheck=1)
		begin
			set @BuildingNameCheck =0
			set @BuildingCount+=1
			set @BuildingName='B'+CAST(@BuildingCount as nvarchar(3))
					if(exists (select * from Building where BldName=@BuildingName and QuoteId=@QuoteId))
					begin
						set @BuildingNameCheck=1
					end
					if(@BuildingNameCheck=0)
					begin
						select @BuildingCount-1 as BuildingCount
						return 0
					end 
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
