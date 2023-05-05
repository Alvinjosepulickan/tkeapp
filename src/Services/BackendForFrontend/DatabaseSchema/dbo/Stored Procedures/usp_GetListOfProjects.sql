-- =============================================
-- Author: Eakhanath Singh V
-- =============================================
CREATE Procedure [dbo].[usp_GetListOfProjects] --'naseer.ahmed@tke.dev','CSC_CE','Canada'
@UserId [nvarchar](200),
@userRole [nvarchar](200),
@Country [nvarchar](200)
AS
BEGIN
	BEGIN TRY
	
		declare @UserValue nvarchar(200)
		set @UserValue= @UserId
		declare @countryFlag int
		SET @Country= trim(@Country)
		if(@Country ='canada')
		begin
		set @countryFlag = 1
		end
		else
		begin
		set @countryFlag = 2
		end
		--declare @userCountryValue nvarchar(20)
		--set @userCountryValue =(select * from Branch where Region)
		select	Projects.OpportunityId,
				Projects.Name, 
				Branch.Branch as BranchName,
				Projects.Salesman,
				projstatus.StatusId as SalesId,
				projstatus.StatusKey as StatusKey,
				projstatus.StatusName as ProjStatusName,
				projstatus.DisplayName as ProjStatusDisplayName,
				projstatus.Description  ProjStatusDescription,
				Projects.CreatedBy,
				Projects.CreatedOn,
				Projects.ModifiedBy,
				Projects.ModifiedOn,
				Projects.ProjectSource,
		Quotes.QuoteId,quotes.VersionId,Quotes.Description  --, --Status.StatusName, Projects.ProjectSource
		from Projects (nolock)
		left join Branch (nolock) on Projects.BranchNumber = Branch.BranchNumber
		left join Quotes (nolock) on Quotes.OpportunityId = Projects.OpportunityId
		left join Status projstatus (nolock) on Projects.workFlowStage = projstatus.StatusKey
		--inner join Status (nolock) on Quotes.QuoteStatusId = Status.StatusKey
		where Projects.ProjectSource = @countryFlag and Projects.CreatedBy = @UserValue and Projects.IsDeleted = 0


		-- get Quote Details
		select distinct Projects.OpportunityId,
			Quotes.QuoteId,
			quotes.VersionId,
			Quotes.Description, 
			Status.StatusId,
			Quotes.CreatedBy,
			Quotes.CreatedOn,
			Quotes.ModifiedBy,
			Quotes.ModifiedOn,
			Status.StatusKey as QuoteStatusKey,
			Status.StatusName as QuoteStatusName,
			Status.DisplayName as QuoteStatusDisplayName,
			Status.Description  QuoteStatusDescription,
			PrimaryQuotes.PrimaryQuoteId as PrimaryQuoteId
			from Quotes (nolock)
			Inner join Projects (nolock) on Quotes.OpportunityId = Projects.OpportunityId
			inner join Status (nolock) on Quotes.QuoteStatusId = Status.StatusKey
			left join PrimaryQuotes on Quotes.QuoteId = PrimaryQuotes.PrimaryQuoteId 
			where Quotes.IsDeleted = 0 and Quotes.CreatedBy = @UserValue
			order by QuoteId


		-- getting count of quotes in all projects
		select Quotes.OpportunityId, count(Quotes.QuoteId) as QuoteCounts
		from Quotes(nolock)
		where Quotes.IsDeleted = 0
		Group by Quotes.opportunityId
		-- getting count of units in all Quotes
		select Quotes.QuoteId,count(Units.UnitId) as UnitsCountInQuotes
		From Quotes (nolock)
		Inner join Building (nolock) on Building.QuoteId = Quotes.QuoteId
		Inner join GroupConfiguration (nolock) on GroupConfiguration.BuildingId = Building.Id
		Inner Join Units (nolock) on Units.GroupConfigurationId = GroupConfiguration.GroupId
		where Quotes.IsDeleted = 0 and Building.IsDeleted = 0 and GroupConfiguration.IsDeleted =0
		and Units.IsDeleted = 0
		Group by Quotes.QuoteId

		-- getting total price for all Quotes
		select Quotes.QuoteId,sum(cast(VariableValue as float)) as PriceForQuote
		From Quotes (nolock)
		Inner join Building (nolock) on Building.QuoteId = Quotes.QuoteId
		Inner join GroupConfiguration (nolock) on GroupConfiguration.BuildingId = Building.Id
		Inner Join Units (nolock) on Units.GroupConfigurationId = GroupConfiguration.GroupId
		Inner Join PriceDetails (nolock) on PriceDetails.UnitId = Units.UnitId
		where Quotes.IsDeleted = 0 and Building.IsDeleted = 0 and GroupConfiguration.IsDeleted =0
		and Units.IsDeleted = 0 and PriceDetails.VariableId = 'totalPrice'
		Group by Quotes.QuoteId

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
--select * from Projects(nolock)
--select * from Branch(nolock) where Region like '%Canada%'
--select * from Branches

