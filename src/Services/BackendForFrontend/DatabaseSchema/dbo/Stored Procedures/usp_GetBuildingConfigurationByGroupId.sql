CREATE Procedure [dbo].[usp_GetBuildingConfigurationByGroupId] 
@id int
as
Begin
	BEGIN TRY
	   set @id = (select buildingId from GroupConfiguration where GroupId = @id)
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
