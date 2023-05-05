CREATE Procedure [dbo].[usp_GetProjectStatusForFDA]
@QuoteId nvarchar(200)
As
Begin
  Select top 1 UPPER(WorkflowStage) as statusKey from Projects
    Where OpportunityId in(
		Select distinct top 1 OpportunityId from Quotes
		   Where QuoteId=@QuoteId)
End