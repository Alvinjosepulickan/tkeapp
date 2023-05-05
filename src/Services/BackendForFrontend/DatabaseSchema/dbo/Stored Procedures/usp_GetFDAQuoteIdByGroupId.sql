 
CREATE Procedure [dbo].[usp_GetFDAQuoteIdByGroupId] --15
@groupId int
As
Begin
    Declare @QuoteId Nvarchar(50)
	 Set @QuoteId = (Select QuoteId from Building where id in(Select BuildingId from  [dbo].[GroupConfiguration]
	  where GroupId=@groupId))
	  Select @QuoteId as quoteId
End