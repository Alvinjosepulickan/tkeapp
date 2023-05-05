CREATE Procedure [dbo].[usp_SaveUpdateProductSelection]
@username nvarchar(250),
@unitList as UnitIDList readonly,
@productSelected nvarchar(250),
@BusinessLine nvarchar(2) null,
@country nvarchar(2) null,
@supplyingFactory nvarchar(2)null,
@controlLanding int,
@FixtureStrategy nvarchar(100),
@defaultConfigurationUHF DefaultConfigurationTable Readonly,
@defaultConfigurationEntrances DefaultConfigurationTable Readonly,
@Result int output
as
begin
	BEGIN TRY
		declare @minUnitid int
		DECLARE @GroupConfigurationId int --STORE Group Configuration id
		DECLARE @SetId int --Store Set Id of selected units
		DECLARE @NumberOfUnits int --
		DECLARE @NumberOfSets int --number of sets for given group configuration id
		DECLARE @SetName nvarchar(10) -- for generating set name
		SET @GroupConfigurationId=(select distinct(GroupConfigurationId) from Units where UnitId in(select unitid from @unitList)) --get group configuration id
		SET @NumberOfSets=(select count(distinct(SetId)) from Units where SetId >0 and GroupConfigurationId=@GroupConfigurationId and isdeleted=0)+1 --
		SET @SetId=(select distinct(SetId) from Units where UnitId in(select unitid from @unitList))
		SET @SetName= 'Set'+cast(@NumberOfSets as nvarchar(5))
		DECLARE @UEID NVARCHAR(30)
		IF(@SetId=0)
		BEGIN
			   INSERT INTO UnitSet([Name],[Description],[ProductName],[CreatedBy],[createdOn]) values(@SetName,@SetName,@productSelected,@username,getdate())
				update units set SetId=SCOPE_IDENTITY(),WorkflowStatus='UNIT_INC' where UnitId in(select UnitId from @unitList)
			   set @Result=SCOPE_IDENTITY()
				set @minUnitId=(select min(unitId) from @unitList)
				--update units table with generated ueid
				declare @maxUnitId int
				set @maxUnitId=(select max(unitId) from @unitList)
				while (@minUnitId<=@maxUnitId)
				begin
					if (exists (select * from @unitList where UnitID=@minUnitid))
					begin
						EXEC usp_GenerateUEID @businessLine, @country, @supplyingFactory, @UEID = @UEID OUTPUT 
						update units set ueid=@ueid where UnitId=@minUnitid
					end
					set @minUnitid=@minUnitid+1
					select @minUnitid
				end
		END
		ELSE
		BEGIN
		SET @NumberOfUnits=(SELECT COUNT(*) FROM Units WHERE SetId=@SetId and isDeleted=0)
		IF(@NumberOfUnits=(SELECT COUNT(*) FROM @unitList))
		BEGIN
		  UPDATE UnitSet SET ProductName=@productSelected where SetId=@SetId
		  set @Result=@setId
		END
		ELSE
		BEGIN
		  INSERT INTO UnitSet([Name],[Description],[ProductName],[CreatedBy],[createdOn]) values(@SetName,@SetName,@productSelected,@username,getdate())
		  UPDATE UNITS SET SetId=SCOPE_IDENTITY() where UnitId in (select UnitID from @unitList)
		  set @Result=SCOPE_IDENTITY()
		END
		if((@productSelected='') and ((select count(*) from @unitList)=1))
		begin
			   declare @product nvarchar(50)
			   set @product=(select ProductName from UnitSet where setid=@SetId)
			   update UnitSet set ProductName=@product where SetId=SCOPE_IDENTITY()
			   set @minUnitid=(select max(UnitConfigurationId) from UnitConfiguration where SetId=@SetId)
			   insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted) select SetId,ConfigureVariables,ConfigureValues,ConfigureJson,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,IsDeleted from UnitConfiguration where SetId=@SetId and IsDeleted=0
			   update UnitConfiguration set SetId=@Result where SetId=@SetId and UnitConfigurationId>@minUnitid
		end
		END


		If exists(select '*' from openinglocation where GroupConfigurationId = @GroupConfigurationId and OcuppiedSpaceBelow = 1)
		Begin
			insert into UnitConfiguration(SetId,ConfigureVariables,ConfigureValues,ConfigureJson, CreatedBy,CreatedOn)
			select @Result,'ELEVATOR.Parameters.CWTSFTY','TRUE','{"variableId":"ELEVATOR.Parameters.CWTSFTY","value":"TRUE"}',@username,getdate()
		End
			
		--default UHF
		EXEC [dbo].[usp_CreateDefaultsForUHF]@Result,@FixtureStrategy,@username,@defaultConfigurationUHF

		--default Entrance
		EXEC [dbo].[usp_CreateDefaultsForEntranceConfiguration]@Result,@controlLanding,@username,@defaultConfigurationEntrances
		
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end


