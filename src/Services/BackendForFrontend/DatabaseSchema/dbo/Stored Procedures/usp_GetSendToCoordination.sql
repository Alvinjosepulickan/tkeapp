
CREATE PROCEDURE [dbo].[usp_GetSendToCoordination] 
@QuoteId nvarchar(25)
AS
BEGIN
	BEGIN TRY
	     Select  bg.id as buildingId
          ,bg.BldName as buildingName
		  ,gc.GroupId as groupId
		  ,gc.GroupName as groupName

		  ,ISNULL((Select QuestionnaireJson from CoordinationQuestions where GroupId = gc.GroupId), '') as questions
		  
		  ,(case when (select count(*) from CoordinationQuestions where GroupId = gc.GroupId) = 0 then 0  else 1 end)  as isSaved
		  
		  , [dbo].[Fn_IsAllCoordinationGroupsSaved](@QuoteId) as enableSendToCoordination

		  from building bg
	  Left Join GroupConfiguration gc on bg.Id=gc.BuildingId
      Where QuoteId=@QuoteId and gc.IsDeleted=0  and bg.IsDeleted=0
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