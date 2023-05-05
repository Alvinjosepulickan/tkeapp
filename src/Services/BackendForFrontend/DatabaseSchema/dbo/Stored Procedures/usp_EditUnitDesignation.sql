CREATE PROC [dbo].[usp_EditUnitDesignation]
	@GroupConfigurationId INT,
	@UnitId int,
	@UserName nvarchar(25),
	@Designation nvarchar(25) null,
	@Description nvarchar(25),
	@Result int output
as
begin
	BEGIN TRY
	--DECLARE @Setid int
	declare @buildingId int
	set @buildingId=(select BuildingId from GroupConfiguration where GroupId=@GroupConfigurationId)
	if(@Designation='' and @Description='')
	begin
		set @Result=-3
		return 1
	end
	if (@Description<>'' and @Designation= '')
	begin
		--if(exists (select * from units where GroupConfigurationId=@GroupConfigurationId and UnitId!= @UnitId and [Description]=@Description))
		--begin
		--	set @Result=-1
		--	return 1
		--end
		--set @Setid=(select setid from Units where UnitId=@UnitId)
		/**Basic log**/
		insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @GroupConfigurationId,N'Unit Description',@Description,Description,@UserName,getdate(),@UserName,getdate()
		from Units where UnitId=@UnitId
		/**Basic log**/
		update units set [Description]=@Description where UnitId=@UnitId
		set @Result=1
		return 1
		end
	if (@Description='' and @Designation<> '')
	begin
			if(exists (select * from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@buildingId) and UnitId!= @UnitId and [Designation]=@Designation))
			begin
				set @Result=-1
				return 1
			end 
			/**Basic log**/
			insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @GroupConfigurationId,N'Unit Designation',@Designation,Designation,@UserName,getdate(),@UserName,getdate()
			from Units where UnitId=@UnitId
			/**Basic log**/
			update units set [Designation]=@Designation where UnitId=@UnitId
			set @Result=1
			return 1
		end
	if (@Description<>'' and @Designation<> '')
	begin
			--set @Setid=(select setid from Units where UnitId=@UnitId)
			--if(exists (select * from units where GroupConfigurationId=@GroupConfigurationId and UnitId!= @UnitId and [Description]=@Description))
			--begin
			--	set @Result=-1
			--	return 1
			--end
			if(exists (select * from units where GroupConfigurationId in (select GroupId from GroupConfiguration where BuildingId=@buildingId) and UnitId!= @UnitId and [Designation]=@Designation))
			begin
				set @Result=-1
				return 1
			end
			/**Basic log**/
			insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @GroupConfigurationId,N'Unit Designation',@Designation,Designation,@UserName,getdate(),@UserName,getdate()
			from Units where UnitId=@UnitId

			insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
			select @GroupConfigurationId,N'Unit Description',@Description,Description,@UserName,getdate(),@UserName,getdate()
			from Units where UnitId=@UnitId
			/**Basic log**/
			update units set [Designation]=@Designation,[Description]=@Description where UnitId=@UnitId
			set @Result=1
			return 1
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
end
