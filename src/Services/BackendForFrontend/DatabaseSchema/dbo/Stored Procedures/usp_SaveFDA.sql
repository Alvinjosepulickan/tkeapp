

CREATE Procedure [dbo].[usp_SaveFDA]
 @groupId int
,@QuoteId nvarchar(25) = NULL
,@OpportunityId nvarchar(25)
--@QuoteId nvarchar(25),
-- @VersionId nvarchar(25)
,@createdBy nvarchar(500)
,@FDAVariables AS FieldDrawingAutomationDataTable READONLY  
,@groupVariables AS GroupVariablesForAutomationDataTable READONLY 
--,@FdaResult INT OUT
AS

Begin
	BEGIN TRY
		--declare @QuoteId nvarchar(20)
		--Select @QuoteId = (Select QuoteId from Quotes where OpportunityId = @OpportunityId and VersionId = @VersionId)

		   declare @drawingMethod nvarchar(100);
		   declare @Result int;
		   declare @StatusKey nvarchar(100);
		   declare @count int;
		   --SET @drawingMethod = (Select [dbo].[Fn_GetDrawingMethod](@groupId))

		   /* Checking If Data is available or not */
			  SET @count = (Select count(*) from [dbo].[FieldDrawingMaster] where GroupId=@groupId)

			  /* If Data is available */
			   Set @StatusKey = (Select  (case when (Select top 1 StatusKey from  [dbo].[FieldDrawingMaster] where GroupId=@groupId and QuoteId=@OpportunityId
						--and QuoteId =@QuoteId  
						order by CreatedOn desc) is NULL then '' 
						   else (Select top 1 StatusKey from  [dbo].[FieldDrawingMaster] where GroupId=@groupId and QuoteId=@OpportunityId --and QuoteId =@QuoteId 
							order by CreatedOn desc) end))
				  

			   IF(@count  <> 0)
			   Begin
				IF(@StatusKey = 'DWG_CMP'  or @StatusKey = 'DWG_ERR') /* Completed or Error */
				Begin
		    
					   Insert into [dbo].[FieldDrawingMaster]
					  (
						 GroupId
						,OpportunityId
						,QuoteId
						,CreatedBy
					   )
					  Values 
					  (
						 @groupId
						,''
						,@OpportunityId
						,@createdBy
					   )

					   SET @Result = SCOPE_IDENTITY()
			 
			  
			 
						/* Inserting data into Field Drawing AUtomation*/
						  Insert into [dbo].[FieldDrawingAutomation]
						  (
							 FieldDrawingId
							,FDAType
							,FDAValue
							,createdBy
						   )
						  SELECT @Result
								,FDAType
								,FDAValue
								,@CreatedBy
								FROM @FDAVariables  

				End
				Else
				Begin /* If Other Status */

					   SET @Result = (Select top 1 Id from  [dbo].[FieldDrawingMaster] where GroupId=@groupId and QuoteId=@OpportunityId 
					   --and QuoteId =@QuoteId 
						order by CreatedOn desc)

							DECLARE @FDAGroupId INT
							DECLARE @FDAValue INT
							DECLARE @FDAType Nvarchar(500)

							DECLARE db_cursor CURSOR FOR SELECT @groupId,FDAType,FDAValue FROM @FDAVariables; 
					
							OPEN db_cursor;
							FETCH NEXT FROM db_cursor INTO @FDAGroupId, @FDAType, @FDAValue;
							WHILE @@FETCH_STATUS = 0  
							BEGIN  

								IF Exists(Select * from FieldDrawingAutomation where FDAType = @FDAType)
								Begin
									   UPDATE FieldDrawingAutomation SET   FDAValue = @FDAValue,
																		   ModifiedBy = @CreatedBy,
																		   ModifiedOn = getdate()
																				  Where FDAType=@FDAType and FieldDrawingId=@Result
								End
								Else
								Begin
								  Insert into [dbo].[FieldDrawingAutomation]
								  (
									 FieldDrawingId
									,FDAType
									,FDAValue
									,createdBy
								   )
								  SELECT @Result
										,FDAType
										,FDAValue
										,@CreatedBy
										FROM @FDAVariables  Where FDAType=@FDAType
								End


								   FETCH NEXT FROM db_cursor INTO @FDAGroupId, @FDAType, @FDAValue;
							END;
							CLOSE db_cursor;
							DEALLOCATE db_cursor	
				 
				End
					 
			   End
			   Else
			   Begin
	       
		     
					   Insert into [dbo].[FieldDrawingMaster]
					  (
						 GroupId
						,OpportunityId
						,QuoteId
						,CreatedBy
					   )
					  Values 
					  (
						 @groupId
						,''
						,@OpportunityId
						,@createdBy
					   )

					   SET @Result = SCOPE_IDENTITY()




				
						/* Inserting data into Field Drawing AUtomation*/
						
						Insert into [dbo].[FieldDrawingAutomation]
						  (
							 FieldDrawingId
							,FDAType
							,FDAValue
							,createdBy
						   )
						  SELECT @Result
								,FDAType
								,FDAValue
								,@CreatedBy
								FROM @FDAVariables  

					 
			   End




							DECLARE @GrpVariableGroupId INT
							DECLARE @GroupConfigurationValue Nvarchar(500)
							DECLARE @GroupConfigurationType Nvarchar(500)

							DECLARE db_cursor CURSOR FOR SELECT @groupId,GroupConfigurationType,GroupConfigurationValue FROM @groupVariables; 
					
							OPEN db_cursor;
							FETCH NEXT FROM db_cursor INTO @GrpVariableGroupId, @GroupConfigurationType, @GroupConfigurationValue;
							WHILE @@FETCH_STATUS = 0  
							BEGIN  

								IF Exists(Select * from GroupConfigurationDetails where GroupConfigurationType = @GroupConfigurationType and GroupId=@groupId)
								Begin
									   UPDATE GroupConfigurationDetails SET   GroupConfigurationValue = @GroupConfigurationValue,
																		   ModifedBy = @CreatedBy,
																		   ModifiedOn = getdate()
																				  Where GroupConfigurationType=@GroupConfigurationType and GroupId=@groupId
								End
								Else
								Begin

						 
								  declare @buildingId int

								  SET @buildingId = (Select BuildingId from GroupConfiguration where GroupId = @groupId)

								  Insert into GroupConfigurationDetails
								  (
									 GroupId
									,BuildingId
									,GroupConfigurationType
									,GroupConfigurationValue
									,createdBy
								   )
								  SELECT @groupId
										,@buildingId
										,GroupConfigurationType
										,GroupConfigurationValue
										,@CreatedBy
										FROM @groupVariables  Where GroupConfigurationType=@GroupConfigurationType
								End


								   FETCH NEXT FROM db_cursor INTO @GrpVariableGroupId, @GroupConfigurationType, @GroupConfigurationValue;
							END;
							CLOSE db_cursor;
							DEALLOCATE db_cursor

				 return @Result
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
  
