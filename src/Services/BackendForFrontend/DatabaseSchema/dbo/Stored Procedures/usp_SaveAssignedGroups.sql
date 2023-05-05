
 

CREATE Procedure [dbo].[usp_SaveAssignedGroups]
 @BuildingId INT  
,@ConsoleNumber INT   
,@ConsoleVariables AS BuildingEquipmentConsoleDataTable READONLY
,@ConsoleConfigurationVariables AS EntranceConfigurationDataTable READONLY
,@AssignedGroupsDataTable AS AssignedGroupsDataTable READONLY
,@ExistingGroupsDataTable AS ExistingGroupsDataTableForBldgEquipment READONLY
,@FutureGroupsDataTable AS FutureGroupsDataTable READONLY
,@CreatedBy NVARCHAR(50)
,@LogHistoryTable AS HistoryTable readonly
,@Result INT OUTPUT
AS
BEGIN
	BEGIN TRY
                    DECLARE @ConsoleId INT;



                   /*Building Equipment Group Mapping*/
					DECLARE db_cursor CURSOR FOR SELECT ConsoleId,ConsoleName,IsController,IsLobbyPanel FROM @ConsoleVariables; 
					DECLARE @BldConsoleId INT
					DECLARE @ConsoleName NVARCHAR(50)
					DECLARE @IsController INT
					DECLARE @IsLobbyPanel INT

					OPEN db_cursor;
					FETCH NEXT FROM db_cursor INTO @BldConsoleId, @ConsoleName, @IsController,@IsLobbyPanel;
					WHILE @@FETCH_STATUS = 0  
					BEGIN  
						   IF NOT EXISTS(Select distinct ConsoleNumber from [dbo].[BldgEquipmentConsole] where ConsoleNumber=@ConsoleNumber and BuildingId=@BuildingId)
						   Begin

						        Declare @Count INT;
								SET @Count = (Select distinct count(*) from [dbo].[BldgEquipmentConsole] where ConsoleNumber=@ConsoleNumber and BuildingId=@BuildingId)
						       
						       IF (@Count > 0)
						       Begin

							   UPDATE [BldgEquipmentConsole] SET  [Name] = @ConsoleName,   
																  IsController = @IsController,  
																  IsLobby= @IsLobbyPanel,
																  ModifiedBy = @CreatedBy,
															      ModifiedOn = getdate()
																     Where ConsoleNumber = @BldConsoleId and BuildingId=@BuildingId 
																	 

							   END
							   ELSE
							   Begin 
								INSERT INTO [BldgEquipmentConsole]
								(
										ConsoleNumber
									   ,BuildingId
									   ,[Name]
									   ,IsController
									   ,IsLobby
									   ,CreatedBy
									   )  
								VALUES 
									 (
									   @BldConsoleId
									  ,@buildingId
									  ,@ConsoleName
									  ,@IsController
									  ,@IsLobbyPanel
									  ,@CreatedBy
									  );  
							   END
                          END


						   FETCH NEXT FROM db_cursor INTO @BldConsoleId, @ConsoleName, @IsController,@IsLobbyPanel;
					END;
					CLOSE db_cursor;
					DEALLOCATE db_cursor;


			--SET @ConsoleId = @@Identity
			

			--END
			--DECLARE @Count INT;
			--SET @Count = (Select count(*) from [dbo].[BldgEquipmentConsole] where ConsoleNumber=@ConsoleNumber and BuildingId=@BuildingId)
			--IF (@Count > 0)
			--BEGIN 
			   SET @ConsoleId = (Select distinct ConsoleId from [dbo].[BldgEquipmentConsole] where ConsoleNumber=@ConsoleNumber and BuildingId=@BuildingId)
			--END


	    /*Building Equipment Console Configuration*/
			DELETE FROM BldgEquipmentConsoleCnfgn
				WHERE ConsoleId  = @ConsoleId

 
				INSERT INTO BldgEquipmentConsoleCnfgn
				 (
					ConsoleId
				   ,VariableType
				   ,[Value]
				   ,CreatedBy
				   ,ModifiedBy
				   ,ModifiedOn
				 )

			SELECT  @ConsoleId
					,VariableType
					,VariableValue
					,@CreatedBy
					,@CreatedBy
				    ,GETDATE()
					FROM @ConsoleConfigurationVariables  
						WHERE EntranceConsoleId = @ConsoleNumber  

 
			    --declare @islobby int

			    --SET @islobby = (Select * from [dbo].[BldgEquipmentConsole] cl
						 -- left join [dbo].[BldgEquipmentGroupMapping] gm on cl.ConsoleId=gm.ConsoleId
						 --    where cl.ConsoleId=1 and cl.IsLobby = 1)


			    DECLARE @value INT
				DECLARE @groupName NVARCHAR(50)
				DECLARE @isChecked INT

				IF EXISTS(Select * from [dbo].[BldgEquipmentConsole] cl
						  left join [dbo].[BldgEquipmentGroupMapping] gm on cl.ConsoleId=gm.ConsoleId
						     where cl.ConsoleId=@ConsoleId and cl.IsLobby = 1)
				Begin
				   /*Building Equipment Group Mapping*/
					DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName,IsChecked FROM @AssignedGroupsDataTable; 
					
					OPEN db_cursor;
					FETCH NEXT FROM db_cursor INTO @value, @groupName, @isChecked;
					WHILE @@FETCH_STATUS = 0  
					BEGIN  
 
						    IF EXISTS(Select gm.GroupId  from [dbo].[BldgEquipmentConsole] cl
						  right join [dbo].[BldgEquipmentGroupMapping] gm on cl.ConsoleId=gm.ConsoleId
						     where cl.Name like '%Lobby Panel%'
                             and GroupId = @value)
						   Begin

							   --UPDATE BldgEquipmentGroupMapping SET   ConsoleId = @ConsoleId,
							   --                                       is_Checked = @isChecked,
										--							  ModifiedBy = @CreatedBy,
										--							  ModifiedOn = getdate()
				      --                                                    Where  GroupId=@value

					           UPDATE u SET u.ConsoleId = @ConsoleId,
											u.is_Checked = @isChecked,
											u.ModifiedBy = @CreatedBy,
											u.ModifiedOn = getdate()
											FROM BldgEquipmentGroupMapping u
												inner join BldgEquipmentConsole s on
													u.ConsoleId = s.ConsoleId
													  WHERE s.Name like '%Lobby Panel%' and GroupId = @value
						   END
						   ELSE
						   Begin 
							INSERT INTO BldgEquipmentGroupMapping
							(
								GroupId
							   ,GroupName
							   ,ConsoleId
							   ,is_Checked
							   ,CreatedBy
							   )  
						 Values (
						         @value
								,@groupName
								,@ConsoleId
								,@isChecked
								,@CreatedBy
								)
						   END


						   FETCH NEXT FROM db_cursor INTO @value, @groupName, @isChecked;
					END;
					CLOSE db_cursor;
					DEALLOCATE db_cursor;

					End
					Else
					Begin
					   /*Building Equipment Group Mapping*/
					DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName,IsChecked FROM @AssignedGroupsDataTable; 
					OPEN db_cursor;
					FETCH NEXT FROM db_cursor INTO @value, @groupName, @isChecked;
					WHILE @@FETCH_STATUS = 0  
					BEGIN  
 
						    IF EXISTS(Select gm.GroupId  from [dbo].[BldgEquipmentConsole] cl
						  right join [dbo].[BldgEquipmentGroupMapping] gm on cl.ConsoleId=gm.ConsoleId
						     where cl.ConsoleId=@ConsoleId
                             and GroupId = @value)
						   Begin
							   UPDATE BldgEquipmentGroupMapping SET   ConsoleId = @ConsoleId,
							                                          is_Checked = @isChecked,
																	  ModifiedBy = @CreatedBy,
																	  ModifiedOn = getdate()
				                                                          Where  ConsoleId=@ConsoleId and GroupId=@value
						   END
						   ELSE
						   Begin 
							INSERT INTO BldgEquipmentGroupMapping
							(
								GroupId
							   ,GroupName
							   ,ConsoleId
							   ,is_Checked
							   ,CreatedBy
							   )  
						 Values (
						         @value
								,@groupName
								,@ConsoleId
								,@isChecked
								,@CreatedBy
								)
						   END


						   FETCH NEXT FROM db_cursor INTO @value, @groupName, @isChecked;
					END;
					CLOSE db_cursor;
					DEALLOCATE db_cursor;
					End

					/*Basic Log*/
					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group Name',eg.GroupName,'',@CreatedBy,getdate(),@CreatedBy,getdate()
					from @ExistingGroupsDataTable eg where eg.GroupName not in (select GroupName from BldgEquipmentCategoryCnfgn 
																				where ConsoleId = @ConsoleId and GroupCategoryId=1)
					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group No. of units',eg.NoOfUnits,'',@CreatedBy,getdate(),@CreatedBy,getdate()
					from @ExistingGroupsDataTable eg where eg.GroupName not in (select GroupName from BldgEquipmentCategoryCnfgn 
																				where ConsoleId = @ConsoleId and GroupCategoryId=1)
					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group Factory Job ID',eg.GroupFactoryId,'',@CreatedBy,getdate(),@CreatedBy,getdate()
					from @ExistingGroupsDataTable eg where eg.GroupName not in (select GroupName from BldgEquipmentCategoryCnfgn 
																				where ConsoleId = @ConsoleId and GroupCategoryId=1)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group Name','',GroupName,@CreatedBy,getdate(),@CreatedBy,getdate()
					from BldgEquipmentCategoryCnfgn eg where ConsoleId = @ConsoleId and GroupCategoryId=1 and 
					GroupName not in (select GroupName from @ExistingGroupsDataTable)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group No. of units','',NoOfUnits,@CreatedBy,getdate(),@CreatedBy,getdate()
					from BldgEquipmentCategoryCnfgn  where ConsoleId = @ConsoleId and GroupCategoryId=1 and 
					GroupName not in (select GroupName from @ExistingGroupsDataTable)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Existing Group Factory Job ID','',FactoryId,@CreatedBy,getdate(),@CreatedBy,getdate()
					from BldgEquipmentCategoryCnfgn where ConsoleId = @ConsoleId and GroupCategoryId=1 and 
					GroupName not in (select GroupName from @ExistingGroupsDataTable)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Future Group Name',eg.GroupName,'',@CreatedBy,getdate(),@CreatedBy,getdate()
					from @FutureGroupsDataTable eg where eg.GroupName not in (select GroupName from BldgEquipmentCategoryCnfgn 
																				where ConsoleId = @ConsoleId and GroupCategoryId=2)
					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Future Group No. of units',eg.NoOfUnits,'',@CreatedBy,getdate(),@CreatedBy,getdate()
					from @FutureGroupsDataTable eg where eg.GroupName not in (select GroupName from BldgEquipmentCategoryCnfgn 
																				where ConsoleId = @ConsoleId and GroupCategoryId=2)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Future Group Name','',GroupName,@CreatedBy,getdate(),@CreatedBy,getdate()
					from BldgEquipmentCategoryCnfgn eg where ConsoleId = @ConsoleId and GroupCategoryId=2 and 
					GroupName not in (select GroupName from @FutureGroupsDataTable)

					insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
					select @BuildingId,'Group Assignment - Future Group No. of units','',NoOfUnits,@CreatedBy,getdate(),@CreatedBy,getdate()
					from BldgEquipmentCategoryCnfgn  where ConsoleId = @ConsoleId and GroupCategoryId=2 and 
					GroupName not in (select GroupName from @FutureGroupsDataTable)
					/*Basic Log*/

					 /*Existing Group*/
					DELETE from BldgEquipmentCategoryCnfgn where ConsoleId = @ConsoleId


					INSERT INTO BldgEquipmentCategoryCnfgn
					  (
					   ConsoleId,
					   GroupCategoryId,
					   GroupName,
					   NoOfUnits,
					   FactoryId,
					   CreatedBy
					  )
					  SELECT @ConsoleId
							,GroupCategoryId
							,GroupName
							,NoOfUnits
							,GroupFactoryId
							,@CreatedBy
						   FROM @ExistingGroupsDataTable  
							 WHERE ConsoleId = @ConsoleNumber


					/*Future Group*/
					INSERT INTO BldgEquipmentCategoryCnfgn
					(
					ConsoleId,
					GroupCategoryId,
					GroupName,
					NoOfUnits,
					CreatedBy,
					CreatedOn
					)
					SELECT @ConsoleId
						,GroupCategoryId
						,GroupName
						,NoOfUnits
						,@CreatedBy
						,GETDATE()
						FROM @FutureGroupsDataTable 
							WHERE ConsoleId = @ConsoleNumber 
	/**HistoryTable**/

		insert into BuildingConfigHistory(BuildingId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @BuildingId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
		from @LogHistoryTable

		/**HistoryTable**/

	          SET @Result = @BuildingId
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
