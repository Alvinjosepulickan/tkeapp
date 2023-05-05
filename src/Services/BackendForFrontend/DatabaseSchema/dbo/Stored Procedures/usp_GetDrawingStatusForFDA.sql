CREATE Procedure [dbo].[usp_GetDrawingStatusForFDA]
 @QuoteId nvarchar(200)
,@groupId int
As
Begin

  Select top 1 UPPER(StatusKey) as StatusKey  from FieldDrawingMaster
    where GroupId=@groupId and QuoteId=@QuoteId 
	  order by CreatedOn desc

End