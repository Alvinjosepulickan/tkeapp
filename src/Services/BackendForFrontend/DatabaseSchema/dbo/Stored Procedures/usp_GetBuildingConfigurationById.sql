CREATE Procedure [dbo].[usp_GetBuildingConfigurationById] 
@id int
as
Begin

   Select '['+ SUBSTRING( 
			( 
		     SELECT ',{"VariableId":"'+k.BuindingType+'","Value":"'+k.BuindingValue+'"}'
			from building b
			Left Join BuildingConfiguration k
			on b.Id = k.BuildingId
			where b.id = @id
			and b.isDeleted=0
			FOR XML PATH('') 
			), 2 , 9999) + ']' As BldJson


END 
