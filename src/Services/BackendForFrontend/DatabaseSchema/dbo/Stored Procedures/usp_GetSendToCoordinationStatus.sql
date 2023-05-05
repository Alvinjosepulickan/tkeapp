CREATE Procedure [dbo].[usp_GetSendToCoordinationStatus]--'US-2021-50000132'
@quoteId nvarchar(100)
AS
Begin
	declare @projectStatus nvarchar(200)
	declare @isPrimaryQuote int,@countLockedGroup int,@noOfGroup int
	--primary quote check
	SET @isPrimaryQuote = (case when exists(select TOP 1 PrimaryQuoteId from PrimaryQuotes where primaryQuoteId = @quoteId) then 1 else 0 end)
	SET @projectStatus= (select distinct workflowstage from Projects where opportunityid =
						(select distinct OpportunityId from Quotes where quoteid=@quoteId))
	SET @noOfGroup= (select Count(*) from groupconfiguration 
	where buildingId in (select id from building where quoteId =@quoteId and IsDeleted = 0) 
	and IsDeleted=0)
	--no: of unlocked group
	select @countLockedGroup= Count(*) from groupconfiguration 
	where buildingId in (select id from building where quoteId =@quoteId and IsDeleted = 0) 
	and IsDeleted=0 and workflowstatus<>'GRP_LOC'

	if(@countLockedGroup=0 and @projectStatus='PRJ_BDAWD' and @noOfGroup<>0)
	begin
		if(@isPrimaryQuote=1)
		begin
			select StatusId,StatusKey,'' Description,DisplayName 
			from status
			where statuskey='STC_ENB'
		end
		else
		begin
			select StatusId,StatusKey, Description,DisplayName 
			from status
			where statuskey='STC_DSB'
		end	
	end
	else
	begin
		select StatusId,StatusKey,'' Description,DisplayName 
			from status
			where statuskey='STC_DSB'
	end
	 
End