 
CREATE Procedure [dbo].[usp_GetOpportunityAndVersionIdByQuoteId]
@quoteId nvarchar(50)
as
Begin
   Select OpportunityId,VersionId from quotes
     where QuoteId=@quoteId
End