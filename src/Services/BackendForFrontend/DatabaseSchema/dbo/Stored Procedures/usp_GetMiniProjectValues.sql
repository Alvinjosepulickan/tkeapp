-- =============================================
-- Author: Eakhanath Singh V
-- =============================================
CREATE Procedure [dbo].[usp_GetMiniProjectValues]-- 'aswathy.ramadass@tke.dev','SC-7'
(
@CreatedBy nvarchar(200),
@projectId nvarchar(200),
@VersionId int = 1
)
AS
BEGIN
	BEGIN TRY
		Declare  @idExists int
		set @idExists =  case when Exists(select * from Projects where OpportunityId = @projectId) 
		then 1 else 0 end	

		-- get the required elements for drop down
		select Branch as BranchDetails from Branch (nolock) where Branch.Region like '%Canada%'
		select Code as MeasuringUnitDetails from MeasuringUnits (nolock)
		--for edit project scenari0.. To provide only possible sales stage
		if @idExists >0
		begin
			select Code as SalesDetails from Sales (nolock) where Id >= (select Sales.Id from Projects 
			left join Status on Projects.WorkflowStage = Status.StatusKey
			left join Sales on Status.StatusName = Sales.Code
			where OpportunityId = @projectId)
		end
		else
		begin
			select Code as SalesDetails from Sales (nolock)
		end		
		if exists (select * from Projects (nolock) where Projects.OpportunityId = @projectId)
		Begin
		declare @projectIdValue int
		set @projectIdValue = (select Distinct Id from Projects (nolock) where Projects.OpportunityId =@projectId)

		select distinct Projects.OpportunityId,
		Quotes.QuoteId,
		Quotes.VersionId ,
		Quotes.Description,
		Projects.Name,
		Branch.Branch,
		MeasuringUnits.Code as MeasuringUnit,
		prjstatus.StatusName as Salesman,
		Projects.SalesmanActiveDirectoryID,
		Projects.CreatedBy,
		Projects.CreatedOn,
		Projects.ModifiedBy,
		Projects.ModifiedOn, 
		AccountDetails.AccountName,
		AccountDetails.AddressLine1,
		AccountDetails.City,
		AccountDetails.County,
		AccountDetails.ZipCode,
		AccountDetails.CustomerNumber,
		AccountDetails.AddressLine2,
		AccountDetails.state,
		AccountDetails.AwardCloseDate,
		Status.StatusId,
		Status.StatusKey as QuoteStatusKey,
		Status.StatusName as QuoteStatusName,
		Status.DisplayName as QuoteStatusDisplayName,
		'lawrence.yarbrough@tkelevator.com' as SalesRepEmail,
		'Spencer.Dalman@tkelevator.com' as OperationContactEmail
		from Projects
		left join Branch on Projects.BranchNumber=Branch.BranchNumber
		left join Status prjstatus on Projects.WorkflowStage=prjstatus.StatusKey
		left join MeasuringUnits on Projects.MeasuringUnitId=MeasuringUnits.Id
		left join Quotes on @projectId= Quotes.OpportunityId
		left join Status on Quotes.QuoteStatusId = Status.StatusKey
		left join AccountDetails on AccountDetails.opportunityid = Projects.OpportunityId
		where Projects.OpportunityId=@projectId and Projects.IsDeleted=0 and exists( select Quotes.VersionId where Quotes.OpportunityId = @projectId and Quotes.VersionId = @VersionId)
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
End

