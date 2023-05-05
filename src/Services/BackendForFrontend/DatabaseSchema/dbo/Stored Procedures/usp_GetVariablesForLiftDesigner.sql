CREATE Procedure [dbo].[usp_GetVariablesForLiftDesigner]
@groupid int
As
Begin
	BEGIN TRY
	 declare @buildingId int
	 set @buildingId=(select distinct(BuildingId) from GroupConfiguration where GroupId=@groupId)

		/* Building Variables */
	  select  k.BuindingType as VariableId
			 ,k.BuindingValue as [Value]
				from building b
				Left Join BuildingConfiguration k on b.Id = k.BuildingId
				where b.id = @buildingId and b.isDeleted=0

		Union All

		/* Group Variables */
		  select distinct gc.GroupConfigurationType as VariableId
						 ,gc.GroupConfigurationValue as [Value]
							from GroupConfiguration g
							Left Join GroupConfigurationDetails gc on g.GroupId = gc.GroupId
							where g.GroupId = @groupId and g.isDeleted=0 


		Union All

		   Select  'ELEVATOR.Parameters.Layout.LAYLANG' as VariableId
				   ,'EN-US' as [Value]
				   from units u
			   Where GroupConfigurationId=@groupId

			   union all

			   Select  'ELEVATOR.Parameters.Layout.LAYUNITS' as VariableId
						 , 'MET' as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId

			   union all

				 Select  'ELEVATOR.Parameters.Basic_Info.ELEVDESG' as VariableId
						 ,Designation as [Value]
				   from units u
			   Where u.GroupConfigurationId=@groupId
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
