
 
CREATE PROCEDURE [dbo].[usp_GetGroupTab] --50
@groupId int,
@hallStationVariables unitConfigurationDataTable Readonly
AS
BEGIN
	BEGIN TRY
	   declare @isDisabledForGroupHallFixtures bit
	   set @isDisabledForGroupHallFixtures = 1
	   declare @isDisabledForOpenings bit
	   set @isDisabledForOpenings = 1

 
	 if ((select Count(*) from OpeningLocation where OpeningLocation.GroupConfigurationId=@groupId and NoOfFloors>0 and OpeningLocation.IsDeleted=0)>1)
	 begin
		SET @isDisabledForGroupHallFixtures=0
	 end
 
				
	if exists(select DoorType from Doors where DoorType like '%rearDoorTypeAndHand_SP%' and GroupConfigurationId=@groupId and DoorType like '%B1%' and IsDeleted=0)
	begin
		set @isDisabledForOpenings = 1
		if(EXISTS(select top 1 HallstationName from UnitHallStationMapping unithallstation inner join 
					@hallStationVariables hsv on unithallstation.HallStationName=hsv.UnitJson 
					where unithallstation.UnitPosition like '%B1%' and HallstationName like '%R_SP%'))
		BEGIN
			set @isDisabledForOpenings = 0
		END
	end



	if exists( select DoorType from Doors where DoorType like '%rearDoorTypeAndHand_SP%' and GroupConfigurationId=@groupId and DoorType like '%B2%'and IsDeleted=0)
	begin
		set @isDisabledForOpenings = 1
		if(EXISTS(select top 1 HallstationName from UnitHallStationMapping unithallstation inner join 
					@hallStationVariables hsv on unithallstation.HallStationName=hsv.UnitJson 
					where unithallstation.UnitPosition like '%B2%' and HallstationName like '%R_SP%'))
		BEGIN
			set @isDisabledForOpenings = 0
		END

	end
	if exists (select MappedLocation from Units where GroupConfigurationId= @groupId and MappedLocation like '%B1%' and IsDeleted=0)
	begin
		set @isDisabledForOpenings = 1
		if(EXISTS(select top 1 HallstationName from UnitHallStationMapping unithallstation inner join 
					@hallStationVariables hsv on unithallstation.HallStationName=hsv.UnitJson 
					where unithallstation.UnitPosition like '%B1%' and HallstationName like '%F_SP%'))
		BEGIN
			set @isDisabledForOpenings = 0
		END
	end
	if exists (select MappedLocation from Units where GroupConfigurationId= @groupId and MappedLocation like '%B2%' and IsDeleted=0)
	begin
		set @isDisabledForOpenings = 1
		if(EXISTS(select top 1 HallstationName from UnitHallStationMapping unithallstation inner join 
					@hallStationVariables hsv on unithallstation.HallStationName=hsv.UnitJson 
					where unithallstation.UnitPosition like '%B2%' and HallstationName like '%F_SP%'))
		BEGIN
			set @isDisabledForOpenings = 0
		END
	end
	if (@isDisabledForOpenings = 1)
	begin
		Set @isDisabledForGroupHallFixtures=1
	end
	 select @isDisabledForGroupHallFixtures as IsDisabledForGroupHallFixtures
	 select @isDisabledForOpenings as IsDisabledForOpenings
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