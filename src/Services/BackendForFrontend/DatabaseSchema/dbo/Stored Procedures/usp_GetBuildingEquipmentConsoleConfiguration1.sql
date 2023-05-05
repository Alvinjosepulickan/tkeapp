create Procedure [dbo].[usp_GetBuildingEquipmentConsoleConfiguration1] --125,'aswathy.ramadass@tke.dev'
 @buildingId INT
,@userName NVARCHAR(100) 
AS 
BEGIN
	BEGIN TRY
		 DECLARE @isBuildingEquipment INT; 
		 declare @consoleIdSmartRescue INT;
		 SET @isBuildingEquipment = (Select [dbo].[Fn_GetBuildingEquipmentBybuildingId](@buildingId))
		 IF(@isBuildingEquipment = 1) 
		 BEGIN
		 
		--Building Equipment status to complete when user is visiting the screen
		update Building set BuildingEquipmentStatus='BLDGEQP_COM'
		where id=@buildingId

		 IF(EXISTS(SELECT *  FROM BldgEquipmentConsole  WHERE  BuildingId = @BuildingId)) 
		 BEGIN
	  
					  /*Get All Groups*/
						declare @GroupMapping table(groupId INT, groupName NVARCHAR(500))
						insert @GroupMapping
						exec usp_GetBuildingEquipmentAllGroups @buildingId
						--Select * from @GroupMapping


					  DECLARE @BldConsoleId INT; 
					  SET @BldConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Lobby Panel 1')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Lobby Panel 1')
											  end

					  DECLARE @RBTConsoleId INT; 
					  SET @RBTConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Robotic Controller Interface')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Robotic Controller Interface')
											  end

					  DECLARE @BACNetConsoleId INT; 
					  SET @BACNetConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'BACNet')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'BACNet')
											  end
	
					  DECLARE @SmartRescuePhone5ConsoleId INT; 
					  SET @SmartRescuePhone5ConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 5-unit')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 5-unit')
											  end
					
					  DECLARE @SmartRescuePhone10ConsoleId INT; 
					  SET @SmartRescuePhone10ConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 10-unit')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 10-unit')
											  end

                      
						DECLARE @BldgroupId INT
						DECLARE @BldgroupName NVARCHAR(50)
						DECLARE @noOfUnits INT;


						IF(@BldConsoleId > 0)
						BEGIN
					  /*Inserting new Groups in Building Equipment Group Mapping*/
						DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName FROM @GroupMapping; 
						OPEN db_cursor;
						FETCH NEXT FROM db_cursor INTO @BldgroupId, @BldgroupName;
						WHILE @@FETCH_STATUS = 0  
						BEGIN  
							   IF NOT EXISTS(Select GroupId from  BldgEquipmentGroupMapping where GroupId = @BldgroupId)
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
									 @BldgroupId
									,@BldgroupName
									,@BldConsoleId
									,0
									,@userName
									)

							 /*Insert into Robotic Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BldgroupId
									,@BldgroupName
									,@RBTConsoleId
									,1
									,@userName
									)

							 /*Insert into BAC Net*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BldgroupId
									,@BldgroupName
									,@BACNetConsoleId
									,1
									,@userName
									)

								set @noOfUnits = (select count(*) from Units where GroupConfigurationId = @BldgroupId)
								if @noOfUnits < 6
								begin
								/*Insert into Smart Rescue Phone5*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BldgroupId
										,@BldgroupName
										,@SmartRescuePhone5ConsoleId
										,1
										,@userName
										)
								end
								else
								begin
								/*Insert into Smart Rescue Phone10*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BldgroupId
										,@BldgroupName
										,@SmartRescuePhone10ConsoleId
										,1
										,@userName
										)
								end

                          

						END
							
							   ELSE IF NOT EXISTS (Select GroupId from  BldgEquipmentGroupMapping where GroupId = @BldgroupId and ConsoleId in (select distinct ConsoleId from BldgEquipmentConsole where Name like '%Lobby Panel%'))
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
									 @BldgroupId
									,@BldgroupName
									,@BldConsoleId
									,0
									,@userName
									)
							   END

							   FETCH NEXT FROM db_cursor INTO @BldgroupId, @BldgroupName;
						END;
						CLOSE db_cursor;
						DEALLOCATE db_cursor;
						END
						ELSE
						BEGIN
					 

						  SET @consoleIdSmartRescue =   (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Dummy console')
				                         
							 /*Inserting new Groups in Building Equipment Group Mapping*/
						DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName FROM @GroupMapping; 
				
						OPEN db_cursor;
						FETCH NEXT FROM db_cursor INTO @BldgroupId, @BldgroupName;
						WHILE @@FETCH_STATUS = 0  
						BEGIN  
							   IF NOT EXISTS(Select GroupId from  BldgEquipmentGroupMapping where GroupId = @BldgroupId)
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
									 @BldgroupId
									,@BldgroupName
									,@consoleIdSmartRescue
									,0
									,@userName
									)

							  /*Insert into Robotic Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BldgroupId
									,@BldgroupName
									,@RBTConsoleId
									,1
									,@userName
									)

								
								/*Insert into BAC Net*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BldgroupId
									,@BldgroupName
									,@BACNetConsoleId
									,1
									,@userName
									)

								set @noOfUnits = (select count(*) from Units where GroupConfigurationId = @BldgroupId)
								if @noOfUnits < 6
								begin
								/*Insert into Smart Rescue Phone5*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BldgroupId
										,@BldgroupName
										,@SmartRescuePhone5ConsoleId
										,1
										,@userName
										)
								end
								else
								begin
								/*Insert into Smart Rescue Phone10*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BldgroupId
										,@BldgroupName
										,@SmartRescuePhone10ConsoleId
										,1
										,@userName
										)
								end

							   END


							   FETCH NEXT FROM db_cursor INTO @BldgroupId, @BldgroupName;
						END;
						CLOSE db_cursor;
						DEALLOCATE db_cursor;
					   
						END


						/*Updating smart rescue phone consoles default assigned groups*/
						IF(@BldConsoleId > 0)
						Begin

						   DECLARE @groupsId INT
					       DECLARE @groupsName NVARCHAR(50)
						   DECLARE @isChecked INT
						   DECLARE @consoleId INT
						   DECLARE @noOfUnit INT;
						   DECLARE @lobbyConsoleId INT;

						   DECLARE db_cursor CURSOR FOR Select ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and Name like '%Lobby Panel%'
							OPEN db_cursor;
							FETCH NEXT FROM db_cursor INTO @lobbyConsoleId;
							WHILE @@FETCH_STATUS = 0  
							BEGIN  
									
									if exists(select * from BldgEquipmentConsoleCnfgn where ((VariableType like '%IsSmartRescue5_Bool_SP%' and Value = 'True') or (VariableType like '%IsSmartRescue10_Bool_SP%' and Value = 'True')) and ConsoleId = @lobbyConsoleId)
									Begin
										DECLARE group_cursor CURSOR FOR SELECT GroupId,GroupName,is_Checked FROM BldgEquipmentGroupMapping where ConsoleId = @lobbyConsoleId 
										OPEN group_cursor;
										FETCH NEXT FROM group_cursor INTO @groupsId, @groupsName, @isChecked;
										WHILE @@FETCH_STATUS = 0  
										BEGIN  
									  
											   if exists(select * from BldgEquipmentConsoleCnfgn where VariableType like '%IsSmartRescue5_Bool_SP%' and Value = 'True' and ConsoleId = @lobbyConsoleId)
											   Begin
													if @isChecked = 1
													begin
														update BldgEquipmentGroupMapping set is_Checked = 0 where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId
													end
													else
													begin
														set @noOfUnits = (select count(*) from Units where GroupConfigurationId = @groupsId)
														if @noOfUnits < 6
														begin
															update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId
														end
														else
														begin
															update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone10ConsoleId and GroupId =@groupsId
														end
													end	
											   End
											   else 
											   Begin
													if @isChecked = 1
													begin
														update BldgEquipmentGroupMapping set is_Checked = 0 where ConsoleId = @SmartRescuePhone10ConsoleId and GroupId =@groupsId
														if exists(select * from BldgEquipmentGroupMapping where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId)
														begin
															update BldgEquipmentGroupMapping set is_Checked = 0 where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId
														end
													end
													else
													begin
														set @noOfUnits = (select count(*) from Units where GroupConfigurationId = @groupsId)
														if @noOfUnits > 5
														begin
															update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone10ConsoleId and GroupId =@groupsId
														end	
														else
														begin
															update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId
														end
													end	
											   End
									   
									   

											   FETCH NEXT FROM group_cursor INTO @groupsId, @groupsName, @isChecked;
										END;
										CLOSE group_cursor;
										DEALLOCATE group_cursor;

									End
									Else
									Begin
										DECLARE groups_cursor CURSOR FOR SELECT GroupId FROM BldgEquipmentGroupMapping where ConsoleId = @lobbyConsoleId 
										OPEN groups_cursor;
										FETCH NEXT FROM groups_cursor INTO @groupsId;
										WHILE @@FETCH_STATUS = 0  
										BEGIN  
									  
											   set @noOfUnits = (select count(*) from Units where GroupConfigurationId = @groupsId)
												if @noOfUnits < 6
												begin
													update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone5ConsoleId and GroupId =@groupsId
												end
												else
												begin
													update BldgEquipmentGroupMapping set is_Checked = 1 where ConsoleId = @SmartRescuePhone10ConsoleId and GroupId =@groupsId
												end
											   
											   FETCH NEXT FROM groups_cursor INTO @groupsId;
										END;
										CLOSE groups_cursor;
										DEALLOCATE groups_cursor;
										
									End


									FETCH NEXT FROM db_cursor INTO @lobbyConsoleId;
							END;
							CLOSE db_cursor;
							DEALLOCATE db_cursor;
							

						End



						IF(@BldConsoleId > 0)
						Begin
						  /*Building Equipment Console*/
						  Select distinct
							   bec.ConsoleId,
							   bec.ConsoleNumber,
							   bec.[Name] AS ConsoleName,
							   bec.IsLobby,
							   bec.IsController,
							   bec.BuildingId,
							  [dbo].[Fn_GetGroupsCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedGroups , 
							  [dbo].[Fn_GetUnitsCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedUnits 
							FROM
							   BldgEquipmentConsole bec 
							   LEFT JOIN
								  [dbo].[BldgEquipmentConsoleCnfgn] becc 
								  ON bec.ConsoleId = becc.ConsoleId 
							   LEFT JOIN
								  [dbo].[BldgEquipmentGroupMapping] gm 
								  ON gm.ConsoleId = bec.ConsoleId 
							Where
							   BuildingId = @buildingId 
						 End
						 Else
						 Begin
							/*Building Equipment Console*/
							  Select distinct
								   bec.ConsoleId,
								   bec.ConsoleNumber,
								   bec.[Name] AS ConsoleName,
								   bec.IsLobby,
								   bec.IsController,
								   bec.BuildingId,
								  [dbo].[Fn_GetGroupsNoLobbyCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedGroups , 
								  [dbo].[Fn_GetUnitsNoLobbyCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedUnits 
								FROM
								   BldgEquipmentConsole bec 
								   LEFT JOIN
									  [dbo].[BldgEquipmentConsoleCnfgn] becc 
									  ON bec.ConsoleId = becc.ConsoleId 
								   LEFT JOIN
									  [dbo].[BldgEquipmentGroupMapping] gm 
									  ON gm.ConsoleId = bec.ConsoleId 
								Where
								   BuildingId = @buildingId 
						 End



							   /*Variable Assignments*/
						  Select distinct
							   bec.ConsoleId,
							   bec.ConsoleNumber,
							   becc.VariableType,
							   becc.[Value]
							FROM
							   BldgEquipmentConsole bec 
							   LEFT JOIN
								  [dbo].[BldgEquipmentConsoleCnfgn] becc 
								  ON bec.ConsoleId = becc.ConsoleId 
							   LEFT JOIN
								  [dbo].[BldgEquipmentGroupMapping] gm 
								  ON gm.ConsoleId = bec.ConsoleId 
							Where
							   BuildingId = @buildingId



					
						IF(@BldConsoleId > 0)
						Begin
						/*Group Mapping*/
						Select
							bec.ConsoleId,
							bec.ConsoleNumber,
							gm.GroupId as groupId,
							gm.GroupName AS groupName,
							gm.is_Checked,
							[dbo].[Fn_GetTotalGroupsCount](gm.GroupId) AS totalGroups , 
							[dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) AS totalUnits 
						FROM
							BldgEquipmentConsole bec 
							LEFT JOIN
								[dbo].[BldgEquipmentGroupMapping] gm 
								ON bec.ConsoleId = gm.ConsoleId 
						Where
							BuildingId = @buildingId 
							and gm.GroupName <> ''  and   bec.Name not in ('Dummy Console')

						--	union all

							--Select
		  --                  bec.ConsoleId,
		  --                  bec.ConsoleNumber,
		  --                  gm.GroupId as groupId,
		  --                  gm.GroupName AS groupName,
		  --                  gm.is_Checked,
		  --                  (
		  --                      case
		  --                      when
		  --                          bec.Name like '%Dummy Panel%' 
		  --                      then
		  --                          [dbo].[Fn_GetTotalGroupsCount](gm.GroupId) 
		  --                      else
		  --                          0 
		  --                      end
		  --                  )
		  --                  AS totalGroups , 
		  --                  (
		  --                      case
		  --                      when
		  --                          bec.Name like '%Dummy Panel%'  
		  --                      then
		  --                          [dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) 
		  --                      else
		  --                          0 
		  --                      end
		  --                  )
		  --                  AS totalUnits 
		  --              FROM
		  --                  BldgEquipmentConsole bec 
		  --                  LEFT JOIN
		  --                      [dbo].[BldgEquipmentGroupMapping] gm 
		  --                      ON bec.ConsoleId = gm.ConsoleId 
		  --              Where
		  --                  BuildingId =  8
		  --                  and   bec.Name like '%Dummy Panel%'   
		  --                  and gm.GroupName <> ''
						END
						ELSE
						Begin
						  Select
							bec.ConsoleId,
							bec.ConsoleNumber,
							gm.GroupId as groupId,
							gm.GroupName AS groupName,
							gm.is_Checked,
							[dbo].[Fn_GetTotalGroupsCount](gm.GroupId) AS totalGroups , 
							[dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) AS totalUnits 
						FROM
							BldgEquipmentConsole bec 
							LEFT JOIN
								[dbo].[BldgEquipmentGroupMapping] gm 
								ON bec.ConsoleId = gm.ConsoleId 
						Where
							BuildingId = @buildingId
							and gm.GroupName <> ''   
						End



							/*Existing and Future Group Data*/
							Select distinct
								bec.ConsoleId,
								bec.ConsoleNumber,
								gcm.GroupCategoryId,
								gcm.GroupCategoryName,
								bcc.GroupName ,
								bcc.NoOfUnits
							FROM
								BldgEquipmentConsole bec 
								LEFT JOIN
								[dbo].[BldgEquipmentCategoryCnfgn] bcc 
								on bec.ConsoleId = bcc.ConsoleId 
								LEFT JOIN
								[dbo].[BldgEquipmentGroupCategoryMaster] gcm 
								ON bcc.GroupCategoryId = gcm.GroupCategoryId 
							Where
								BuildingId = @buildingId 
								and bec.Name like '%Lobby Panel%' 
								and gcm.GroupCategoryName <> '' 

								END
		 ELSE
			BEGIN
						/*Inserting Default Coonsoles*/
						DECLARE @isLobby INT; 
						SET @isLobby = (Select [dbo].[Fn_GetLobbyChecked](@buildingId))
						IF (@isLobby = 1) 
								BEGIN
								/*Lobby Panel console*/
								INSERT INTO
									BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
								VALUES
									(
										1,
										@buildingId,
										'Lobby Panel 1',
										1,
										1,
										@userName,
										getdate() 
									)
									/*Smart Rescue Phone 5 Console*/
									INSERT INTO
										BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
									VALUES
										(
											2,
											@buildingId,
											'Smart Rescue Phone, 5-unit',
											0,
											1,
											@userName,
											getdate() 
										)
										/*Smart Rescue Phone 10 Console*/
									INSERT INTO
										BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
									VALUES
										(
											3,
											@buildingId,
											'Smart Rescue Phone, 10-unit',
											0,
											1,
											@userName,
											getdate() 
										)
										/*Robotic Controller Console*/
										INSERT INTO
											BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
										VALUES
											(
											4,
											@buildingId,
											'Robotic Controller Interface',
											0,
											1,
											@userName,
											getdate() 
											)
											/*BACNet Console*/
											INSERT INTO
											BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
											VALUES
											(
												5,
												@buildingId,
												'BACNet',
												0,
												1,
												@userName,
												getdate() 
											)
								END
								ELSE
								BEGIN
								/*Dummy Console*/
									INSERT INTO
										BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
									VALUES
										(
											1,
											@buildingId,
											'Dummy Console',
											0,
											1,
											@userName,
											getdate() 
										)
									/*Smart Rescue Phone 5 Console*/
									INSERT INTO
										BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
									VALUES
										(
											2,
											@buildingId,
											'Smart Rescue Phone, 5-unit',
											0,
											1,
											@userName,
											getdate() 
										)
										/*Smart Rescue Phone 10 Console*/
									INSERT INTO
										BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
									VALUES
										(
											3,
											@buildingId,
											'Smart Rescue Phone, 10-unit',
											0,
											1,
											@userName,
											getdate() 
										)
										/*Robotic Controller Console*/
										INSERT INTO
											BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
										VALUES
											(
											4,
											@buildingId,
											'Robotic Controller Interface',
											0,
											1,
											@userName,
											getdate() 
											)
											/*BACNet Console*/
											INSERT INTO
											BldgEquipmentConsole ( ConsoleNumber , BuildingId , [Name] , IsLobby , IsController , CreatedBy , CreatedOn ) 
											VALUES
											(
												5,
												@buildingId,
												'BACNet',
												0,
												1,
												@userName,
												getdate() 
											)
								END

                    
						/*Get All Groups*/
						declare @tblGroupMapping table(groupId INT, groupName NVARCHAR(500))
						insert @tblGroupMapping
						exec usp_GetBuildingEquipmentAllGroups @buildingId
						--Select * from @tblGroupMapping


					  DECLARE @BEConsoleId INT; 
					  SET @BEConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Lobby Panel 1')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Lobby Panel 1')
											  end
		  
					  DECLARE @RBTCCConsoleId INT; 
					  SET @RBTCCConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Robotic Controller Interface')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Robotic Controller Interface')
											  end

					 DECLARE @BACNetCConsoleId INT; 
					  SET @BACNetCConsoleId = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'BACNet')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'BACNet')
											  end
					 DECLARE @SmartRescuePhone5Id INT; 
					  SET @SmartRescuePhone5Id = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 5-unit')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 5-unit')
											  end
					
					  DECLARE @SmartRescuePhone10Id INT; 
					  SET @SmartRescuePhone10Id = case when (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 10-unit')
											 is null then 0 else (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Smart Rescue Phone, 10-unit')
											  end

						DECLARE @BEgroupId INT
						DECLARE @BEgroupName NVARCHAR(50)
						DECLARE @noOfUnitss INT;

						IF(@BEConsoleId > 0)
						BEGIN
					  /*Inserting new Groups in Building Equipment Group Mapping*/
						DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName FROM @tblGroupMapping; 
					
						OPEN db_cursor;
						FETCH NEXT FROM db_cursor INTO @BEgroupId, @BEgroupName;
						WHILE @@FETCH_STATUS = 0  
						BEGIN  
							   IF NOT EXISTS(Select GroupId from  BldgEquipmentGroupMapping where GroupId = @BEgroupId)
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
									 @BEgroupId
									,@BEgroupName
									,@BEConsoleId
									,1
									,@userName
									)

								/*Insert into Robotic Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BEgroupId
									,@BEgroupName
									,@RBTCCConsoleId
									,1
									,@userName
									)

								/*Insert into BAC Net Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BEgroupId
									,@BEgroupName
									,@BACNetCConsoleId
									,1
									,@userName
									)
							 set @noOfUnitss = (select count(*) from Units where GroupConfigurationId = @BEgroupId)
								if @noOfUnitss < 6
								begin
								/*Insert into Smart Rescue Phone5*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BEgroupId
										,@BEgroupName
										,@SmartRescuePhone5Id
										,1
										,@userName
										)
								end
								else
								begin
								/*Insert into Smart Rescue Phone10*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BEgroupId
										,@BEgroupName
										,@SmartRescuePhone10Id
										,1
										,@userName
										)
								end

							   END


							   FETCH NEXT FROM db_cursor INTO @BEgroupId, @BEgroupName;
						END;
						CLOSE db_cursor;
						DEALLOCATE db_cursor;
						END
						ELSE
						BEGIN

						  SET @consoleIdSmartRescue =   (Select distinct ConsoleId from BldgEquipmentConsole where buildingId = @buildingId and [Name] = 'Dummy console')
							 /*Inserting new Groups in Building Equipment Group Mapping*/
						DECLARE db_cursor CURSOR FOR SELECT GroupId,GroupName FROM @tblGroupMapping; 
					
						OPEN db_cursor;
						FETCH NEXT FROM db_cursor INTO @BEgroupId, @BEgroupName;
						WHILE @@FETCH_STATUS = 0  
						BEGIN  
							   IF NOT EXISTS(Select GroupId from  BldgEquipmentGroupMapping where GroupId = @BEgroupId)
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
									 @BEgroupId
									,@BEgroupName
									,@consoleIdSmartRescue
									,0
									,@userName
									)

								/*Insert into Robotic Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BEgroupId
									,@BEgroupName
									,@RBTCCConsoleId
									,1
									,@userName
									)

								/*Insert into BAC Net Console*/
								INSERT INTO BldgEquipmentGroupMapping
								(
									GroupId
								   ,GroupName
								   ,ConsoleId
								   ,is_Checked
								   ,CreatedBy
								   )  
							  Values (
									 @BEgroupId
									,@BEgroupName
									,@BACNetCConsoleId
									,1
									,@userName
									)

							  set @noOfUnitss = (select count(*) from Units where GroupConfigurationId = @BEgroupId)
								if @noOfUnitss < 6
								begin
								/*Insert into Smart Rescue Phone5*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BEgroupId
										,@BEgroupName
										,@SmartRescuePhone5Id
										,1
										,@userName
										)
								end
								else
								begin
								/*Insert into Smart Rescue Phone10*/ 
									INSERT INTO BldgEquipmentGroupMapping
									(
										GroupId
										,GroupName
										,ConsoleId
										,is_Checked
										,CreatedBy
										)  
									Values (
											@BEgroupId
										,@BEgroupName
										,@SmartRescuePhone10Id
										,1
										,@userName
										)
								end
							   END


							   FETCH NEXT FROM db_cursor INTO @BEgroupId, @BEgroupName;
						END;
						CLOSE db_cursor;
						DEALLOCATE db_cursor;
						END


							IF(@BldConsoleId > 0)
						Begin
						  /*Building Equipment Console*/
						  Select distinct
							   bec.ConsoleId,
							   bec.ConsoleNumber,
							   bec.[Name] AS ConsoleName,
							   bec.IsLobby,
							   bec.IsController,
							   bec.BuildingId,
							  [dbo].[Fn_GetGroupsCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedGroups , 
							  [dbo].[Fn_GetUnitsCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedUnits 
							FROM
							   BldgEquipmentConsole bec 
							   LEFT JOIN
								  [dbo].[BldgEquipmentConsoleCnfgn] becc 
								  ON bec.ConsoleId = becc.ConsoleId 
							   LEFT JOIN
								  [dbo].[BldgEquipmentGroupMapping] gm 
								  ON gm.ConsoleId = bec.ConsoleId 
							Where
							   BuildingId = @buildingId 
						 End
						 Else
						 Begin
							/*Building Equipment Console*/
							  Select distinct
								   bec.ConsoleId,
								   bec.ConsoleNumber,
								   bec.[Name] AS ConsoleName,
								   bec.IsLobby,
								   bec.IsController,
								   bec.BuildingId,
								  [dbo].[Fn_GetGroupsNoLobbyCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedGroups , 
								  [dbo].[Fn_GetUnitsNoLobbyCount](bec.BuildingId, bec.ConsoleId,bec.[Name])  AS AssignedUnits 
								FROM
								   BldgEquipmentConsole bec 
								   LEFT JOIN
									  [dbo].[BldgEquipmentConsoleCnfgn] becc 
									  ON bec.ConsoleId = becc.ConsoleId 
								   LEFT JOIN
									  [dbo].[BldgEquipmentGroupMapping] gm 
									  ON gm.ConsoleId = bec.ConsoleId 
								Where
								   BuildingId = @buildingId 
						 End



							   /*Variable Assignments*/
						  Select distinct
							   bec.ConsoleId,
							   bec.ConsoleNumber,
							   becc.VariableType,
							   becc.[Value]
							FROM
							   BldgEquipmentConsole bec 
							   LEFT JOIN
								  [dbo].[BldgEquipmentConsoleCnfgn] becc 
								  ON bec.ConsoleId = becc.ConsoleId 
							   LEFT JOIN
								  [dbo].[BldgEquipmentGroupMapping] gm 
								  ON gm.ConsoleId = bec.ConsoleId 
							Where
							   BuildingId = @buildingId


						IF(@BldConsoleId > 0)
						Begin
						/*Group Mapping*/
						Select
							bec.ConsoleId,
							bec.ConsoleNumber,
							gm.GroupId as groupId,
							gm.GroupName AS groupName,
							gm.is_Checked,
							[dbo].[Fn_GetTotalGroupsCount](gm.GroupId) AS totalGroups , 
							[dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) AS totalUnits 
						FROM
							BldgEquipmentConsole bec 
							LEFT JOIN
								[dbo].[BldgEquipmentGroupMapping] gm 
								ON bec.ConsoleId = gm.ConsoleId 
						Where
							BuildingId = @buildingId 
							and gm.GroupName <> ''  and   bec.Name not in ('Dummy Console')

						--	union all

							--Select
		  --                  bec.ConsoleId,
		  --                  bec.ConsoleNumber,
		  --                  gm.GroupId as groupId,
		  --                  gm.GroupName AS groupName,
		  --                  gm.is_Checked,
		  --                  (
		  --                      case
		  --                      when
		  --                          bec.Name like '%Dummy Panel%' 
		  --                      then
		  --                          [dbo].[Fn_GetTotalGroupsCount](gm.GroupId) 
		  --                      else
		  --                          0 
		  --                      end
		  --                  )
		  --                  AS totalGroups , 
		  --                  (
		  --                      case
		  --                      when
		  --                          bec.Name like '%Dummy Panel%'  
		  --                      then
		  --                          [dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) 
		  --                      else
		  --                          0 
		  --                      end
		  --                  )
		  --                  AS totalUnits 
		  --              FROM
		  --                  BldgEquipmentConsole bec 
		  --                  LEFT JOIN
		  --                      [dbo].[BldgEquipmentGroupMapping] gm 
		  --                      ON bec.ConsoleId = gm.ConsoleId 
		  --              Where
		  --                  BuildingId =  8
		  --                  and   bec.Name like '%Dummy Panel%'   
		  --                  and gm.GroupName <> ''
						END
						ELSE
						Begin
						  Select
							bec.ConsoleId,
							bec.ConsoleNumber,
							gm.GroupId as groupId,
							gm.GroupName AS groupName,
							gm.is_Checked,
							[dbo].[Fn_GetTotalGroupsCount](gm.GroupId) AS totalGroups , 
							[dbo].[Fn_GetTotalUnitsCount]( gm.GroupId) AS totalUnits 
						FROM
							BldgEquipmentConsole bec 
							LEFT JOIN
								[dbo].[BldgEquipmentGroupMapping] gm 
								ON bec.ConsoleId = gm.ConsoleId 
						Where
							BuildingId = @buildingId
							and gm.GroupName <> ''   
						End


							/*Existing and Future Group Data*/
							Select distinct
								bec.ConsoleId,
								bec.ConsoleNumber,
								gcm.GroupCategoryId,
								gcm.GroupCategoryName,
								bcc.GroupName ,
								bcc.NoOfUnits
							FROM
								BldgEquipmentConsole bec 
								LEFT JOIN
								[dbo].[BldgEquipmentCategoryCnfgn] bcc 
								on bec.ConsoleId = bcc.ConsoleId 
								LEFT JOIN
								[dbo].[BldgEquipmentGroupCategoryMaster] gcm 
								ON bcc.GroupCategoryId = gcm.GroupCategoryId 
							Where
								BuildingId = @buildingId 
								and bec.Name like '%Lobby Panel%' 
								and gcm.GroupCategoryName <> '' 
		  END
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
END