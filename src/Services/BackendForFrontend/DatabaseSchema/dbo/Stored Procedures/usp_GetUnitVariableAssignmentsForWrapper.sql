 
 CREATE Procedure [dbo].[usp_GetUnitVariableAssignmentsForWrapper] --95
 @groupId int 
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
as
Begin
				  	/* Unit Variables - Conversion Not Needed */
				 Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='REAROPENVARIABLEID') as VariableId
			 	     , (Case when (SELECT CASE WHEN (SELECT top 1 DoorType FROM [dbo].[Doors]
					  WHERE GroupConfigurationId=555 and UnitId in(select distinct UnitId from Units where SetId=u.SetId) and DoorType like  concat('%' , (Select VariableType from @ConstantMapperList where VariableKey ='REARDOORHANDVARIABLEID'),'%')) is null
						THEN 0 ELSE 1 END ) = 1 then  'TRUE' else 'FALSE' end) as [Value]
						   from units u
			          Where u.GroupConfigurationId=@groupId and u.IsDeleted=0  

					   Union all
			   /* CarId*/
			      Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='CARIDVARIABLEID')  as VariableId
			 	         ,Cast(ROW_NUMBER() OVER(ORDER BY name ASC) as nvarchar(5)) as [Value]
						   from units u
			   Where GroupConfigurationId=@groupId and IsDeleted=0 


		  Union all
			
			/* System Variable */
			      Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Replace(SystemVariableKeys,(Select VariableType from @ConstantMapperList where VariableKey ='PARAMVARIABLEID'),(Select VariableType from @ConstantMapperList where VariableKey ='ELEVATORPARAMVARIABLEID'))) as VariableId
			 	         ,SystemVariableValues as [Value]
						   from units u
						left join SystemsVariables sv on u.SetId = sv.SetId
			   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0  and  sv.IsDeleted=0

			    
				  union all
 
 
	 /*Unit Variables  */
		 Select distinct ut.GroupConfigurationId as groupConfigurationId 
						 ,ut.UnitId as unitId
						 ,ut.[Name]
						 ,ut.MappedLocation
						 ,uc.ConfigureVariables as VariableId
						,cast(uc.ConfigureValues  as nvarchar(50)) as [Value]
						   from Units ut
						left Join UnitConfiguration uc on ut.SetId = uc.SetId
						  Where ut.GroupConfigurationId = @groupId and ut.IsDeleted=0 and uc.isDeleted=0

		   union all
	      
 
				  Select  u.GroupConfigurationId as groupConfigurationId 
						 ,u.UnitId as unitId
						 ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='CARPOSVARIABLEID') as VariableId
						 ,cast(MappedLocation  as nvarchar(50)) as [Value]
				   from units u
			   Where GroupConfigurationId=@groupId and IsDeleted=0

			      union all
			     Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='TOPFVARIABLEID')   as VariableId
			 	         ,cast((Select top 1 FrontOpening from OpeningLocation
			                        where GroupConfigurationId=@groupId order by  FrontOpening desc ) as nvarchar(5)) as [Value]
						   from units u
			                   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 

			         union all

			     Select  u.GroupConfigurationId as groupConfigurationId 
			             ,u.UnitId as unitId
			             ,u.[Name]
						 ,u.MappedLocation
						 ,(Select VariableType from @ConstantMapperList where VariableKey ='TOPRVARIABLEID')  as VariableId
			 	         ,cast((Select top 1 RearOpening from OpeningLocation
			                        where GroupConfigurationId=@groupId order by  RearOpening desc) as nvarchar(5)) as [Value]
						   from units u
			                   Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 

End