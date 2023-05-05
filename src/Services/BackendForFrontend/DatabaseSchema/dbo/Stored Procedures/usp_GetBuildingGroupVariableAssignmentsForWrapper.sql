
 CREATE Procedure [dbo].[usp_GetBuildingGroupVariableAssignmentsForWrapper]
 @groupId int 
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
as
Begin 

                   /* FDA Variables */
					DECLARE @unitTable TABLE
					(
					unitId NVARCHAR(50),
					unitname NVARCHAR(30)
					)
						insert into @unitTable
					Select UnitId, concat('U' , ROW_NUMBER() OVER(ORDER BY name ASC) ) as unitName from Units
						where GroupConfigurationId=@groupId

					DECLARE @fdatable TABLE
					(
					groupid int,
					VariableId NVARCHAR(50),
					[Value] NVARCHAR(30),
					unitname NVARCHAR(30)
					)
									   
					insert into @fdatable
				Select GroupId,GroupConfigurationType,GroupConfigurationValue,[dbo].[Fn_GetFDAUnitNameByGroupid](GroupId,GroupConfigurationType) from GroupConfigurationDetails
					where GroupId=@groupId and GroupConfigurationType like '%ELEVATOR00%' 



                 /* Building Variables */
				Select distinct   ut.GroupConfigurationId as groupConfigurationId 
								,ut.UnitId as unitId
								,ut.[Name] 
								,ut.MappedLocation
								,bg.BuindingType as VariableId
								,cast(bg.BuindingValue as nvarchar(50)) as [Value] from BuildingConfiguration bg
					Left Join GroupConfiguration gc on bg.BuildingId= gc.BuildingId
					Left Join units ut on gc.GroupId=ut.GroupConfigurationId
					where gc.GroupId=@groupId and bg.IsDeleted=0 and gc.IsDeleted=0 and ut.IsDeleted=0


					  union all

 						   
								 
								
					Select uts.GroupConfigurationId
							,uts.UnitId as unitId
							,uts.[Name]
							,uts.MappedLocation
							,ft.VariableId
							,ft.[Value]
							from @unitTable ut
						Left Join @fdatable ft on ut.unitname =ft.unitname 
						Left Join Units uts on uts.UnitId =ut.unitId
									     
							  Union all

					    /* Group Variables */
						Select GroupId
							,ut.UnitId as unitId
							,ut.[Name]
							,ut.MappedLocation
							,GroupConfigurationType as VariableId
							,GroupConfigurationValue as [Value]
							from GroupConfigurationDetails gcd
							Left Join Units ut on gcd.GroupId =ut.GroupConfigurationId
						where GroupId=@groupId and GroupConfigurationType not like '%ELEVATOR00%' 



						  Union all



	     
					   /*  Need to check in stub */
						 Select distinct cl.GroupConfigurationId as groupConfigurationId  
								  ,ut.UnitId as unitId
								  ,ut.[Name]
								  ,ut.MappedLocation
								  ,cl.ControlLocationType as VariableId
								  ,cast(cl.ControlLocationValue  as nvarchar(50)) as [Value]from ControlLocation cl
								  Left Join units ut on cl.GroupConfigurationId=ut.GroupConfigurationId
								   where cl.GroupConfigurationId=@groupId and cl.IsDeleted=0 and ut.IsDeleted=0

End