--SC-30/1/ss-2021-00000032

CREATE proc [dbo].[usp_DuplicateQuoteByQuoteId] --'1038','',1221,1618,'aravind.chakragiri@tke.dev','US'
(
	@DestinationProjectId nvarchar(150),
	@SourceQuoteId nvarchar(150),
	@VersionId int,
	@ParentVersionId int=0,
	@userName nvarchar(150),
	@VariableMapperDataTable AS VariableMapper READONLY,
	@Country nvarchar(150)
)
as
begin
	BEGIN TRY

		declare @BuildingList as BuildingIDList
		DECLARE @QuoteId nvarchar(100)

		if(@SourceQuoteId = '')
		begin			
			set @SourceQuoteId = (select distinct QuoteId from Quotes where VersionId = @ParentVersionId and OpportunityId = @DestinationProjectId)			
			if exists (select * from Quotes where VersionId = @VersionId)
			begin
				
				delete from Quotes where VersionId = @VersionId and OpportunityId = @DestinationProjectId
			end
		end
		
		if exists(select UPPER(SUBSTRING(County,1,2)) from AccountDetails where opportunityid= @DestinationProjectId)
		begin
			set @Country = (select UPPER(SUBSTRING(County,1,2)) from AccountDetails where opportunityid= @DestinationProjectId)		
		end
		else
		begin
			set @Country = (select UPPER(SUBSTRING(@Country,1,2)))
		end
		

		insert into @BuildingList select Id from Building where QuoteId=@SourceQuoteId
		insert into Quotes(OpportunityId,VersionId,QuoteStatusId,Description,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @DestinationProjectId, case when @VersionId <> 0 then  @VersionId else (select count(*) from Quotes where OpportunityId=@DestinationProjectId)+1 end ,QuoteStatusId,Description,@userName,GETDATE(),@userName,GETDATE() from Quotes where QuoteId = @SourceQuoteId
		
		--update Quotes set QuoteId=@Country+'-'+cast(year(GETDATE()) as nvarchar(4))+'-'
		--			+(
		--			case when LEN(SCOPE_IDENTITY()) = 1 then '0000000'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 2 then '000000'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 3 then '00000'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 4 then '0000'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 5 then '000'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 6 then '00'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 7 then '0'+CAST(SCOPE_IDENTITY() as varchar(6))
		--			when LEN(SCOPE_IDENTITY()) = 8 then +CAST(SCOPE_IDENTITY() as varchar(6))
		--			end) where id=SCOPE_IDENTITY()

		EXEC usp_GenerateQuoteId @country, @QuoteId = @QuoteId OUTPUT 
		update Quotes set QuoteId=@QuoteId where id=SCOPE_IDENTITY()

		declare @DestinationQuoteId nvarchar(150)
		set @DestinationQuoteId=(select QuoteId from Quotes where Id=SCOPE_IDENTITY())
		--set @Result = 1
		select OpportunityId, QuoteId, VersionId from Quotes
		where QuoteId = @DestinationQuoteId
				
		exec [dbo].[usp_DuplicateBuildingByBuildingid] @BuildingList,@DestinationQuoteId,@VariableMapperDataTable,0
		
		--update Building
		--			set WorkflowStatus = case when (select Step from Status where StatusKey = workflowstatus )>(select Step from Status where StatusKey = 'BLDG_COM') 
		--			then 'BLDG_' else workflowstatus end
		--			where Id = @Result	


	END TRY
	BEGIN CATCH 
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end

--select * from Quotes where VersionId = 1618
--select * from Building where QuoteId = 'US-2021-70000004'
--delete from Quotes 
--where VersionId = 1225 and OpportunityId= '1038'