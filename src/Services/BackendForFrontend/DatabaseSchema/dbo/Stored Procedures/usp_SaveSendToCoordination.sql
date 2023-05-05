
CREATE PROCEDURE [dbo].[usp_SaveSendToCoordination] 

@QuoteId nvarchar(25)
,@OpportunityId nvarchar(25)
,@createdBy nvarchar(500)
,@coordinationData AS  SendToCordinationDataTable READONLY  

AS
BEGIN
	BEGIN TRY
			declare @count int
			DECLARE @groupId INT
			DECLARE @questionsJson nvarchar(max)

			DECLARE db_cursor CURSOR FOR SELECT GroupId, QuestionsJson FROM @coordinationData; 
					
			OPEN db_cursor;
			FETCH NEXT FROM db_cursor INTO @groupId, @questionsJson;
			WHILE @@FETCH_STATUS = 0  
			BEGIN  

				/* Checking If Data is available or not */
				SET @count = (Select count(*) from [dbo].[CoordinationQuestions] where GroupId=@groupId)
				
				IF @count <> 0
				Begin

					UPDATE CoordinationQuestions SET   QuoteId = @QuoteId,
														  GroupId = @groupId,
														  QuestionnaireJson = @questionsJson,
														  ModifiedBy = @CreatedBy,
														  ModifiedOn = getdate()
														  where GroupId = @groupId
														
				End
				ELSE
				Begin

					Insert into [dbo].[CoordinationQuestions]
					(
					  OpportunityId,
					  GroupId,
					  QuestionnaireJson,
					  CreatedBy,
					  QuoteId
					)
					values( 
						@OpportunityId
						,@groupId
						,@questionsJson
						,@createdBy
						,@QuoteId)

				End
				

					FETCH NEXT FROM db_cursor INTO @groupId, @questionsJson;
			END;
			CLOSE db_cursor;
			DEALLOCATE db_cursor
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