

CREATE PROCEDURE [dbo].[usp_SaveBuildingEquipmentConfiguration]
	-- Add the parameters for the stored procedure here
	@BuildngId int,
	@buildingVariables as unitConfigurationDataTable READONLY,
	@CreatedBy nvarchar(50),
	@Result int OUTPUT
AS
BEGIN
	BEGIN TRY
		if(exists (select * from BldgEquipmentConfiguration where BuildingId=@BuildngId))
		begin
		
			insert into [BuildingConfigHistory]
			(
				BuildingId,
				VariableId,
				CurrentValue,
				PreviousValue,
				CreatedBy,
				CreatedOn,
				ModifiedBy,
				ModifiedOn
			)
			Select 
				@BuildngId,
				t.UnitJson,
				t.value,
				b.Value,
				@CreatedBy,
				GETDATE(),
				@CreatedBy,
				GETDATE()
				from @buildingVariables t join BldgEquipmentConfiguration b
				on t.UnitJson=b.VariableType and t.value<>b.Value
				where b.BuildingId = @BuildngId

			update BldgEquipmentConfiguration set VariableType=t.UnitJson,Value=t.value,modifiedby=@CreatedBy,ModifiedOn=getdate()
			from @buildingVariables t join BldgEquipmentConfiguration g on t.UnitJson=g.VariableType
			where g.BuildingId = @BuildngId

			--insert new variable assignments

			insert into [BuildingConfigHistory]
			(
				BuildingId,
				VariableId,
				CurrentValue,
				PreviousValue,
				CreatedBy,
				CreatedOn,
				ModifiedBy,
				ModifiedOn
			)
			Select 
				@BuildngId,
				UnitJson,
				value,
				'',
				@CreatedBy,
				GETDATE(),
				@CreatedBy,
				GETDATE()
				from @buildingVariables 
				where unitjson not in(select VariableType from BldgEquipmentConfiguration where BuildingId = @BuildngId)


			insert into BldgEquipmentConfiguration(BuildingId,VariableType,value, CreatedBy,CreatedOn)
			select @BuildngId,UnitJson,value,@CreatedBy,getdate()
			from @buildingVariables where unitjson not in(select VariableType from BldgEquipmentConfiguration where BuildingId = @BuildngId)


			set @Result=@BuildngId



		end
		else
		begin

			insert into [BuildingConfigHistory]
			(
				BuildingId,
				VariableId,
				CurrentValue,
				PreviousValue,
				CreatedBy,
				CreatedOn,
				ModifiedBy,
				ModifiedOn
			)
			Select 
				@BuildngId,
				UnitJson,
				value,
				'',
				@CreatedBy,
				GETDATE(),
				@CreatedBy,
				GETDATE()
				from @buildingVariables

			insert into BldgEquipmentConfiguration(BuildingId,VariableType,value, CreatedBy,CreatedOn)
			select @BuildngId,UnitJson,value,@CreatedBy,getdate()
			from @buildingVariables
			set @Result=@BuildngId
		end
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
