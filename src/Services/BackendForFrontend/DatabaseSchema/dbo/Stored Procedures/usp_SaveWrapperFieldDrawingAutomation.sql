 

CREATE Procedure [dbo].[usp_SaveWrapperFieldDrawingAutomation]
 @groupId int
,@quoteId nvarchar(25)
,@createdBy nvarchar(500)
,@statusId nvarchar(200)
,@IntegratedSystemId  nvarchar(500)
,@IntegratedSystemRef nvarchar(500)
 --,@FdaResult INT OUT

AS

Begin
	BEGIN TRY
			   --declare @QuoteId nvarchar(20)
		       --Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)

			  declare @drawingMethod nvarchar(100)
			  declare @count int
			  declare @FieldDrawingIntegrationId int
 
	          Set @FieldDrawingIntegrationId = (Select  top 1 Id  from  [dbo].[FieldDrawingMaster] where GroupId=@groupId  and QuoteId=@quoteId  
		       --and QuoteId =@QuoteId  
		        order by CreatedOn desc)




			IF EXISTS(Select * from FieldDrawingIntegrationMaster where FieldDrawingIntegrationId=@FieldDrawingIntegrationId)
			BEGIN
			   declare @statusKey nvarchar(30);

			   SET @statusKey = (Select StatusKey from FieldDrawingIntegrationMaster where FieldDrawingIntegrationId=@FieldDrawingIntegrationId)

			   update FieldDrawingMaster set StatusKey=@statusKey where GroupId=@groupId and Id=@FieldDrawingIntegrationId;
			END


 			  IF NOT EXISTS (Select * from [dbo].[FieldDrawingIntegrationMaster] where FieldDrawingIntegrationId=@FieldDrawingIntegrationId)
			  BEGIN

			  declare @Result int;
 
					  Insert into [dbo].[FieldDrawingIntegrationMaster]
					  (
						FieldDrawingIntegrationId
						,IntegratedSystemId
						,IntegratedSystemRef
						,StatusKey
						,CreatedBy
						,CreatedOn
					   )
					  Values 
					  (
						 @FieldDrawingIntegrationId
						,@IntegratedSystemId
						,@IntegratedSystemRef
						,@StatusId
						,@CreatedBy
						,getdate()
					   )
					   SET  @Result =(Select  @@IDENTITY as identityvalue)
				       
					   update FieldDrawingMaster set StatusKey='DWG_SUBD' where GroupId=@groupId and Id=@FieldDrawingIntegrationId;


					   return @Result

			  END
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
	   
	  