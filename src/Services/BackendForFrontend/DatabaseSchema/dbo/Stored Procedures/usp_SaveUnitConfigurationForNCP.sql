 
CREATE Procedure [dbo].[usp_SaveUnitConfigurationForNCP]
 @SetId int
,@UnitVariables AS unitConfigurationDataTable READONLY
,@CreatedBy nvarchar(50)
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
,@Result int OUTPUT
as
Begin
           declare @productCategory nvarchar(200);
		   declare @model nvarchar(200);
	       SET @productCategory = (Select VariableType from @ConstantMapperList where VariableKey ='PRODUCTCATEGORY')
		   SET @model = (Select VariableType from @ConstantMapperList where VariableKey ='PARAMMODEL')



                	 IF((Select top 1 GroupConfigurationValue from GroupConfigurationDetails
						where GroupConfigurationType =@productCategory and GroupId=(Select distinct  GroupConfigurationId from units where SetId=@SetId)) = 'TWIN Elevator' )
						Begin
							  Insert into UnitConfiguration
							(
							  SetId
							 ,ConfigureVariables
							 ,ConfigureValues
							 ,ConfigureJson
							 ,CreatedBy
						     ,CreatedOn
							)
							Values
							(
							  @SetId
							 ,@model
							 ,'TWIN'
							 ,'{"variableId":'+ @model +',"value":"TWIN"}'
							 , @CreatedBy
						     , getdate()
							)


							Update UnitSet
							    set ProductName ='TWIN'
								  Where SetId=@SetId

						End
                


				DECLARE db_cursor CURSOR FOR SELECT UnitJson,value,UnitJsonData FROM @UnitVariables  
				DECLARE @UnitJson NVARCHAR(50)
				DECLARE @value NVARCHAR(200)
				DECLARE @UnitJsonData NVARCHAR(max)

				OPEN db_cursor;
				FETCH NEXT FROM db_cursor INTO @UnitJson, @value, @UnitJsonData;
				WHILE @@FETCH_STATUS = 0  
				BEGIN  


				   

					IF Exists (Select * from UnitConfiguration where SetId=@SetId and ConfigureVariables = @UnitJson)
					BEGIN

					  --   /* If already Log History Exists */
					     insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
							Select @SetId,t.UnitJson, t.value,g.ConfigureValues,@CreatedBy,GETDATE(),@CreatedBy,GETDATE()
							from @UnitVariables t join UnitConfiguration g 
							on t.UnitJson=g.ConfigureVariables and t.value<>g.ConfigureValues
							where g.SetId = @SetId


					    Update UnitConfiguration
						set ConfigureValues = @value
							,ConfigureJson = @UnitJson
							,ModifiedBy = @CreatedBy
							,ModifiedOn = getdate()
							  where SetId=@SetId and ConfigureVariables = @UnitJson

							 
					END
					ELSE
					BEGIN
	                    

						
				

					    insert into UnitConfiguration
							(
							SetId
						,ConfigureVariables
						,ConfigureValues
						,ConfigureJson
						,CreatedBy
						,CreatedOn
						)
						select 
						 @SetId
						,@UnitJson
						,@value
						,@UnitJsonData
						,@CreatedBy
						,getdate()

						-- /* If Log History Not Exists */
						insert into UnitConfigHistory(SetId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
						select @SetId,UnitJson,value,'',@CreatedBy,getdate(),@CreatedBy,getdate()
						from @UnitVariables 
					END
								  

						FETCH NEXT FROM db_cursor INTO  @UnitJson, @value, @UnitJsonData;
				END;
				CLOSE db_cursor;
				DEALLOCATE db_cursor;

				
				declare @productname nvarchar(200)

				Select * from @UnitVariables

				IF Exists(Select * from @UnitVariables where UnitJson = @model)
				Begin
				   /*Updating the productName in Set table*/
					set @productname = (Select distinct [value] from @UnitVariables
						  where UnitJson = @model)
                
				   
				   	Select @productname

					Select @SetId


					update UnitSet
					  set ProductName = @productname
						   where SetId=@SetId
				
				 End


                /*Adding NCP Manual Logic in FDA*/

				declare @groupId int
				Set @groupId = (Select top 1 GroupConfigurationId from Units
					where SetId=@SetId) 

				declare @quoteId nvarchar(50)
				Set @quoteId = (Select top 1 QuoteId from Building where Id in(Select distinct BuildingId from GroupConfiguration 
				                where GroupId in (Select top 1 GroupConfigurationId from Units where SetId=@SetId))) 


			IF NOT EXISTS (Select * from FieldDrawingMaster where GroupId=@groupId and QuoteId=@quoteId)
			Begin
				Insert into FieldDrawingMaster
				(
				 GroupId
				,StatusKey
				,OpportunityId
				,DrawingMethod
				,CreatedBy
				,CreatedOn
				,QuoteId
				)
				Values
				(
				 @groupId
				,'DWG_NA'
				,''
				,1
				,@CreatedBy
				,getdate()
				,@quoteId
				)
			END


	   Update Units
	     set WorkflowStatus='UNIT_VAL'
		     ,ConflictStatus ='UNIT_VAL'
			    where SetId=@setId


	   exec [dbo].[usp_UpdateWorkflowStatus]@groupId,'group'


		SET @Result=@SetId
End