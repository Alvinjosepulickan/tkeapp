
CREATE PROCEDURE [dbo].[usp_DeleteProjectById] --'SC-8','','Nagaraja_Manjappa',0
@projectId nvarchar(50),
@VersionId nvarchar(50) 
,@userName varchar(50)
,@Result int output

AS
BEGIN
	BEGIN TRY
		declare @deleteQuoteId nvarchar(20)
		declare @buildingId int
		declare @groupConfigurationId int
		declare @unitsID int

		if isnull(@projectId,'') <> '' and isnull(@VersionId,'') <> ''  -----delete by version
		begin

		--set nocount on;
			IF EXISTS (SELECT * from Projects where OpportunityId = @projectId and isDeleted=0) 
			BEGIn 
	
				--update Projects set modifiedOn=getdate(),isDeleted=1,modifiedBy=@userName where OpportunityId = @projectId;

				IF EXISTS (SELECT * from Quotes where OpportunityId = @projectId and VersionId = @VersionId) 
					BEGIN
						update quotes set modifiedOn=getdate(),isDeleted=1,modifiedBy=@userName where OpportunityId = @projectId and VersionId = @VersionId
					END

				--IF EXISTS (SELECT * from AccountDetails where opportunityid = @projectId) 
				--	BEGIN
				--		update AccountDetails set isDeleted=1 where OpportunityId = @projectId and isDeleted=0
				--	END

				select @deleteQuoteId = QuoteId from Quotes where OpportunityId = @projectId and VersionId = @VersionId
		
				update Building set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where QuoteId = @deleteQuoteId
				update BuildingConfiguration set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where BuildingId in (select id from Building where QuoteId = @deleteQuoteId)
				update GroupConfiguration set ModifedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where GroupId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId))
				update GroupConfigurationDetails set ModifedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where GroupId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId))
				--update Units set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where UnitId in (select UnitId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId)))

				--select opportunityId as 'projectId', VersionId as 'versionId', QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId and VersionId = @VersionId
		
				--select opportunityId as 'projectId'--, VersionId as 'versionId', QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId

				--select VersionId as 'versionId'--, QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId

				--select QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId

				set @Result= 1;
				--select @Result
				End
				ELSE
					BEGIN
					set @Result= 0;
					--select 100
					END
				End
		Else if isnull(@projectId,'') <> '' and isnull(@VersionId,'') = ''   ------delete by project
				Begin

					IF EXISTS (SELECT * from Projects where OpportunityId = @projectId and isDeleted=0) 
						BEGIn 
				
					update Projects set modifiedOn=getdate(),isDeleted=1,modifiedBy=@userName where OpportunityId = @projectId;
			
					IF EXISTS (SELECT * from Quotes where OpportunityId = @projectId) 
						BEGIN
							update quotes set modifiedOn=getdate(),isDeleted=1,modifiedBy=@userName where OpportunityId = @projectId
						END
			
					IF EXISTS (SELECT * from AccountDetails where opportunityid = @projectId) 
						BEGIN
							update AccountDetails set isDeleted=1 where OpportunityId = @projectId and isDeleted=0
						END			

				select @deleteQuoteId = QuoteId from Quotes where OpportunityId = @projectId and VersionId = @VersionId

				update Building set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where QuoteId = @deleteQuoteId
				update BuildingConfiguration set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where BuildingId in (select id from Building where QuoteId = @deleteQuoteId)
				update GroupConfiguration set ModifedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where GroupId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId))
				update GroupConfigurationDetails set ModifedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where GroupId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId))
				--update Units set ModifiedBy = @userName, ModifiedOn = getdate(), IsDeleted = 1 where UnitId in (select UnitId from Units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId in (select id from Building where QuoteId = @deleteQuoteId)))

				--select opportunityId as 'projectId'--, VersionId as 'versionId', QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId

				--select VersionId as 'versionId'--, QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId

				--select QuoteId as 'quoteId'
				--from Quotes where OpportunityId = @projectId
				set @Result= 1;
				--select 100
				End
				ELSE
					BEGIN
					set @Result= 0;
					--select 200
					END

			END
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@projectId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH

END



