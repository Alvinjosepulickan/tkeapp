
 
CREATE PROCEDURE [dbo].[usp_WrapperAPIXmlGeneration]   
 @groupId int 
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
AS
BEGIN
	BEGIN TRY
			    declare @opportunityId nvarchar(20)
				SET @opportunityId = (Select top 1 OpportunityId from Quotes where QuoteId = (Select distinct OpportunityId from Building where Id in (Select distinct BuildingId from GroupConfiguration
				where GroupId =@groupId)))



					Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='ELEVSYSVARIABLEID') as VariableId
							 ,cast('EVO' as nvarchar(10)) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0


			   Union all


			     Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='LAYLANGVARIABLEID') as VariableId
						 ,(case when @opportunityId like 'SC-%' then 'EN-CA' else 'EN-US' end) as [Value]
				   from units u
			   Where GroupConfigurationId=@groupId and IsDeleted=0


			  Union all

			  
               Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='LIFTDESIGNERBANKDIST') as VariableId
						 ,(Select distinct GroupConfigurationValue from GroupConfigurationDetails where GroupConfigurationType=(Select VariableType from @ConstantMapperList where VariableKey ='PARAMETERSBANKDIST')  and  GroupId=u.GroupConfigurationId)  as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0

			  Union all

			   Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='LIFTDESIGNERSUMPTYP') as VariableId
						 ,(Select distinct GroupConfigurationValue from GroupConfigurationDetails where GroupConfigurationType=(Select VariableType from @ConstantMapperList where VariableKey ='PARAMETERSSUMPTYP') and  GroupId=u.GroupConfigurationId)  as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0

			    
		  

			       Union all

			    Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='LAYUNITSVARIABLEID') as VariableId
			    ,   (case when (Select distinct Code from MeasuringUnits where Id  = (Select MeasuringUnitId from   [dbo].[Projects] where OpportunityId =@opportunityId)) = 'Metric' then 'MET'
             when (Select distinct Code from MeasuringUnits where Id  = (Select MeasuringUnitId from   [dbo].[Projects] where OpportunityId =@opportunityId)) = 'Imperial' then 'IMP'
			 when (Select distinct Code from MeasuringUnits where Id  = (Select MeasuringUnitId from   [dbo].[Projects] where OpportunityId =@opportunityId)) = 'Dual' then 'DUAL' end) as [Value]
			    from units u
			   Where GroupConfigurationId=@groupId and IsDeleted=0

			

			   Union all


							 Select  GroupConfigurationId as groupConfigurationId
									,UnitId as unitId
									,[Name]
									,MappedLocation
									,(Select VariableType from @ConstantMapperList where VariableKey ='LAYGRPSZVARIABLEID') as VariableId
									,cast((Select count(*) from units
										  where GroupConfigurationId=@groupId  and IsDeleted=0) as nvarchar(50)) as [Value]
									  from units
										  where GroupConfigurationId=@groupId and IsDeleted=0
						                 
								

			   union all

				 Select  u.GroupConfigurationId as groupConfigurationId 
						 ,u.UnitId as unitId
						 ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='ELEVDESGVARIABLEID') as VariableId
						 ,cast(Designation  as nvarchar(50)) as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId and IsDeleted=0


			   union all

				 Select  u.GroupConfigurationId as groupConfigurationId 
						 ,u.UnitId as unitId
						 ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='PITLADCLRVARIABLEID') as VariableId
						 ,cast('4.5'  as nvarchar(50)) as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId and IsDeleted=0

			   

			  
				 union all
 
						 /*  Need to check in stub */
					 Select distinct gc.GroupId as groupConfigurationId  
			          ,ut.UnitId as unitId
			          ,ut.[Name]
				      ,ut.MappedLocation
			          ,(Select VariableType from @ConstantMapperList where VariableKey ='MAINFLRVARIABLEID') as VariableId 
				      ,cast(floorNumber as nvarchar(50)) as Value 
					                                          from BuildingElevation be
																    Left Join GroupConfiguration gc on be.BuildingId = gc.BuildingId
																	Left Join units ut on ut.GroupConfigurationId=gc.GroupId
																	 where  gc.GroupId=@groupId and be.MainEgress=1 and gc.IsDeleted=0 and ut.IsDeleted=0

		        union all

 				 /*  Need to check in stub */
					 Select distinct gc.GroupId as groupConfigurationId  
			          ,ut.UnitId as unitId
			          ,ut.[Name]
				      ,ut.MappedLocation
			          ,(Select VariableType from @ConstantMapperList where VariableKey ='FIREFLRVARIABLEID') as VariableId 
				      ,cast(floorNumber as nvarchar(50)) as Value 
					                                          from BuildingElevation be
																    Left Join GroupConfiguration gc on be.BuildingId = gc.BuildingId
																	Left Join units ut on ut.GroupConfigurationId=gc.GroupId
																	 where  gc.GroupId=@groupId and be.MainEgress=1 and gc.IsDeleted=0 and ut.IsDeleted=0



				  union all
                	
				 /*  Need to check in stub */
					 Select distinct gc.GroupId as groupConfigurationId  
			          ,ut.UnitId as unitId
			          ,ut.[Name]
				      ,ut.MappedLocation
			          ,(Select VariableType from @ConstantMapperList where VariableKey ='FIREFLRALTVARIABLEID') as VariableId ,cast(Replace(Name,'U','') as nvarchar(50)) as Value from BuildingElevation be
																    Left Join GroupConfiguration gc on be.BuildingId = gc.BuildingId
																	Left Join units ut on ut.GroupConfigurationId=gc.GroupId
																	 where  gc.GroupId=@groupId and be.alternateEgress=1 and gc.IsDeleted=0 and ut.IsDeleted=0
                      
					   union all

					  

				   /* Front */
				  	Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='ENTWLTHKFVARIABLEID') as VariableId
							 ,([dbo].[Fn_GetEntranceFrontOpeningForLD](GroupConfigurationId,UnitId,'ENTWLTHKF')) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0

				   union all
				   
				   /* Rear */
					Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 , (Select VariableType from @ConstantMapperList where VariableKey ='ENTWLTHKRVARIABLEID') as VariableId
							 ,([dbo].[Fn_GetEntranceRearOpeningForLD](GroupConfigurationId,UnitId,'ENTWLTHKF')) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0

				   
                  union all

				   /* Front */
				   	Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='COLFACEFVARIABLEID') as VariableId
							 ,([dbo].[Fn_GetEntranceFrontOpeningForLD](GroupConfigurationId,UnitId,'COLFACEF')) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0


				    union all
				   
				   /* Rear */
					Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='COLFACERVARIABLEID') as VariableId
							 ,([dbo].[Fn_GetEntranceRearOpeningForLD](GroupConfigurationId,UnitId,'COLFACEF')) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0

				     union all

				   /* System Variable */
			      Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Replace(SystemVariableKeys,(Select VariableType from @ConstantMapperList where VariableKey ='PARAMPREFIXVARIABLEID'),(Select VariableType from @ConstantMapperList where VariableKey ='LDPREFIXVARIABLEID'))) as VariableId
			 	         ,SystemVariableValues as [Value]
						   from units u
						left join SystemsVariables sv on u.SetId = sv.SetId
			   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0  and  sv.IsDeleted=0


			     union all

	 
			Select  distinct u.GroupConfigurationId as groupConfigurationId 
						,u.UnitId as unitId
						,u.[Name]
						,u.MappedLocation
						,(Select VariableType from @ConstantMapperList where VariableKey ='FLRMRKFVARIABLEID') as VariableId
						,([dbo].[Fn_GetFLRMRKFrontForLD](u.GroupConfigurationId,u.UnitId)) as [Value]
				from units u
				 Left join GroupConfiguration gc on u.GroupConfigurationId =gc.GroupId
				 Left Join BuildingElevation be on be.BuildingId = gc.BuildingId
			Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 and be.IsDeleted=0 and gc.IsDeleted=0

			   union all

			 	Select  distinct u.GroupConfigurationId as groupConfigurationId 
				,u.UnitId as unitId
				,u.[Name]
				,u.MappedLocation
				,(Select VariableType from @ConstantMapperList where VariableKey ='ENTFVARIABLEID') as VariableId
				,([dbo].[Fn_GetENTFForLD](u.GroupConfigurationId,u.UnitId)) as [Value]
					from units u
  				Where u.GroupConfigurationId=@groupId and u.IsDeleted=0  

	           union all

		 	Select  distinct u.GroupConfigurationId as groupConfigurationId 
				,u.UnitId as unitId
				,u.[Name]
				,u.MappedLocation
				,(Select VariableType from @ConstantMapperList where VariableKey ='ENTRVARIABLEID') as VariableId
				,([dbo].[Fn_GetENTRForLD](u.GroupConfigurationId,u.UnitId)) as [Value]
					from units u
  				Where u.GroupConfigurationId=@groupId and u.IsDeleted=0  

				Union all

				 Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='ROOFHEIGHTVARIABLEID') as VariableId
							 ,[dbo].[Fn_GetRoofHeight](gc.BuildingId)  as [Value]
					   from  GroupConfiguration gc 
						 Left Join units u on gc.GroupId =u.GroupConfigurationId
				   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 

				Union all

				Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='TRAVELVARIABLEID') as VariableId
							 ,cast(cast((select distinct (Travelfeet*12) + TravelInch as Travel from OpeningLocation where UnitId = u.UnitId) as int) as nvarchar(200)) as [Value]
					   from units u
				   Where GroupConfigurationId=@groupId and IsDeleted=0


				Union all

				Select  u.GroupConfigurationId as groupConfigurationId 
							 ,u.UnitId as unitId
							 ,u.[Name]
							 ,u.MappedLocation
							 ,(Select VariableType from @ConstantMapperList where VariableKey ='FLLDISTANCEVARIABLEID') as VariableId
							 ,[dbo].[Fn_GetFLLDistances](gc.BuildingId)  as [Value]
					   from  GroupConfiguration gc 
						 Left Join units u on gc.GroupId =u.GroupConfigurationId
				   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 

 



			 /* Getting the Unit Deatils */
			Select u.UnitId
				  ,u.MappedLocation
				  ,u.[Name]
				  ,u.GroupConfigurationId
				   from units u
			   Where GroupConfigurationId=@groupId and IsDeleted=0
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
